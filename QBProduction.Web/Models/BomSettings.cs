using FluentNHibernate.Mapping;

namespace QBProduction.Web.Models
{
    public class BomSettings
    {
        public virtual int Id { get; set; }
        public virtual int bomrefno { get; set; }
        public virtual string bomcode { get; set; }
        public virtual int stockrefno { get; set; }
        public virtual string stockcode { get; set; }
    }

    public class BomSettingsMap : ClassMap<BomSettings>
    {
        public BomSettingsMap()
        {
            Id(x => x.Id);
            Map(x => x.bomrefno);
            Map(x => x.bomcode);
            Map(x => x.stockcode);
            Map(x => x.stockrefno);
            Table("BomSettings");
        }
    }
}
