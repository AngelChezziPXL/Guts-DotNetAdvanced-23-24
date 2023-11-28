using ContactManager.AppLogic.Contracts;
using ContactManager.Domain;
using ContactManager.Pages;
using Guts.Client.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Reflection;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
    @"ContactManager\Pages\Index.cshtml.cs")]

    public class IndexPageModelTests
    {
        private Mock<IContactRepository> _mockRepository = null!;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IContactRepository>();

        }

        [MonitoredTest("Index Model Tests - The Index Model has to inherit from PageModel")]
        public void _01_IndexModel_InheritsPageModel()
        {  
            // Act
            var indexModel = new IndexModel(_mockRepository.Object);

            // Assert
            Assert.That(indexModel, Is.InstanceOf<PageModel>(), "The indexModel class has to inherit from PageModel"); ;
        }

        [MonitoredTest("Index Model Tests - Contacts property has a BindProperty attribute")]
        public void _02_ContactsProperty_HasBindPropertyAttribute()
        {
            // Arrange
            var contactsProperty = typeof(IndexModel).GetProperty("Contacts");

            // Act
            var hasBindPropertyAttribute = contactsProperty.GetCustomAttributes(typeof(BindPropertyAttribute), false).Any();

            // Assert
            Assert.That(hasBindPropertyAttribute, Is.True, "The Contacts property should have the [BindProperty] attribute.");
        }

        [MonitoredTest("Index Model Tests - OnGet populates the Contacts property with repository data")]
        public void _03_OnGet_PopulatesContacts_WithRepositoryData()
        {
            // Arrange
            var mockRepository = new Mock<IContactRepository>();
            var expectedContacts = new List<Contact>
            {
                new Contact { Id = 1, Name = Guid.NewGuid().ToString() },
                new Contact { Id = 2, Name = Guid.NewGuid().ToString() },
            };

            mockRepository.Setup(repo => repo.GetAllContacts()).Returns(expectedContacts);

            var indexModel = new IndexModel(mockRepository.Object);

            // Act
            indexModel.OnGet();

            // Assert
            Assert.That(indexModel.Contacts, Is.Not.Null, "The Contacts property has to be populated by the Repository method");
            Assert.That(expectedContacts, Is.EquivalentTo(indexModel.Contacts), "The Contacts property doesn't contain all the contacts");
            mockRepository.Verify(repo => repo.GetAllContacts(), Times.Once, "The GetAllContacts method of the ContactsRepository has to be called just once.");
        }
    }
}
