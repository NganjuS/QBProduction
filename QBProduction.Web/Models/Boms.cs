using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;

namespace QBProduction.Web.Models
{
    public class Boms
    {
        public virtual int Id { get; set; }
        public virtual string bomname { get; set; }
        public virtual string assemblylistid { get; set; }
        public virtual string assemblyitem { get; set; }
        public virtual string uom { get; set; }
        public virtual bool isactive { get; set; }
        public virtual DateTime createdon { get; set; }
        public virtual DateTime modifiedon { get; set; }
        public virtual string createdby { get; set; }
        public virtual string modifiedby { get; set; }
        public virtual IList<BomItems> _bomitems { get; set; }
    }

    public class BomsMap : ClassMap<Boms>
    {
        public BomsMap()
        {
            Id(x => x.Id);
            Map(x => x.bomname);
            Map(x => x.assemblylistid);
            Map(x => x.assemblyitem);
            Map(x => x.isactive);
            Map(x => x.uom);
            Map(x => x.createdon);
            Map(x => x.modifiedon);
            Map(x => x.createdby);
            Map(x => x.modifiedby);
            HasMany(x => x._bomitems);
            Table("Boms");
        }
    }
}
