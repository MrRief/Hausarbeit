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
using System.Windows.Threading;

using NAudio.Wave.SampleProviders;
using Newtonsoft.Json;
using static System.Net.WebRequestMethods;

namespace Client
{
    /// <summary>
    /// Interaktionslogik für MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {


        private MainWindow _mainWindow;
        private bool repeat = false;
        private bool isUsingSlider = false;
        private DispatcherTimer _timer;
        private int UserID;


        public MainPage(MainWindow wnd, int UID)
        {
            InitializeComponent();
            ContentFrame.NavigationService.Navigate(new _Suche(this));
            _mainWindow = wnd;
            UserID = UID;
            Favorit.Visibility = Visibility.Collapsed;

        }

        private void InitTimer()
        {
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!isUsingSlider)
            {
                PositionSlider.Value = mediaElement.Position.TotalSeconds;
            }
            CurrentTimeTextBlock.Text = mediaElement.Position.ToString(@"mm\:ss");
        }

        private void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            PositionSlider.Value = 0;
            if (repeat)
            {
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
            }
        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            PositionSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            TotalTimeTextBlock.Text = mediaElement.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        public void SongAusSuche(string titel, string kuenstler)
        {
            ATitel.Visibility = Visibility.Visible;
            AKuenstler.Visibility = Visibility.Visible;
            ATitel.Text = titel;
            AKuenstler.Text = kuenstler;

            string query = Uri.EscapeDataString(kuenstler + " - " + titel);
            string audioUrl = $"https://localhost:44351/api/stream?filetoget={query}";
            mediaElement.Source = new Uri(audioUrl);

            Favorit.Visibility = Visibility.Collapsed;
            InitTimer();
            mediaElement.Play();
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement != null)
            {
                mediaElement.Play();
            }

        }



        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement != null)
            {
                mediaElement.Pause();
            }
        }

        private void Lautstärke_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (mediaElement != null)
            {
                mediaElement.Volume = e.NewValue;
            }
        }



        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement != null)
            {
                mediaElement.Position = TimeSpan.Zero;
            }
        }

        private void Loop_Checked(object sender, RoutedEventArgs e)
        {
            repeat = true;
        }

        private void Loop_Unchecked(object sender, RoutedEventArgs e)
        {
            repeat = false;
        }



        private void Benutzer_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Benutzer(_mainWindow, this, UserID));

        }

        private void Favoriten_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Favoriten(UserID));

        }

        private void Playlists_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Playlists());

        }

        private void Suche_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Suche(this));

        }

        public void Stopthemusic()
        {
            mediaElement.Stop();
        }


        private void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (isUsingSlider)
            {
                if (mediaElement != null)
                {
                    mediaElement.Position = TimeSpan.FromSeconds(PositionSlider.Value);
                }
            }
        }



        private void PositionSlider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            isUsingSlider = true;
        }



        private void PositionSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            isUsingSlider = false;
            if (mediaElement != null)
            {
                mediaElement.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            }
        }

        private void Favorit_Checked(object sender, RoutedEventArgs e)
        {
            FavoritHinzufuegen(true);
        }

        private async void FavoritHinzufuegen(bool isFavorite)
        {
            int liedid = await GetSongID();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/favorite";
                    var content = new StringContent(JsonConvert.SerializeObject(new { favorit = isFavorite, nutzerid = UserID, liedid = liedid }), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                }
            }
            catch
            {
                throw;
            }
        }

        private async Task<int> GetSongID()
        {
            string titel = ATitel.Text;
            string kuenstler = AKuenstler.Text;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/get_songid";
                    var content = new StringContent(JsonConvert.SerializeObject(new { titel = titel, kuenstler = kuenstler }), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                    return await response.Content.ReadAsAsync<int>();


                }
            }
            catch
            {
                throw;
            }
        }

        private void Favorit_Unchecked(object sender, RoutedEventArgs e)
        {
            FavoritHinzufuegen(false);
        }


    }
}
