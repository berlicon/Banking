using System;
using System.Windows;
using Banking.Client.ServiceReference;

namespace Banking.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new ServiceClient())
            {
                try
                {
                    var result = client.GetUserOnATM(tbLogin.Text, tbPin.Text);
                    MessageBox.Show("Hello, " + result.Item2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Data.Constants.ErrorUserNotExist);
                }
            }
        }

        private void btLogout_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
