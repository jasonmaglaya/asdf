namespace Remy.Gambit.Models;

public class AppSetting
{
    public required string SettingKey { get; set; }
    public required string Name { get; set; }
    public required object Value { get; set; }
    public required string DataType { get; set; }
}
