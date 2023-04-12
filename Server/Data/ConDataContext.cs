using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using RadzenSchoolTenants.Server.Models.ConData;

namespace RadzenSchoolTenants.Server.Data
{
    public partial class ConDataContext : DbContext
    {
        public ConDataContext()
        {
        }

        public ConDataContext(DbContextOptions<ConDataContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AddNewUserToAppRole>().HasNoKey();

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>().HasKey(table => new {
                table.LoginProvider, table.ProviderKey
            });

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>().HasKey(table => new {
                table.UserId, table.RoleId
            });

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>().HasKey(table => new {
                table.UserId, table.LoginProvider, table.Name
            });

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.Student>()
              .HasOne(i => i.School)
              .WithMany(i => i.Students)
              .HasForeignKey(i => i.SchoolID)
              .HasPrincipalKey(i => i.SchoolID);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim>()
              .HasOne(i => i.AspNetRole)
              .WithMany(i => i.AspNetRoleClaims)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetRole>()
              .HasOne(i => i.AspNetTenant)
              .WithMany(i => i.AspNetRoles)
              .HasForeignKey(i => i.TenantId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserClaims)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserLogins)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>()
              .HasOne(i => i.AspNetRole)
              .WithMany(i => i.AspNetUserRoles)
              .HasForeignKey(i => i.RoleId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserRoles)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUser>()
              .HasOne(i => i.AspNetTenant)
              .WithMany(i => i.AspNetUsers)
              .HasForeignKey(i => i.TenantId)
              .HasPrincipalKey(i => i.Id);

            builder.Entity<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken>()
              .HasOne(i => i.AspNetUser)
              .WithMany(i => i.AspNetUserTokens)
              .HasForeignKey(i => i.UserId)
              .HasPrincipalKey(i => i.Id);
            this.OnModelBuilding(builder);
        }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.School> Schools { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.Student> Students { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetRoleClaim> AspNetRoleClaims { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetRole> AspNetRoles { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetTenant> AspNetTenants { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserClaim> AspNetUserClaims { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserLogin> AspNetUserLogins { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserRole> AspNetUserRoles { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetUser> AspNetUsers { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AspNetUserToken> AspNetUserTokens { get; set; }

        public DbSet<RadzenSchoolTenants.Server.Models.ConData.AddNewUserToAppRole> AddNewUserToAppRoles { get; set; }
    }
}