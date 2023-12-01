using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AnswerCompiler.DataAccess;

public sealed class DataContext: DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<Survey> Surveys { get; set; } = null!;

    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;

        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>().ToContainer("Users").HasNoDiscriminator();
        modelBuilder.Entity<UserEntity>().HasKey(u => u.LineUserId);
        
        modelBuilder.Entity<Survey>().ToContainer("Surveys").HasNoDiscriminator();
        modelBuilder.Entity<Survey>().HasKey(survey => survey.SurveyId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string configurationString = _configuration.GetConnectionString("Default")!;
        optionsBuilder.UseCosmos(configurationString, "AnswerCompilerDB");
    }
}