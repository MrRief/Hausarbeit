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

namespace Client
{
    /// <summary>
    /// Interaktionslogik für _Benutzer.xaml
    /// </summary>
    public partial class _Benutzer : UserControl
    {
        private MainWindow _mainWindow;
        private MainPage _mainPage;
        public _Benutzer(MainWindow wnd, MainPage page)
        {
            InitializeComponent();
            _mainWindow = wnd;
            _mainPage = page;
        }

        private void Aendern_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Loeschen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Ausloggen_Click(object sender, RoutedEventArgs e)
        {
            _mainPage.Stopthemusic();
            _mainWindow.MainFrame.NavigationService.Navigate(new LoginPage_1(_mainWindow));
        }
    }
}
