using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LMS.Repositories.Entities;

public class Subject
{
    [Key]
    public int SubjectId { get; set; }

    [Required]
    [Column(TypeName = "varchar(20)")]
    [MaxLength(20)]
    public string SubjectCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string SubjectName { get; set; } = string.Empty;

    public int Credit { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
