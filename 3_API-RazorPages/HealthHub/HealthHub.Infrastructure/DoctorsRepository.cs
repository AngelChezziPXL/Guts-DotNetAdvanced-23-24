using HealthHub.AppLogic;
using HealthHub.Domain;
using Microsoft.EntityFrameworkCore;

namespace HealthHub.Infrastructure
{
    internal class DoctorsRepository : IDoctorsRepository
    {
        public DoctorsRepository(HealthHubDbContext context)
        {                
        }
        public IEnumerable<Doctor> GetAll()
        {
            throw new NotImplementedException();
        }

        public Doctor GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Doctor> GetDoctorsBySpecialty(int specialtyId)
        {
            throw new NotImplementedException();
        }

        public void Add(Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public void Update(Doctor doctor)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}
