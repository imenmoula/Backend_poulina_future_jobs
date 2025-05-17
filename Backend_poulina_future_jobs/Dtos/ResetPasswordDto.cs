
using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Dtos
{

    public class ResetPasswordDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}


}