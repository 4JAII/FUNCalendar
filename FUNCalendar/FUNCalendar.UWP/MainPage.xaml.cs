using Prism.Unity;
using Microsoft.Practices.Unity;
using FUNCalendar.Models;

namespace FUNCalendar.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new FUNCalendar.App(new UwpInitializer()));
        }
    }

    public class UwpInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IUnityContainer container)
        {
           
        }
    }

}
