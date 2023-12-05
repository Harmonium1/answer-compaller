using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AnswerCompiler.DataAccess;

public sealed class DataContext: DbContext
{
    public DbSet<UserEntity> Users { get; set; } = null!;
    public DbSet<SurveyEntity> Surveys { get; set; } = null!;

    private readonly IConfiguration _configuration;

    public DataContext(IConfiguration configuration)
    {
        _configuration = configuration;

        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .ToContainer("Users")
            .HasNoDiscriminator()
            .HasPartitionKey(u => u.LineUserId)
            .HasKey(u => u.LineUserId);

        modelBuilder.Entity<SurveyEntity>()
            .ToContainer("Surveys")
            .HasNoDiscriminator()
            .HasPartitionKey(u => u.SurveyId)
            .HasKey(u => u.SurveyId);

        modelBuilder.Entity<SurveyEntity>().OwnsMany(
            o => o.AppliedUsers,
            u =>
            {
                u.ToJsonProperty("AppliedUsers");
            });
        
        modelBuilder.Entity<SurveyEntity>().OwnsMany(
            o => o.Answers,
            u =>
            {
                u.ToJsonProperty("Answers");
            });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string configurationString = _configuration.GetConnectionString("Default")!;
        optionsBuilder.UseCosmos(configurationString, "AnswerCompilerDB");
    }
}