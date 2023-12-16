using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using ContactManager.Pages.Companies;
using Guts.Client.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;

namespace ContactManager.Tests
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
    @"ContactManager\Pages\Companies\AddCompany.cshtml.cs")]

    public class AddCompanyPageModelTests
    {
        private Mock<ICompanyRepository> _companyRepositoryMock = null!;

        [SetUp]
        public void Setup()
        {
            _companyRepositoryMock = new Mock<ICompanyRepository>();

        }

        [MonitoredTest("AddCompanyModel - Should inherit from PageModel")]
        public void _01_AddCompanyModel_ImplementsPageModel()
        {
            // Act
            var addCompanyModel = new AddCompanyModel(_companyRepositoryMock.Object);

            // Assert
            Assert.IsInstanceOf<PageModel>(addCompanyModel);
        }

        [MonitoredTest("AddCompanyModel - Company property has a BindProperty attribute")]
        public void _02_CompanyProperty_HasBindPropertyAttribute()
        {
            // Arrange
            var companyProperty = typeof(AddCompanyModel).GetProperty("Company");

            // Act
            var hasBindPropertyAttribute = companyProperty!.GetCustomAttributes(typeof(BindPropertyAttribute), false).Any();

            // Assert
            Assert.That(hasBindPropertyAttribute, Is.True, "The Company property should have the [BindProperty] attribute.");
        }

        [MonitoredTest("AddCompanyModel - Constructor should initialize an empty contact")]
        public void _03_Constructor_ShouldInitializeAnEmptyContact()
        {
            //Act
            var model = new AddCompanyModel(_companyRepositoryMock.Object);

            //Assert
            Assert.That(model.Company, Is.Not.Null);
        }

        [MonitoredTest("AddCompanyModel - OnPost - WithValidModel - Should add the Company an Redirect to the Index Page")]
        public void _04_OnPost_WithValidModel_CallsAddCompanyAndRedirects()
        {
            // Arrange
            var company = new Company { Name = Guid.NewGuid().ToString() };
            var addCompanyModel = new AddCompanyModel(_companyRepositoryMock.Object)
            {
                Company = company
            };

            // Act
            IActionResult result = addCompanyModel.OnPost();

            // Assert
            Assert.That(result, Is.InstanceOf<RedirectToPageResult>(), "The OnPost method should return a redirectToPage result");
            var redirectToPageResult = (RedirectToPageResult)result;
            Assert.That(redirectToPageResult.PageName == "/Index", Is.True, "The OnPost method should redirect to the Index Page");

            _companyRepositoryMock.Verify(repo => repo.AddCompany(company), Times.Once, "The repository is not used correctly to add the company");
        }

        [MonitoredTest("AddCompanyModel - OnPost - WithInValidModel - Should stay on the page")]
        public void _05_OnPost_WithInvalidModel_ShouldStayOnPage()
        {
            // Arrange
            var company = new Company();
            var addCompanyModel = new AddCompanyModel(_companyRepositoryMock.Object)
            {
                Company = company
            };
            addCompanyModel.ModelState.AddModelError("Company.Name", "The Name field is required.");

            // Act
            PageResult? result = addCompanyModel.OnPost() as PageResult;

            // Assert
            Assert.That(result, Is.Not.Null, "A 'PageResult' should be returned");
            Assert.That(result!.Page, Is.Null, "A 'PageResult' with 'Page' null should be returned");

            _companyRepositoryMock.Verify(repo => repo.AddCompany(It.IsAny<Company>()), Times.Never);
        }
    }
}
