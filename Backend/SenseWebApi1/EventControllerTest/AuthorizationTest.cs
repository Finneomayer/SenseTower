using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;



namespace EventControllerTest;

public class AuthorizationTest
{
    
#pragma warning disable CS8618
    // ReSharper disable once IdentifierTypo
    private string _jwtTocken="";
#pragma warning restore CS8618
    
    
    [Fact]
    public async Task TestAuthorization()
    {
        var value = new
        {
            login = "admin",
            password = "12345"
        };
        var content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        using var client = new HttpClient();
        var response =await client.PostAsync("http://localhost:5286/stub/authstub",content);
        _jwtTocken = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]

    public async Task TestGetEvents()
    {
        
        await TestAuthorization();
        using var client = new HttpClient();
        var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("http://localhost:5286/api/events")
            };
       
        request.Headers.Add("Authorization","Bearer "+_jwtTocken);
        var response = await client.SendAsync(request);
        var result =await response.Content.ReadAsStringAsync();
        var path=Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        using var test = new StreamReader(path+@"\testcontent\eventsfrombd.txt");
        var defaultString=await test.ReadLineAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(defaultString,result);
    }

    
}