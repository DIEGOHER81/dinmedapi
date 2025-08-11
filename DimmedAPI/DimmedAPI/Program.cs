using DimmedAPI;
using DimmedAPI.Interfaces;
using DimmedAPI.BO;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;
using DinkToPdf.Contracts;
using DinkToPdf;

var builder = WebApplication.CreateBuilder(args);

var OrigenesPermitidos = builder.Configuration.GetValue<string>("AllowedHosts")!.Split(',');


// Agrega la política de CORS
builder.Services.AddCors(options =>
{
    //options.AddPolicy("AllowAngularDev",
    //    policy => policy
    //        .WithOrigins("http://localhost:4200")
    //        .AllowAnyHeader()
    //        .AllowAnyMethod());


    options.AddDefaultPolicy(optionsCORS =>
    {
        optionsCORS.WithOrigins(OrigenesPermitidos).AllowAnyMethod().AllowAnyHeader();
    }
    );

});



// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
    });
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDBContext>(opciones => opciones.UseSqlServer("name=DefaultConnection"));

builder.Services.AddOutputCache(opciones =>
{
    //opciones.DefaultExpirationTimeSpan = TimeSpan.FromDays(1);
    opciones.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(15);
}
);

// Register Business Objects
builder.Services.AddScoped<IntBCConex, bcConn>();
builder.Services.AddScoped<ICustomerBO, CustomerBO>();
builder.Services.AddScoped<IEquipmentBO, EquipmentBO>();
builder.Services.AddScoped<ICustomerAddressBO, CustomerAddressBO>();
builder.Services.AddScoped<EntryRequestTraceBO>();
builder.Services.AddScoped<IEmployeeBO, EmployeeBO>();

// Register Dynamic Connection Service
builder.Services.AddScoped<IDynamicConnectionService, DynamicConnectionService>();

// Register Dynamic BC Connection Service
builder.Services.AddScoped<IDynamicBCConnectionService, DynamicBCConnectionService>();

// Register Email Service
builder.Services.AddScoped<IEmailService, EmailService>();

// Register PDF Service
builder.Services.AddScoped<IPdfService, PdfService>();

// Register DinkToPdf Converter with error handling
try
{
    var converter = new SynchronizedConverter(new PdfTools());
    builder.Services.AddSingleton(typeof(IConverter), converter);
    Console.WriteLine("DinkToPdf converter registrado exitosamente");
}
catch (Exception ex)
{
    Console.WriteLine($"Error al registrar DinkToPdf converter: {ex.Message}");
    Console.WriteLine("El servicio PDF usará métodos alternativos");
    
    // Registrar null como converter - el servicio manejará este caso
    builder.Services.AddSingleton(typeof(IConverter), (IConverter)null);
}

// Eliminada la configuración explícita de Kestrel para certificados SSL

var app = builder.Build();

// Usa la política de CORS
app.UseCors();
//app.UseCors("AllowAngularDev");


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();

}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Habilitar archivos estáticos para servir logos y otros archivos
app.UseStaticFiles();

app.UseOutputCache();
app.UseAuthorization();
app.MapControllers();
app.Run();
