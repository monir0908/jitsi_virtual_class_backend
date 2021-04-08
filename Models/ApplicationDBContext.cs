using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Commander.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Core Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Core Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // https://stackoverflow.com/questions/49326769/entity-framework-core-delete-cascade-and-required/49326983

            // builder.Entity<ApplicationUser>(entity => {entity.ToTable(name:"IdentityUser"); });
            // builder.Entity<IdentityRole>(entity => {entity.ToTable(name:"IdentityRole"); });
            



            // modelBuilder.Entity<ApplicationUser>().ToTable("IdentityUser");
            // //AspNetRoles -> Role
            // modelBuilder.Entity<IdentityRole>().ToTable("IdentityRole");
            // //AspNetUserRoles -> UserRole
            // modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            // //AspNetUserClaims -> UserClaim
            // modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            // //AspNetUserLogins -> UserLogin
            // modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");
            // modelBuilder.Entity<Conference>().HasRequired(s => s.Participant).WithMany().WillCascadeOnDelete(false);
            // modelBuilder.Entity<BatchHost>().HasRequired(s => s.Project).WithMany().WillCascadeOnDelete(false);
            // modelBuilder.Entity<BatchHostParticipant>().HasRequired(s => s.Host).WithMany().WillCascadeOnDelete(false);
            // modelBuilder.Entity<BatchHostParticipant>().HasRequired(s => s.Project).WithMany().WillCascadeOnDelete(false);

            // modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                    relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

        }


        
        #region DbSet
        public DbSet<Project> Project { get; set; }
        public DbSet<Batch> Batch { get; set; }


        public DbSet<ProjectBatch> ProjectBatch { get; set; }
        public DbSet<ProjectBatchHost> ProjectBatchHost { get; set; }
        public DbSet<ProjectBatchHostParticipant> ProjectBatchHostParticipant { get; set; }



        public DbSet<VClass> VClass{ get; set; }
        public DbSet<VClassInvitation> VClassInvitation{ get; set; }
        public DbSet<VClassDetail> VClassDetail { get; set; }


        public DbSet<Conference> Conference{ get; set; }
        public DbSet<ConferenceHistory> ConferenceHistory { get; set; }

        public DbSet<HeadRoles> HeadRoles { get; set; }
        public DbSet<HeadRoles_Roles> HeadRoles_Roles { get; set; }

        


        #endregion

        
    }
}