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
        [MonitoredTest("Domain - Company - should contain the correct properties")]
        public void _01_CompanyShouldContainCorrectProperties()
        {
            Type companyType = typeof(Company);

            PropertyInfo[] properties = companyType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            Assert.That(properties.Count, Is.EqualTo(6), "Class Company should have 5 properties (with getter and setter)");
            Assert.That(properties.Any(p => p.Name == "Id"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Name"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Address"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Zip"), Is.True);
            Assert.That(properties.Any(p => p.Name == "City"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Contacts"), Is.True);


            ConstructorInfo[] constructors = companyType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");
            Assert.That(parameters.Length, Is.EqualTo(0), "The constructor should have 0 parameters.");
        }

        [MonitoredTest("Domain - Contact = should contain the correct properties")]
        public void _02_ContactShouldContainCorrectProperties()
        {
            Type contactType = typeof(Contact);

            PropertyInfo[] properties = contactType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            Assert.That(properties.Count, Is.EqualTo(7), "Class Contact should have 7 properties (with getter and setter)");
            Assert.That(properties.Any(p => p.Name == "Id"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Name"), Is.True);
            Assert.That(properties.Any(p => p.Name == "FirstName"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Email"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Phone"), Is.True);

            Assert.That(properties.Any(p => p.Name == "CompanyId"), Is.True);
            Assert.That(properties.Any(p => p.Name == "Company"), Is.True);

            ConstructorInfo[] constructors = contactType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            ConstructorInfo constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            Assert.That(constructors.Length, Is.EqualTo(1), "There should be exactly one public constructor.");
            Assert.That(parameters.Length, Is.EqualTo(0), "The constructor should have 0 parameters.");
        }

        [MonitoredTest("Domain - Company and Contact - should do the necessary null checks and initializations")]
        public void _03_NullChecks()
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
