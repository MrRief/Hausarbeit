﻿using System;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Client
{
    /// <summary>
    /// Interaktionslogik für LoginPage_1.xaml
    /// </summary>
    public partial class LoginPage_1 : UserControl
    {
        
        private MainWindow _mainwindow;
        private int UserId;
        public LoginPage_1(MainWindow wnd)
        {
            InitializeComponent();
           
            _mainwindow = wnd;



        }
        private  void Loginbutton_Click(object sender, RoutedEventArgs e)
        {

            Login();


        }

        private async Task<bool> AuthentifiziereNutzer(string email, string passwort)
        {
            using(HttpClient client = new HttpClient())
            {
                string apiUrl = $"https://localhost:44351/api/login";
                var LoginModel = new {Email =  email, Passwort = passwort};

                HttpResponseMessage responseMessage = await client.PostAsJsonAsync(apiUrl,LoginModel);
                if (responseMessage.IsSuccessStatusCode)
                {
                    UserId = await responseMessage.Content.ReadAsAsync<int>();

                }
                else
                {
                    UserId = 0;
                }
                return responseMessage.IsSuccessStatusCode;
            }
        }

        private void Registerbutton_Click(object sender, RoutedEventArgs e)
        {
            _mainwindow.MainFrame.NavigationService.Navigate(new LoginPage_2(_mainwindow));
        }

        private void Password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login();
            }
        }

        private async void Login()
        {
            string email = Email.Text;
            string passwort = Password.Password;

            if (string.IsNullOrEmpty(passwort) || string.IsNullOrEmpty(email))
            {
                ErrorText.Visibility = Visibility.Visible;
                ErrorText.Text = "Email oder Passwort nicht eingegeben!";
            }
            else
            {

                bool istregistriert = await AuthentifiziereNutzer(email, passwort);
                if (istregistriert)
                {
                    _mainwindow.MainFrame.NavigationService.Navigate(new MainPage(_mainwindow,UserId));
                }
                else
                {
                    ErrorText.Visibility = Visibility.Visible;
                    ErrorText.Text = "Nutzer nicht registriert!";
                }
            }
        }
    }
}
