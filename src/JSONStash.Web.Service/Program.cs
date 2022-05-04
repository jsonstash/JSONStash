using JSONStash.Common.Context;
using JSONStash.Common.Services;
using JSONStash.Common.Services.IServices;
using JSONStash.Web.Service.Middleware;
using JSONStash.Web.Service.Models;
using JSONStash.Web.Service.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration["DefaultConnection"];

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore; })
    .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase; });

builder.Services
    .AddDbContext<JSONStashContext>(options => options.UseLazyLoadingProxies().UseSqlite(connectionString))
    .AddScoped<IAuthenticateService, AuthenticateService>()
    .AddScoped<IJSONStashService, JSONStashService>()
    .AddApiVersioning(config =>
    {
        config.DefaultApiVersion = new ApiVersion(1, 0); config.AssumeDefaultVersionWhenUnspecified = true;
    })
    .AddSingleton(builder.Configuration)
    .AddHttpContextAccessor()
    .AddEndpointsApiExplorer()
    .AddAuthentication();

var app = builder.Build();

app.UseCors(config =>
{
    config.AllowAnyOrigin();
    config.AllowAnyMethod();
    config.AllowAnyHeader();
});

app.UseHttpsRedirection()
   .UseAuthorization();

app.UseMiddleware<AuthenticationMiddleware>();

app.MapControllers();

IServiceScope scope = app.Services.CreateScope();
JSONStashContext context = scope.ServiceProvider.GetService<JSONStashContext>();
await context.Database.EnsureCreatedAsync();

app.Run();