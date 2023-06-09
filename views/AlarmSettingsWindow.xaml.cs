using DovizKuru.viewmodels;

namespace DovizKuru.views
{
    /// <summary>
    /// Interaction logic for AlarmSettingsWindow.xaml
    /// </summary>
    public partial class AlarmSettingsWindow
    {
        public AlarmSettingsWindow()
        {
            this.DataContext = App.Current.ServiceProvider.GetService(typeof(MainViewModel));
            InitializeComponent();
        }
    }
}
