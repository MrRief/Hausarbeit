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
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    /// <summary>
    /// Interaktionslogik für LoginPage_2.xaml
    /// </summary>
    public partial class LoginPage_2 : UserControl
    {
       
        private MainWindow _mainWindow;
        public LoginPage_2(MainWindow wnd)
        {
            InitializeComponent();
           
            _mainWindow = wnd;
        }

        private async void Registrieren_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Name.Text) || string.IsNullOrEmpty(Vorname.Text) || string.IsNullOrEmpty(Email.Text) || string.IsNullOrEmpty(P1.Password) || string.IsNullOrEmpty(P2.Password))
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = "Es sind nicht alle Felder ausgefüllt!";
            }
            else
            {
                if (Name.Text.Length > 20 || Vorname.Text.Length > 20 || Email.Text.Length > 20 || P1.Password.Length > 20 || P2.Password.Length > 20)
                {
                    Error.Visibility = Visibility.Visible;
                    Error.Text = "Die eingegebenen Werte sollten nicht länger als 20 Chars lang sein!";
                }
                else
                {


                    if (P1.Password == P2.Password)
                    {
                        string name = Name.Text;
                        string vorname = Vorname.Text;
                        string email = Email.Text;
                        string passwort = P1.Password;

                        bool iscreated = await ErstelleNutzer(name, vorname, email, passwort);

                        if (iscreated)
                        {
                            _mainWindow.MainFrame.NavigationService.Navigate(new LoginPage_1(_mainWindow));
                        }
                        else
                        {
                            Error.Visibility = Visibility.Visible;
                            Error.Text = "Nutzer wurde nicht erfolgreich erstellt!";
                        }
                    }
                    else
                    {
                        Error.Visibility = Visibility.Visible;
                        Error.Text = "Passwörter stimmen nicht überein!";
                    }
                }
            }

        }

        private async Task<bool> ErstelleNutzer(string name, string vorname, string email, string passwort)
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://localhost:44351/api/create_user";
                var neuerNutzer = new
                {
                    Name = name,
                    Vorname = vorname,
                    Email = email,
                    Passwort = passwort
                };
                HttpResponseMessage response = await client.PostAsJsonAsync(apiUrl, neuerNutzer);



                return response.IsSuccessStatusCode;
            }
        }

        private void Zurueck_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.MainFrame.NavigationService.Navigate(new LoginPage_1(_mainWindow));
        }
    }
}
