using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RadzenSchoolTenants.Server.Models.ConData
{
    [Table("AspNetTenants", Schema = "dbo")]
    public partial class AspNetTenant
    {

        [NotMapped]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("@odata.etag")]
        public string ETag
        {
                get;
                set;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Name { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Hosts { get; set; }

        public ICollection<AspNetRole> AspNetRoles { get; set; }

        public ICollection<AspNetUser> AspNetUsers { get; set; }

    }
}