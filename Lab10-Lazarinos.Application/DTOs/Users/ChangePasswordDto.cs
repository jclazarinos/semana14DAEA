namespace Lab10_Lazarinos.Application.DTOs.Users;

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}