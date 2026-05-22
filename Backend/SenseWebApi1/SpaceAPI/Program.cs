using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SC.Internship.Common.ScResult;
using SpaceAPI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata=false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "MyAuthServer",
            ValidAudience = "MyAuthClient",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("mysupersecret_secretkey!123")
            )
        };
    }); 


var app = builder.Build();

app.Use(async (context, next) =>
{

    using var sr = new StreamReader(context.Request.Body,Encoding.UTF8, true, 1024, true);
    // ReSharper disable once IdentifierTypo
    var bodystring = await sr.ReadToEndAsync();
    // ReSharper disable once IdentifierTypo
    var headerstring = context.Request.Headers.Aggregate("", (current, item) => current + $"{item.Key}={item.Value}\n");
    app.Logger.LogInformation("Request {0} \n {1} \n {2} ", context.Request.Method,bodystring,headerstring);
    await next.Invoke();
});



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var spaces = new List<Space>
{
    new()
    {
        Id = Guid.Parse("1f072c8c-b770-4cae-a587-d5e7bb2777ba")
    },
    new()
    {
        Id = Guid.Parse("558b3257-bb3d-4b40-a2bf-207c77d3149c")
    },
    new()
    {
        Id = Guid.Parse("8274dc1e-3ae6-472b-84f7-e7b740002ba2")
    }
};
app.MapGet("/spaces/{spaceId:guid}", ([FromRoute] Guid spaceId) =>
    {
        if (spaces.FirstOrDefault(p => p.Id == spaceId) == null)
        {
            throw new ArgumentException("Пространство не найдено");
        }
        return new ScResult<Guid>(spaceId);
    })
.WithOpenApi().RequireAuthorization();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<GlobalErrorHandler>();

app.Run();