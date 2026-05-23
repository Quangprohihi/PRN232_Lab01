using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.RequestModels;

public class CourseRequestModel
{
    [Required]
    [MaxLength(100)]
    public string CourseName { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue)]
    public int SubjectId { get; set; }

    [Required]
    [Range(1, int.MaxValue)]
    public int SemesterId { get; set; }
}
