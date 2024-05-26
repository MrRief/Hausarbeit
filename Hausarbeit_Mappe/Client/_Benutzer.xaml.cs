using Microsoft.VisualBasic.ApplicationServices;
using Newtonsoft.Json;
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

namespace Client
{
    /// <summary>
    /// Interaktionslogik für _Benutzer.xaml
    /// </summary>
    public partial class _Benutzer : UserControl
    {
        private MainWindow _mainWindow;
        private MainPage _mainPage;
        private int UserId;
        public _Benutzer(MainWindow wnd, MainPage page, int id)
        {
            InitializeComponent();
            _mainWindow = wnd;
            _mainPage = page;
            UserId = id;
            Init();
        }

        private async void Aendern_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text.Length > 20 || Vorname.Text.Length > 20 || Email.Text.Length > 20 || NeusePasswort.Password.Length > 20)
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = "Die eingegebenen Werte sollten nicht länger als 20 Chars lang sein!";
            }
            else
            {


                UpdateNutzerDTO updateNutzerDTO = new UpdateNutzerDTO
                {
                    NutzerId = UserId,
                    Name = Name.Text,
                    Vorname = Vorname.Text,
                    Email = Email.Text,
                    OldPassword = AltesPasswort.Password,
                    NewPassword = NeusePasswort.Password,
                };
                try
                {
                    string result = await UpdateNutzer(updateNutzerDTO);
                    Error.Visibility = Visibility.Visible;
                    Error.Text = result;

                }
                catch (Exception ex)
                {
                    Error.Visibility = Visibility.Visible;
                    Error.Text = ex.Message;
                }
            }
        }

        private async void Loeschen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/delete_user";
                    var content = new StringContent(JsonConvert.SerializeObject(UserId), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        _mainPage.Stopthemusic();
                        _mainWindow.MainFrame.NavigationService.Navigate(new LoginPage_1(_mainWindow));

                    }
                    else
                    {
                        Error.Visibility = Visibility.Visible;
                        Error.Text = "Fehler beim Löschen";
                    }
                }
            }
            catch (Exception ex)
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = ex.Message;
            }
        }

        private void Ausloggen_Click(object sender, RoutedEventArgs e)
        {
            _mainPage.Stopthemusic();
            _mainWindow.MainFrame.NavigationService.Navigate(new LoginPage_1(_mainWindow));
        }
        private async Task<NutzerDTO> GetNutzerAsync(int userID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/get_user";
                    var content = new StringContent(JsonConvert.SerializeObject(userID), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<NutzerDTO>(responseBody);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        private async Task<string> UpdateNutzer(UpdateNutzerDTO updatenutzer)
        {
            try
            {


                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/update_user";
                    var content = new StringContent(JsonConvert.SerializeObject(updatenutzer), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    return await response.Content.ReadAsStringAsync();



                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private async void Init()
        {
            try
            {
                NutzerDTO nutzer = await GetNutzerAsync(UserId);
                Name.Text = nutzer.Name;
                Vorname.Text = nutzer.Vorname;
                Email.Text = nutzer.Email;
            }
            catch (Exception ex)
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = $"Fehler beim Aktualisieren: {ex.Message}";
            }
        }
    }
}
