namespace Application.Options;

public class AuthOptions
{
    public int PasswordMinLength { get; set; }
    public int PasswordMaxLength { get; set; }
    public int NameMaxLength { get; set; }
    public int TitleMinLength { get; set; }
    public int TitleMaxLength { get; set; }
}