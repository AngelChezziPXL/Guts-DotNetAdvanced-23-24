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
        public void _05_Add_ShouldAddaDoctor()
        {
            int doctorId = Random.Shared.Next(100, int.MaxValue);
            var newDoctor = new Doctor()
            {
                Id = doctorId,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Phone = Guid.NewGuid().ToString(),
                SpecialtyId = 1
            };
            using (var actContext = CreateDbContext(true))
            {
                var repository = new DoctorsRepository(actContext);
                repository.Add(newDoctor);
            }

            using (var assertContext = CreateDbContext(true))
            {
                Doctor? addedDoctor = assertContext.Doctors.FirstOrDefault(d => d.Id == doctorId);
                Assert.That(addedDoctor, Is.Not.Null, "The doctor is not added correctly in the database");
            }
        }

        [MonitoredTest("DoctorsRepository - Update - Should update a Doctor")]
        public void _06_Update_ShouldUpdateaDoctor()
        {
            //Arrange
            int doctorId = Random.Shared.Next(100, int.MaxValue);
            int originalNumberOfDoctors = 0;
            var doctor = new Doctor()
            {
                Id = doctorId,
                FirstName = Guid.NewGuid().ToString(),
                LastName = Guid.NewGuid().ToString(),
                Email = Guid.NewGuid().ToString(),
                Phone = Guid.NewGuid().ToString(),
                SpecialtyId = 1
            };
            using (var arrangeContext = CreateDbContext(true))
            {
                arrangeContext.Doctors.Add(doctor);
                arrangeContext.SaveChanges();
                originalNumberOfDoctors = arrangeContext.Doctors.Count();
            }

            //Act
            string updatedFirstName = Guid.NewGuid().ToString();
            doctor.FirstName = updatedFirstName;

            using (var actContext = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(actContext);
                repository.Update(doctor);
            }

            //Assert
            using (var assertContext = CreateDbContext(false))
            {
                Doctor? updatedDoctor = assertContext.Set<Doctor>().FirstOrDefault(d => d.Id == doctorId);
                Assert.IsNotNull(updatedDoctor);
                int numberOfDoctors = assertContext.Doctors.Count();
                Assert.That(numberOfDoctors, Is.EqualTo(originalNumberOfDoctors),
                    "A doctor was added or deleted instead of updated");
                Assert.That(updatedDoctor!.FirstName, Is.EqualTo(updatedFirstName), "The doctor is not updated correctly");
            }
        }

        [MonitoredTest("DoctorsRepository - Delete - Should delete a Doctor")]
        public void _07_Delete_ShouldDeleteaDoctor()
        {
            Doctor doctorToDelete = null!;
            using (var actContext = CreateDbContext(true))
            {
                IDoctorsRepository repository = new DoctorsRepository(actContext);

                doctorToDelete = actContext.Doctors.First();
                repository.Delete(doctorToDelete.Id);
            }

            using (var assertContext = CreateDbContext(true))
            {
                Doctor? deletedDoctor = assertContext.Doctors.FirstOrDefault(d => d.Id == doctorToDelete.Id);
                Assert.That(deletedDoctor, Is.Null, "The doctor is not deleted in the database.");
            }
        }
    }
}
