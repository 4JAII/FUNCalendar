using Android.App;
using FUNCalendar.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(FUNCalendar.Droid.Dependency.LoadingMessageAndroid))]

namespace FUNCalendar.Droid.Dependency
{
    public class LoadingMessageAndroid : ILoadingMessage
    {
        private ProgressDialog progress;
        /// <summary>ローディングを開始する</summary>
        /// <param name="message"></param>
        public void Show(string message)
        {
            progress = new ProgressDialog(Forms.Context);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetCancelable(false);
            progress.SetMessage(message);
            progress.Show();
            ishow = true;
        }

        /// <summary>ローディングを終了する</summary>
        public void Hide()
        {
            progress?.Dismiss();
            ishow = false;
        }

        /// <summary>状態</summary>
        public bool IsShow => ishow;

        private bool ishow = false;
    }
}