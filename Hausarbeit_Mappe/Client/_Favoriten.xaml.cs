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

                var songlist = await response.Content.ReadAsAsync<List<Song>>();

                Create_Button(songlist);


            }
        }

        private void Create_Button(List<Song> songs)
        {
            foreach (var song in songs)
            {
                Button lied = new Button();
                lied.Content = $"{song.Titel} - {song.Kuenstler}";
                lied.Click += (sender, e) =>
                {
                    _mainPage.SongAusSuche(song.Titel, song.Kuenstler);
                };
                Button favorit = new Button();
                favorit.Content = "X";
                favorit.Click += (sender, e) =>
                {
                    _mainPage.FavoritHinzufuegen(false, song.Titel, song.Kuenstler);
                    Lieder.Children.Remove(lied);
                    var favoritContainer = (sender as Button).Parent as Panel;
                    if (favoritContainer != null)
                    {
                        favoritContainer.Children.Remove(sender as Button);
                    }
                };

                Lieder.Children.Add(lied);
                Favorit.Children.Add(favorit);
            }


        }

    }
}
