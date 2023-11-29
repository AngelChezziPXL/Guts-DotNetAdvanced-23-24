using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using ContactManager.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Reflection;
using ContactManager.Pages.Companies;
using Guts.Client.Core;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
    @"ContactManager\Pages\Companies\AddCompany.cshtml.cs")]

    public class AddCompanyPageModelTests
    {
        private Mock<ICompanyRepository> _mockRepository = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ICompanyRepository>();

        }

        [MonitoredTest("AddCompanyModel Tests - Should inherit from PageModel")]
        public void _01_AddCompanyModel_ImplementsPageModel()
        {
            // Act
            var addCompanyModel = new AddCompanyModel(_mockRepository.Object);

            // Assert
            Assert.That(addCompanyModel, Is.InstanceOf<PageModel>(),"The class should inherit from the PageModel class.");
        }
        
        [MonitoredTest("AddCompanyModel Tests - Company property has a BindProperty attribute")]
        public void _02_CompanyProperty_HasBindPropertyAttribute()
        {
            // Arrange
            var companyProperty = typeof(AddCompanyModel).GetProperty("Company");

            Assert.That(companyProperty, Is.Not.Null, "The AddCompanyModel class musth have a Company property");
            // Act
            var hasBindPropertyAttribute = companyProperty.GetCustomAttributes(typeof(BindPropertyAttribute), false).Any();

            // Assert
            Assert.That(hasBindPropertyAttribute, Is.True, "The Company property should have the [BindProperty] attribute.");
        }

        [MonitoredTest("AddCompanyModel Tests - OnPostWithValidModel should add the Company an Redirect to the Index Page")]
        public void _03_OnPost_WithValidModel_CallsAddCompanyAndRedirects()
        {
            // Arrange
            var company = new Company { Name = Guid.NewGuid().ToString() };
            var addCompanyModel = new AddCompanyModel(_mockRepository.Object);
            addCompanyModel.Company = company;

            // Act
            IActionResult result = addCompanyModel.OnPost();

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToPageResult>(), "The OnPost method should return a redirectToPage result.");
            var redirectToPageResult = (RedirectToPageResult)result;
            Assert.That(redirectToPageResult.PageName, Is.EqualTo("/Index"), "The OnPost method should redirect to the Index Page.");

            _mockRepository.Verify(repo => repo.AddCompany(company), Times.Once, "The AddCompany repository method should be called once.");
        }

        [MonitoredTest("AddCompanyModel Tests - OnPostWithInValidModel should not add the Company an Redirect to the Index Page")]
        public void _04_OnPost_WithInvalidModel_DoesNotCallAddCompanyAndRedirectsToIndex()
        {
            // Arrange
            var company = new Company();
            var addCompanyModel = new AddCompanyModel(_mockRepository.Object);
            addCompanyModel.Company = company;
            addCompanyModel.ModelState.AddModelError("Company.Name", "The Name field is required.");

            // Act
            IActionResult result = addCompanyModel.OnPost();

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToPageResult>(), "The Post action method with an invalid Model should return a redirectToPage result. ");
            var redirectToPageResult = (RedirectToPageResult)result;
            Assert.That(redirectToPageResult.PageName, Is.EqualTo("/Index"), "The OnPost method should return a redirectToPage result");

            _mockRepository.Verify(repo => repo.AddCompany(It.IsAny<Company>()), Times.Never, "The repository method AddCompany has to be called 0 times.");
        }
    }
}
