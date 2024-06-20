using GettingStartedMassTransit.Web;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders().AddConsole();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();

    if (bool.Parse(builder.Configuration["enabledSaaS"]))
    {
        x.UsingAmazonSqs((context, cfg) =>
        {
            cfg.Host(builder.Configuration["AWS:region"], h =>
            {
                h.AccessKey(builder.Configuration["AWS:accessKeyID"]);
                h.SecretKey(builder.Configuration["AWS:secretAccessKey"]);
            });

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

            cfg.ConfigureEndpoints(context);
        });
    }
});
builder.Services.AddHostedService<BackgroundWorker>();

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
