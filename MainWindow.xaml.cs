using DovizKuru.viewmodels;

namespace DovizKuru
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.DataContext = App.Current.ServiceProvider.GetService(typeof(MainViewModel));
            InitializeComponent();
        }
    }
}
