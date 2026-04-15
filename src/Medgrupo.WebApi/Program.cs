using Medgrupo.WebApi.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppMvc();
builder.Services.AddAppSwagger();
builder.Services.AddAppDatabase(builder.Configuration);
builder.Services.AddAppDependencies();
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddAppHealthChecks(builder.Configuration);
}

var app = builder.Build();

app.UseAppSwagger();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

if (!app.Environment.IsEnvironment("Testing"))
{
    app.MapAppHealthChecks();
    await app.EnsureDatabaseCreatedAsync();
}

app.Run();

public partial class Program { }
