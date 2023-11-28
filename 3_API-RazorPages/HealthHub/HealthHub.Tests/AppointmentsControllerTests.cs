using Guts.Client.Core;
using HealthHub.AppLogic;
using HealthHub.Controllers.api;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;

namespace HealthHub.Tests
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "HealthHub",
@"HealthHub\AppointmentsController.cs")]

    public class AppointmentsControllerTests
    {
        private Mock<IAppointmentsRepository> _appointmentsRepositoryMock = null!;
        private AppointmentsController _appointmentsController = null!;

        [SetUp]
        public void SetUp()
        {
            _appointmentsRepositoryMock = new Mock<IAppointmentsRepository>();
            _appointmentsController = new AppointmentsController(_appointmentsRepositoryMock.Object);

        }

        [MonitoredTest("AppointmentsController Tests - Get method should return an Ok result with a list of appointments")]
        public void _01_Get_ReturnsOkResultWithData()
        {
            // Arrange
            var appointmentList = new List<Appointment> { new Appointment() };
            _appointmentsRepositoryMock.Setup(repo => repo.GetAll()).Returns(appointmentList);

            // Act
            var result = _appointmentsController.Get() as OkObjectResult;

            // Assert
            Assert.That(result,Is.Not.Null);
            var appointments = result.Value as IEnumerable<Appointment>;
            Assert.That(appointments, Is.Not.Null, "The Get action method should return a list of appointments.");
            Assert.That(appointments, Is.SameAs(appointmentList), "The Get action method should return the correct list of appointments.");
        }

        [MonitoredTest("AppointmentsController Tests - Get with a Valid Id should return the appointment with the given Id")]
        public void _02_Get_WithValidId_ReturnsOkResultWithAppointment()
        {
            // Arrange
            var appointmentId = 1;
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(appointmentId)).Returns(new Appointment { Id = appointmentId });

            // Act
            var result = _appointmentsController.Get(appointmentId) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The Get method with a valid Id parameter, should return a result, different from null.");
            var appointment = result.Value as Appointment;
            Assert.That(appointment, Is.Not.Null, "The Get method with a valid Id parameter, should return an appointment.");
            Assert.That(appointmentId, Is.EqualTo(appointment.Id), "The Get method with a valid Id parameter, should return the correct appointment with the given Id."); ;
        }

        [MonitoredTest("AppointmentsController Tests - Get with a Invalid Id should return a NotFound result")]
        public void _03_Get_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var appointmentId = 1;
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(appointmentId)).Returns((Appointment)null);

            // Act
            var result = _appointmentsController.Get(appointmentId);

            // Assert
            Assert.That(result, Is.InstanceOf<NotFoundResult>(), "The Get method with an invalid Id parameter, should return a NotFound result.");
        }

        [MonitoredTest("AppointmentsController Tests - Post with a valid ModelState should add a new appointment an return a CreatedAtAction result")]
        public void _04_Post_ValidModelState_AddsNewAppointment_ReturnsCreatedAtAction()
        {
            // Arrange
            var newAppointment = new Appointment { Id = 3, AppointmentDate=DateTime.Today, DoctorId=1, PatientNationalNumber=Guid.NewGuid().ToString(), Reason=Guid.NewGuid().ToString() };

            // Act
            var result = _appointmentsController.Post(newAppointment) as CreatedAtActionResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The result of the post method with a valid modelstate, should not be null.");
            Assert.That(result.StatusCode, Is.EqualTo(201), "The statuscode of the post method with a valid modelstate, should return a statuscode 201 (resource created).");

            var createdAppointment = result.Value as Appointment;
            Assert.That(createdAppointment, Is.Not.Null, "The created appointment by the post method with a valid modelstate, should not be null.");
            Assert.That(newAppointment.Id, Is.EqualTo(createdAppointment.Id), "The Id of the new appointment created by the post method with a valid modelstate, should have the correct value.");
            Assert.That(newAppointment.AppointmentDate, Is.EqualTo(createdAppointment.AppointmentDate), "The Appointment Date of the new appointment created by the post method with a valid modelstate, should have the correct value.");
            Assert.That(newAppointment.PatientNationalNumber, Is.EqualTo(createdAppointment.PatientNationalNumber), "PatientNationalNumber of the new appointment created by the post method with a valid modelstate, should have the correct value.");
            Assert.That(newAppointment.DoctorId, Is.EqualTo(createdAppointment.DoctorId), "The DoctorId of the new appointment created by the post method with a valid modelstate, should have the correct value.");
            Assert.That(newAppointment.Reason, Is.EqualTo(createdAppointment.Reason), "The Reason of the new appointment created by the post method with a valid modelstate, should have the correct value.");
            Assert.That(newAppointment, Is.SameAs(createdAppointment), "The correct appointment object is created by the Post method.");

            // Verify
            _appointmentsRepositoryMock.Verify(repo => repo.Add(newAppointment), Times.Once,"The Add repository method should be called Once.");
        }

        [MonitoredTest("AppointmentsController Tests - Post with an invalid ModelState should return a BadRequest result")]
        public void _05_Post_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _appointmentsController.ModelState.AddModelError("AppointmentDate", "AppointmentDate is required");

            var invalidAppointment = new Appointment { Id = 3 };

            // Act
            var result = _appointmentsController.Post(invalidAppointment) as BadRequestObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null,"The Post method with an invalid modelstate, should return a BadRequestObjectResult.");
            Assert.That(result.StatusCode, Is.EqualTo(400), "The Post method with an invalid modelstate, should return a 400 statuscode.");

            // Verify
            _appointmentsRepositoryMock.Verify(repo => repo.Add(It.IsAny<Appointment>()), Times.Never, "The Add repository method should not be called.");
        }

        [MonitoredTest("AppointmentsController Tests - Put should update an appointment if the appointment exists")]
        public void _06_Put_UpdateExistingAppointment_ReturnsNoContent()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment { Id = appointmentId, AppointmentDate = DateTime.Today };
            var updatedAppointment = new Appointment { Id = appointmentId, AppointmentDate = DateTime.Today.AddDays(10)};

            _appointmentsRepositoryMock.Setup(repo => repo.GetById(existingAppointment.Id)).Returns(existingAppointment);

            // Act
            var result = _appointmentsController.Put(existingAppointment.Id, updatedAppointment) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The Put method should return an OkObjectResult.");
            Assert.That(result.StatusCode, Is.EqualTo(200), "The Put method should return a statuscode");

            // Verify
            _appointmentsRepositoryMock.Verify(repo => repo.GetById(updatedAppointment.Id), Times.Once, "The GetById repository method should be called once.");
            _appointmentsRepositoryMock.Verify(repo => repo.Update(existingAppointment), Times.Once, "The Update repository method should be called once.");
        }

        [MonitoredTest("AppointmentsController Tests - Delete should return a NotFound result if the appointment doesn't exist")]
        public void _07_Delete_WithInvalidId_ReturnsNotFoundResult()
        {
            // Arrange
            var invalidAppointment = new Appointment() { Id = 999 };
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(invalidAppointment.Id)).Returns((Appointment)null);

            // Act
            var result = _appointmentsController.Delete(invalidAppointment.Id) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The delete method with an invalid Id should return a NotFoundResult. ");
            Assert.That(result.StatusCode, Is.EqualTo(404), "The delete method with an invalid Id should return statuscode 404");
            _appointmentsRepositoryMock.Verify(repo => repo.Delete(invalidAppointment), Times.Never, "The Delete method with an invalid Id should not call the Delete repository method.");
        }

        [MonitoredTest("AppointmentsController Tests - Delete should remove the appointment if it exists")]
        public void _08_Delete_WithValidId_ReturnsNoContentResult()
        {
            // Arrange
            var appointmentId = 1;
            var existingAppointment = new Appointment { Id = appointmentId, AppointmentDate = DateTime.Today, DoctorId = 1, PatientNationalNumber = Guid.NewGuid().ToString(), Reason = Guid.NewGuid().ToString() };
            _appointmentsRepositoryMock.Setup(repo => repo.GetById(existingAppointment.Id)).Returns(existingAppointment);

            // Act
            var result = _appointmentsController.Delete(existingAppointment.Id) as NoContentResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The delete method with valid Id, should return a NoContentResult.");
            Assert.That(result.StatusCode, Is.EqualTo(204), "The delete method with valid Id, should return statuscode 204.");
            _appointmentsRepositoryMock.Verify(repo => repo.Delete(existingAppointment), Times.Once, "The delete method with valid Id, should call the repository delete method once.");
        }

    }
}
