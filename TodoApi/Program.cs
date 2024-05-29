using NSwag.AspNetCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppointmentDb>(opt => opt.UseInMemoryDatabase("AppointmentList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.DocumentName = "AppointmentAPI";
    config.Title = "AppointmentAPI v1";
    config.Version = "v1";
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi(config =>
    {
        config.DocumentTitle = "AppointmentAPI";
        config.Path = "/swagger";
        config.DocumentPath = "/swagger/{documentName}/swagger.json";
        config.DocExpansion = "list";
    });
}

var AppointmentItems = app.MapGroup("/appointmentitems");

AppointmentItems.MapGet("/", GetAllAppointments);
AppointmentItems.MapGet("/{id}", GetAppointment);
AppointmentItems.MapPost("/", CreateAppointment);
AppointmentItems.MapPut("/{id}", UpdateAppointment);
AppointmentItems.MapDelete("/{id}", DeleteAppointment);

app.Run();

// GET Get All Appointments
static async Task<IResult> GetAllAppointments(AppointmentDb db)
{
    return TypedResults.Ok(await db.Appointments.ToArrayAsync());
}

// GET Get Appointment By ID
static async Task<IResult> GetAppointment(int id, AppointmentDb db)
{
    return await db.Appointments.FindAsync(id)
        is Appointment Appointment
            ? TypedResults.Ok(Appointment)
            : TypedResults.NotFound();
}

// POST Create New Appointment
static async Task<IResult> CreateAppointment(Appointment Appointment, AppointmentDb db)
{
    db.Appointments.Add(Appointment);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/Appointmentitems/{Appointment.Id}", Appointment);
}

// PUT Update/Modify Appointment By ID 
// TODO change to by cat name/owner/smth else?
static async Task<IResult> UpdateAppointment(int id, Appointment inputAppointment, AppointmentDb db)
{
    var Appointment = await db.Appointments.FindAsync(id);

    if (Appointment is null) return TypedResults.NotFound();

    Appointment.DateTime = inputAppointment.DateTime;
    Appointment.Cat = inputAppointment.Cat;
    Appointment.CatOwner = inputAppointment.CatOwner;

    await db.SaveChangesAsync();

    return TypedResults.NoContent();
}

// DELETE Delete Appointment
static async Task<IResult> DeleteAppointment(int id, AppointmentDb db)
{
    if (await db.Appointments.FindAsync(id) is Appointment Appointment)
    {
        db.Appointments.Remove(Appointment);
        await db.SaveChangesAsync();
        return TypedResults.NoContent();
    }

    return TypedResults.NotFound();
}