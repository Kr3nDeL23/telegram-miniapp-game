using Microsoft.EntityFrameworkCore;
using Presentation.Common.Domain.Entities;

namespace Presentation.Common.Domain.Contexts;
public class ApplicationDBContext : DbContext
{
    public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
    { }
    public DbSet<LevelEntity> Levels { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<SquadEntity> Squads { get; set; }
    public DbSet<SkinEntity> Skins { get; set; }
    public DbSet<LeagueEntity> Leagues { get; set; }


    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseEntity
            && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entity in entries)
        {
            ((BaseEntity)entity.Entity).DateModified = DateTime.Now;

            if (entity.State == EntityState.Added)
                ((BaseEntity)entity.Entity).DateCreation = DateTime.Now;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SquadEntity>()
            .HasMany(e => e.Members)
            .WithOne(e => e.Squad).HasForeignKey(e => e.SquadId);

        modelBuilder.Entity<UserEntity>()
            .HasMany(e => e.Tasks).WithMany(e => e.Users);
        modelBuilder.Entity<UserEntity>()
            .HasMany(e => e.Challenges).WithMany(e => e.Users);

        modelBuilder.Entity<TaskEntity>()
            .HasOne(e => e.Challenge).WithMany(e => e.Tasks).HasForeignKey(x => x.ChallengeId);

        modelBuilder.Entity<UserEntity>()
            .HasMany(e => e.Children)
            .WithOne(e => e.Parent).HasForeignKey(e => e.ParentId);


        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.League).WithMany().HasForeignKey(e => e.LeagueId);

        modelBuilder.Entity<SquadEntity>()
            .HasOne(e => e.League).WithMany().HasForeignKey(e => e.LeagueId);

        modelBuilder.Entity<UserEntity>().HasMany(e => e.Skins).WithMany(x => x.Users);

        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.Skin).WithMany().HasForeignKey(e => e.SkinId);

        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.LimitEnergyLevel).WithMany().HasForeignKey(e => e.LimitEnergyLevelId);

        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.MultipleClickLevel).WithMany().HasForeignKey(e => e.MultipleClickLevelId);

        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.RechargeEnergyLevel).WithMany().HasForeignKey(e => e.RechargeEnergyLevelId);

        modelBuilder.Entity<UserEntity>()
            .HasOne(e => e.RoBotLevel).WithMany().HasForeignKey(e => e.RoBotLevelId);

        modelBuilder.Entity<SquadEntity>().Navigation(e => e.League).AutoInclude();

        modelBuilder.Entity<UserEntity>().Navigation(e => e.Skin).AutoInclude();
        modelBuilder.Entity<UserEntity>().Navigation(e => e.Squad).AutoInclude();
        modelBuilder.Entity<UserEntity>().Navigation(e => e.League).AutoInclude();

        modelBuilder.Entity<UserEntity>().Navigation(e => e.RoBotLevel).AutoInclude();
        modelBuilder.Entity<UserEntity>().Navigation(e => e.LimitEnergyLevel).AutoInclude();
        modelBuilder.Entity<UserEntity>().Navigation(e => e.MultipleClickLevel).AutoInclude();
        modelBuilder.Entity<UserEntity>().Navigation(e => e.RechargeEnergyLevel).AutoInclude();


        modelBuilder.Entity<LeagueEntity>().HasData(
            new List<LeagueEntity>
            {
                new LeagueEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Bronze",
                    AvailableCoin = 0,
                    Image = "/Images/leagues/bronze-small.png"
                },
                new LeagueEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Silver",
                    AvailableCoin = 10000,
                    Image = "/Images/leagues/silver-small.png"
                },
                new LeagueEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Gold",
                    AvailableCoin = 100000,
                    Image = "/Images/leagues/gold-small.png"
                },
                new LeagueEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Platinum",
                    AvailableCoin = 1000000,
                    Image = "/Images/leagues/platinum-small.png"
                },
                new LeagueEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Diamond",
                    AvailableCoin = 10000000,
                    Image = "/Images/leagues/diamond-small.png"
                },
            }
        );

        modelBuilder.Entity<SkinEntity>().HasData(
            new List<SkinEntity>
            {
                new SkinEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Bin Coin",
                    Image = "/images/skins/default_skin.png",
                    AvailableCoin = 0,
                },
                new SkinEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = "Clown Coin",
                    Image = "/images/skins/clown_skin.png",
                    AvailableCoin = 1000000,
                },
            }
        );
        modelBuilder.Entity<LevelEntity>().HasData(
            new List<LevelEntity>
            {

                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 1,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 0,Value = 1
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 2,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 1000,Value = 2
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 3,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 10000,Value = 3
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 4,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 15000,Value = 4
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 5,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 20000,Value = 5
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 6,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 30000,Value = 6
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 7,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 60000,Value = 7
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 8,
                    LevelType = Enums.LevelTypeEnum.MultipleClickLevel,
                    AvailableCoin = 100000,Value = 8
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 1,
                    LevelType = Enums.LevelTypeEnum.RechargeEnergyLevel,
                    AvailableCoin = 0,Value = 1
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 2,
                    LevelType = Enums.LevelTypeEnum.RechargeEnergyLevel,
                    AvailableCoin = 1000,Value = 2
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 3,
                    LevelType = Enums.LevelTypeEnum.RechargeEnergyLevel,
                    AvailableCoin = 10000,Value = 3
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 4,
                    LevelType = Enums.LevelTypeEnum.RechargeEnergyLevel,
                    AvailableCoin = 20000,Value = 4
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 5,
                    LevelType = Enums.LevelTypeEnum.RechargeEnergyLevel,
                    AvailableCoin = 50000,Value = 5
                },

                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 1,
                    LevelType = Enums.LevelTypeEnum.LimitEnergyLevel,
                    AvailableCoin = 0,Value = 1000
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 2,
                    LevelType = Enums.LevelTypeEnum.LimitEnergyLevel,
                    AvailableCoin = 1000,Value = 1500
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 2,
                    LevelType = Enums.LevelTypeEnum.LimitEnergyLevel,
                    AvailableCoin = 5000,Value = 2000
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 3,
                    LevelType = Enums.LevelTypeEnum.LimitEnergyLevel,
                    AvailableCoin = 10000,Value = 5000
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 4,
                    LevelType = Enums.LevelTypeEnum.LimitEnergyLevel,
                    AvailableCoin = 30000,Value = 8000
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 5,
                    LevelType = Enums.LevelTypeEnum.LimitEnergyLevel,
                    AvailableCoin = 100000,Value = 10000
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 1,
                    LevelType = Enums.LevelTypeEnum.RoBotLevel,
                    AvailableCoin = 10000,Value = 2
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 2,
                    LevelType = Enums.LevelTypeEnum.RoBotLevel,
                    AvailableCoin = 100000,Value = 4
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 3,
                    LevelType = Enums.LevelTypeEnum.RoBotLevel,
                    AvailableCoin = 1000000,Value = 6
                },
                new LevelEntity
                {
                    Id = Guid.NewGuid().ToString(),
                    Level = 4,
                    LevelType = Enums.LevelTypeEnum.RoBotLevel,
                    AvailableCoin = 10000000,Value = 8
                },
            }
        );


        base.OnModelCreating(modelBuilder);
    }
}
