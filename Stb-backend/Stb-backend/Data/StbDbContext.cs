using Microsoft.EntityFrameworkCore;
using stb_backend.Domain;

namespace stb_backend.Data
{

    public class StbDbContext : DbContext
    {
        public StbDbContext(DbContextOptions<StbDbContext> options) : base(options) { }

        public DbSet<DeclarationCadeau> DeclarationsCadeaux { get; set; }
        public DbSet<DeclarationCorruption> DeclarationsCorruption { get; set; }
        public DbSet<DemandeConseil> DemandesConseils { get; set; }
        public DbSet<Employe> Employes { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<DocumentFile> DocumentFiles { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Héritage TPT
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Employe>().ToTable("Employes");
            modelBuilder.Entity<Manager>().ToTable("Managers");


            // Stocker les enums en string
            modelBuilder.Entity<DeclarationCadeau>()
                .Property(d => d.Statut)
                .HasConversion<string>();
            modelBuilder.Entity<DeclarationCadeau>()
                .Property(d => d.TypeRelation)
                .HasConversion<string>();

            modelBuilder.Entity<DeclarationCorruption>()
                .Property(d => d.Statut)
                .HasConversion<string>();
            modelBuilder.Entity<DeclarationCorruption>()
                .Property(d => d.TypeDomaine)
                .HasConversion<string>();

            modelBuilder.Entity<DemandeConseil>()
                .Property(d => d.Statut)
                .HasConversion<string>();
            modelBuilder.Entity<DocumentFile>()
        .HasOne(df => df.DeclarationCadeaux)
        .WithMany(dc => dc.DocumentFiles)
        .HasForeignKey(df => df.IdCadeaux)
        .OnDelete(DeleteBehavior.Cascade); // ou Restrict si tu préfères

            // Tu peux faire pareil pour Corruption et Conseil :
            modelBuilder.Entity<DocumentFile>()
                .HasOne(df => df.DeclarationCorruption)
                .WithMany() // si tu ne veux pas navigation inverse
                .HasForeignKey(df => df.IdCorruption);

            modelBuilder.Entity<DocumentFile>()
                .HasOne(df => df.DemandeConseil)
                .WithMany() // pareil
                .HasForeignKey(df => df.IdConseil);
        }
    }
}