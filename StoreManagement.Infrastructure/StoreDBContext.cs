using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StoreManagement.Bases;
using StoreManagement.Domain.Entities;
using System.Reflection;
using System.Security.Claims;

namespace StoreManagement.Infrastructure;

public class StoreDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public StoreDbContext(DbContextOptions<StoreDbContext> options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Ignore<IdentityUserClaim<Guid>>();
        builder.Ignore<IdentityRoleClaim<Guid>>();

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
        ConfigureIdentity(builder);
    }

    private static void ConfigureIdentity(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>(b =>
        {
            b.HasMany(a => a.UserRoles).WithOne(a => a.User).HasForeignKey(a => a.UserId);
            b.Property(a => a.Email_Token).HasMaxLength(20);
        });

        builder.Entity<ApplicationRole>(b =>
        {
            b.HasMany(a => a.UserRoles).WithOne(a => a.Role).HasForeignKey(a => a.RoleId);
        });
        builder.Entity<ApplicationUserRole>(b =>
        {
            b.HasKey(a => new { a.UserId, a.RoleId });
            b.HasOne(a => a.User).WithMany(a => a.UserRoles).HasForeignKey(a => a.UserId);
            b.HasOne(a => a.Role).WithMany(a => a.UserRoles).HasForeignKey(a => a.RoleId);
        });
    }
    #region Tables
    //  public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Category> Category { get; set; }
    public virtual DbSet<SubCategory> SubCategories { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderProduct> ProductsProducts { get; set; }
    public virtual DbSet<Client> Clients { get; set; }
    public virtual DbSet<Phone> Phones { get; set; }
    public virtual DbSet<Image> Images { get; set; }

    #endregion

    #region AuditSaveChanges




    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var superAdminId = SeedDatabase.SuperAdminId;
        var currentUserId = _httpContextAccessor.HttpContext?.User?.Claims?.Where(x => x.Type == ClaimTypes.NameIdentifier).FirstOrDefault()?.Value;


        foreach (var entry in ChangeTracker.Entries<BaseCommonEntity<Guid>>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Created_By = !string.IsNullOrEmpty(currentUserId) ? Guid.Parse(currentUserId) : Guid.Parse(superAdminId);
                    entry.Entity.Creation_Time = DateTime.UtcNow;

                    break;

                case EntityState.Modified:
                    entry.Entity.Last_Modify_By = entry.Entity.Last_Modify_By != null ? entry.Entity.Last_Modify_By : Guid.Parse(currentUserId ?? superAdminId);
                    entry.Entity.Last_Modify_Time = DateTime.UtcNow;
                    break;

            }
        }
        if (ChangeTracker.Entries<ApplicationUser>().Any())
        {
            foreach (var entry in ChangeTracker.Entries<ApplicationUser>().ToHashSet())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created_By = Guid.Parse(currentUserId ?? superAdminId);
                        entry.Entity.Creation_Time = DateTime.UtcNow;

                        break;

                    case EntityState.Modified:
                        entry.Entity.Last_Modify_By = entry.Entity.Last_Modify_By != null ? entry.Entity.Last_Modify_By : Guid.Parse(currentUserId ?? superAdminId);
                        entry.Entity.Last_Modify_Time = DateTime.UtcNow;

                        break;
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }





    #endregion
}

