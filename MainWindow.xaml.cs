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

namespace Western
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    
    public partial class MainWindow : Window
    {
        static public double difficulty = 1;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            Game win = new Game();
            win.Topmost = true;
            this.Close();
            win.Show();
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            Information win = new Information();
            win.Topmost = true;
            win.Show();
        }

        private void Difficulty_Click(object sender, RoutedEventArgs e)
        {
            difficulty++;
            if (difficulty > 3) difficulty = 1;
            InitializeComponent();
            diff_text.Content = difficulty.ToString();
        }
    }
}
