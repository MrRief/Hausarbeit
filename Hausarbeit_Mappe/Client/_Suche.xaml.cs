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
    /// Interaktionslogik für _Suche.xaml
    /// </summary>
    public partial class _Suche : UserControl
    {
        private MainPage _mainPage;
        private _Playlists _playlists;
        public class Lied
        {
            public string? Titel { get; set; }
            public string? Kuenstler { get; set; }
        }
        public _Suche(MainPage page)
        {
            InitializeComponent();
            _mainPage = page;
            
            GetSongsAsync();
        }

        async Task GetSongsAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                string apiUrl = "https://localhost:44351/api/songs_in_db";
                var response = await client.GetAsync(apiUrl);

                var songlist = await response.Content.ReadAsAsync<List<Lied>>();

                Create_Button(songlist);


            }
        }

        private void Create_Button(List<Lied> songs)
        {
            foreach (var song in songs)
            {
                Button Herbert = new Button();
                Herbert.Content = $"{song.Titel} - {song.Kuenstler}";
                Herbert.Click += (sender, e) =>
                {
                    _mainPage.SongAusSuche(song.Titel, song.Kuenstler);
                };
              
                  
                Suche.Children.Add(Herbert);
                Button add = new Button();
                add.Content = "+";
                add.Click +=  (sender, e) =>
                {
                    _mainPage.AddSongToQueue(song.Titel, song.Kuenstler);
                };
                Buttonpanel.Children.Add(add);
            }


        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = Filter.Text.ToLower();

            foreach (Button b in Suche.Children.OfType<Button>())
            {
                string buttonContent = b.Content.ToString().ToLower();
                b.Visibility = string.IsNullOrEmpty(filter) || buttonContent.Contains(filter) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
