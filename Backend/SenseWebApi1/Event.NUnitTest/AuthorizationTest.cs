using System.Net;
using System.Text;
using System.Text.Json;

namespace Event.NUnitTest;

public class AuthorizationTest
{
    private readonly HttpClient _client;
    [SetUp]
    public void Setup()
    {
    }
    
    [Fact]
    public async Task TestAuthorization()
    {
        var value = new
        {
            login = "admin",
            password = "12345"
        };
        var content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        
        var response =await _client.PostAsync("http://localhost:5000/stub/authstub",content);
        string stop = "";
        Assert.Equals(HttpStatusCode.OK, response.StatusCode);
    }
}