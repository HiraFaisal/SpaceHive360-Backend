using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace SpaceHive360.Admin.Domain.Entities
{
    [Table("tbl_plans")]
    public class Plan
    {
        [Key]
        [Column("rec_id")]
        public Guid RecId { get; set; } = Guid.NewGuid();

        [Column("fk_company")]
        [Required]
        public Guid FkCompany { get; set; }

        [Column("fk_workspace_type")]
        [Required]
        public Guid FkWorkspaceType { get; set; }

        [Column("fk_location")]
        [Required]
        public Guid FkLocation { get; set; }

        [Column("fk_city")]
        [Required]
        public Guid FkCity { get; set; }

        [Column("fk_payment_term")]
        public Guid? FkPaymentTerm { get; set; }

        [Column("name")]
        [Required]
        public string Name { get; set; } = null!;

        [Column("description")]
        public string? Description { get; set; }

        [Column("duration_months")]
        [Required]
        public int DurationMonths { get; set; }

        [Column("price")]
        [Required]
        public decimal Price { get; set; }

        [Column("is_recurring")]
        public bool IsRecurring { get; set; } = false;

        [Column("allow_cancellation")]
        public bool AllowCancellation { get; set; } = false;

        [Column("requires_approval")]
        public bool RequiresApproval { get; set; } = false;

        // JSONB fields
        //[Column("images")]
        public string? ImagesJson { get; set; }

        //[Column("features")]
        public string? FeaturesJson { get; set; }

        [NotMapped]
        public List<string>? Images
        {
            get => string.IsNullOrEmpty(ImagesJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(ImagesJson);

            set => ImagesJson = JsonConvert.SerializeObject(value);
        }

        [NotMapped]
        public List<string>? Features
        {
            get => string.IsNullOrEmpty(FeaturesJson)
                ? new List<string>()
                : JsonConvert.DeserializeObject<List<string>>(FeaturesJson);

            set => FeaturesJson = JsonConvert.SerializeObject(value);
        }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        [Column("created_by")]
        public Guid? CreatedBy { get; set; }

        [Column("updated_by")]
        public Guid? UpdatedBy { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}