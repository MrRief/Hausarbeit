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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using static Client._Suche;

namespace Client
{
    /// <summary>
    /// Interaktionslogik für _Playlists.xaml
    /// </summary>
    public partial class _Playlists : UserControl
    {
        private MainPage _mainPage;
        private int UserID;

        public _Playlists(MainPage page, int id)
        {
            InitializeComponent();
            _mainPage = page;
            UserID = id;
            LoadPlaylists();


        }

        private async void Anlegen_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Playlistname.Text))
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = "Geben Sie einen Namen ein.";
            }
            else
            {
                ErstellePlaylist(UserID, Playlistname.Text);
            }
        }
        private async Task ErstellePlaylist(int userid, string name)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/create_playlist";
                    var request = new { nutzerid = userid, name };
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Error.Visibility= Visibility.Visible;
                        Error.Text = "Playlist erfolgreich erstellt";
                        LoadPlaylists();
                    }
                    else
                    {
                        Error.Visibility= Visibility.Visible;
                        Error.Text = await response.Content.ReadAsStringAsync();

                    }
                }
            }
            catch (Exception ex)
            {
                Error.Visibility = Visibility.Visible;
                throw new Exception(Error.Text = ex.Message);
            }
        }
        private async Task<List<PlaylistDTO>> GetPlaylistsAsync()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"https://localhost:44351/api/get_playlists?nutzerid={UserID}";
                    HttpResponseMessage response = await client.GetAsync(apiUrl);


                    string json = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<PlaylistDTO>>(json);


                }
            }
            catch (Exception ex)
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = ex.Message;
                return new List<PlaylistDTO>();
            }
        }
        private async void LoadPlaylists()
        {

            List<PlaylistDTO> playlists = await GetPlaylistsAsync();
            
            PlaylistItemControl.ItemsSource = playlists;
        }
        public async Task<List<PlaylistDTO>> ReturnPlaylists()
        {
            return await GetPlaylistsAsync();
        }
        
        
       



    }
}
