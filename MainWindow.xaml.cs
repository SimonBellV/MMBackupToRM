using System;
using System.Collections.Generic;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
            upd = new Updater();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            upd.Auth(MMToken.Text, RMToken.Text);
        }
    }
}
