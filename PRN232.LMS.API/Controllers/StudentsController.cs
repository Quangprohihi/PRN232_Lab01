using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helpers;
using PRN232.LMS.API.RequestModels;
using PRN232.LMS.API.ResponseModels;
using PRN232.LMS.API.Wrappers;
using PRN232.LMS.Services.BusinessModels;
using PRN232.LMS.Services.Interfaces;

namespace PRN232.LMS.API.Controllers;

[ApiController]
[Route("api/students")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    private static readonly IReadOnlyDictionary<string, Func<StudentResponseModel, object?>> FieldSelectors =
        new Dictionary<string, Func<StudentResponseModel, object?>>(StringComparer.OrdinalIgnoreCase)
        {
            ["studentId"] = student => student.StudentId,
            ["fullName"] = student => student.FullName,
            ["email"] = student => student.Email,
            ["dateOfBirth"] = student => student.DateOfBirth
        };

    public StudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<List<StudentResponseModel>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResponse<List<StudentResponseModel>>>> GetStudents(
        [FromQuery] string? search,
        [FromQuery] string? sort,
        [FromQuery] int page = 1,
        [FromQuery(Name = "size")] int size = 10,
        [FromQuery] string? fields = null)
    {
        var selectedFields = FieldSelectionHelper.ParseFields(fields);
        var invalidFields = FieldSelectionHelper.GetInvalidFields(selectedFields, FieldSelectors);
        if (invalidFields.Count > 0)
        {
            return BadRequest(new ApiResponse<object>(
                success: false,
                message: "One or more requested fields are invalid.",
                errors: new { fields = invalidFields }));
        }

        var result = await _studentService.GetStudentsAsync(search, sort, page, size);
        var students = result.Items.Select(MapToResponseModel).ToList();

        var response = new PagedResponse<object>(
            success: true,
            message: "Students retrieved successfully.",
            data: FieldSelectionHelper.Apply(students, selectedFields, FieldSelectors),
            pagination: PaginationHelper.Create(result));

        return Ok(response);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseModel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<StudentResponseModel>>> GetStudentById(int id)
    {
        var student = await _studentService.GetStudentByIdAsync(id);
        if (student is null)
        {
            return NotFound(new ApiResponse<object>(
                success: false,
                message: $"Student with id {id} was not found."));
        }

        return Ok(new ApiResponse<StudentResponseModel>(
            success: true,
            message: "Student retrieved successfully.",
            data: MapToResponseModel(student)));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<StudentResponseModel>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse<StudentResponseModel>>> CreateStudent(
        [FromBody] StudentRequestModel request)
    {
        var createdStudent = await _studentService.CreateStudentAsync(new StudentBusinessModel
        {
            FullName = request.FullName,
            Email = request.Email,
            DateOfBirth = request.DateOfBirth.ToDateTime(TimeOnly.MinValue)
        });

        return CreatedAtAction(
            nameof(GetStudentById),
            new { id = createdStudent.StudentId },
            new ApiResponse<StudentResponseModel>(
                success: true,
                message: "Student created successfully.",
                data: MapToResponseModel(createdStudent)));
    }

    private static StudentResponseModel MapToResponseModel(StudentBusinessModel student)
    {
        return new StudentResponseModel
        {
            StudentId = student.StudentId,
            FullName = student.FullName,
            Email = student.Email,
            DateOfBirth = DateOnly.FromDateTime(student.DateOfBirth)
        };
    }
}
