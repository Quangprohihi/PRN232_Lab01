using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.API.RequestModels;

public class SemesterRequestModel
{
    [Required]
    [MaxLength(100)]
    public string SemesterName { get; set; } = string.Empty;

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}
