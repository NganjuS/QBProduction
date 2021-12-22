using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBProduction
{
    public class BomRun
    {
        public virtual int Id { get; set; }
        public virtual string bomrunref { get; set; }
        public virtual string transactionid { get; set; }
        public virtual string batchstatus { get; set; }
        public virtual string productionitem { get; set; }
        public virtual string uom { get; set; }
        public virtual double totalqtyproduced { get; set; }
        public virtual double totalvalue { get; set; }
        public virtual string comments { get; set; }
        public virtual DateTime bomrundate { get; set;}
        public virtual IList<BomRunItems> _bomrunsitems { get; set; }
        public virtual string processedby { get; set; }

    }

   public  class BomRunMap : ClassMap<BomRun>
    {
        public BomRunMap()
        {
            Id(x => x.Id);
            Map(x => x.bomrunref).Unique();
            Map(x => x.bomrundate);
            Map(x => x.transactionid);
            Map(x => x.productionitem);
            Map(x => x.uom);
            Map(x => x.totalqtyproduced);
            Map(x => x.totalvalue);
            Map(x => x.comments);
            Map(x => x.batchstatus);
            Map(x => x.processedby);
            HasMany(x => x._bomrunsitems).Cascade.Delete().Inverse();

        }
    }
}
