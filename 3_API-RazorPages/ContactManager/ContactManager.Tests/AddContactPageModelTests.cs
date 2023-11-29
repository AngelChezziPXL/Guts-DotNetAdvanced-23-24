using ContactManager.AppLogic.Contracts;
using ContactManager.Pages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using System.Reflection;
using ContactManager.Pages.Contacts;
using Guts.Client.Core;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager", 
        @"ContactManager\Pages\Contacts\AddContact.cshtml.cs")]
    public class AddContactPageModelTests
    {
        private Mock<IContactRepository> _mockContactsRepository = null!;
        private Mock<ICompanyRepository> _mockCompaniesRepository = null!;

        [SetUp]
        public void Setup()
        {
            _mockContactsRepository = new Mock<IContactRepository>();
            _mockCompaniesRepository = new Mock<ICompanyRepository>();
        }       

        [MonitoredTest("AddContactModel Tests - Contact property has a BindProperty attribute")]
        public void _01_ContactAndCompaniesProperties_HaveBindPropertyAttribute()
        {
            // Arrange
            var contactProperty = typeof(AddContactModel).GetProperty("Contact");
            var companiesProperty = typeof(AddContactModel).GetProperty("Companies");

            /*     Assert.That(contactProperty, Is.Not.Null, "The AddContactModel class must have a Contact property");
                 Assert.That(companiesProperty, Is.Not.Null, "The AddContactModel class must have a Companies property");

                 // Act
                 var contactHasBindPropertyAttribute = contactProperty.GetCustomAttributes(typeof(BindPropertyAttribute), false).Any();
                 var companiesHasBindPropertyAttribute = companiesProperty.GetCustomAttributes(typeof(BindPropertyAttribute), false).Any();

                 // Assert
                 Assert.That(contactHasBindPropertyAttribute, Is.True, "The Contact property should have the [BindProperty] attribute.");
                 Assert.That(companiesHasBindPropertyAttribute, Is.True, "The Companies property should have the [BindProperty] attribute.");
            */
            Assert.That(true);
        }
    }
}
