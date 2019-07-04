using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
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
        private const string apikey = "820cfae6eb216a92f7c9bec42a668b92";
        private const string apisecret = "9e4cb8b2e708cf1e";
        private Connections db;
        private string accessTokenMM;
        private string accessTokenRM;
        private string authTokenMM;

        public Updater()
        {
            //connections = new List<DataUnit>();
        }

        public void Auth(string mmt, string rmt)
        {
            accessTokenMM = mmt;
            accessTokenRM = rmt;
        }

        public void GetCardsToUpdate()
        {
            /*if (db.ConnectionsDB.Count() == 0)
            {
                MessageBox.Show("База данных пуста!");
                return;
            }*/
            string URLString = "https://www.mindmeister.com/services/rest?api_key=" + apikey + "&method=mm.auth.getFrob&response_format=xml";
            URLString = GenerateSignature(URLString);
            string res = "";
            using (var client = new WebClient()) // to download card
            {
                res = client.DownloadString(URLString);
            }
            MessageBox.Show(res);
            var xDoc = XDocument.Parse(res);
            string frob = xDoc.Root.Element("frob").Value;

            //URLString = "http://www.mindmeister.com/services/auth/?api_key=" + apikey + "&perms=delete";// &api_sig=zxy987";
            URLString = "https://www.mindmeister.com/services/rest?api_key=" + apikey + "&frob=" + frob + "&method=mm.auth.getToken&response_format=xml";// &api_sig=zxy987"
            URLString = GenerateSignature(URLString);
            MessageBox.Show(URLString);
            using (var client = new WebClient()) // to download card
            {
                res = client.DownloadString(URLString);
            }
            /*xDoc = XDocument.Parse(res);
            foreach (var el in xDoc.Root.Elements())
                MessageBox.Show(el.ToString());*/
            //authTokenMM = xDoc.Root.Element("frob").Value;
            MessageBox.Show(frob + "\n" + res);

            //URLString = "https://www.mindmeister.com/services/rest?api_key="+ apikey +" &auth_token=KykOYg01ni3PNScRqXEo&method=mm.auth.getToken&response_format=xml&api_sig=b5d688daab8d236d7bbbfd1e2443467a";
            
            /*URLString = "api_key=" + apikey + "&auth_token=" + accessTokenMM + "&method=mm.maps.getList&response_format=xml";
            string hash = GenerateSignature(URLString);
            URLString = "https://www.mindmeister.com/services/rest?" + URLString + "&api_sig=" + hash;
                        
            MessageBox.Show(URLString);
            MessageBox.Show(res);*/
            /*using (var client = new WebClient()) // to download card
            {
                foreach (var conn in db.ConnectionsDB)
                {
                    client.DownloadFile("https://www.mindmeister.com/api/v2/maps/"+conn.MindCardNumber+".mind?access_token=" + accessTokenMM, conn.RMProjectName+".mind");
                }
            }*/
        }

        private string MD5Hash(string apisig)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(apisig));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
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

        private string GenerateSignature(string parames)
        {
            string hash = parames.Remove(0, parames.IndexOf("?")+1);
            hash = hash.Replace("&", "");
            hash = hash.Replace("=", "");
            hash = apisecret + hash;            
            hash = MD5Hash(hash);
            parames = parames + "&api_sig=" + hash;
            return parames;
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
