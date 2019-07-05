using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Auto_MindMeister_Backup_to_Redmine
{
    public class Connection : INotifyPropertyChanged
    {
        private string mindCardNum;
        private string redmineIssue;
        private string date;

        public Connection() { }

        public Connection(string mindCardNum, string redmineIssue, string date)
        {
            this.mindCardNum = mindCardNum ?? throw new ArgumentNullException(nameof(mindCardNum));
            this.redmineIssue = redmineIssue ?? throw new ArgumentNullException(nameof(redmineIssue));
            this.date = date ?? throw new ArgumentNullException(nameof(date));
        }

        public int ID { get; set; }

        public string MindCardNumber
        {
            get { return mindCardNum; }
            set
            {
                mindCardNum = value;
                OnPropertyChanged("MindCardNumber");
            }
        }

        public string RedmineIssueNumber
        {
            get { return redmineIssue; }
            set
            {
                redmineIssue = value;
                OnPropertyChanged("RedmineIssueNumber");
            }
        }

        public string LastUpdateDate
        {
            get { return date; }
            set
            {
                date = value;
                OnPropertyChanged("LastUpdateDate");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
