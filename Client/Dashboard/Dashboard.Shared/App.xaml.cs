using Microsoft.Extensions.Logging;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Microsoft.Extensions.DependencyInjection;
using Sanet.SmartSkating.Services.Account;
using Sanet.SmartSkating.Dto.Services.Account;
using Sanet.SmartSkating.ViewModels;
using Sanet.SmartSkating.Services.Api;
using Sanet.SmartSkating.Dto;
using Refit;
using Sanet.SmartSkating.Services;
using Sanet.SmartSkating.Dashboard.Services;
using Sanet.SmartSkating.Dashboard.Views;
using Sanet.SmartSkating.Dto.Services;
using Sanet.SmartSkating.Services.Tracking;
using System.Net.Http;
using Sanet.SmartSkating.Dashboard.Services.Dummy;
using Sanet.SmartSkating.Services.Location;

#if __WASM__
using Uno.UI.Wasm;
#endif

namespace Sanet.SmartSkating.Dashboard
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
	{
		private IServiceProvider _serviceProvider;

		public IServiceProvider Container => _serviceProvider;
		public INavigationService NavigationService => _navigationService;

		private readonly IServiceCollection _services;
		private INavigationService _navigationService;

		/// <summary>
		/// Initializes the singleton application object.  This is the first line of authored code
		/// executed, and as such is the logical equivalent of main() or WinMain().
		/// </summary>
		public App()
		{
			_services = new ServiceCollection();
			ConfigureServices(_services);

			ConfigureFilters(Uno.Extensions.LogExtensionPoint.AmbientLoggerFactory);

			InitializeComponent();
			Suspending += OnSuspending;
		}

        private void ConfigureServices(IServiceCollection services)
        {
#if __WASM__
			var httpHandler = new WasmHttpHandler();
#else
			var httpHandler = new HttpClientHandler();
#endif
			var httpClient = new HttpClient(httpHandler,false)
			{
				BaseAddress = new Uri(ApiNames.BaseUrl)
			};
			// Services
			_services.AddSingleton(RestService.For<IApiService>(httpClient));
			_services.AddSingleton<ILoginService, LoginService>();
			_services.AddSingleton<IAccountService, AccountService>();
			_services.AddSingleton<IDeviceInfo,EssentialsDeviceInfo>();
			_services.AddSingleton<IPreferences,EssentialsPreferences>();
			_services.AddSingleton<ISessionProvider,SessionProvider>();
			_services.AddSingleton<ISettingsService, SettingsService>();
			_services.AddSingleton<ITrackService, TrackService>();
			_services.AddSingleton<ITrackProvider,LocalTrackProvider>();
			_services.AddSingleton<IResourceReader,EmbeddedResourceReader>();
			_services.AddSingleton<ISessionManager, SessionManager>();
			_services.AddSingleton<ISyncService, SignalRService>();
			_services.AddSingleton<IDateProvider, DateProvider>();
			
			_services.AddSingleton<ILocationService , DummyLocationService>();
			_services.AddSingleton<IDataSyncService , DummyDataSyncService>();
			_services.AddSingleton<IBleLocationService, DummyBleService>();

			// ViewModels
			_services.AddSingleton<LoginViewModel, LoginViewModel>();
			_services.AddSingleton<SessionsViewModel, SessionsViewModel>();
			_services.AddSingleton<SessionDetailsViewModel, SessionDetailsViewModel>();
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
		{
            _serviceProvider = _services.BuildServiceProvider();
#if DEBUG
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// this.DebugSettings.EnableFrameRateCounter = true;
			}
#endif

#if NET5_0 && WINDOWS
			var window = new Window();
			window.Activate();
#else
			var window = Window.Current;
#endif

			Frame rootFrame = window.Content as Frame;

			// Do not repeat app initialization when the Window already has content,
			// just ensure that the window is active
			if (rootFrame == null)
			{
				// Create a Frame to act as the navigation context and navigate to the first page
				rootFrame = new Frame();

				rootFrame.NavigationFailed += OnNavigationFailed;

				if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
				{
					//TODO: Load state from previously suspended application
				}

				// Place the frame in the current Window
				window.Content = rootFrame;
			}

			_navigationService = new UwpNavigationService(rootFrame, _serviceProvider);

#if !(NET5_0 && WINDOWS)
			if (e.PrelaunchActivated == false)
#endif
			{
				if (rootFrame.Content == null)
				{
					// When the navigation stack isn't restored navigate to the first page,
					// configuring the new page by passing required information as a navigation
					// parameter
					rootFrame.Navigate(typeof(LoginView), e.Arguments);
				}
				// Ensure the current window is active
				window.Activate();
			}
		}

		/// <summary>
		/// Invoked when Navigation to a certain page fails
		/// </summary>
		/// <param name="sender">The Frame which failed navigation</param>
		/// <param name="e">Details about the navigation failure</param>
		void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			throw new Exception($"Failed to load {e.SourcePageType.FullName}: {e.Exception}");
		}

		/// <summary>
		/// Invoked when application execution is being suspended.  Application state is saved
		/// without knowing whether the application will be terminated or resumed with the contents
		/// of memory still intact.
		/// </summary>
		/// <param name="sender">The source of the suspend request.</param>
		/// <param name="e">Details about the suspend request.</param>
		private void OnSuspending(object sender, SuspendingEventArgs e)
		{
			var deferral = e.SuspendingOperation.GetDeferral();
			//TODO: Save application state and stop any background activity
			deferral.Complete();
		}

		/// <summary>
        /// Configures global logging
        /// </summary>
        /// <param name="factory"></param>
		static void ConfigureFilters(ILoggerFactory factory)
		{
			factory
				.WithFilter(new FilterLoggerSettings
					{
						{ "Uno", LogLevel.Warning },
						{ "Windows", LogLevel.Warning },

						// Debug JS interop
						// { "Uno.Foundation.WebAssemblyRuntime", LogLevel.Debug },

						// Generic Xaml events
						// { "Windows.UI.Xaml", LogLevel.Debug },
						// { "Windows.UI.Xaml.VisualStateGroup", LogLevel.Debug },
						// { "Windows.UI.Xaml.StateTriggerBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.UIElement", LogLevel.Debug },

						// Layouter specific messages
						// { "Windows.UI.Xaml.Controls", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Layouter", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.Panel", LogLevel.Debug },
						// { "Windows.Storage", LogLevel.Debug },

						// Binding related messages
						// { "Windows.UI.Xaml.Data", LogLevel.Debug },

						// DependencyObject memory references tracking
						// { "ReferenceHolder", LogLevel.Debug },

						// ListView-related messages
						// { "Windows.UI.Xaml.Controls.ListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.GridView", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelLayout", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.NativeListViewBase", LogLevel.Debug },
						// { "Windows.UI.Xaml.Controls.ListViewBaseSource", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.ListViewBaseInternalContainer", LogLevel.Debug }, //iOS
						// { "Windows.UI.Xaml.Controls.NativeListViewBaseAdapter", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.BufferViewCache", LogLevel.Debug }, //Android
						// { "Windows.UI.Xaml.Controls.VirtualizingPanelGenerator", LogLevel.Debug }, //WASM
					}
				)
#if DEBUG
				.AddConsole(LogLevel.Debug);
#else
				.AddConsole(LogLevel.Information);
#endif
		}
    }
}
