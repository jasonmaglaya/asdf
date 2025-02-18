#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Remy.Gambit.Api.Web/Remy.Gambit.Api.Web.csproj", "Remy.Gambit.Api.Web/"]
RUN dotnet restore "Remy.Gambit.Api.Web/Remy.Gambit.Api.Web.csproj"
COPY . .
WORKDIR "/src/Remy.Gambit.Api.Web"
RUN dotnet build "Remy.Gambit.Api.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Remy.Gambit.Api.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Remy.Gambit.Api.Web.dll"]