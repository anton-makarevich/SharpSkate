// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using Sanet.SmartSkating.Dashboard.Views.Base;
using Sanet.SmartSkating.ViewModels;

namespace Sanet.SmartSkating.Dashboard.Views
{
    public class LoginPageBase : BasePage<LoginViewModel> { }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
    }
}
