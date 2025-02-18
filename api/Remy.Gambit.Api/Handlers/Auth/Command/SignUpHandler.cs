using AutoMapper;
using FluentValidation;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Api.Handlers.Auth.Command.Dto;
using Remy.Gambit.Core.Cqs;
using Remy.Gambit.Data.Users;

namespace Remy.Gambit.Api.Handlers.Auth.Command
{
    public class SignUpHandler(IValidator<SignUpRequest> validator, IMapper mapper, IUsersRepository userRepository) : ICommandHandler<SignUpRequest, SignUpResult>
    {
        private readonly IValidator<SignUpRequest> _validator = validator;
        private readonly IMapper _mapper = mapper;
        private readonly IUsersRepository _userRepository = userRepository;
        private readonly SemaphoreSlim _semaphore = new(1);

        public async ValueTask<SignUpResult> HandleAsync(SignUpRequest command, CancellationToken token = default)
        {
            var validationResult = await _validator.ValidateAsync(command, token);
            if (!validationResult.IsValid)
            {
                return new SignUpResult { IsSuccessful = false, ValidationResults = validationResult.Errors.Select(x => x.ErrorMessage) };
            }

            try
            {
                await _semaphore.WaitAsync(token);

                var isUsernameAvailable = await _userRepository.CheckUsernameAvailabilityAsync(command.Username, token);

                if (!isUsernameAvailable)
                {
                    return new SignUpResult { IsSuccessful = false, ValidationResults = ["Username already exists"] };
                }

                string password = BCrypt.Net.BCrypt.HashPassword(command.Password);

                var role = Constants.Roles.Player;
                Guid? upline = null;
                string? agentCode = null;

                if (!string.IsNullOrEmpty(command.ReferralCode))
                {
                    var referrer = await _userRepository.GetUserByReferralCodeAsync(command.ReferralCode, token);

                    if(referrer is not null && referrer.UserRole!.IsAgent)
                    {
                        upline = referrer!.Id;

                        if (referrer.Role == Constants.Roles.Incorporator)
                        {
                            agentCode = AgentCodes.SuperMasterAgent;
                            role = Constants.Roles.SuperMasterAgent;
                        }
                    }
                }

                var succeeded = await _userRepository.SignUpAsync(command.Username, password, command.ContactNumber, role, upline, agentCode, token: token);

                if (!succeeded)
                {
                    return new SignUpResult { IsSuccessful = false, Errors = ["Unable to sign up"] };
                }

                return new SignUpResult { IsSuccessful = true };
            }
            catch
            {
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
