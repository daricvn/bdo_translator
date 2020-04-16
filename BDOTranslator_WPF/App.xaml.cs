using BDOTranslator_WPF.Implementation;
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
using System.Threading.Tasks;
using System.Windows;

namespace BDOTranslator_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppBuilder
               .Create()
               .UseApp<TranslatorApp>()
               .UseConfiguration<DefaultConfiguration>(new DefaultConfiguration() { 
                    CefDownloadOptions=new CefDownloadOptions(true,true),
                    StartUrl="app://bdo-ui/index.html",
                    WindowOptions = new WindowOptions()
                    {
                        Title="Black Desert Translator",
                         DisableResizing = true,
                         StartCentered = true,
                         DisableMinMaximizeControls= true,
                         Size= new WindowSize(1080,640)
                    },
                    UrlSchemes=new List<UrlScheme>()
                    {
                        new UrlScheme("app", "app", "bdo-ui", string.Empty, UrlSchemeType.AssemblyResource, assemblyOptions: new AssemblyOptions(Assembly.GetExecutingAssembly(),null, "UI.bdoapp.dist")),
                        new UrlScheme("Controller", "http", "bdo-command",string.Empty, UrlSchemeType.Custom)
                    }
               })
               .Build()
               .Run(new string[] { });
            Application.Current.Shutdown();
        }
    }
}
