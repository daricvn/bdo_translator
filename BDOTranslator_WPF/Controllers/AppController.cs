using BDOTranslator.Models;
using BDOTranslator.Utils;
using BDOTranslator_WPF.Models;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Network;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace BDOTranslator_WPF.Controllers
{
    [ControllerProperty(Name = "AppController", Route ="app")]
    public class AppController: ChromelyController
    {
        private readonly IChromelyConfiguration _cfg;
        private TextLine[] lines = null;

        public AppController(IChromelyConfiguration config)
        {
            this._cfg = config;
        }

        public IChromelyJavaScriptExecutor JavascriptEngine
        {
            get
            {
                if (_cfg != null)
                    return _cfg.JavaScriptExecutor;
                return null;
            }
        }

        public bool ExecuteScript(string script)
        {
            if (JavascriptEngine != null)
            {
                JavascriptEngine.ExecuteScript(script);
                return true;
            }
            return false;
        }

        [HttpGet(Route = "/app/line")]
        public ChromelyResponse Get(ChromelyRequest req)
        {
            var index = int.Parse(req.Parameters["offset"]);
            if (index >= 0 && index < lines.Length)
                return Response.Success(lines[index].Stringify());
            return Response.NotFound;
        }

        [HttpGet(Route = "/app/browse")]
        public ChromelyResponse Browse(ChromelyRequest request)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt";
            dialog.FileOk += Dialog_FileOk;
            dialog.ShowDialog();
            return Response.OK;
        }

        [HttpPost(Route = "/app/save")]
        public ChromelyResponse Save(ChromelyRequest req)
        {
            var path = req.PostData.ToString();
            if (lines != null && lines.Length > 0)
            {
                File.WriteAllLines(path, lines.Select(x => x.ToString()));
                return Response.OK;
            }
            return Response.BadRequest;
        }

        [HttpPost(Route = "/findIndex")]
        public ChromelyResponse FindIndex(ChromelyRequest req)
        {
            var body = req.PostData.ToString().ToJson<Dictionary<string, object>>();
            var pattern = body["pattern"].ToString();
            var currentIndex = int.Parse(body["currentIndex"].ToString());
            var exact = body.ContainsKey("exact") && body["exact"].Equals(1);
            var caseInsenstive = body.ContainsKey("ignoreSensitive") && body["ignoreSensitive"].Equals(1);
            var direction = body.ContainsKey("direction") && body["direction"].ToString().Equals("up");
            if (direction)
                currentIndex--;
            else currentIndex++;
            var step = direction ? -1 : 1;
            var index = currentIndex;
            RegexOptions option = RegexOptions.None;
            while (index >=0 && index< lines.Length)
            {
                if (exact)
                {
                    if (caseInsenstive)
                    {
                        if (lines[index].Text.ToLower() == pattern.ToLower())
                            return Response.Success(index);
                    }
                    else
                    {
                        if (lines[index].Text == pattern)
                            return Response.Success(index);
                    }
                }
                else
                {
                    if (caseInsenstive)
                        option = RegexOptions.IgnoreCase;
                    else
                        option = RegexOptions.None;
                    var regex = new Regex(pattern, option);
                    if (regex.IsMatch(lines[index].Text))
                        return Response.Success(index);
                }
                index += step;
            }
            return Response.Success(-1);
        }

        private void Dialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var dialog = (OpenFileDialog)sender;
            var fileName = dialog.FileName;
            ExecuteScript($"window.$app.setLoading(true)");
            ExecuteScript($"window.$app.setFilePath('{fileName.Replace("\\","/")}')");
            Task.Run(()=>
            {
                var reader = new LocalizationFile(fileName);
                lines = reader.Process();
                var data = lines.Length;
                ExecuteScript($"window.$app.setListCount({data})");
                ExecuteScript($"window.$app.setLoading(false)");
            });
        }

        [HttpPut(Route = "/update/text")]
        public ChromelyResponse Update(ChromelyRequest req)
        {
            var index = int.Parse(req.Parameters["index"]);
            var text = req.PostData.ToString();

            if (index >= 0 && index < lines.Length)
            {
                if (req.Parameters.ContainsKey("autoReplace") && req.Parameters["autoReplace"]=="1")
                {
                    var oldText = lines[index].Text;
                    for (var i = 0; i < lines.Length; i++)
                        if (lines[i].Text == oldText)
                        {
                            lines[i].Text = text;
                            ExecuteScript($"window.$app.setLine({i},'{text}')");
                        }
                }
                lines[index].Text = text;
            }

            return Response.OK;
        }

        [HttpPost(Route = "/regex")]
        public ChromelyResponse ReplaceRegex(ChromelyRequest req)
        {
            var data=req.PostData.ToString().ToJson<Dictionary<string, object>>();
            if (data != null)
            {
                RegexOptions opt = RegexOptions.None;
                if (data.ContainsKey("ignoreSensitive") && data["ignoreSensitive"].Equals(1))
                {
                    opt = RegexOptions.IgnoreCase;
                }
                var regex = new Regex(data["pattern"].ToString(), opt);
                var result = data["text"].ToString();
                var count = 0;
                if (lines != null)
                {
                    string old = string.Empty;
                    for (var i = 0; i < lines.Length; i++)
                    {
                        old = lines[i].Text;
                        lines[i].Text = regex.Replace(lines[i].Text, result);
                        if (old != lines[i].Text)
                        {
                            count++;
                            ExecuteScript($"window.$app.setLine({i},'{lines[i].Text}')");
                        }
                    }
                }
                return Response.Success(count);
            }
            return Response.BadRequest;
        }
    }
}
