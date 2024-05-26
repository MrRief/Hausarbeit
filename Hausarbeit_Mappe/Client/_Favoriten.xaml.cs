using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.RightsManagement;
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
using static Client._Suche;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    /// <summary>
    /// Interaktionslogik für _Favoriten.xaml
    /// </summary>
    public partial class _Favoriten : UserControl
    {
        private int UserID;
        private MainPage _mainPage;
        public _Favoriten(MainPage page,int id)
        {
            InitializeComponent();
            _mainPage = page;
            UserID = id;
            GetSongsAsync();
        }
        async Task GetSongsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://localhost:44351/api/get_favoritedb?nutzerid=" + UserID;
                var response = await client.GetAsync(apiUrl);

                var songlist = await response.Content.ReadAsAsync<List<Lied>>();

                Create_Button(songlist);


            }
        }

        private async void Create_Button(List<Lied> songs)
        {
            List<PlaylistDTO> list = await GetPlaylistsAsync();
            foreach (var song in songs)
            {
                Button lied = new Button();
                lied.Content = $"{song.Titel} - {song.Kuenstler}";
                lied.Click += (sender, e) =>
                {
                    _mainPage.SongAusSuche(song.Titel, song.Kuenstler);
                };


                Menu playlistmenu = new Menu();
                MenuItem item = new MenuItem();
                item.Header = "Playlists";

                foreach(var playlist in list)
                {
                    MenuItem menuItem = new MenuItem();
                    menuItem.Header = playlist.Name;
                    menuItem.Click += (sender, e) =>
                    {
                        AddSongToPlaylist(song.Titel,song.Kuenstler,playlist.Id);
                    };
                    item.Items.Add(menuItem);
                }
                playlistmenu.Items.Add(item);
                Playlist.Children.Add(playlistmenu);

                Button favorit = new Button();
                favorit.Content = "X";
                favorit.Click += (sender, e) =>
                {
                    _mainPage.FavoritEntfernen(song.Titel, song.Kuenstler);
                    
                    _mainPage.Favorit.IsChecked = false;
                    Lieder.Children.Remove(lied);
                    var favoritContainer = (sender as Button).Parent as Panel;
                    Playlist.Children.Remove(item);
                    if (favoritContainer != null)
                    {
                        favoritContainer.Children.Remove(sender as Button);
                    }
                };

                Lieder.Children.Add(lied);
                Favorit.Children.Add(favorit);
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
        public async void AddSongToPlaylist(string titel,string kuenstler, int playlistid)
        {
            int liedid = await _mainPage.GetSongID(titel, kuenstler);
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    string apiUrl = "https://localhost:44351/api/add_song_to_playlist";
                    var request = new { PlaylistID = playlistid, LiedID = liedid };
                    var content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    Error.Visibility = Visibility.Visible; 
                    Error.Text = await response.Content.ReadAsStringAsync();

                }
            }
            catch(Exception ex)
            {
                Error.Visibility = Visibility.Visible;
                Error.Text = ex.Message;
            }
        }

    }
}
