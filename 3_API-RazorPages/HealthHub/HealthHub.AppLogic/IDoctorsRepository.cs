using HealthHub.Domain;

namespace HealthHub.AppLogic
{
    public interface IDoctorsRepository
    {
        IEnumerable<Doctor> GetAll();
        Doctor GetById(int id);
        void Add(Doctor doctor);
        void Update(Doctor doctor);
        void Delete(int id);
        public IEnumerable<Doctor> GetDoctorsBySpecialty(int specialtyId);

    }
}