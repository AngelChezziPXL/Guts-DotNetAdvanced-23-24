using ContactManager.Domain;
using Guts.Client.Core;
using System.Reflection;
using System.Security.AccessControl;

namespace ContactManager.Tests.Domain
{  
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
    @"ContactManager.Domain\Company.cs;ContactManager.Domain\Contact.cs")]

    internal class BasicDomainTests
    {
        [MonitoredTest("Domain - Company and Contact - should do the necessary null checks and initializations")]
        public void _01_NullChecks()
        {
            Company company = new Company();
            Assert.That(company.Name, Is.Not.Null, "The name of the company can not be null after constructing a Company.");
            Assert.That(company.Address, Is.Not.Null, "The name of the company can not be null after construction of a Company.");
            Assert.That(company.Zip, Is.Not.Null, "The zip of the company can not be null after constructing a Company.");
            Assert.That(company.City, Is.Not.Null, "The city of the company can not be null after constructing a Company.");
            Assert.That(company.Contacts, Is.Not.Null, "The contacts list should be initialised after constructing a Company.");


            Contact contact = new Contact();
            Assert.That(contact.Name, Is.Not.Null, "The name of the contact can not be null after constructing a Contact.");
            Assert.That(contact.FirstName, Is.Not.Null, "The firstname of the contact can not be null after constructing a Contact.");
            Assert.That(contact.Email, Is.Not.Null, "The email of the contact can not be null after constructing a Contact.");
            Assert.That(contact.Phone, Is.Not.Null, "The phone of the contact can not be null after constructing a Contact.");
        }
    }
}
