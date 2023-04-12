using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using RadzenSchoolTenants.Server.Models;

namespace RadzenSchoolTenants.Server.Data
{
    public partial class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : base(options)
        {
        }

        public ApplicationIdentityDbContext()
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                   .HasMany(u => u.Roles)
                   .WithMany(r => r.Users)
                   .UsingEntity<IdentityUserRole<string>>();


            builder.Entity<ApplicationUser>()
                .HasOne(i => i.ApplicationTenant)
                .WithMany(i => i.Users)
                .HasForeignKey(i => i.TenantId)
                .HasPrincipalKey(i => i.Id);

            builder.Entity<ApplicationRole>()
                .HasOne(i => i.ApplicationTenant)
                .WithMany(i => i.Roles)
                .HasForeignKey(i => i.TenantId)
                .HasPrincipalKey(i => i.Id);
            this.OnModelBuilding(builder);
        }

        public DbSet<ApplicationTenant> Tenants
        {
            get;
            set;
        }

        public async Task SeedTenantsAdmin()
        {
            var user = new ApplicationUser
            {
                UserName = "tenantsadmin",
                NormalizedUserName = "TENANTSADMIN",
                Email = "tenantsadmin",
                NormalizedEmail = "TENANTSADMIN",
                EmailConfirmed = true,
                LockoutEnabled = false,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            if (!this.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new Microsoft.AspNetCore.Identity.PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "tenantsadmin");
                user.PasswordHash = hashed;
                var userStore = new UserStore<ApplicationUser>(this);
                await userStore.CreateAsync(user);
            }

            await this.SaveChangesAsync();
        }
    }

    public class MultiTenancyUserStore : UserStore<ApplicationUser, ApplicationRole, ApplicationIdentityDbContext>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public MultiTenancyUserStore(IHttpContextAccessor httpContextAccessor, ApplicationIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        private ApplicationTenant GetTenant()
        {
            var tenants = Context.Set<ApplicationTenant>().ToList();

            var host = httpContextAccessor.HttpContext.Request.Host.Value;

            return tenants.Where(t => t.Hosts.Split(',').Where(h => h.Contains(host)).Any()).FirstOrDefault();
        }

        protected override async Task<ApplicationRole> FindRoleAsync(string normalizedRoleName, System.Threading.CancellationToken cancellationToken)
        {
            var tenant = GetTenant();
            ApplicationRole role = null;

            if (tenant != null)
            {
                role = await Context.Set<ApplicationRole>().SingleOrDefaultAsync(r => r.NormalizedName == normalizedRoleName && r.TenantId == tenant.Id, cancellationToken);
            }

            return role;
        }

        public override async Task<ApplicationUser> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            if (normalizedName.ToLower() == "tenantsadmin")
            {
                return await base.FindByNameAsync(normalizedName, cancellationToken);
            }

            var tenant = GetTenant();
            ApplicationUser role = null;

            if (tenant != null)
            {
                role = await Context.Set<ApplicationUser>().SingleOrDefaultAsync(r => r.NormalizedUserName == normalizedName && r.TenantId == tenant.Id, cancellationToken);
            }

            return role;
        }
    }
}