using System.ComponentModel.DataAnnotations;

namespace dotnet_rpg.DTOs.User;

public class UserRegisterDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    [Required]
    [Compare("Password")]
    public string Password { get; set; } = string.Empty;
}