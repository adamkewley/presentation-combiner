using System.Windows;

namespace PresentationCombiner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Helpers.ClientHasPowerpointInstalled())
            {
                MessageBox.Show("Could not find Powerpoint installed on this computer.");
            }
            base.OnStartup(e);
        }
    }
}
