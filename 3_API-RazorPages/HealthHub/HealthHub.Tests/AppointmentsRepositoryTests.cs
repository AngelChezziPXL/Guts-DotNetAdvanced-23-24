using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using HealthHub.AppLogic;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using System;

namespace HealthHub.Tests
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
@"HealthHub.Infrastructure\AppointmentsRepository.cs")]

    internal class AppointmentsRepositoryTests : DataBaseTests
    {
        [MonitoredTest("AppointmentsRepository - Interface should not have been changed")]
        public void _01_ShouldNotHaveChangedContracts()
        {
            var filePath = @"HealthHub.AppLogic\IAppointmentsRepository.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("6B-BB-19-72-45-A2-EB-63-CA-89-3E-04-F4-CE-05-56"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("AppointmentsRepository - should implement IAppointmentsRepository")]
        public void _02_ShouldImplementIAppointmentsRepository()
        {
            Type appointmentsRepositoryType = typeof(AppointmentsRepository);
            Assert.That(typeof(AppointmentsRepository).IsAssignableFrom(appointmentsRepositoryType), Is.True,"The AppointmentRepository class should implement the IAppointmentRepository interface.");
        }

        [MonitoredTest("AppointmentsRepository - GetAll  - should return all Appointments")]
        public void _03_GetAll_ShouldReturnAllAppointments()
        {
            using (var context = CreateDbContext(true))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(context);
                var guid = Guid.NewGuid().ToString();
                var appointment1 = new Appointment() { Id = 1, DoctorId = 1, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = guid, Reason = guid };
                context.Appointments.Add(appointment1);

                var guid2 = Guid.NewGuid().ToString();
                var appointment2 = new Appointment() { Id = 2, DoctorId = 1, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = guid2, Reason = guid2 };
                context.Appointments.Add(appointment2);

                context.SaveChanges();

                IReadOnlyList<Appointment> appointments = (IReadOnlyList<Appointment>)repository.GetAll();

                Assert.That(appointments, Has.Count.EqualTo(3), "The appointment list has to contain 3 appointments.");
                Assert.IsTrue(appointments.Any(a => a.PatientNationalNumber == guid), "The appointment has the wrong PatientNationalNumber.");
                Assert.IsTrue(appointments.Any(a => a.PatientNationalNumber == guid2), "The appointment has the wrong PatientNationalNumber.");
                Assert.IsTrue(appointments.Any(a => a.PatientNationalNumber == "111111"), "The appointment has the wrong PatientNationalNumber.");

            }
        }

        [MonitoredTest("AppointmentsRepository - GetById  - should return an Appointments")]
        public void _04_GetById_ShouldReturnTheCorrectAppointment()
        {
            using (var context = CreateDbContext(true))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(context);

                var guid = Guid.NewGuid().ToString();
                var appointment = new Appointment() { Id = 3, DoctorId = 2, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = guid, Reason = guid };
                context.Appointments.Add(appointment);
                context.SaveChanges();
                Appointment returnedAppointment = repository.GetById(3);

                Assert.That(returnedAppointment.AppointmentDate, Is.EqualTo(DateTime.Today.AddDays(-10)), "The appointment list contains a appointment for today.");
                Assert.That(returnedAppointment.Reason, Is.EqualTo(guid), "The appointment has the wrong Reason.");

            }
        }

        [MonitoredTest("AppointmentsRepository - GetAppointmentsForDoctor  - should return all Appointments for Doctor")]
        public void _05_GetForDoctor_ShouldReturnAllAppointmentsForDoctor()
        {
            using (var context = CreateDbContext(true))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(context);

                var guid = Guid.NewGuid().ToString();
                var appointment1 = new Appointment() { Id = 1024, DoctorId = 3, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = guid, Reason = guid };
                context.Appointments.Add(appointment1);

                var guid2 = Guid.NewGuid().ToString();
                var appointment2 = new Appointment() { Id = 1025, DoctorId = 3, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = guid2, Reason = guid2 };
                context.Appointments.Add(appointment2);

                context.SaveChanges();
                IEnumerable<Appointment> returnedAppointments = repository.GetAppointmentsForDoctor(3);
                Assert.That(returnedAppointments.Count, Is.EqualTo(2), "The appointment list should contain 2 appointments.");
            }
        }

        [MonitoredTest("AppointmentsRepository - GetAppointmentsForDoctor  - should return all Appointments for Doctor")]
        public void _06_GetForPatient_ShouldReturnAllAppointmentsForPatient()
        {
            using (var context = CreateDbContext(true))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(context);

                var guid = Guid.NewGuid().ToString();
                var appointment1 = new Appointment() { Id = 5024, DoctorId = 4, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = guid, Reason = guid };
                context.Appointments.Add(appointment1);

                var guid2 = Guid.NewGuid().ToString();
                var appointment2 = new Appointment() { Id = 5025, DoctorId = 4, AppointmentDate = DateTime.Today.AddDays(-10), PatientNationalNumber = guid, Reason = guid2 };
                context.Appointments.Add(appointment2);

                context.SaveChanges();

                IEnumerable<Appointment> returnedAppointments = repository.GetAppointmentsForPatient(guid);

                Assert.That(returnedAppointments.Count, Is.EqualTo(2), "The appointment list should contain 2 appointments.");
            }
        }

        [MonitoredTest("AppointmentsRepository - GetUpcomingAppointments  - should return all upcoming Appointments")]
        public void _07_GetUpcoming_ShouldReturnAllUpcomingAppointments()
        {
            using (var context = CreateDbContext(true))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(context);

                var guid = Guid.NewGuid().ToString();
                var appointment1 = new Appointment() { Id = 2024, DoctorId = 5, AppointmentDate = DateTime.Today.AddDays(10), PatientNationalNumber = guid, Reason = guid };
                context.Appointments.Add(appointment1);

                var guid2 = Guid.NewGuid().ToString();
                var appointment2 = new Appointment() { Id = 2025, DoctorId = 5, AppointmentDate = DateTime.Today.AddDays(10), PatientNationalNumber = guid, Reason = guid2 };
                context.Appointments.Add(appointment2);

                context.SaveChanges();
                IEnumerable<Appointment> returnedAppointments = repository.GetAppointmentsForPatient(guid);

                Assert.That(returnedAppointments.Count, Is.EqualTo(2), "The appointment list should contain 2 appointments.");
            }
        }

        [MonitoredTest("AppointmentsRepository - Add  - Should add an Appointment")]
        public void _08_Add_ShouldAddAnAppointment()
        {
            using (var context = CreateDbContext(true))
            {
                var repository = new AppointmentsRepository(context);

                var guid = Guid.NewGuid().ToString();
                var appointment = new Appointment() { Id = 3024, AppointmentDate = DateTime.Today, DoctorId = 6, PatientNationalNumber = guid, Reason = guid };
                repository.Add(appointment);
                Assert.That(context.Appointments, Does.Contain(appointment), "The appointment list should contain the new Appointment.");
            }
        }


        [MonitoredTest("AppointmentsRepository - Update - Should update an Appointment")]
        public void _09_Update_ShouldUpdateaDoctor()
        {
            using (var context = CreateDbContext(true))
            {
                string guid = Guid.NewGuid().ToString();
                IAppointmentsRepository repository = new AppointmentsRepository(context);
                var appointment = new Appointment() { Id = 4024, AppointmentDate = DateTime.Today, DoctorId = 7, PatientNationalNumber = guid, Reason = guid };
                context.Add(appointment);
                var guid2 = Guid.NewGuid().ToString();
                appointment.PatientNationalNumber = guid2;
                repository.Update(appointment);
                Assert.That(context.Appointments, Does.Contain(appointment), "The appointment list should contain the updated appointment.");
            }
        }

        [MonitoredTest("AppointmentsRepository - Delete - Should delete an Appointment")]
        public void _10_Delete_ShouldDeleteAnAppointment()
        {
            using (var context = CreateDbContext(true))
            {
                IAppointmentsRepository repository = new AppointmentsRepository(context);

                var appointmentToDelete = context.Appointments.FirstOrDefault();
                repository.Delete(appointmentToDelete);
                Assert.That(context.Appointments, Does.Not.Contain(appointmentToDelete), "The appointment list shouldn't contain the deleted appointments.");
            }
        }
        
    }
}
