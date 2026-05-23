using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.RequestModels;

public class EnrollmentRequestModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int StudentId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int CourseId { get; set; }

    [Required]
    public DateOnly EnrollmentDate { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;
}
