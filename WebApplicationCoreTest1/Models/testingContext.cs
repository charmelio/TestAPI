using Microsoft.EntityFrameworkCore;

namespace WebApplicationCoreTest1.Models
{
    public partial class testingContext : DbContext
    {
        public DbSet<WebApplicationCoreTest1.Models.Account> Account { get; set; }
        public virtual DbSet<Character> Character { get; set; }
        public virtual DbSet<Class> Class { get; set; }

        public testingContext()
        {
        }

        public testingContext(DbContextOptions<testingContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL("server=localhost;user id=root;password=e3zXNMIIrnWHGmPMO71M;database=testing");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Character>(entity =>
            {
                entity.ToTable("character", "testing");

                entity.HasIndex(e => e.CharacterId)
                    .HasName("CharacterId_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.ClassId)
                    .HasName("ClassId_idx");

                entity.HasIndex(e => e.Name)
                    .HasName("Name_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.CharacterId).HasColumnType("int(10) unsigned");

                entity.Property(e => e.Agility)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Dexterity)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Experience)
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Intellegence)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Level)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Strength)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Vitality)
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.HasOne(d => d.Class)
                    .WithMany(p => p.Character)
                    .HasForeignKey(d => d.ClassId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClassId");
            });

            modelBuilder.Entity<Class>(entity =>
            {
                entity.ToTable("class", "testing");

                entity.HasIndex(e => e.ClassId)
                    .HasName("Id_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.ClassId)
                    .HasColumnType("int(11)")
                    .ValueGeneratedNever();

                entity.Property(e => e.ClassName)
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });
        }
    }
}