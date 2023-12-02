using AngleSharp.Html.Parser;
using Guts.Client.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
@"ContactManager\Pages\Contacts\AddContact.cshtml")]

    public class AddContactPageTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [MonitoredTest("Html Integration Tests - Razor Page - AddContact - Should return a success Status Code")]
        public async Task _01_AddContactPage_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("Contacts/AddContact");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),"The AddContact page should return a status code 200.");
        }

        [MonitoredTest("Html Integration Tests - Razor Page - AddContact - Should return html containing input fields for each contact property and a submit button")]
        public async Task _02_AddContactPage_ShouldContainExpectedHtmlContent()
        {
            // Act
            var response = await _client.GetAsync("Contacts/AddContact");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(content);

            var form = document.QuerySelector("form");

            Assert.That(form, Is.Not.Null, "The page has to contain a <form> element");

            var fields = form.QuerySelectorAll("input");
            var button = form.QuerySelector("button");

            var selectElement = document.QuerySelector("select#CompanyId");

            Assert.That(selectElement, Is.Not.Null, "The page has to contain a select list");
            var selectOptions = selectElement.QuerySelectorAll("option");


            List<string> expectedNames = new List<string> { "FirstName", "Name", "Email", "Phone" };

            List<string?> actualNames = fields.Where(input => !string.IsNullOrEmpty(input.GetAttribute("id")))
            .Select(input => input.GetAttribute("name"))
            .ToList();

            Assert.That(actualNames.Count(), Is.EqualTo(4), "The form has to contain 4 input fields");
            Assert.That(actualNames, Is.EquivalentTo(expectedNames), "The form should contain the expected input fields");

            Assert.That(
        selectOptions.Any(option => option.GetAttribute("value") == ""),
        Is.True,
        "The 'Select a company' option should be present in the select element."
    );
        }
    }
}

