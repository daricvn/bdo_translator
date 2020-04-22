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
using System.IO;
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
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var f = new Loading();
            Thread thread = new Thread(() =>
            {
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
                if (File.Exists("build"))
                {
                    File.Delete("build");
                }

                Environment.Exit(0);
            });
            thread.IsBackground = false;
            thread.Start();
            f.Show();
            while (!File.Exists("build"))
                await Task.Delay(500);
            f.Close();
        }

        private void Application_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }
    }
}
