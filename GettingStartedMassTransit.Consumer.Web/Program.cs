using GettingStartedMassTransit.Common.EventBus.Abstractions;
using GettingStartedMassTransit.Common.EventBus;
using MassTransit;
using Amazon.Runtime;
using GettingStartedMassTransit.Consumer.Web.Consumer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    //x.AddConsumer<EventConsumer>();
    x.AddConsumer<MessageEventWithJsonConsumer>();
    x.AddConsumer<MessageEventWithJsonObjectConsumer>();
    x.AddConsumer<MessageEventOtherTypeConsumer>();

    if (bool.Parse(builder.Configuration["enabledSaaS"]!))
    {
        x.UsingAmazonSqs((context, cfg) =>
        {
            cfg.Host(builder.Configuration["AWS:region"], h =>
            {
                //h.AccessKey(builder.Configuration["AWS:accessKeyID"]);
                //h.SecretKey(builder.Configuration["AWS:secretAccessKey"]);
                h.Credentials(new SessionAWSCredentials(builder.Configuration["AWS:accessKeyID"], builder.Configuration["AWS:secretAccessKey"], builder.Configuration["AWS:sessionToken"]));
            });

            //cfg.ConfigureEndpoints(context);
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
            });
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
