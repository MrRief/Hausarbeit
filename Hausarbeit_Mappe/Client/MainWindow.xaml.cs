using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http.Json;
using System.Net.Http;

namespace Client
{
    public partial class MainWindow : Window
    {
        private readonly NavigationService _navigationService;

        public MainWindow()
        {
            InitializeComponent();
            _navigationService = new NavigationService(MainFrame);
           

            _navigationService.RegisterPage("LoginPage1", typeof(LoginPage_1));
            _navigationService.RegisterPage("LoginPage2", typeof(LoginPage_2));
            _navigationService.RegisterPage("MainWindow", typeof(MainWindow));

            _navigationService.NavigateTo("LoginPage1");
           
        }
      
    }
}