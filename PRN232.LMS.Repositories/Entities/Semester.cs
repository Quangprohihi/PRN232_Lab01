using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LMS.Repositories.Entities;

public class Semester
{
    [Key]
    public int SemesterId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SemesterName { get; set; } = string.Empty;

    [Column(TypeName = "timestamp without time zone")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime EndDate { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
