using System;

using FastOrdering.Models;
using FastOrdering.Services;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FastOrdering
{
    public sealed partial class App : Application
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }
        public bool issuspend = false;
        public App()
        {
            InitializeComponent();

            EnteredBackground += App_EnteredBackground;
            //this.Suspending += OnSuspending;
            var instance = SampleDataService.GetInstance();
            var mySQL = SampleOrderSQLManagement.GetInstance();
            // get all elements in sql
            mySQL.GetAll();
            if (instance.allItems.Count != 0)
            {
                instance.allItems.Clear();
            }
            for (int i = 0; i < mySQL.allItems.Count; i++)
            {
                instance.allItems.Add(mySQL.allItems[i]);
            }
            
            // Deferred execution until used. Check https://msdn.microsoft.com/library/dd642331(v=vs.110).aspx for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);
        }


        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (!args.PrelaunchActivated)
            {
                await ActivationService.ActivateAsync(args);
            }
        }

        protected override async void OnActivated(IActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(this, typeof(Views.MainPage), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            return new Views.ShellPage();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e) {
            var deferral = e.GetDeferral();
            await Helpers.Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }

        //private void OnSuspending(object sender, SuspendingEventArgs e) {
        //    var deferral = e.SuspendingOperation.GetDeferral();
        //    //TODO: 保存应用程序状态并停止任何后台活动
        //    issuspend = true;
        //    Frame frame = Window.Current.Content as Frame;
        //    if (frame != null)
        //        ApplicationData.Current.LocalSettings.Values["NavigationState"] = frame.GetNavigationState();
        //    deferral.Complete();
        //}

    }
}
