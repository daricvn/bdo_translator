using BDOTranslator_WPF.Controllers;
using Chromely;
using Chromely.Core;
using Chromely.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Xilium.CefGlue;

namespace BDOTranslator_WPF.Implementation
{
    public class TranslatorApp:ChromelyBasicApp
    {
        public override void Configure(IChromelyContainer container)
        {
            base.Configure(container);
            container.RegisterSingleton(typeof(ChromelyController), Guid.NewGuid().ToString(), typeof(AppController));
            container.RegisterSingleton(typeof(IChromelyCustomHandler), Guid.NewGuid().ToString(), typeof(CefFrameLoadHandler));
        }

        public class CefFrameLoadHandler:CefLoadHandler
        {
            protected override void OnLoadEnd(CefBrowser browser, CefFrame frame, int httpStatusCode)
            {
                base.OnLoadEnd(browser, frame, httpStatusCode);
                if (frame.IsValid)
                {
                    if (App.DisplayLoader)
                    {
                        App.DisplayLoader = false;
                        Dispatcher.FromThread(App.LoaderThread).Invoke(() =>
                        {
                            App.Loader.Close();
                        });
                        Dispatcher.FromThread(App.LoaderThread).InvokeShutdown();
                    }
                }
            }
        }
    }
}
