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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    
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
              await  ErstellePlaylist(UserID, Playlistname.Text);
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
                        Error.Visibility = Visibility.Visible;
                        Error.Text = "Playlist erfolgreich erstellt";
                        LoadPlaylists();
                    }
                    else
                    {
                        Error.Visibility = Visibility.Visible;
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
            PlaylistComboBox.Items.Clear();
            Playlisttostart.Items.Clear();
            foreach (PlaylistDTO playlist in playlists)
            {
                
                bool itemExists = false;
                foreach (ComboBoxItem existingItem in PlaylistComboBox.Items)
                {
                    if (existingItem.Tag is int existingId && existingId == playlist.Id)
                    {
                        itemExists = true;
                        break;
                    }
                }

                if (!itemExists)
                {
                    ComboBoxItem item = new ComboBoxItem
                    {
                        Content = playlist.Name,
                        Tag = playlist.Id
                        
                    };
                    ComboBoxItem clone = new ComboBoxItem
                    {

                        Content = playlist.Name,
                        Tag = playlist.Id
                    };
                    PlaylistComboBox.Items.Add(item);
                    Playlisttostart.Items.Add(clone);
                }
            }
        }


        private async void PlaylistComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlaylistComboBox.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is int playlistid)
            {
                await LoeschePlaylist(UserID, playlistid);

            }




        }
        private async Task LoeschePlaylist(int userid, int playlistid)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/drop_playlist";
                    var request = new { nutzerid = userid, playlistid };
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    if (response.IsSuccessStatusCode)
                    {
                        Error.Visibility = Visibility.Visible;
                        Error.Text = "Playlist erfolgreich erstellt";
                        LoadPlaylists();
                    }
                    else
                    {
                        Error.Visibility = Visibility.Visible;
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
        private async Task LoescheLiedAusPlaylist(int liedid,int playlistid)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    string apiUrl = "https://localhost:44351/api/del_song_from_playlist";
                    var request = new { PlaylistID = playlistid, LiedID = liedid };
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    Error.Visibility = Visibility.Visible;
                    Error.Text = await response.Content.ReadAsStringAsync();
                    LoadPlaylists();

                }
            }
            catch (Exception ex)
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = ex.Message;
            }
        }

        private async void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                if (textBlock.DataContext is LiedDTO song)
                {
                    
                    var parent = VisualTreeHelper.GetParent(textBlock);
                    while (parent != null && !(parent is Expander))
                    {
                        parent = VisualTreeHelper.GetParent(parent);
                    }

                    if (parent is Expander expander && expander.DataContext is PlaylistDTO playlist)
                    {
                        
                       await LoescheLiedAusPlaylist(song.Id,playlist.Id);
                    }
                }
            }
        }

        private async void Starte_selected_playlist_Click(object sender, RoutedEventArgs e)
        {
            if (Playlisttostart.SelectedIndex == -1)
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = "Wählen Sie eine Playlist aus um sie zu starten.";
            }
            
            else
            {
                if (Playlisttostart.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is int playlistid)
                {
                    try
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            string apiUrl = $"https://localhost:44351/api/get_playlist_songs?playlistId={playlistid}";
                            HttpResponseMessage response = await client.GetAsync(apiUrl);

                            List<LiedDTO> liederausplaylist = await response.Content.ReadAsAsync<List<LiedDTO>>();
                            if(liederausplaylist.Count == 0)
                            {
                                Error.Visibility= Visibility.Visible;
                                Error.Text = "Die Playlist ist leer";
                            }
                            _mainPage.SongAusPlaylist(liederausplaylist);



                        }
                    }
                    catch (Exception ex)
                    {
                        Error.Visibility= Visibility.Visible;
                        Error.Text= ex.Message;
                    }

                }
            }
        }
    }
}
