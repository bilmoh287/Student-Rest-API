using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAPI.DataSimulation;
using StudentAPI.Model;

namespace StudentAPI.Controllers
{
    //[Route("api/[controller]")]
    // Sets the route for this controller to "students", based on the controller name.
    [Route("api/Students")]
    [ApiController]
    public class StudentAPIController : ControllerBase
    {
        [HttpGet("All", Name = "GetAllStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Student>> GetAllStudents()
        {
            return Ok(StudentDataSimulationcs.StudentsList);
        }

        [HttpGet("Passed", Name = "PassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Student>> PassedStudents()
        {
            var PassedSudents = StudentDataSimulationcs.StudentsList.Where(Student => Student.Grade >= 50).ToList();
            return Ok(PassedSudents);
        }

        [HttpGet("Average", Name = "GetAverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Student>> GetAverageGrade()
        {
            //StudentDataSimulationcs.StudentsList.Clear();
            if(StudentDataSimulationcs.StudentsList.Count == 0)
            {
                return NotFound("No Students Found!");
            }

            var AverageGrade = StudentDataSimulationcs.StudentsList.Average(Student => Student.Grade);
            return Ok(AverageGrade);
        }

        [HttpGet("{id}", Name = "GetStudentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<Student>> GetStudentByID(int id)
        {
            if (id < 1)
            {
                return BadRequest("");
            }

            var Student = StudentDataSimulationcs.StudentsList.FirstOrDefault(Student => Student.ID == id);

            if (Student == null)
            {
                return NotFound($"Student with id {id} is Not Found");
            }
            return Ok(Student);
        }
    }
}
