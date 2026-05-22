using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using PaymentAPI.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IPaymentTransaction, PaymentTransactionContext>();

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

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();