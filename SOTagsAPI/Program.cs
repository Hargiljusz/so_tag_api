using DataCommon;
using DataService.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("so_tags").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
});

builder.Services.AddSingleton<IDbClient, MongoDbClient>(x =>
{
    var configuration = x.GetRequiredService<IConfiguration>();
    return new MongoDbClient(configuration);
});

builder.Services.AddTransient<ITagService, TagServiceDb>();



var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();

app.Run();
