using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server_API.Models.Entities;

namespace Server_API.Contexts;

public class DataContext : IdentityDbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    //Database sets for accessing and manipulating different types of data entities

    public DbSet<UnitEntity> Units { get; set; }
    public DbSet<TemperatureDataEntity> TemperatureData { get; set; }
    public DbSet<UserEntity> UserEntities { get; set; }
}