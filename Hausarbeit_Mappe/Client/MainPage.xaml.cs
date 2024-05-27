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
        private bool donttrigger = false;
        private int liedindex = -1;
        private List<LiedDTO> warteschlange;


        public MainPage(MainWindow wnd, int UID)
        {
            InitializeComponent();
            ContentFrame.NavigationService.Navigate(new _Suche(this));
            _mainWindow = wnd;
            UserID = UID;
            mediaElement.MediaOpened += MediaElement_MediaOpened;
            mediaElement.MediaEnded += MediaElement_MediaEnded;


        }

        private void InitTimer()
        {

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

        private async void MediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            PositionSlider.Value = 0;

            if (repeat)
            {
                mediaElement.Position = TimeSpan.Zero;
                mediaElement.Play();
            }
            else if (warteschlange != null && liedindex >= 0 && liedindex < warteschlange.Count - 1)
            {
                liedindex++;
                var nextSong = warteschlange[liedindex];
                UpdateWarteschlange();
                await PlaySong(nextSong.Titel, nextSong.Kuenstler);
            }
            else if (Lieder.Children.Count == 0 && Entfernen.Children.Count == 0)
            {
                warteschlange = null;
                liedindex = -1;
            }
            else
            {
                UpdateWarteschlange();
            }

        }
        public void UpdateWarteschlange()
        {
            if (Lieder.Children.Count > 0 && Entfernen.Children.Count > 0)
            {
                UIElement todelete = Lieder.Children[0];
                UIElement todelete2 = Entfernen.Children[0];
                Lieder.Children.Remove(todelete);
                Entfernen.Children.Remove(todelete2);
            }
        }
        public void SetWarteschlange()
        {

            foreach (var song in warteschlange)
            {
                if (song != warteschlange[0])
                {
                    bool check = Lieder.Children.OfType<TextBlock>().Any(x => x.Text == $"{song.Titel} - {song.Kuenstler}");
                    if (!check)
                    {

                        TextBlock text = new TextBlock();
                        text.Text = $"{song.Titel} - {song.Kuenstler}";
                        Lieder.Children.Add(text);

                        Button btn = new Button();
                        btn.Content = "X";


                        LiedDTO capturedSong = song;

                        btn.Click += (sender, e) =>
                        {
                            warteschlange.Remove(capturedSong);
                            Lieder.Children.Remove(text);
                            Entfernen.Children.Remove(btn);

                        };

                        Entfernen.Children.Add(btn);
                    }


                }
            }


        }
        private async Task PlaySong(string titel, string kuenstler)
        {
            ATitel.Text = titel;
            AKuenstler.Text = kuenstler;

            string query = Uri.EscapeDataString(kuenstler + " - " + titel);
            string audioUrl = $"https://localhost:44351/api/stream?filetoget={query}";
            mediaElement.Source = new Uri(audioUrl);

            Favorit.Visibility = Visibility.Visible;
            if (await IsFavorite())
            {
                donttrigger = true;
            }
            Favorit.IsChecked = donttrigger;
            donttrigger = false;
            InitTimer();
            mediaElement.Play();
        }
        public async void SongAusPlaylist(List<LiedDTO> liste)
        {
            if (liste == null || liste.Count == 0) return;

            warteschlange = liste;
            liedindex = 0;
            var firstSong = warteschlange[liedindex];
            ATitel.Text = firstSong.Titel;
            AKuenstler.Text = firstSong.Kuenstler;
            SetWarteschlange();
            await PlaySong(firstSong.Titel, firstSong.Kuenstler);
        }
        public async void AddSongToQueue(string titel, string kuenstler)
        {

            LiedDTO neuinderschlange = new LiedDTO();
            neuinderschlange.Kuenstler = kuenstler;
            neuinderschlange.Titel = titel;
            if (warteschlange != null)
            {
                warteschlange.Add(neuinderschlange);
                TextBlock text = new TextBlock();
                text.Text = $"{neuinderschlange.Titel} - {neuinderschlange.Kuenstler}";
                Lieder.Children.Add(text);

                Button btn = new Button();
                btn.Content = "X";


                LiedDTO capturedSong = neuinderschlange;

                btn.Click += (sender, e) =>
                {
                    warteschlange.Remove(capturedSong);
                    Lieder.Children.Remove(text);
                    Entfernen.Children.Remove(btn);

                };

                Entfernen.Children.Add(btn);
            }
            else if (liedindex == -1)
            {
                warteschlange = new List<LiedDTO>();
                warteschlange.Add(neuinderschlange);
                liedindex = 0;
                await PlaySong(titel, kuenstler);
            }
        }


        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            PositionSlider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
            TotalTimeTextBlock.Text = mediaElement.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        public async void SongAusSuche(string titel, string kuenstler)
        {
            ATitel.Visibility = Visibility.Visible;
            AKuenstler.Visibility = Visibility.Visible;
            ATitel.Text = titel;
            AKuenstler.Text = kuenstler;

            string query = Uri.EscapeDataString(kuenstler + " - " + titel);
            string audioUrl = $"https://localhost:44351/api/stream?filetoget={query}";
            mediaElement.Source = new Uri(audioUrl);
            Favorit.Visibility = Visibility.Visible;
            if (await IsFavorite())
            {
                donttrigger = true;
            }
            Favorit.IsChecked = donttrigger;
            donttrigger = false;
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
            ContentFrame.NavigationService.Navigate(new _Favoriten(this, UserID));

        }

        private void Playlists_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Playlists(this, UserID));

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
            if (!donttrigger)
            {
                FavoritHinzufuegen(ATitel.Text, AKuenstler.Text);


            }
        }
        private void Favorit_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!donttrigger)
            {
                FavoritEntfernen(ATitel.Text, AKuenstler.Text);

            }
        }

        public async void FavoritHinzufuegen(string titel, string kuenstler)
        {
            int liedid = await GetSongID(titel, kuenstler);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"https://localhost:44351/api/add_favorite?nutzerid={UserID}&liedid={liedid}";
                    HttpResponseMessage response = await client.PostAsync(apiUrl, null);


                }
            }
            catch
            {
                throw;
            }
        }
        public async void FavoritEntfernen(string titel, string kuenstler)
        {
            int liedid = await GetSongID(titel, kuenstler);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = $"https://localhost:44351/api/del_favorite?nutzerid={UserID}&liedid={liedid}";
                    HttpResponseMessage response = await client.PostAsync(apiUrl, null);


                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> GetSongID(string titel, string kuenstler)
        {
            string _titel = Uri.EscapeDataString(titel);
            string _kuenstler = Uri.EscapeDataString(kuenstler);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/get_songid?titel=" + titel + "&kuenstler=" + kuenstler;



                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    return await response.Content.ReadAsAsync<int>();


                }
            }
            catch
            {
                throw;
            }
        }
        private async Task<bool> IsFavorite()
        {
            int liedid = await GetSongID(ATitel.Text, AKuenstler.Text);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string apiUrl = "https://localhost:44351/api/is_favorite?nutzerid=" + UserID + "&liedid=" + liedid;
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    return await response.Content.ReadAsAsync<bool>();


                }
            }
            catch
            {
                throw;
            }
        }


    }
}
