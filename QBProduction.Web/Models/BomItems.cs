using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBProduction.Web.Models
{
    [Table("BomItems")]
    public class BomItems
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        public string itemid { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal cost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal qty { get; set; }

        [StringLength(50)]
        public string uom { get; set; }

        // Foreign key
        public int? BomsId { get; set; }

        // Navigation property
        [ForeignKey("BomsId")]
        public virtual Boms bomid { get; set; }
    }
}
