using MassTransit;
using Amazon.Runtime;
using GettingStartedMassTransit.Common.EventBus.Abstractions;
using GettingStartedMassTransit.Common.EventBus;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders().AddConsole();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    if (bool.Parse(builder.Configuration["enabledSaaS"]!))
    {
        x.UsingAmazonSqs((context, cfg) =>
        {
            cfg.Host(builder.Configuration["AWS:region"], h =>
            {
                h.Credentials(new SessionAWSCredentials(builder.Configuration["AWS:accessKeyID"], builder.Configuration["AWS:secretAccessKey"], builder.Configuration["AWS:sessionToken"]));
            });

            //cfg.UseBsonSerializer();
            //cfg.DefaultContentType = new System.Net.Mime.ContentType("application/vnd.masstransit+bson");
            cfg.ConfigureEndpoints(context);
        });
    }
    else
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            //cfg.UseBsonSerializer();

            cfg.ConfigureEndpoints(context);
        });
    }
});
builder.Services.AddTransient<IEventBus, EventBus>();
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
