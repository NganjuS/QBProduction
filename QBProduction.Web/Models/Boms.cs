using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QBProduction.Web.Models
{
    [Table("Boms")]
    public class Boms
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(255)]
        public string bomname { get; set; }

        [StringLength(255)]
        public string assemblylistid { get; set; }

        [StringLength(255)]
        public string assemblyitem { get; set; }

        [StringLength(50)]
        public string uom { get; set; }

        public bool isactive { get; set; }

        public DateTime createdon { get; set; }

        public DateTime modifiedon { get; set; }

        [StringLength(100)]
        public string createdby { get; set; }

        [StringLength(100)]
        public string modifiedby { get; set; }

        // Navigation property
        public virtual ICollection<BomItems> BomItems { get; set; }

        public Boms()
        {
            BomItems = new HashSet<BomItems>();
            isactive = true;
            createdon = DateTime.Now;
            modifiedon = DateTime.Now;
        }
    }
}
