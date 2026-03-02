using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentAPI.DataSimulation;
using StudentAPI.Model;
using StudentAPIDataAccessLayer;

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
        public ActionResult<IEnumerable<StudentDTO>> GetAllStudents()
        {
            //return Ok(StudentDataSimulation.StudentsList);
            List<StudentDTO> StudentsList = StudentAPIBusinessLayer.clsStudent.GetAllStudents();
            if(StudentsList.Count == 0)
            {
                return NotFound("No Student Found!");
            }
            return Ok(StudentsList);
        }

        [HttpGet("Passed", Name = "PassedStudents")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<StudentDTO>> PassedStudents()
        {
            List<StudentDTO> StudentsList = StudentAPIBusinessLayer.clsStudent.GetPassedStudents();
            if (StudentsList.Count == 0)
            {
                return NotFound("No Student Found!");
            }
            return Ok(StudentsList);
        }

        [HttpGet("Average", Name = "GetAverageGrade")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<double> GetAverageGrade()
        {
            //StudentDataSimulationcs.StudentsList.Clear();

            //var AverageGrade = StudentDataSimulation.StudentsList.Average(Student => Student.Grade);
            var AverageGrade = StudentAPIBusinessLayer.clsStudent.GetAverageGrade();
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
                return BadRequest($"Invalid id {id}");
            }

            //var Student = StudentDataSimulation.StudentsList.FirstOrDefault(Student => Student.ID == id);
            var Student = StudentAPIBusinessLayer.clsStudent.FindStudentByID(id);

            if (Student == null)
            {
                return NotFound($"Student with id {id} is Not Found");
            }

            // he we return the DTO not the enire object becase the API is stateless
            StudentDTO SDTO = Student.SDTO;
            return Ok(SDTO);
        }

        [HttpPost(Name = "AddStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<Student> AddStudent(Student newStudent)
        {
            if (newStudent == null || string.IsNullOrEmpty(newStudent.Name) || newStudent.Age <= 0 || newStudent.Grade < 0)
            {
                return BadRequest("Invalid Student data!");
            }

            newStudent.ID = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.ID) + 1 : 1;
            StudentDataSimulation.StudentsList.Add(newStudent);

            return CreatedAtRoute("GetStudentByID", new { id = newStudent.ID }, newStudent);
        }

        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteStudent(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not Accepted ID {id}");
            }

            var Student = StudentDataSimulation.StudentsList.FirstOrDefault(Student => Student.ID == id);

            if (Student == null)
            {
                return NotFound($"Student with id {id} is Not Found");
            }

            StudentDataSimulation.StudentsList.Remove(Student);
            return Ok($"Student with ID {id} has been Deleted!");
        }

        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Student> UpdateStudent(int id, Student updatedStudent)
        {
            if (updatedStudent == null || string.IsNullOrEmpty(updatedStudent.Name) || updatedStudent.Age <= 0 || updatedStudent.Grade < 0)
            {
                return BadRequest("Invalid Student data!");
            }

            var Student = StudentDataSimulation.StudentsList.FirstOrDefault(Student => Student.ID == id);
            if (Student == null)
            {
                return NotFound($"Student with id {id} is Not Found");
            }

            Student.Name = updatedStudent.Name;
            Student.Age = updatedStudent.Age;
            Student.Grade = updatedStudent.Grade;
            
            return Ok(Student);
        }
    }
}
