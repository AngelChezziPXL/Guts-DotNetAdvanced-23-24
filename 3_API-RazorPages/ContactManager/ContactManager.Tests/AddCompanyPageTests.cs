using System.Net;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using Guts.Client.Core;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactManager.Tests
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
    @"ContactManager\Pages\Companies\AddCompany.cshtml")]
    public class AddCompanyPageTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }

        [MonitoredTest("Html Integration Tests - Razor Page - AddCompany - Should return a success Status Code")]
        public async Task _01_AddCompanyPage_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("Companies/AddCompany");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "The AddCompany Page should return a statuscode 200.");
        }

        [MonitoredTest("Html Integration Tests - Razor Page - AddCompany - Should return html containing input fields for each company property and a submit button")]
        public async Task _02_AddCompanyPage_ShouldContainExpectedHtmlContent()
        {
            var response = await _client.GetAsync("/Companies/AddCompany");
            var content = await response.Content.ReadAsStringAsync();

            // Parse the HTML content
            var parser = new HtmlParser();
            var document = parser.ParseDocument(content);

            // Find the form and count the fields
            var form = document.QuerySelector("form");

            Assert.That(form, Is.Not.Null, "The Page has to contain a <form> element");

            var fields = form.QuerySelectorAll("input");

            List<string> expectedNames = new List<string> { "Company.Name", "Company.Address", "Company.Zip", "Company.City" };

            List<string?> actualNames = fields.Where(input => !string.IsNullOrEmpty(input.GetAttribute("id")))
                .Select(input => input.GetAttribute("name"))
                .ToList();

            Assert.That(actualNames.Count(), Is.EqualTo(4), "The form has to contain 4 input fields");
            Assert.That(actualNames, Is.EquivalentTo(expectedNames), "The form should contain the expected input fields");

            IHtmlButtonElement? button = form.QuerySelector("button") as IHtmlButtonElement;
            Assert.That(button, Is.Not.Null, "The form should have a button");
            Assert.That(button!.Type, Is.EqualTo("submit"), "The button should be a submit button");

            IElement? validationSummaryDiv = document.QuerySelector("div.validation-summary-valid");
            Assert.That(validationSummaryDiv, Is.Not.Null,
                "The page has to contain a div in which a summary of validation errors can be displayed");
        }
    }
}








