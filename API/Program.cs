using API.Enums;
using API.Model;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.NameTranslation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var conString = builder.Configuration.GetConnectionString("BloggingDatabase") ?? throw new InvalidOperationException("Connection string 'BloggingDatabase' not found.");

var clientAddress = builder.Configuration["Addresses:Client"] ?? throw new InvalidOperationException("Address string 'Client' not found");

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PostgresContext>(options => options.UseLazyLoadingProxies().UseNpgsql(conString, o =>
         o.MapEnum<Role>("role", "skywhysales", new NpgsqlNullNameTranslator())
        .MapEnum<TicketStatus>("ticket_status", "skywhysales", new NpgsqlNullNameTranslator())
        .MapEnum<ClassOfService>("class_of_service", "skywhysales", new NpgsqlNullNameTranslator())
       ), ServiceLifetime.Singleton);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
                          policy =>
                          {
                              policy.WithOrigins(clientAddress)
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod()
                                                  .AllowAnyOrigin();
                          });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}



//app.MapStaticAssets();
app.UseStaticFiles();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
