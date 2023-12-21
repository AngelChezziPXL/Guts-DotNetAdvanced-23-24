using Guts.Client.Core;
using HealthHub.AppLogic;
using HealthHub.Controllers.api;
using HealthHub.Domain;
using HealthHub.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Reflection;

namespace HealthHub.Tests
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "HealthHub",
@"HealthHub\Controllers\api\DoctorsController.cs")]

    public class DoctorsControllerTests
    {
        private Mock<IDoctorsRepository> _doctorsRepositoryMock = null!;
        private DoctorsController _doctorsController = null!;

        [SetUp]
        public void SetUp() 
        {
            _doctorsRepositoryMock = new Mock<IDoctorsRepository>();
            _doctorsController = new DoctorsController(_doctorsRepositoryMock.Object);
        }

        [MonitoredTest("DoctorsController Tests - Get method should return an Ok result with a list of doctors")]
        public void _01_Get_ReturnsOkWithListOfDoctors()
        {
            // Arrange

            var doctors = new List<Doctor>
            {
            new Doctor { Id = 1, LastName = "Dr. Smith" },
            new Doctor { Id = 2, LastName = "Dr. Johnson" }
            };

            _doctorsRepositoryMock.Setup(repo => repo.GetAll()).Returns(doctors);

            // Act
            var result = _doctorsController.Get() as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The Get method should return a OkObjectResult.");
            Assert.That(result!.StatusCode, Is.EqualTo(200), "The Get method should return a StatusCode 200.");

            IEnumerable<Doctor>? resultDoctors = result.Value as IEnumerable<Doctor>;
            Assert.That(resultDoctors, Is.Not.Null, "The Get method should return a list of Doctors") ;
            Assert.That(resultDoctors.Count(), Is.EqualTo(doctors.Count()), "The Get method should return a list of doctors with the correct amount of doctors.");
            Assert.That(resultDoctors, Is.EquivalentTo(doctors), "The Get method should return the correct list of doctors.");
            _doctorsRepositoryMock.Verify(repo => repo.GetAll(), Times.Once,"The GetAll repository method should be called once.");
        }

        [MonitoredTest("DoctorsController Tests - Get with a Valid Id should return the doctor with the given Id")]
        public void _02_GetWithExistingId_ReturnsOkWithDoctor()
        {
            // Arrange         
            var doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(doctorId)).Returns(existingDoctor);

            // Act
            var result = _doctorsController.Get(doctorId) as OkObjectResult;

            // Assert
            Assert.That(result,Is.Not.Null, "The Get method (with existing Id), should return an OkObjectResult.");
            Assert.That(result.StatusCode, Is.EqualTo(200), "The Get method (with existing Id), should return a statuscode 200.");

            var resultDoctor = result.Value as Doctor;
            Assert.That(resultDoctor, Is.Not.Null, "The Get method (with existing Id), should return a Doctor object in the response body.");
            Assert.That(resultDoctor, Is.SameAs(existingDoctor),"The Get method (with existing Id), should return the correct Doctor.");
            Assert.That(resultDoctor.Id, Is.EqualTo(existingDoctor.Id),"The Get method (with existing Id), should return a doctor with the correct Id.");
            Assert.That(resultDoctor.LastName, Is.EqualTo(existingDoctor.LastName),"The Get method (with existing Id), should return a doctor with the correct Id.");
            _doctorsRepositoryMock.Verify(repo => repo.GetById(doctorId), Times.Once ,"The Get method (with existing Id), should call the GetById repository method once.");
        }

        [MonitoredTest("DoctorsController Tests - Get with a Invalid Id should return a NotFound result")]
        public void _03_GetWithNonExistingId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingDoctorId = 99;

            _doctorsRepositoryMock.Setup(repo => repo.GetById(nonExistingDoctorId)).Returns((Doctor)null);

            // Act
            var result = _doctorsController.Get(nonExistingDoctorId) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The Get method (with nonexisting Id) should return a NotFoundResult.");
            Assert.That(result.StatusCode, Is.EqualTo(404), "The Get method (with nonexisting Id) should return a NotFoundResult.");
            _doctorsRepositoryMock.Verify(repo => repo.GetById(nonExistingDoctorId), Times.Once, "The Get method (with nonexisting Id) should call the ");
        }

        [MonitoredTest("DoctorsController Tests - Post with a valid ModelState should add a new doctor and return a CreatedAtAction result")]
        public void _04_Post_ValidModelState_AddsNewDoctor_ReturnsCreatedAtAction()
        {
            // Arrange
            var newDoctor = new Doctor { Id = 3, LastName = "Dr. Anderson" };

            // Act
            var result = _doctorsController.Post(newDoctor) as CreatedAtActionResult;

            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(201), "The Post method (with valid modelstate), should return a 201 statuscode");

            var createdDoctor = result.Value as Doctor;
            Assert.That(createdDoctor, Is.Not.Null, "The Post method (with valid ModelState) should return have the created doctor in the response body.");
            Assert.That(newDoctor.Id, Is.EqualTo(createdDoctor.Id), "The Post method (with valid ModelState) should have a doctor with the correct Id in the response body");
            Assert.That(newDoctor.LastName, Is.EqualTo(createdDoctor.LastName), "The Post method (with valid ModelState) should have a doctor with the correct Name in the response body");
            Assert.That(newDoctor, Is.SameAs(createdDoctor), "The Post method (with valid ModelState) should have a doctor in the response body");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.Add(newDoctor), Times.Once, "The Post method (with valid ModelState) should call the Add repository method once.");
        }

        [MonitoredTest("DoctorsController Tests - Post with an invalid ModelState should return a BadRequest result")]
        public void _05_Post_InvalidModelState_ReturnsBadRequest()
        {
            // Arrange
            _doctorsController.ModelState.AddModelError("LastName", "LastName is required");

            var invalidDoctor = new Doctor { Id = 3 }; 

            // Act
            var result = _doctorsController.Post(invalidDoctor) as BadRequestObjectResult;

            // Assert
            Assert.That(result,Is.Not.Null, "The Post method (with an invalid modelstate), should return a BaddRequestObjectResult");
            Assert.That(result.StatusCode, Is.EqualTo(400), "The Post method (with an invalid modelstate), should return a statuscode 400");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.Add(It.IsAny<Doctor>()), Times.Never, "The Post method (with an invalid modelstate), should not call the Add repository method");
        }

        [MonitoredTest("DoctorsController Tests - Put should update a doctor if the doctor exists")]
        public void _06_Put_UpdateExistingDoctor_ReturnsNoContent()
        {
            // Arrange
            var doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };
            var updatedDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith Jr." };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(existingDoctor.Id)).Returns(existingDoctor);
            
            // Act
            var result = _doctorsController.Put(existingDoctor.Id, updatedDoctor) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The Put method should return an OkObjectResult");
            Assert.That(result.StatusCode, Is.EqualTo(200), "The Put method should return a 200 StatusCode.");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.GetById(updatedDoctor.Id), Times.Once, "The Put method should call the GetById repository method once.");
            _doctorsRepositoryMock.Verify(repo => repo.Update(existingDoctor), Times.Once, "The Put method should call the Update repository method once.");
        }

        [MonitoredTest("DoctorsController Tests - Delete should remove the doctor if the doctor exists and return a NoContent result.")]
        public void _07_Delete_ExistingDoctor_ReturnsNoContentResult()
        {
            // Arrange
            var doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(doctorId)).Returns(existingDoctor);

            // Act
            var result = _doctorsController.Delete(doctorId) as NoContentResult;

            // Assert
            Assert.That(result,Is.Not.Null, "The Delete method should return a NoContentResult.");
            Assert.That(result.StatusCode, Is.EqualTo(204), "The Delete method should return a 204 statuscode.");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.Delete(doctorId), Times.Once, "The Delete method should call the Delete repository method once.");
        }

        [MonitoredTest("DoctorsController Tests - Delete return a NotFoundResult if the doctor doesn't exist.")]
        public void _08_Delete_NonExistingDoctor_ReturnsNotFoundResult()
        {
            // Arrange
            var doctorId = 1;
            var existingDoctor = new Doctor { Id = doctorId, LastName = "Dr. Smith" };

            _doctorsRepositoryMock.Setup(repo => repo.GetById(doctorId)).Returns((Doctor)null);

            // Act
            var result = _doctorsController.Delete(doctorId) as NotFoundResult;

            // Assert
            Assert.That(result, Is.Not.Null,"The Delete method with an invalid doctorId should return a NotFoundResult.");
            Assert.That(result.StatusCode, Is.EqualTo(404), "The Delete method with an invalid doctorId should return a 404 statuscode.");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.Delete(doctorId), Times.Never, "The Delete method with an invalid doctorId, should not call the repository delete method.");
        }

        [MonitoredTest("DoctorsController Tests - GetDoctorsBySpecialty should return an ok result with the doctors of the given specialty.")]
        public void _09_GetDoctorsBySpecialty_ReturnsOkWithListOfDoctors()
        {
            // Arrange
            var doctors = new List<Doctor>
            {
            new Doctor { Id = 1, LastName = "Dr. Smith", SpecialtyId = 1 },
            new Doctor { Id = 2, LastName = "Dr. Johnson", SpecialtyId = 1 }
            };

            _doctorsRepositoryMock.Setup(repo => repo.GetDoctorsBySpecialty(1)).Returns(doctors);

            // Act
            var result = _doctorsController.GetDoctorsBySpecialty(1) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null, "The GetDoctorBySpecialty method, should return an OkObjectResult. ");
            Assert.That(result.StatusCode, Is.EqualTo(200), "The GetDoctorBySpecialty method, should return a 200 StatusCode.");

            var resultDoctors = result.Value as IEnumerable<Doctor>;
            Assert.That(resultDoctors,Is.Not.Null, "The GetDoctorBySpecialty method, should return a list of Doctors."); 
            Assert.That(doctors.Count(), Is.EqualTo(resultDoctors.Count()), "The GetDoctorBySpecialty method, should return the correct amount of Doctors.");
            Assert.That(doctors, Is.SameAs(resultDoctors), "The GetDoctorBySpecialty method, should return the correct list of Doctors.");

            // Verify
            _doctorsRepositoryMock.Verify(repo => repo.GetDoctorsBySpecialty(It.IsAny<int>()), Times.Once, "The GetDoctorBySpecialty method, should call the GetDoctorsBySpecialty repository method once.");
        }

        [MonitoredTest("DoctorsController Tests - Controller should have a route attribute")]
        public void _10_DoctorsController_Should_Have_RouteAttribute()
        {
            // Arrange
            var controllerType = typeof(DoctorsController);

            // Act
            var routeAttribute = controllerType.GetCustomAttributes(typeof(RouteAttribute), true)
                                               .FirstOrDefault() as RouteAttribute;

            // Assert
            Assert.That(routeAttribute, Is.Not.Null, "DoctorsController should have a Route attrubute");
            Assert.That(routeAttribute.Template, Is.EqualTo("api/[controller]"), "DoctorsController should have a Route attrubute with the correct template");
        }


        [MonitoredTest("DoctorsController Tests - Get Action without parameter should have a HttpGetAttribute")]
        public void _11_GetActionWithoutParameter_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodWithoutParametersInfo(nameof(DoctorsController.Get));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The Get method (without parameter) of the DoctorsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.Null, "The Get method (without parameter) of the DoctorsController, shouldn't have a template");
        }

        [MonitoredTest("DoctorsController Tests - Get Action with parameter should have a HttpGetAttribute")]
        public void _12_GetActionWithParameter_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(DoctorsController.Get), typeof(int));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The Get method (with parameter) of the DoctorsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.EqualTo("{id}"), "The Get method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest("DoctorsController Tests - GetDoctorsBySpecialty should have a HttpGetAttribute")]
        public void _13_GetDoctorsBySpecialty_ShouldHaveHttpGetAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(DoctorsController.GetDoctorsBySpecialty), typeof(int));

            // Act
            var httpGetAttribute = methodInfo.GetCustomAttributes(typeof(HttpGetAttribute), true)
                                             .FirstOrDefault() as HttpGetAttribute;

            // Assert
            Assert.That(httpGetAttribute, Is.Not.Null, "The GetDoctorsBySpecialty method of the DoctorsController should have a HttpGet attrubute");
            Assert.That(httpGetAttribute.Template, Is.EqualTo("specialty/{specialtyId}"), "The Get method (with parameter) should have a HttpGet attribute with the correct template");
        }

        [MonitoredTest("DoctorsController Tests - Post Action should have a HttpPostAttribute")]
        public void _14_PostAction_ShouldHaveHttpPostAttribute()
        {
            // Arrange
            var methodInfo = GetMethodInfo(nameof(DoctorsController.Post), typeof(Doctor));

            // Act
            var httpPostAttribute = methodInfo.GetCustomAttributes(typeof(HttpPostAttribute), true)
                                             .FirstOrDefault() as HttpPostAttribute;

            // Assert
            Assert.That(httpPostAttribute, Is.Not.Null, "The Post method of the DoctorsController should have a HttpPost attrubute");
        }

        [MonitoredTest("DoctorsController Tests - Put Action should have a HttpPutAttribute")]
        public void _15_PutAction_ShouldHaveHttpPutAttribute()
        {
            // Arrange
            Type[] typesArray = { typeof(int), typeof(Doctor) };
            var methodInfo = GetMethodInfo(nameof(DoctorsController.Put), typesArray);

            // Act
            var httpPutAttribute = methodInfo.GetCustomAttributes(typeof(HttpPutAttribute), true)
                                             .FirstOrDefault() as HttpPutAttribute;

            // Assert
            Assert.That(httpPutAttribute, Is.Not.Null, "The Put method of the DoctorsController should have a HttpPost attrubute");
            Assert.That(httpPutAttribute.Template, Is.EqualTo("{id}"), "The Put method should have a HttpPut attribute with the correct template");

        }

        [MonitoredTest("DoctorsController Tests - Delete Action should have a HttpDelete Attribute")]
        public void _16_DeleteAction_ShouldHaveHttpDeleteAttribute()
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
            var controllerType = typeof(DoctorsController);
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