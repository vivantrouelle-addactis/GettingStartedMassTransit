using GettingStartedMassTransit.Common.EventBus.Abstractions;
using GettingStartedMassTransit.Common.EventBus;
using MassTransit;
using Amazon.Runtime;
using GettingStartedMassTransit.Consumer.Web.Consumer;
using GettingStartedMassTransit.Consumer.Web.Services;
using GettingStartedMassTransit.Consumer.Web.Models.Settings;
using GettingStartedMassTransit.Common.EventBus.Entity.Application;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using GettingStartedMassTransit.Consumer.Web.Consumer.AuditTrails;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders().AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<EventStoreDatabaseSettings>(builder.Configuration.GetSection("EventStoreDatabase"));
builder.Services.Configure<AuditTrailDatabaseSettings>(builder.Configuration.GetSection("AuditTrailDatabase"));

builder.Services.AddSingleton<MessageEventsService>();
builder.Services.AddSingleton<AuditTrailsService<ApplicationBetaEntity>>();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<AuditTrailDatabaseSettings>>().Value;
    return new MongoClient(settings.ConnectionStrings);
});

builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<EventConsumer>();
    //x.AddConsumer<MessageEventWithJsonConsumer>();
    //x.AddConsumer<MessageEventWithJsonObjectConsumer>();
    //x.AddConsumer<MessageEventOtherTypeConsumer>();
    //x.AddConsumer<AuditTrailEventBsonConsumer>();
    //x.AddConsumer<AuditTrailBetaEventConsumer>();
    //x.AddConsumer<BsonDocumentConsumer>();
    x.AddConsumersFromNamespaceContaining<AuditTrailEventConsumer>();
    if (bool.Parse(builder.Configuration["enabledSaaS"]!))
    {
        x.UsingAmazonSqs((context, cfg) =>
        {
            cfg.Host(builder.Configuration["AWS:region"], h =>
            {
                h.Credentials(new SessionAWSCredentials(builder.Configuration["AWS:accessKeyID"], builder.Configuration["AWS:secretAccessKey"], builder.Configuration["AWS:sessionToken"]));
            });

            //cfg.UseBsonSerializer();
            //cfg.ConfigureEndpoints(context);
            /*
            cfg.ReceiveEndpoint("message-event-queue", e =>
            {
                e.ConfigureConsumer<MessageEventWithJsonConsumer>(context);
                e.ConfigureConsumer<MessageEventWithJsonObjectConsumer>(context);
                e.ConfigureConsumer<MessageEventOtherTypeConsumer>(context);
                
                //e.ConfigureConsumeTopology = false;

                /*
                e.Subscribe("message-event", s =>
                {
                    s.TopicAttributes["DisplayName"] = "Message Event Topic";
                    s.TopicTags.Add("environment", "development");
                });
                */
            //});
            //cfg.ConfigureEndpoints(context);
            
            cfg.ReceiveEndpoint("audit-trail", e =>
            {
                //e.ClearMessageDeserializers();
                //e.DefaultContentType = new ContentType("application/vnd.masstransit+bson");

                //e.UseBsonDeserializer();
                //e.ConfigureConsumer<AuditTrailBetaEventConsumer>(context);
                //e.ConfigureConsumer<BsonDocumentConsumer>(context);
                e.ConfigureConsumers(context);
            });

            //cfg.UseBsonSerializer(); 
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
/*
            cfg.ReceiveEndpoint("message-event-queue", e =>
            {
                e.ConfigureConsumer<MessageEventWithJsonConsumer>(context);
                e.ConfigureConsumer<MessageEventWithJsonObjectConsumer>(context);
                e.ConfigureConsumer<MessageEventOtherTypeConsumer>(context);
                //e.ConfigureConsumeTopology = false;
            });
*/
            cfg.ReceiveEndpoint("audit-trail-queue", e =>
            {
                //e.UseBsonDeserializer();
                //e.ConfigureConsumer<BsonDocumentConsumer>(context);
            });

            //cfg.ConfigureEndpoints(context);
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
