using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Auto_MindMeister_Backup_to_Redmine
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Updater upd;
        private Connections db;
        private string accessTokenMM;
        private string accessTokenRM;
        private string authTokenMM;
        public MainWindow()
        {
            InitializeComponent();
            db = new Connections();
            upd = new Updater();
        }

        public void GetCardsToUpdate()
        {
            if (db.ConnectionsDB.Count() == 0)
            {
                MessageBox.Show("База данных пуста!");
                return;
            }
            foreach (var conn in db.ConnectionsDB)
            {
                WebRequest reqGET = WebRequest.Create(@"https://www.mindmeister.com/api/v2/maps/" + conn.MindCardNumber + "?access_token=" + accessTokenMM);
                WebResponse resp = reqGET.GetResponse();
                Stream stream = resp.GetResponseStream();
                StreamReader sr = new StreamReader(stream);
                string s = sr.ReadToEnd();
                JObject parsed = JObject.Parse(s);
                if (Convert.ToDateTime(conn.LastUpdateDate) < Convert.ToDateTime(parsed.Property(@"updated_at\")))
                    LeftLB.Items.Add(conn.MindCardNumber);
                //add code to analyze last update time and if bad - add to leftLB
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            accessTokenMM = MMToken.Text;
            accessTokenRM = RMToken.Text;
            GetCardsToUpdate();
        }
    }
}
