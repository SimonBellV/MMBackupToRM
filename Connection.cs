using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Auto_MindMeister_Backup_to_Redmine
{
    class Connection : INotifyPropertyChanged
    {
        private string mindCardNum;
        private string redmineIssue;
        private string date;

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
