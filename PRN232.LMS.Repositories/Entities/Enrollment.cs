using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LMS.Repositories.Entities;

public class Enrollment
{
    [Key]
    public int EnrollmentId { get; set; }

    public int StudentId { get; set; }

    [ForeignKey(nameof(StudentId))]
    public Student? Student { get; set; }

    public int CourseId { get; set; }

    [ForeignKey(nameof(CourseId))]
    public Course? Course { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime EnrollDate { get; set; }

    [Required]
    [Column(TypeName = "varchar(20)")]
    [MaxLength(20)]
    public string Status { get; set; } = string.Empty;
}
