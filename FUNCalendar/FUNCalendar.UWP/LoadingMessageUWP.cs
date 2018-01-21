using FUNCalendar.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly:Dependency(typeof(FUNCalendar.UWP.Dependency.LoadingMessageUWP))]

namespace FUNCalendar.UWP.Dependency
{
    class LoadingMessageUWP : ILoadingMessage
    {

        public void Hide()
        {

        }

        public void Show(string message)
        {

        }
    }
}
