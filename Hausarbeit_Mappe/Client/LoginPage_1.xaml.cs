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
            string email = Email.Text;
            string passwort = Password.Password;

            if (string.IsNullOrEmpty(passwort) || string.IsNullOrEmpty(email))
            {
                ErrorText.Text = "Email oder Passwort nicht eingegeben!";
            }
            else
            {

                bool istregistriert = await AuthentifiziereNutzer(email, passwort);
                if (istregistriert)
                {
                    _mainwindow.MainFrame.NavigationService.Navigate(new MainPage(_mainwindow));
                }
                else
                {
                    ErrorText.Text = "Nutzer nicht registriert!";
                }
            }



        }

        private async Task<bool> AuthentifiziereNutzer(string email, string passwort)
        {
            using(HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://localhost:44351/api/login";
                var LoginModel = new {Email =  email, Passwort = passwort};

                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(apiUrl,LoginModel);

                return responseMessage.IsSuccessStatusCode;
            }
        }

        private void Registerbutton_Click(object sender, RoutedEventArgs e)
        {
            _mainwindow.MainFrame.NavigationService.Navigate(new LoginPage_2(_mainwindow));
        }
    }
}
