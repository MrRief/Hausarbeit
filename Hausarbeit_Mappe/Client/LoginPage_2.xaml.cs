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
        public HttpClient client;
        public LoginPage_2()
        {
            InitializeComponent();
            client = new HttpClient();
        }

        private async void Registrieren_Click(object sender, RoutedEventArgs e)
        {
            if(P1.Password == P2.Password)
            {
                //Registrieren
                client.BaseAddress = new Uri("https//localhost:44351/");

                using (client)
                {
                    try
                    {
                        string jsonData = $"{{\"name\": \"{Name.Text}\", \"vorname\": \"{Vorname.Text}\", \"email\": \"{Email.Text}\", \"passwort\": \"{P1.Password}\"}}";
                        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                        HttpResponseMessage response = await client.PostAsync("api/createuser", content);
                        if(response.IsSuccessStatusCode)
                        {
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Error.Text = "Benutzer erfolgreich erstellt.";
                        }
                        else
                        {
                            Error.Text = "Fehler";
                        }
                    }
                    catch (Exception ex)
                    {
                        Error.Text = "Fehler:" + ex.Message;
                    }
                }
            }
            else
            {
                Error.Text = "Passwörter stimmen nicht überein";
            }
        }

        private void Zurueck_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
