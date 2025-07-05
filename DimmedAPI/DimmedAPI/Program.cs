using DimmedAPI;
using DimmedAPI.Interfaces;
using DimmedAPI.BO;
using DimmedAPI.Services;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddControllers();
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

// Register Dynamic Connection Service
builder.Services.AddScoped<IDynamicConnectionService, DynamicConnectionService>();

// Register Dynamic BC Connection Service
builder.Services.AddScoped<IDynamicBCConnectionService, DynamicBCConnectionService>();

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
app.UseOutputCache();
app.UseAuthorization();
app.MapControllers();
app.Run();
