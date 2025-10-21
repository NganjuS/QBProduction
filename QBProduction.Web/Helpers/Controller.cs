using System;
using System.IO;
using QBProduction.Web.Models;
using QBProduction.Web.Data;
using System.Linq;

namespace QBProduction.Web.Helpers
{
    public class Controller
    {
        public static string ReturnConnection()
        {
            string filename = @"C:\QBProd\dbsettings.inf";
            string servername = "";
            string userid = "";
            string pwd = "";
            string dbname = "";

            if (!File.Exists(filename))
                return "";

            using (StreamReader streamReader = File.OpenText(filename))
            {
                string str;
                while ((str = streamReader.ReadLine()) != null)
                {
                    string[] strArray = str.Split(':');
                    if (strArray[0] == "server")
                        servername = strArray[1];
                    else if (strArray[0] == "uid")
                        userid = strArray[1];
                    else if (strArray[0] == "pwd")
                        pwd = strArray[1];
                    else if (strArray[0] == "database")
                        dbname = strArray[1];
                }
            }
            return $"server={servername};uid={userid};pwd={pwd};database={dbname};persistsecurityinfo=True;";
        }

        public static T GetOneRecord<T>() where T : class
        {
            using (var db = new QBProductionContext())
            {
                return db.Set<T>().FirstOrDefault();
            }
        }

        public static IQueryable<T> GetAllRecords<T>() where T : class
        {
            using (var db = new QBProductionContext())
            {
                return db.Set<T>();
            }
        }
    }
}
