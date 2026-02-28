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
        [HttpGet]
        public ActionResult<IEnumerable<Student>> GetAllStudents()
        {
            return Ok(StudentDataSimulationcs.StudentsList);
        }
    }
}
