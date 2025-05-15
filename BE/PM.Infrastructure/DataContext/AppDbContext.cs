using Microsoft.EntityFrameworkCore;
using PM.Domain.Entities;
using System.Threading.Tasks;

namespace PM.Infrastructure.DataContext
{
    public class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Permissions> Permissions { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<ConfirmEmail> ConfirmEmails { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Project> Projects { get; set; }

        public virtual DbSet<Design> Designs { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<PrintJobs> PrintJobs { get; set; }
        public virtual DbSet<ResourceForPrintJob> ResourceForPrintJobs { get; set; }
        public virtual DbSet<ResourcePropertyDetail> ResourcePropertyDetail { get; set; }
        public virtual DbSet<ResourceProperty> ResourceProperties { get; set; }
        public virtual DbSet<Resources> Resources { get; set; }

        public virtual DbSet<Delivery> Deliveries { get; set; }
        public virtual DbSet<ShippingMethod> ShippingMethods { get; set; }

        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Step> Step { get; set; }
        public virtual DbSet<StepTemplate> StepTemplates { get; set; }
        public virtual DbSet<FlowTemplate> FlowTemplates { get; set; }
        public async Task<int> CommitChangeAsync()
        {
            return await SaveChangesAsync();
        }

        public DbSet<TEntity> SetEntity<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                modelBuilder.Entity<Team>()
                .HasOne(t => t.Manager)
                .WithMany()
                .HasForeignKey(t => t.ManagerId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Team)
                .WithMany(t => t.Users) 
                .HasForeignKey(u => u.TeamId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(r => r.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ConfirmEmail>()
                .HasOne(c => c.User)
                .WithMany(u => u.ConfirmEmails)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Permissions>()
                .HasOne(p => p.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Permissions>()
                .HasOne(p => p.Role)
                .WithMany(r => r.Permissions)
                .HasForeignKey(p => p.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectStatus)
                .HasConversion(
                v => v.ToString(),
                v => (ProjectStatus)Enum.Parse(typeof(ProjectStatus), v));

            modelBuilder.Entity<Design>()
                .HasOne(d => d.Project)
                .WithMany()
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Design>()
                .HasOne(d => d.Designer)
                .WithMany()
                .HasForeignKey(d => d.DesignerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .Property(p => p.ProjectStatus)
                .HasConversion(
                v => v.ToString(),
                v => (ProjectStatus)Enum.Parse(typeof(ProjectStatus), v));

            modelBuilder.Entity<Design>()
                .Property(d => d.DesignStatus)
                .HasConversion(
                v => v.ToString(),
                v => (DesignStatus)Enum.Parse(typeof(DesignStatus), v));
            modelBuilder.Entity<Resources>()
                .Property(r => r.ResourceType)
                .HasConversion(
                v => v.ToString(),
                v => (ResourceType)Enum.Parse(typeof(ResourceType), v));

            modelBuilder.Entity<Resources>()
                .Property(r => r.ResourceStatus)
                .HasConversion(
                v => v.ToString(),
                v => (ResourceStatus)Enum.Parse(typeof(ResourceStatus), v));
            modelBuilder.Entity<PrintJobs>()
                .Property(pj => pj.PrintJobStatus)
                .HasConversion(
                v => v.ToString(),
                v => (PrintJobStatus)Enum.Parse(typeof(PrintJobStatus), v));
            modelBuilder.Entity<Delivery>()
                .Property(pj => pj.DeliveryStatus)
                .HasConversion(
                v => v.ToString(),
                v => (DeliveryStatus)Enum.Parse(typeof(DeliveryStatus), v));
  
            base.OnModelCreating(modelBuilder);
        }
    }
}
