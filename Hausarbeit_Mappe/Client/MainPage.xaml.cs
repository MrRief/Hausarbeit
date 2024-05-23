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
using System.Windows.Threading;

using NAudio.Wave.SampleProviders;
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


        public MainPage(MainWindow wnd)
        {
            InitializeComponent();
            ContentFrame.NavigationService.Navigate(new _Suche(this));
            _mainWindow = wnd;
           
        }

        public void SongAusSuche(string titel,string kuenstler) 
        {
            ATitel.Visibility = Visibility.Visible;
            AKuenstler.Visibility = Visibility.Visible;
            ATitel.Text= titel;
            AKuenstler.Text= kuenstler;

            string audioUrl = "https://localhost:44351/api/stream" + Uri.EscapeDataString(kuenstler+" - "+titel);
            mediaElement.Source = new Uri(audioUrl);
        }
        private void Start_Click(object sender, RoutedEventArgs e)
        {
           mediaElement.Play();
            
        }

      

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();
        }

        private void Lautstärke_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(mediaElement != null)
            {
                mediaElement.Volume = e.NewValue;
            }
        }

        private void PositionSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mediaElement != null)
            {
                
            }
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            if(mediaElement != null)
            {
               
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
            ContentFrame.NavigationService.Navigate(new _Benutzer(_mainWindow, this));

        }

        private void Favoriten_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.NavigationService.Navigate(new _Favoriten());

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
            _waveOutEvent.Stop();
        }

        public void SongChange(string name)
        {
            
        }
    }
}
