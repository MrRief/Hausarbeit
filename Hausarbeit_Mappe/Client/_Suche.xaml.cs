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
        public _Suche(MainPage page)
        {
            InitializeComponent();
            _mainPage = page;
            GetSongsAsync();
        }

        async void GetSongsAsync()
        {
            using(HttpClient client = new HttpClient())
            {
                string apiUrl = "http://localhost:44351/api/songs_in_db";
                var response = await client.GetAsync(apiUrl);

                var songlist = await response.Content.ReadAsAsync<List<string>>();

                foreach(var song in songlist)
                {
                    Create_Button(song.Split('-')[1], song.Split('-')[0]);
                }
            }
        }
        C:\Users\Tim\Desktop\Hausarbeit\Hausarbeit\Hausarbeit_Mappe\_StreamingServer\Lieder\David Kushner - Daylight.mp3
        private void Create_Button(string song, string artist)
        {
            Button _song = new Button();
            Button _artist = new Button();
            _song.Content = song;
            _artist.Content = artist;

            Titel.Children.Add(_song);
            Künstler.Children.Add(_artist);
            
        }
    }
}
