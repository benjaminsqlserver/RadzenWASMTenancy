using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RadzenSchoolTenants.Server.Models.ConData
{
    [Table("Schools", Schema = "dbo")]
    public partial class School
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
        public long SchoolID { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string SchoolName { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string StreetAddress { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Email { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string MobileNumber { get; set; }

        public ICollection<Student> Students { get; set; }

    }
}