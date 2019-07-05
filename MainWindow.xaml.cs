using Newtonsoft.Json.Linq;
using Redmine.Net.Api;
using Redmine.Net.Api.Types;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;

namespace Auto_MindMeister_Backup_to_Redmine
{
    [Serializable]
    public class Connections : DbContext
    {
        public Connections() : base("DefaultConnection")
        { }
        public DbSet<Connection> ConnectionsDB { get; set; }
    }

    public struct MindCard
    {
        public string ID;
        public string name;
        public DateTime lastUpdateTime;

        public MindCard(string iD, string name, DateTime lastUpdateTime)
        {
            ID = iD ?? throw new ArgumentNullException(nameof(iD));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
            this.lastUpdateTime = lastUpdateTime;
        }
    }

    public partial class MainWindow : Window
    {
        private readonly Connections db;
        private string accessTokenMM;
        private string accessTokenRM;
        private List<MindCard> dbCards;

        public MainWindow()
        {
            InitializeComponent();
            UpdB.IsEnabled = false;
            db = new Connections();
        }

        public void GetCardsToUpdate()
        {
            if (UpdB.IsEnabled)
            {
                LeftLB.Items.Clear();
                RightLB.Items.Clear();
                if (db.ConnectionsDB.Count() == 0)
                {
                    MessageBox.Show("База данных пуста!");
                    return;
                }
                dbCards = new List<MindCard>();
                foreach (var conn in db.ConnectionsDB)
                {
                    WebRequest reqGET = WebRequest.Create(@"https://www.mindmeister.com/api/v2/maps/" + conn.MindCardNumber + "?access_token=" + accessTokenMM);
                    WebResponse resp = reqGET.GetResponse();
                    Stream stream = resp.GetResponseStream();
                    StreamReader sr = new StreamReader(stream);
                    string s = sr.ReadToEnd();
                    JObject parsed = JObject.Parse(s);
                    dbCards.Add(new MindCard(parsed[@"root_id"].ToString(),
                                             parsed[@"title"].ToString(),
                                             Convert.ToDateTime(parsed[@"updated_at"].ToString())));
                    if (Convert.ToDateTime(conn.LastUpdateDate) < dbCards.Last().lastUpdateTime)
                    {
                        string t = (string)(dbCards.Last().name).Normalize();
                        LeftLB.Items.Add(t);
                    }
                }
                if (LeftLB.Items.Count == 0)
                    MessageBox.Show("Нет новых карт для обновления!");
            }
            else
            {
                MessageBox.Show("Введите токены!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (MMToken.Text == "" || RMToken.Text == "")
            {
                MessageBox.Show("Введите токены!");
                UpdB.IsEnabled = false;
                return;
            }
            accessTokenMM = MMToken.Text;
            accessTokenRM = RMToken.Text;
            UpdB.IsEnabled = true;
            GetCardsToUpdate();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (RightLB.Items.Count == 0)
            {
                MessageBox.Show("Задайте карты для обновления!");
                return;
            }
            if (!PDFCB.IsChecked.Value && !MINDCB.IsChecked.Value)
            {
                MessageBox.Show("Задайте формат сохранения данных!");
                return;
            }
            foreach (var item in RightLB.Items)
            {
                var id = dbCards.Find(x => x.name == item.ToString()).ID;
                var res = db.ConnectionsDB.Where(b => b.MindCardNumber == id).FirstOrDefault();
                if (res != null)
                {
                    var parameters = new NameValueCollection { { RedmineKeys.INCLUDE, RedmineKeys.RELATIONS } };
                    RedmineManager manager = new RedmineManager("https://redmine.minsvyazdnr.ru", accessTokenRM);
                    var issue = manager.GetObject<Issue>(res.RedmineIssueNumber, parameters);            

                    IList<Upload> attachments = new List<Upload>();
                    if (PDFCB.IsChecked.Value) attachments.Add(CreateAttachment(res.MindCardNumber, "pdf", ref manager));
                    if (MINDCB.IsChecked.Value) attachments.Add(CreateAttachment(res.MindCardNumber, "mind", ref manager));
                    issue.Uploads = attachments;
                    manager.UpdateObject(res.RedmineIssueNumber, issue);

                    var conInDB = db.ConnectionsDB.SingleOrDefault(b => b.MindCardNumber == id);
                    if (conInDB != null)
                    {
                        conInDB.LastUpdateDate = DateTime.Now.ToString();
                        db.SaveChanges();
                    }
                }
                else
                    MessageBox.Show("bud");
            }
            MessageBox.Show("Все файлы обновлены!");
            LeftLB.Items.Clear();
            RightLB.Items.Clear();
        }

        private Upload CreateAttachment(string mindCardNumber, string type, ref RedmineManager manager)
        {
            string documentPath = mindCardNumber.ToString() + "." + type;
            using (var client = new WebClient())
            {
                client.DownloadFile(@"https://www.mindmeister.com/api/v2/maps/" + mindCardNumber + "." + type + "?access_token=" + accessTokenMM, documentPath);
            }
            byte[] documentData = System.IO.File.ReadAllBytes(documentPath);
            Upload attachment = manager.UploadFile(documentData);
            attachment.FileName = documentPath;
            attachment.Description = "Файл загружен при помощи подпрограммы автобекапирования";
            attachment.ContentType = "application/pdf";
            return attachment;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (LeftLB.SelectedItem != null)
            {
                RightLB.Items.Add(LeftLB.SelectedItem);
                LeftLB.Items.Remove(LeftLB.SelectedItem);
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (RightLB.SelectedItem != null)
            {
                LeftLB.Items.Add(RightLB.SelectedItem);
                RightLB.Items.Remove(RightLB.SelectedItem);
            }
        }
    }
}