namespace PRN232.LMS.API.ResponseModels;

public class CourseResponseModel
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public int SubjectId { get; set; }

    public int SemesterId { get; set; }

    public SubjectResponseModel? Subject { get; set; }

    public SemesterResponseModel? Semester { get; set; }
}
