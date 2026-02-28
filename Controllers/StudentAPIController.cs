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
        public ActionResult<IEnumerable<Student>> GetAllStudents()
        {
            return Ok(StudentDataSimulationcs.StudentsList);
        }

        [HttpGet("Passed", Name = "PassedStudents")]
        public ActionResult<IEnumerable<Student>> PassedStudents()
        {
            var PassedSudents = StudentDataSimulationcs.StudentsList.Where(Student => Student.Grade >= 50).ToList();
            return Ok(PassedSudents);
        }
    }
}
