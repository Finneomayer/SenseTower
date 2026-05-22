using System.Text;
using ImageAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SC.Internship.Common.ScResult;

// ReSharper disable IdentifierTypo


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
    var bodystring = await sr.ReadToEndAsync();
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
var images=new List<Image>
{
    new()
    {
        Id=Guid.Parse("5a448705-8ad7-4456-9847-f3b07e26edf9")
    },
    new()
    {
        Id=Guid.Parse("08e8f2cf-7594-4e6d-9df9-77f524ad1e3e")
    },
    new()
    {
        Id=Guid.Parse("0e034023-5f16-4a4e-852d-d76e511147df")
    }
};


app.MapGet("/images/{imageId:guid}", ([FromRoute] Guid imageId) =>
        {
            if (images.FirstOrDefault(p => p.Id == imageId) == null)
            {
                throw new ArgumentException("Изображение не найдено");
            }
            return new ScResult<Guid>(imageId);
        }
)
    .WithOpenApi().RequireAuthorization();



app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<GlobalErrorHandler>();

app.Run();

