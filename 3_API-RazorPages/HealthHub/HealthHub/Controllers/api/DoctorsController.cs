using HealthHub.AppLogic;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace HealthHub.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        public DoctorsController(IDoctorsRepository doctorRepository)
        {
        }

        public IActionResult Get()
        {
            throw new NotImplementedException();    
        }
        
        public IActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IActionResult Post([FromBody] Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public IActionResult Put(int id, [FromBody] Doctor updatedDoctor)
        {
            throw new NotImplementedException();
        }

        public IActionResult Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IActionResult GetDoctorsBySpecialty(int specialtyId)
        {
            throw new NotImplementedException();
        }
    }
}
