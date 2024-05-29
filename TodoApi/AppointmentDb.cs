using Microsoft.EntityFrameworkCore;

class AppointmentDb : DbContext
{
    public AppointmentDb(DbContextOptions<AppointmentDb> options)
        : base(options) { }

    public DbSet<Appointment> Appointments => Set<Appointment>();
}