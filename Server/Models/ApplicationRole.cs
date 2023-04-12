using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace RadzenSchoolTenants.Server.Models
{
    public partial class ApplicationRole : IdentityRole
    {
        [JsonIgnore]
        public ICollection<ApplicationUser> Users { get; set; }


        public int? TenantId { get; set; }

        [ForeignKey("TenantId")]
        public ApplicationTenant ApplicationTenant { get; set; }
    }


    [Table("AspNetTenants")]
    public partial class ApplicationTenant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id  { get; set; }

        public ICollection<ApplicationUser> Users { get; set; }

        public ICollection<ApplicationRole> Roles { get; set; }

        public string Name { get; set; }

        public string Hosts { get; set; }
    }
}