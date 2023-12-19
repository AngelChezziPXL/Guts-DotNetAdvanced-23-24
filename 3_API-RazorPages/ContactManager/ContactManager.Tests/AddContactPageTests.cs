using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Guts.Client.Core;
using Guts.Client.Core.TestTools;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactManager.Tests
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
@"ContactManager\Pages\Contacts\AddContact.cshtml")]

    public class AddContactPageTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;
        private string _razorContentLowerCase = string.Empty;

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
            _razorContentLowerCase = Solution.Current.GetFileContent("ContactManager/Pages/Contacts/AddContact.cshtml").ToLower();
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
            HttpResponseMessage response = await _client.GetAsync("Contacts/AddContact");

            // Assert
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();
            IHtmlDocument document = parser.ParseDocument(content);

            IElement? form = document.QuerySelector("form");

            Assert.That(form, Is.Not.Null, "The page has to contain a <form> element");

            IHtmlCollection<IElement> fields = form!.QuerySelectorAll("input");

            var selectElement = form.QuerySelector("select");
            Assert.That(selectElement, Is.Not.Null, "The page has to contain a select list");

            var selectOptions = selectElement!.QuerySelectorAll("option");

            List<string> expectedNames = new List<string> { "Contact.FirstName", "Contact.Name", "Contact.Email", "Contact.Phone" };
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

            IHtmlButtonElement? button = form.QuerySelector("button") as IHtmlButtonElement;
            Assert.That(button, Is.Not.Null, "The form should have a button");
            Assert.That(button!.Type, Is.EqualTo("submit"), "The button should be a submit button");

            IElement? validationSummaryDiv = document.QuerySelector("div.validation-summary-valid");
            Assert.That(validationSummaryDiv, Is.Not.Null,
                "The page has to contain a div in which a summary of validation errors can be displayed");
        }

        [MonitoredTest("Html Integration Tests - Razor Page - AddContact - Company dropdown should be rendered using tag helpers")]
        public void _03_AddContactPage_CompanyDropDown_ShouldBeRenderedUsingTagHelpers()
        {
            string message = "Do not use option tags, but use a tag helper. " +
                             "See https://learn.microsoft.com/en-us/aspnet/core/mvc/views/working-with-forms?view=aspnetcore-6.0#the-select-tag-helper";

            Assert.That(_razorContentLowerCase, Does.Not.Contain("<option"), message);
            Assert.That(_razorContentLowerCase, Does.Contain("asp-items"), message);
        }
    }
}

