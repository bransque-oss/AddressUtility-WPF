using AddressUtility.ViewModels;
using System.Windows;

namespace AddressUtility.Views
{
    //
    // Главное окно
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            MainViewModel viewModel = new();
            DataContext = viewModel;
        }
    }
}
