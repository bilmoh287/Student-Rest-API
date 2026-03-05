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
        public ActionResult<StudentDTO> GetStudentByID(int id)
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
        public ActionResult<StudentDTO> AddStudent(StudentDTO newStudent)
        {
            if (newStudent == null || string.IsNullOrEmpty(newStudent.Name) || newStudent.Age <= 0 || newStudent.Grade < 0)
            {
                return BadRequest("Invalid Student data!");
            }

            //newStudent.ID = StudentDataSimulation.StudentsList.Count > 0 ? StudentDataSimulation.StudentsList.Max(s => s.ID) + 1 : 1;
            //StudentDataSimulation.StudentsList.Add(newStudent);

            StudentAPIBusinessLayer.clsStudent student = new StudentAPIBusinessLayer.clsStudent(newStudent);
            student.Save();
            newStudent.Id = student.ID;
            return CreatedAtRoute("GetStudentByID", new { id = newStudent.Id }, newStudent);
        }

        [HttpDelete("{id}", Name = "DeleteStudent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult DeleteStudent(int id)
        {
            if (id < 1)
            {
                return BadRequest($"Not Accepted ID {id}");
            }

            //var Student = StudentDataSimulation.StudentsList.FirstOrDefault(Student => Student.ID == id);
            StudentAPIBusinessLayer.clsStudent Student = StudentAPIBusinessLayer.clsStudent.FindStudentByID(id);

            if (Student == null)
            {
                return NotFound($"Student with id {id} is Not Found");
            }

            if(StudentAPIBusinessLayer.clsStudent.DeleteStudent(id))
                return Ok($"Student with id {id} has been deleted!");
            else
                return StatusCode(500, new { message = "Error Deleting Student" });
        }

        [HttpPut("{id}", Name = "UpdateStudent")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<StudentDTO> UpdateStudent(int id, StudentDTO updatedStudent)
        {
            if (updatedStudent == null || string.IsNullOrEmpty(updatedStudent.Name) || updatedStudent.Age <= 0 || updatedStudent.Grade < 0)
            {
                return BadRequest("Invalid Student data!");
            }

            //var Student = StudentDataSimulation.StudentsList.FirstOrDefault(Student => Student.ID == id);
            StudentAPIBusinessLayer.clsStudent Student = StudentAPIBusinessLayer.clsStudent.FindStudentByID(id);
            if (Student == null)
            {
                return NotFound($"Student with id {id} is Not Found");
            }

            Student.Name = updatedStudent.Name;
            Student.Age = updatedStudent.Age;
            Student.Grade = updatedStudent.Grade;

            if (Student.Save())
                return Ok(Student.SDTO);
            else
                return StatusCode(500, new { message = "Error Updating Student" }); 
        }

        // Endpoint to handle image upload
        [HttpPost("Upload")]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            // Check if no file is uploaded
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file uploaded.");

            // Directory where files will be uploaded
            var uploadDirectory = @"C:\MyUploads";

            // Generate a unique filename
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Ensure the uploads directory exists, create if it doesn't
            if (!Directory.Exists(uploadDirectory))
            {
                Directory.CreateDirectory(uploadDirectory);
            }

            // Save the file to the server
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Return the file path as a response
            return Ok(new { filePath });
        }

        // Endpoint to retrieve image from the server
        [HttpGet("GetImage/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            // Directory where files are stored
            var uploadDirectory = @"C:\MyUploads";
            var filePath = Path.Combine(uploadDirectory, fileName);

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
                return NotFound("Image not found.");

            // Open the image file for reading
            var image = System.IO.File.OpenRead(filePath);
            var mimeType = GetMimeType(filePath);

            // Return the file with the correct MIME type
            return File(image, mimeType);
        }

        // Helper method to get the MIME type based on file extension
        /*
         This code defines a  method called GetMimeType that takes a file path as a parameter 
         and returns the corresponding MIME type as a string. 
         MIME types are used to indicate the nature and format of a file, 
         especially in web contexts where you need to specify the type of content you're sending, 
         like images, text, etc.

        MIME type stands for Multipurpose Internet Mail Extensions type. 
        It's a standard way to indicate the nature and format of a file or content. 
        MIME types are used to tell browsers, email clients, and 
        other software about the type of data they're handling, so they can process it correctly.
         */
        private string GetMimeType(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                _ => "application/octet-stream",
            };
        }
    }
}
