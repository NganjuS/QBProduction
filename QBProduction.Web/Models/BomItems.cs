using FluentNHibernate.Mapping;

namespace QBProduction.Web.Models
{
    public class BomItems
    {
        public virtual int Id { get; set; }
        public virtual string itemid { get; set; }
        public virtual decimal cost { get; set; }
        public virtual decimal qty { get; set; }
        public virtual Boms bomid { get; set; }
        public virtual string uom { get; set; }
    }

    public class BomItemsMap : ClassMap<BomItems>
    {
        public BomItemsMap()
        {
            Id(x => x.Id);
            Map(x => x.itemid);
            Map(x => x.cost);
            Map(x => x.qty);
            Map(x => x.uom);
            References(x => x.bomid);
        }
    }
}
