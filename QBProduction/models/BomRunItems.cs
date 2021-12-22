using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QBProduction
{
    public class BomRunItems
    {
        public virtual int Id { get; set; }
        public virtual string itemid { get; set; }
        public virtual string product { get; set; }
        public virtual decimal qtyavailable { get; set; }
        public virtual decimal cost { get; set; }
        public virtual decimal qtyperunit { get; set; }
        public virtual decimal valueperunit { get; set; }
        public virtual decimal totalqtyused { get; set; }
        public virtual decimal totalvalue { get; set; }
        public virtual BomRun bomid { get; set; }
        public virtual string uom { get; set; }
    }
    public class BomRunItemsMap : ClassMap<BomRunItems>
    {
        public BomRunItemsMap()
        {
            Id(x => x.Id);
            Map(x => x.itemid);
            Map(x => x.product);
            Map(x => x.qtyavailable);
            Map(x => x.valueperunit);
            Map(x => x.cost);
            Map(x => x.totalqtyused);
            Map(x => x.qtyperunit);
            Map(x => x.totalvalue);
            Map(x => x.uom);
            References(x => x.bomid);


        }

    }
}
