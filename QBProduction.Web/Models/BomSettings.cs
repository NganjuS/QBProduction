using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBProduction.Web.Models
{
    [Table("BomSettings")]
    public class BomSettings
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int bomrefno { get; set; }

        [StringLength(50)]
        public string bomcode { get; set; }

        public int stockrefno { get; set; }

        [StringLength(50)]
        public string stockcode { get; set; }
    }
}
