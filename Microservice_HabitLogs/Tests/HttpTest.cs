using Microservice_HabitLogs.Controllers;
using Microservice_HabitLogs.Data;
using Microservice_HabitLogs.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using System.Net;
namespace Microservice_HabitLogs.Tests
{
    public class HttpTest
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            // Cria o cliente HTTP apontando para sua API local
            _client = new HttpClient
            {
                BaseAddress = new System.Uri("https://localhost:7285")
            };
        }

        [Test]
        public async Task GET_returnsOk()
        {
            var response = await _client.GetAsync("/badges?badge=bronze&clientId=Daniel");

            // Assert: status 200 OK
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "O endpoint /habits deve retornar status 200.");

            // Assert: Content-Type JSON
            Assert.That(response.Content.Headers.ContentType?.MediaType,
                Is.EqualTo("application/json"),
                "A resposta deve estar no formato JSON.");

            // (Opcional) Lê o conteúdo
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(content, "A resposta JSON não deve estar vazia.");
        }
        [Test]
        public async Task GET_returnsNotFound()
        {
            var response = await _client.GetAsync("/badges");

            // Assert: status 200 OK
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "O endpoint /habits deve retornar status 404.");
        }

        [Test]
        public async Task GET_returnsWeeklyOk()
        {
            var response = await _client.GetAsync("stats/weekly?clientId=Daniel");

            // Assert: status 200 OK
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),
                "O endpoint /habits deve retornar status 200.");

            // Assert: Content-Type JSON
            Assert.That(response.Content.Headers.ContentType?.MediaType,
                Is.EqualTo("application/json"),
                "A resposta deve estar no formato JSON.");

            // (Opcional) Lê o conteúdo
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotEmpty(content, "A resposta JSON não deve estar vazia.");
        }

        [Test]
        public async Task GET_returnsWeeklyNotFound()
        {
            var response = await _client.GetAsync("stats/weekly");

            // Assert: status 200 OK
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound),
                "O endpoint /habits deve retornar status 404.");

        }
    }
}
