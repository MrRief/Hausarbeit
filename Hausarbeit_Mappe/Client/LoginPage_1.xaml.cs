using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    /// <summary>
    /// Interaktionslogik für LoginPage_1.xaml
    /// </summary>
    public partial class LoginPage_1 : UserControl
    {
        public readonly HttpClient client;
        private MainWindow _mainwindow;
        public LoginPage_1(MainWindow wnd)
        {
            InitializeComponent();
            client = HttpClientSingleton.Instance;
            _mainwindow = wnd;
            


        }
        private async void Loginbutton_Click(object sender, RoutedEventArgs e)
        {
            //if (Email.Text.Length == 0 || Password.Password.Length == 0)
            //{
            //    ErrorText.Text = "Email oder Passwort fehlt!";
            //    ErrorText.Visibility = Visibility.Visible;
            //}
            //else
            //{
               
            //    using (client)
            //    {
            //        try
            //        {
            //            string jsonData = $"{{\"{Email.Text}\", \"passwort\": \"{Password.Password}\"}}";
            //            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            //            HttpResponseMessage response = await client.PostAsync("api/login", content);
            //            if (response.IsSuccessStatusCode)
            //            {
            //                string responseBody = await response.Content.ReadAsStringAsync();
                            _mainwindow.MainFrame.NavigationService.Navigate(new MainPage(_mainwindow));
            //            }
            //            else
            //            {
            //                ErrorText.Text = "Fehler";
            //            }
            //        }
            //        catch (Exception ex)
            //        {
            //            ErrorText.Text = "Fehler:" + ex.Message;
            //        }

            //    }
            //}


        }

        private void Registerbutton_Click(object sender, RoutedEventArgs e)
        {
            _mainwindow.MainFrame.NavigationService.Navigate(new LoginPage_2(_mainwindow));
        }
    }
}
