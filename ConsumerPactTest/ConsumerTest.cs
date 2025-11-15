using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using PactNet;
using PactNet.Matchers;
using Xunit;

namespace SampleProvider.Tests
{
    public class ConsumerPactTest
    {
        [Fact]
        public async Task GetSampleData_ReturnsValidResponse()
        {
            var pact = Pact.V4("SampleConsumer", "SampleProvider", new PactConfig());
            var builder = pact.WithHttpInteractions();

            builder
                .UponReceiving("A GET request for sample data")
                .WithRequest(HttpMethod.Get, "/api/sample/data")
                .WillRespond()
                .WithStatus(HttpStatusCode.OK)
                .WithJsonBody(new
                {
                    id = Match.Type(1),
                    name = Match.Type("SampleName"),
                    description = Match.Type("Sample description")
                });
            await builder.VerifyAsync(async ctx =>
            {
                using var client = new HttpClient { BaseAddress = ctx.MockServerUri };
                var response = await client.GetAsync("/api/sample/data");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                Assert.Contains("\"id\"", json);
            });
        }
    }
}
