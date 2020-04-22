using BDOTranslator_WPF.Implementation;
using BDOTranslator_WPF.Utils;
using Chromely;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Infrastructure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace BDOTranslator_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static volatile Loading Loader=null;
        public static volatile bool DisplayLoader = false;
        public static volatile Thread LoaderThread = null;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    DisplayLoader = true;
                    var f = new Loading();
                    f.Loaded += (a, b) =>
                    {
                        //Task.Run(() =>
                        //{
                        //    while (DisplayLoader)
                        //        Thread.Sleep(250);
                        //});
                    };
                    Loader = f;
                    f.Show();
                    Dispatcher.Run();
                }
                catch
                {
                    Loader.Close();
                }
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            LoaderThread = thread;
            thread.Start();
            var assembly = Assembly.GetExecutingAssembly();
            AppBuilder
               .Create()
               .UseApp<TranslatorApp>()
               .UseConfiguration<DefaultConfiguration>(new DefaultConfiguration()
               {
                   CefDownloadOptions = new CefDownloadOptions(true, true),
                   StartUrl = "app://bdo-ui/index.html",
                   DebuggingMode = true,
                   WindowOptions = new WindowOptions()
                   {
                       Title = "Black Desert Translator",
                       RelativePathToIconFile = "./app.ico",
                       DisableResizing = true,
                       StartCentered = true,
                       DisableMinMaximizeControls = true,
                       Size = new WindowSize(1080, 640)
                   },
                   UrlSchemes = new List<UrlScheme>()
                    {
                        new UrlScheme("app", "app", "bdo-ui", string.Empty, UrlSchemeType.AssemblyResource, assemblyOptions: new AssemblyOptions(assembly,"BDOTranslator_WPF", "UI.bdoapp.dist")),
                        new UrlScheme("Controller", "http", "bdo-command",string.Empty, UrlSchemeType.Custom)
                    }
               })
               .Build()
               .Run(new string[] {
               });
            if (Loader != null)
                Loader.Dispatcher.BeginInvoke(() =>
                {
                    Loader.Close();
                });
            Application.Current.Shutdown();
        }

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }
    }
}
