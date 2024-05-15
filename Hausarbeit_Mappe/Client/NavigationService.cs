using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace Client
{
    public class NavigationService
    {
        private readonly Dictionary<string, Type> _pages = new Dictionary<string, Type>();
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame ?? throw new ArgumentNullException(nameof(frame));
        }

        public void RegisterPage(string pageKey, Type pageType)
        {
            if (!_pages.ContainsKey(pageKey))
            {
                _pages.Add(pageKey, pageType);
            }
        }

        public void NavigateTo(string pageKey)
        {
            if (_pages.TryGetValue(pageKey, out Type pageType))
            {
                _frame.Navigate(Activator.CreateInstance(pageType));
            }
            else
            {
                MessageBox.Show($"Page with key '{pageKey}' not found.");
            }
        }
        public void NavigateToMainWindow()
        {
            _frame.Content = new MainWindow();
        }
    }
}
