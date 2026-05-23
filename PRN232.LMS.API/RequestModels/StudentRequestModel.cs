using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.RequestModels;

public class StudentRequestModel
{
    [Required]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public DateOnly DateOfBirth { get; set; }
}
