using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBProduction.Web.Models
{
    [Table("BomRun")]
    public class BomRun
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        [Index(IsUnique = true)]
        public string bomrunref { get; set; }

        [StringLength(100)]
        public string transactionid { get; set; }

        [StringLength(50)]
        public string batchstatus { get; set; }

        [StringLength(255)]
        public string productionitem { get; set; }

        [StringLength(50)]
        public string uom { get; set; }

        public double totalqtyproduced { get; set; }

        public double totalvalue { get; set; }

        [StringLength(1000)]
        public string comments { get; set; }

        public DateTime bomrundate { get; set; }

        [StringLength(100)]
        public string processedby { get; set; }

        // Navigation property
        public virtual ICollection<BomRunItems> BomRunItems { get; set; }

        public BomRun()
        {
            BomRunItems = new HashSet<BomRunItems>();
            bomrundate = DateTime.Now;
            batchstatus = "Pending";
        }
    }
}
