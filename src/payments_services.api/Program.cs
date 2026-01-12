using payments_services.application.Commands.Commands;
using payments_services.application.Interfaces;
using payments_services.domain.Interfaces;
using payments_services.infrastructure.ExternalServices.Stripe.Adapters;
using payments_services.infrastructure.ExternalServices.Stripe.Services;
using payments_services.infrastructure.Persistence.Context;
using payments_services.infrastructure.Persistence.Repositories;
using payments_services.infrastructure.Services;
using Stripe;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost3000", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//crear variable para la cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("ConnectionPostgre"); //ConnectionPostgre es el parametro de conexion que creamos en el appsetting
//registrar servicio para la conexion


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString,
        b => b.MigrationsAssembly("payments_services.infrastructure")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
builder.Services.AddHttpClient<UsuarioService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:7181/api/users/");
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(RegistrarMedioPagoCommand).Assembly));

builder.Services.AddSignalR();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IStripeCustomerService, StripeCustomerService>();
builder.Services.AddScoped<IStripePaymentMethodService, StripePaymentMethodService>();
builder.Services.AddScoped<IStripePaymentIntentService, StripePaymentIntentService>();
builder.Services.AddScoped<IReservaService, ReservaService>();
builder.Services.AddScoped<IHistorialPagosRepositoryPostgres, HistorialPagosRepositoryPostgres>();
builder.Services.AddScoped<INotificationServices, NotificationServices>();
builder.Services.AddScoped<ICouponServices, CouponServices>();

builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var secretKey = config["Stripe:SecretKey"];

    if (string.IsNullOrWhiteSpace(secretKey))
        throw new InvalidOperationException("Stripe:SecretKey no está configurado.");

    return new StripeClient(secretKey);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseCors("AllowLocalhost3000");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Obtiene el DbContext
        var context = services.GetRequiredService<AppDbContext>();

        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al aplicar las migraciones a la base de datos.");
    }
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
