using Guts.Client.Core;
using HealthHub.AppLogic;
using HealthHub.Controllers.api;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Reflection;

namespace HealthHub.Tests
{
    //3_API-RazorPages\HealthHub\HealthHub\AppointmentsController.cs
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "HealthHub",
@"HealthHub\Controllers\api\AppointmentsController.cs")]

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
            Assert.That(result, Is.Not.Null);
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
            var newAppointment = new Appointment { Id = 3, AppointmentDate = DateTime.Today, DoctorId = 1, PatientNationalNumber = Guid.NewGuid().ToString(), Reason = Guid.NewGuid().ToString() };

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
            _appointmentsRepositoryMock.Verify(repo => repo.Add(newAppointment), Times.Once, "The Add repository method should be called Once.");
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
            Assert.That(result, Is.Not.Null, "The Post method with an invalid modelstate, should return a BadRequestObjectResult.");
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
            var updatedAppointment = new Appointment { Id = appointmentId, AppointmentDate = DateTime.Today.AddDays(10) };

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



        [MonitoredTest("AppointmentsController Tests - Controller should have Route Attribute")]
        public void _9_AppointmentsController_Should_Have_RouteAttribute()
        {
            // Arrange
            var controllerType = typeof(AppointmentsController);

            // Act
            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), true)
                                               .FirstOrDefault() as RouteAttribute;

            // Assert
            Assert.That(routeAttribute, Is.Not.Null, "AppointmentsController should have a Route attrubute");
            Assert.That(routeAttribute.Template, Is.EqualTo("api/[controller]"), "DoctorsController should have a Route attrubute with the correct template");
        }


        [MonitoredTest("AppointmentsController Tests - Get Action without parameters Should have a HttpGet Attribute")]
        public void _10_GetActionWithoutParameter_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodWithoutParametersInfo(nameof(AppointmentsController.Get));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The Get method (without parameter) of the AppointmentsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.Null, "The Get method (without parameter) of the AppointmentsController, shouldn't have a template");
        }

        [MonitoredTest("AppointmentsController Tests - Get Action with one integer parameter should have a HttpGet attribute")]
        public void _11_GetActionWithParameter_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(AppointmentsController.Get), typeof(int));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The Get method (with parameter) of the AppointmentsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.EqualTo("{id}"), "The Get method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest("AppointmentsController Tests - GetAppointmentsForDoctor method Should Have a HttpGet Attribute")]
        public void _12_GetAppointmentsForDoctor_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(AppointmentsController.GetAppointmentsForDoctor), typeof(int));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetAppointmentsForDoctor method of the AppointmentsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.EqualTo("doctor/{doctorId}"), "The Get method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest("AppointmentsController Tests - GetAppointmentsForPatient method Should Have a HttpGet Attribute")]
        public void _13_GetAppointmentsForPatient_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(AppointmentsController.GetAppointmentsForPatient), typeof(string));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetAppointmentsForPatient method of the AppointmentsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.EqualTo("patient/{patientNationalNumber}"), "The GetAppointmentsForPatient method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest("AppointmentsController Tests - GetUpcomingAppointments method Should Have a HttpGet Attribute")]
        public void _14_GetUpcomingAppointments_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(AppointmentsController.GetUpcomingAppointments), typeof(int));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetUpcomingAppointments method of the AppointmentsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.EqualTo("upcoming"), "The GetUpcomingAppointments method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest("AppointmentsController Tests - Post action method Should Have a HttpPost Attribute")]
        public void _15_PostAction_ShouldHaveHttpPostAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(AppointmentsController.Post), typeof(Appointment));

            // Act
            var httpPostAttribute = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute), true)
                                             .FirstOrDefault() as HttpPostAttribute;

            // Assert
            Assert.That(httpPostAttribute, Is.Not.Null, "The Post method of the DoctorsController should have a HttpPost attrubute");
        }

        [MonitoredTest("AppointmentsController Tests - Put action method Should Have a HttpPut Attribute")]
        public void _16_PutAction_ShouldHaveHttpPutAttribute()
        {
            // Arrange
            Type[] typesArray = { typeof(int), typeof(Appointment) };
            var methodInfo = GetMethodInfo(nameof(AppointmentsController.Put), typesArray);

            // Act
            var httpPutAttribute = methodInfo.GetCustomAttributes(typeof(HttpPutAttribute), true)
                                             .FirstOrDefault() as HttpPutAttribute;

            // Assert
            Assert.That(httpPutAttribute, Is.Not.Null, "The Put method of the appointmentsController should have a HttpPut attrubute");
            Assert.That(httpPutAttribute.Template, Is.EqualTo("{id}"), "The Put method should have a HttpPut attribute with the correct template");

        }

        [MonitoredTest("AppointmentsController Tests - Delete action method Should Have a HttpDelete Attribute")]
        public void _17_DeleteAction_ShouldHaveHttpDeleteAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(DoctorsController.Delete), typeof(int));

            // Act
            var httpDeleteAttribute = methodInfo.GetCustomAttributes(typeof(HttpDeleteAttribute), true)
                                             .FirstOrDefault() as HttpDeleteAttribute;

            // Assert
            Assert.That(httpDeleteAttribute, Is.Not.Null, "The Delete method of the DoctorsController should have a HttpPost attrubute");
            Assert.That(httpDeleteAttribute.Template, Is.EqualTo("{id}"), "The Delete method should have a HttpDelete attribute with the correct template");

        }


        private static MethodInfo GetMethodInfo(string methodName, params Type[] parameterTypes)
        {
            var controllerType = typeof(AppointmentsController);
            var methodInfo = controllerType.GetMethod(methodName, parameterTypes);

            Assert.That(methodInfo, Is.Not.Null, $"Method with name '{methodName}' and specified parameters not found in {controllerType.Name}.");

            return methodInfo;
        }

        private static MethodInfo GetMethodWithoutParametersInfo(string methodName, params Type[] parameterTypes)
        {
            var controllerType = typeof(DoctorsController);
            var methodInfo = controllerType.GetMethod(methodName, Type.EmptyTypes);

            Assert.That(methodInfo, Is.Not.Null, $"Method with name '{methodName}' and specified parameters not found in {controllerType.Name}.");

            return methodInfo;
        }
    }
}
