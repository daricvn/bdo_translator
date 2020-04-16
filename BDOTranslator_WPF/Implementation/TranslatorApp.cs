using BDOTranslator_WPF.Controllers;
using Chromely;
using Chromely.Core;
using Chromely.Core.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDOTranslator_WPF.Implementation
{
    public class TranslatorApp:ChromelyBasicApp
    {
        public override void Configure(IChromelyContainer container)
        {
            base.Configure(container);
            container.RegisterSingleton(typeof(ChromelyController), Guid.NewGuid().ToString(), typeof(AppController));
        }
    }
}
