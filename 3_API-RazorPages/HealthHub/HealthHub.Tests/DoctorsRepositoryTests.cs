using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using HealthHub.AppLogic;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Moq;

namespace HealthHub.Tests
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "HealthHub",
@"HealthHub.Infrastructure\DoctorsRepository.cs")]
    internal class DoctorsRepositoryTests : DataBaseTests
    {
        [MonitoredTest("DoctorsRepository - Interface should not have been changed")]
        public void _01_ShouldNotHaveChangedContracts()
        {
            var filePath = @"HealthHub.AppLogic\IDoctorsRepository.cs";
            var fileHash = Solution.Current.GetFileHash(filePath);
            Assert.That(fileHash, Is.EqualTo("0D-B9-FC-C6-E7-10-E0-B1-8E-EF-30-19-ED-6F-BE-6A"),
                $"The file '{filePath}' has changed. " +
                "Undo your changes on the file to make this test pass.");
        }

        [MonitoredTest("DoctorsRepository - should implement IDoctorsRepository")]
        public void _02_ShouldImplementIDoctorsRepository()
        {
            Type doctorsRepositoryType = typeof(DoctorsRepository);
            Assert.That(typeof(IDoctorsRepository).IsAssignableFrom(doctorsRepositoryType), Is.True);
        }

        [MonitoredTest("DoctorsRepository - GetAll  - should return all Doctors")]
        public void _03_GetAll_ShouldReturnAllDoctors()
        {
            using (var context = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(context);
                IReadOnlyList<Doctor> doctors = (IReadOnlyList<Doctor>)repository.GetAll();

                Assert.That(doctors.Count(), Is.GreaterThanOrEqualTo(4), "The doctors list contains minimum 4 doctors. There are already 2 doctors in the seed data.");
                Assert.IsTrue(doctors.Any(d => d.LastName == "De Vos"), "There should exist a doctor with a LastName \"De Vos\".");
                Assert.IsTrue(doctors.Any(q => q.LastName == "Van Damme"), "There should exist a doctor with a LastName \"Van Damme\".");
            }
        }

        [MonitoredTest("DoctorsRepository - GetById  - should return a Doctor")]
        public void _04_GetById_ShouldReturnDoctor()
        {
            using (var context = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(context);

                Doctor doctor = repository.GetById(2);

                Assert.That(doctor.LastName, Is.EqualTo("De Vos"));
                Assert.That(doctor.FirstName, Is.EqualTo("Thomas"));
                Assert.That(doctor.SpecialtyId, Is.EqualTo(1));
            }
        }

        [MonitoredTest("DoctorsRepository - GetBySpeciality - should return Doctors of speciality")]
        public void _04_GetBySpecialty_ShouldReturnDoctorsOfSpecialty()
        {
            using (var context = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(context);

                IEnumerable<Doctor> doctorsInSpecialty = repository.GetDoctorsBySpecialty(1);

                Assert.That(doctorsInSpecialty.Count, Is.EqualTo(2), "There should be 2 doctors with specialty 2");
            }
        }

        [MonitoredTest("DoctorsRepository - Add  - Should add a Doctor")]
        public void _05_Add_ShouldAddaDoctors()
        {
            using (var context = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(context);

                var guid= Guid.NewGuid().ToString();
                var doctor = new Doctor() { Id=9999, FirstName = guid, LastName = guid, Email = guid, Phone = guid, SpecialtyId = 1 };
                repository.Add(doctor);
                Assert.That(context.Doctors, Does.Contain(doctor));
            }
        }

        [MonitoredTest("DoctorsRepository - Update - Should update a Doctor")]
        public void _06_Update_ShouldUpdateaDoctor()
        {
            using (var context = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(context);

                string guid = Guid.NewGuid().ToString();
                var doctor = new Doctor() { Id=1, FirstName = guid, LastName = guid, Email = guid, Phone = guid, SpecialtyId = 1 };
                repository.Update(doctor);
                Assert.That(context.Doctors, Does.Contain(doctor));
            }
        }

        [MonitoredTest("DoctorsRepository - Delete - Should delete a Doctor")]
        public void _07_Delete_ShouldDeleteaDoctor()
        {
            using (var context = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(context);

                var doctorToDelete = context.Doctors.Find(1);
                repository.Delete(1);
                Assert.That(context.Doctors, Does.Not.Contain(doctorToDelete));
            }
        }


    }
}
