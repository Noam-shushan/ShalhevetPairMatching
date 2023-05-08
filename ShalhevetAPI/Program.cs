using PairMatching.DomainModel.DataAccessFactory;
using PairMatching.DomainModel.Services;
using PairMatching.Root;
using PairMatching.Loggin;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var starup = new Startup();
var configurations = starup.GetConfigurations();

builder.Services.AddSingleton(configurations);
builder.Services.AddSingleton(new Logger(configurations.ConnctionsStrings));
builder.Services.AddScoped<IDataAccessFactory, DataAccessFactory>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
