using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using FUNCalendar.Services;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(FUNCalendar.iOS.Dependency.LoadingMessageiOS))]
namespace FUNCalendar.iOS.Dependency
{
    public class LoadingMessageiOS : ILoadingMessage
    {
        private LoadingOverlay loadpop;
        /// <summary>ローディングを開始する</summary>
        /// <param name="message"></param>
        public void Show(string message)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var vc = window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            var bounds = UIScreen.MainScreen.Bounds;
            loadpop = new LoadingOverlay(bounds, message);
            vc.Add(loadpop);
            ishow = true;
        }

        /// <summary>ローディングを終了する</summary>
        public void Hide()
        {
            if(loadpop != null)
                loadpop.Hide();
            ishow = false;
        }

        /// <summary>状態</summary>
        public bool IsShow => ishow;

        private bool ishow = false;
    }

    public class LoadingOverlay : UIView
    {
        // control declarations
        UIActivityIndicatorView activitySpinner;
        UILabel loadingLabel;

        public LoadingOverlay(CGRect frame, string message) : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Black;
            Alpha = 0.75f;
            AutoresizingMask = UIViewAutoresizing.All;

            nfloat labelHeight = 22;
            nfloat labelWidth = Frame.Width - 20;

            // derive the center x and y
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);
            activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(activitySpinner);
            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            loadingLabel = new UILabel(new CGRect(
                centerX - (labelWidth / 2),
                centerY + 20,
                labelWidth,
                labelHeight
                ));
            loadingLabel.BackgroundColor = UIColor.Clear;
            loadingLabel.TextColor = UIColor.White;
            loadingLabel.Text = message;
            loadingLabel.TextAlignment = UITextAlignment.Center;
            loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(loadingLabel);

        }

        /// <summary>
        /// Fades out the control and then removes it from the super view
        /// </summary>
        public void Hide()
        {
            UIView.Animate(
                0.5, // duration
                () => { Alpha = 0; },
                () => { RemoveFromSuperview(); }
            );
        }
    }
}