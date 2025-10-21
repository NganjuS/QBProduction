using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBProduction.Web.Models
{
    [Table("BomRunItems")]
    public class BomRunItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        public string itemid { get; set; }

        [StringLength(255)]
        public string product { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal qtyavailable { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal qtyperunit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal valueperunit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal totalqtyused { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal totalvalue { get; set; }

        [StringLength(50)]
        public string uom { get; set; }

        // Foreign key
        public int? BomRunId { get; set; }

        // Navigation property
        [ForeignKey("BomRunId")]
        public virtual BomRun bomid { get; set; }
    }
}
