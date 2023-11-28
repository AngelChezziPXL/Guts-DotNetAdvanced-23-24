using AngleSharp.Html.Parser;
using ContactManager.AppLogic.Contracts;
using ContactManager.Pages;
using Guts.Client.Core;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using System.Net;

namespace ContactManager.Tests.Web
{
    [ExerciseTestFixture("dotnet2", "3-RAZORWEBAPI", "ContactManager",
    @"ContactManager\Pages\Index.cshtml")]

    public class IndexPageTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }
        
        [MonitoredTest("Html Integration Tests - Razor Page - Index - Should return a success Status Code")]
        public async Task _01_IndexPage_ReturnsSuccessStatusCode()
        {
            // Act
               var response = await _client.GetAsync("/Index"); 

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "The Index Page should return a status code 200.");
        }

        [MonitoredTest("Html Integration Tests - Razor Page - Index - Should return html containing a table with 5 columns and the correct column headers")]
        public async Task _02_IndexPage_ShouldContainExpectedHtmlContent()
        {
            // Act
            var response = await _client.GetAsync("/Index"); 
            var content = await response.Content.ReadAsStringAsync();

            var parser = new HtmlParser();
            var document = parser.ParseDocument(content);

            var table = document.QuerySelector("table");
            Assert.That(table, Is.Not.Null, "The Page has to contain a <table> element");
            var headerRow = table.QuerySelector("thead tr");
            Assert.That(headerRow, Is.Not.Null, "The table has to contain a <thead> element and <tr> elements in it"); 
            var columns = headerRow.QuerySelectorAll("th");


            string[] expectedPropertyNames = { "Name", "First Name", "Email", "Phone", "Company" };

            for (int i = 0; i < columns.Length; i++)
            {
                string columnName = columns[i].TextContent.Trim();
                Assert.That(columnName, Is.EqualTo(expectedPropertyNames[i]), $"Column {i} should match property name {expectedPropertyNames[i]}");
            }

            // Assert
            Assert.That(content.Contains("<h1>Contacts</h1>"), Is.True,"The page should have a title with the text \"Contacts\"");
            Assert.That(columns.Length, Is.EqualTo(5), "The table should contain 5 columns.");

            var dataRows = document.QuerySelectorAll("table tbody tr");
            Assert.That(dataRows.Length, Is.GreaterThan(0), "The table should contain at least one data row.");
            foreach (var row in dataRows)
            {
                var cells = row.QuerySelectorAll("td");
                Assert.That(cells.Length, Is.EqualTo(5), "The data row should contain 5 cells."); 
            }
        }

    }

}


