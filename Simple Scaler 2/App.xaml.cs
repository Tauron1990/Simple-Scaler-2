using System.Diagnostics;
using System.Windows;
using Microsoft.HockeyApp;
using Simple_Scaler_2.Properties;

namespace Simple_Scaler_2
{
    /// <summary>
    ///     Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            //main configuration of HockeySDK
            HockeyClient.Current.Configure("b88853cfd2a1491f844830b7fa4bd8b3")
                        .UseCustomResourceManager(HokeyLocals.ResourceManager) //register your own resourcemanager to override HockeySDK i18n strings
                        //.RegisterCustomUnhandledExceptionLogic((eArgs) => { /* do something here */ }) // define a callback that is called after unhandled exception
                        //.RegisterCustomUnobserveredTaskExceptionLogic((eArgs) => { /* do something here */ }) // define a callback that is called after unobserved task exception
                        //.RegisterCustomDispatcherUnhandledExceptionLogic((args) => { }) // define a callback that is called after dispatcher unhandled exception
                        //.SetApiDomain("https://your.hockeyapp.server")
                        .SetContactInfo("Support E-mail", "support+a6fd853d72034b7ba7c5372ca8eea991@feedback.hockeyapp.net");

            //optional should only used in debug builds. register an event-handler to get exceptions in HockeySDK code that are "swallowed" (like problems writing crashlogs etc.)
            #if DEBUG
            ((HockeyClient)HockeyClient.Current).OnHockeySDKInternalException += (sender, args) =>
                                                                                 {
                                                                                     if (Debugger.IsAttached) { Debugger.Break(); }
                                                                                 };
            #endif
            #if RELEASE
            
            //send crashes to the HockeyApp server
            await HockeyClient.Current.SendCrashesAsync(true);

            #endif
        }
    }
}