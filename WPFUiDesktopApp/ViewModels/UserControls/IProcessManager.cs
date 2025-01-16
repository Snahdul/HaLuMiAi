using System.Diagnostics;

namespace WPFUiDesktopApp.ViewModels.UserControls
{
    public interface IProcessManager
    {
        bool StartProcess(ProcessStartInfo processStartInfo);

        bool OpenUrlInDefaultBrowser(string url);
    }
}