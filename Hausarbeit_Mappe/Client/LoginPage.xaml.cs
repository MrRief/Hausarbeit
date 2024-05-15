﻿using System;
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
    /// Interaktionslogik für LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Loginbutton_Click(object sender, RoutedEventArgs e)
        {
            if(Email.Text.Length == 0 || Password.Password.Length == 0) 
            {
                ErrorText.Content = "Email oder Passwort fehlt!";
                ErrorText.Visibility = Visibility.Visible;
            }
            else
            {
                
            }
        }

        private void Createbutton_Click(object sender, RoutedEventArgs e)
        {
            //navigiere zu [Nutzer erstellen]
        }
    }
}