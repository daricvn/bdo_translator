using BDOTranslator_WPF.Controllers;
using Chromely;
using Chromely.Core;
using Chromely.Core.Network;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            protected override void OnLoadStart(CefBrowser browser, CefFrame frame, CefTransitionType transitionType)
            {
                base.OnLoadStart(browser, frame, transitionType);
                if (!File.Exists("./build"))
                {
                    File.WriteAllText("./build", "1");
                }
            }
        }
    }
}
