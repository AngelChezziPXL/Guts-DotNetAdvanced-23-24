using HealthHub.Domain;

namespace HealthHub.AppLogic
{
    public interface IAppointmentsRepository
    {
        IEnumerable<Appointment> GetAll();
        Appointment? GetById(int id);
        void Add(Appointment appointment);
        void Update(Appointment appointment);
        void Delete(Appointment appointment);
        IEnumerable<Appointment> GetAppointmentsForDoctor(int doctorId);
        IEnumerable<Appointment> GetUpcomingAppointments(int daysAhead);
        IEnumerable<Appointment> GetAppointmentsForPatient(string patientNationalNumber);
    }
}
