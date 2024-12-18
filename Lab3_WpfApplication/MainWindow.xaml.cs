using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lab3.WpfApplication.ViewModels;

namespace Lab3.WpfApplication
{
    public partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; }
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowViewModel();
            DataContext = ViewModel;
        }
        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out _);
        }

    }
}

