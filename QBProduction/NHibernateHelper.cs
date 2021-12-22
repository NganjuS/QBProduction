using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Configuration;


namespace QBProduction
{
    class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;
        private static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                    InitializeSessionFactory(); return _sessionFactory;
                
            }
        }
        private static void InitializeSessionFactory()
        {
            //"server=127.0.0.1;uid=root;pwd=12345;database=test" "server=127.0.0.1;uid=root;pwd=metacity;database=qbproduction"
            _sessionFactory = Fluently.Configure()
            .Database(MySQLConfiguration.Standard
            .ConnectionString(Controller.ReturnConnection()).ShowSql())
            .Mappings(m => m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly())).ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false,true))
            .BuildSessionFactory();


        }
        /* very important code
         *private static void BuildSchema(Configuration config, bool create = false, bool update = false) {  
        if (create) {  
            new SchemaExport(config).Create(false, true);  
        } else {  
            new SchemaUpdate(config).Execute(false, update);  
        }  
         */
        public static ISession OpenSession()
        {
          
            return SessionFactory.OpenSession();
        }

    }
}
