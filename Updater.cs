using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;

namespace Auto_MindMeister_Backup_to_Redmine
{
    [Serializable]

    public class Connections : DbContext
    {
        public Connections() : base("DefaultConnection")
        {
        }
        public DbSet<Connection> ConnectionsDB { get; set; }
    }

    class Updater
    {
        private Connections db;
        private string accessTokenMM;
        private string csvPath;
        private string accessTokenRM;

        public Updater()
        {
            //connections = new List<DataUnit>();
        }

        public void Auth(string mmt, string rmt)
        {
            accessTokenMM = mmt;
            accessTokenRM = rmt;
        }

        public void GetCardAsync()
        {
            /*if (db.ConnectionsDB. .Count == 0) return;
            using (var client = new WebClient())
            {
                foreach (var conn in connections)
                {
                    client.DownloadFile("https://www.mindmeister.com/api/v2/maps/"+conn.RMProjectName+".mind?access_token=" + accessTokenMM, conn.RMProjectName+".mind");
                }
            }*/
        }

        public void GetConnections()
        {
            /*Microsoft.Win32.OpenFileDialog openFileDialog1 = new Microsoft.Win32.OpenFileDialog();
            openFileDialog1.Filter = "CSV файлы(*.csv)|*.csv";
            openFileDialog1.ShowDialog();
            csvPath = openFileDialog1.FileName;
            using (var reader = new StreamReader(csvPath, Encoding.UTF8, false))
            using (var csv = new CsvReader(reader))
            {
                var records = csv.GetRecords<DataUnit>();
                connections = new List<DataUnit>();
                connections.AddRange(records);
            }*/
        }

        public void SaveConnections()
        {
            /*using (var writer = new StreamWriter(csvPath, false, Encoding.UTF8))
            using (var csv = new CsvWriter(writer))
            {
                csv.WriteRecords(connections);
            }*/
        }

        public void UpdateTask()
        {
            RedmineManager manager = new RedmineManager("https://redmine.minsvyazdnr.ru", accessTokenRM);
            var parameters = new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.RELATIONS } };

            var issue = manager.GetObject<Issue>("56841", parameters);
            MessageBox.Show("Issue: " + issue); 

        }
    }
}
