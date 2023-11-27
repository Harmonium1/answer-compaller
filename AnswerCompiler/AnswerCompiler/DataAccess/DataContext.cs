using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AnswerCompiler.DataAccess;

public class DataContext: DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;

    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().ToContainer("Users").HasNoDiscriminator();
        modelBuilder.Entity<UserEntity>().HasKey(u => u.LineUserId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string configurationString = _configuration.GetConnectionString("Default")!;
        optionsBuilder.UseCosmos(configurationString, "AnswerCompilerDB");
    }
}