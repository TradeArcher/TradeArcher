using System;
using System.Collections.Generic;

using Caliburn.Micro;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

using TradeArcher.Core.Helpers;
using TradeArcher.Services;
using TradeArcher.ViewModels;

using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using TradeArcher.Core.Services;

namespace TradeArcher
{
    [Windows.UI.Xaml.Data.Bindable]
    public sealed partial class App
    {
        private Lazy<ActivationService> _activationService;

        private ActivationService ActivationService
        {
            get { return _activationService.Value; }
        }

        public App()
        {
            InitializeComponent();

            EnteredBackground += App_EnteredBackground;
            Resuming += App_Resuming;

            AppCenter.Start("39b15924-59cb-43b7-ac49-f5ff9f622526", typeof(Analytics), typeof(Crashes));
            UnhandledException += OnAppUnhandledException;

            Initialize();

            // Deferred execution until used. Check https://docs.microsoft.com/dotnet/api/system.lazy-1 for further info on Lazy<T> class.
            _activationService = new Lazy<ActivationService>(CreateActivationService);

            DbInitializer.Initialize();
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

        private WinRTContainer _container;

        protected override void Configure()
        {
            // This configures the framework to map between MainViewModel and MainPage
            // Normally it would map between MainPageViewModel and MainPage
            var config = new TypeMappingConfiguration
            {
                IncludeViewSuffixInViewModelNames = false
            };

            ViewLocator.ConfigureTypeMappings(config);
            ViewModelLocator.ConfigureTypeMappings(config);

            _container = new WinRTContainer();
            _container.RegisterWinRTServices();

            _container.PerRequest<ShellViewModel>();
            _container.PerRequest<MainViewModel>();
            _container.PerRequest<SettingsViewModel>();
            _container.PerRequest<AccountsViewModel>();
            _container.PerRequest<BrokersViewModel>();
            _container.PerRequest<AnalyzeViewModel>();
            _container.PerRequest<ImportViewModel>();
            _container.PerRequest<JournalViewModel>();
            _container.PerRequest<StrategiesPageViewModel>();
            _container.PerRequest<TradesViewModel>();
            _container.PerRequest<TradesImporterService>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        private void OnAppUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // TODO WTS: Please log and handle the exception as appropriate to your scenario
            // For more info see https://docs.microsoft.com/uwp/api/windows.ui.xaml.application.unhandledexception
        }

        private ActivationService CreateActivationService()
        {
            return new ActivationService(_container, typeof(ViewModels.MainViewModel), new Lazy<UIElement>(CreateShell));
        }

        private UIElement CreateShell()
        {
            var shellPage = new Views.ShellPage();
            return shellPage;
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await Singleton<SuspendAndResumeService>.Instance.SaveStateAsync();
            deferral.Complete();
        }

        private void App_Resuming(object sender, object e)
        {
            Singleton<SuspendAndResumeService>.Instance.ResumeApp();
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            await ActivationService.ActivateAsync(args);
        }
    }
}
