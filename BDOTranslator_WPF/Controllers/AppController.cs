using BDOTranslator.Models;
using BDOTranslator.Utils;
using BDOTranslator_WPF.Models;
using BDOTranslator_WPF.Utils;
using Chromely.Core;
using Chromely.Core.Configuration;
using Chromely.Core.Network;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace BDOTranslator_WPF.Controllers
{
    [ControllerProperty(Name = "AppController", Route ="app")]
    public class AppController: ChromelyController
    {
        private const int MAX_HISTORY = 32;
        private const string SCRIPT_PATH = "tools";
        private const string ENC_EXE = "encrypt.exe";
        private const string DEC_EXE = "decrypt.exe";
        private const int DEFAULT_SCAN_THRESHOLD = 20000;
        private readonly IChromelyConfiguration _cfg;
        private TextLine[] lines = null;
        private List<LineTrace[]> history = null;
        private List<LineTrace[]> redo = null;

        public AppController(IChromelyConfiguration config)
        {
            this._cfg = config;
        }

        private void PushHistory(LineTrace[] item)
        {
            if (history == null)
                history = new List<LineTrace[]>(MAX_HISTORY);
            if (history.Count >= MAX_HISTORY)
                history.RemoveAt(0);
            history.Add(item);
        }
        private LineTrace[] PopHistory()
        {
            if (history == null || history.Count==0)
                return null;
            var result = history[history.Count - 1];
            history.RemoveAt(history.Count - 1);
            return result;
        }
        private void PushRedo(LineTrace[] item)
        {
            if (redo == null)
                redo = new List<LineTrace[]>(MAX_HISTORY);
            if (redo.Count >= MAX_HISTORY)
                redo.RemoveAt(0);
            redo.Add(item);
        }
        private LineTrace[] PopRedo()
        {
            if (redo == null || redo.Count==0)
                return null;
            var result = redo[redo.Count - 1];
            redo.RemoveAt(redo.Count - 1);
            return result;
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
            dialog.CheckPathExists = true;
            dialog.CheckFileExists = true;
            dialog.ShowDialog();
            return Response.OK;
        }

        [HttpPost(Route = "/app/save")]
        public ChromelyResponse Save(ChromelyRequest req)
        {
            var path = req.PostData.ToString();
            if (lines != null && lines.Length > 0)
            {
                File.WriteAllLines(path, lines.Select(x => x.ToString()), Encoding.Unicode);
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
            var useRegex = body.ContainsKey("regex") && body["regex"].ToString().Equals("1");
            var exact = body.ContainsKey("exact") && body["exact"].ToString().Equals("1");
            var caseInsenstive = body.ContainsKey("ignoreSensitive") && body["ignoreSensitive"].ToString().Equals("1");
            var direction = body.ContainsKey("direction") && body["direction"].ToString().Equals("up");
            if (direction)
                currentIndex--;
            else currentIndex++;
            var step = direction ? -1 : 1;
            var index = currentIndex;
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
                    if (useRegex)
                    {
                        var regex = new Regex(pattern, caseInsenstive? RegexOptions.IgnoreCase:RegexOptions.None);
                        if (regex.IsMatch(lines[index].Text))
                            return Response.Success(index);
                    }
                    else
                    {
                        if (lines[index].Text.IndexOf(pattern, 
                            caseInsenstive ? 
                            StringComparison.InvariantCultureIgnoreCase : 
                            StringComparison.InvariantCulture)>=0)
                                return Response.Success(index);
                    }
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
            ExecuteScript($"window.$app.setFilePath('{fileName.ReplaceAll("\\","/")}')");
            history?.Clear();
            redo?.Clear();
            Task.Run(()=>
            {
                ExecuteScript($"window.$app.refresh()");
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
                var trace = new List<LineTrace>();
                if (req.Parameters.ContainsKey("autoReplace") && req.Parameters["autoReplace"]=="1")
                {
                    var oldText = lines[index].Text;
                    for (var i = 0; i < lines.Length; i++)
                        if (lines[i].Text == oldText)
                        {
                            trace.Add(new LineTrace(i, lines[i]));
                            lines[i].Text = text.Replace(Environment.NewLine, "\n");
                            ExecuteScript($"window.$app.setLine({i},\"{text}\")");
                        }
                }
                trace.Add(new LineTrace(index, lines[index]));
                lines[index].Text = text.Replace(Environment.NewLine, "\n");
                PushHistory(trace.ToArray());
            }

            return Response.OK;
        }

        [HttpPost(Route = "/regex")]
        public ChromelyResponse ReplaceRegex(ChromelyRequest req)
        {
            var data=req.PostData.ToString().ToJson<Dictionary<string, object>>();
            if (data != null)
            {
                var useRegex = data.ContainsKey("regex") && data["regex"].ToString().Equals("1");
                var caseInsensitive = data.ContainsKey("ignoreSensitive") && data["ignoreSensitive"].ToString().Equals("1");
                var trace = new List<LineTrace>();
                RegexOptions opt = RegexOptions.None;
                if (caseInsensitive)
                {
                    opt = RegexOptions.IgnoreCase;
                }
                var pattern = data["pattern"].ToString();
                var regex = new Regex(pattern, opt);
                var result = data["text"].ToString();
                string text = string.Empty;
                if (lines != null)
                {
                    for (var i = 0; i < lines.Length; i++)
                    {
                        if (useRegex)
                            text = regex.Replace(lines[i].Text, result);
                        else
                        {
                            text = lines[i].Text.ReplaceAll(pattern, result, caseInsensitive);
                        }
                        if (text != lines[i].Text)
                        {
                            trace.Add(new LineTrace(i, lines[i]));
                            lines[i].Text = text.Replace(Environment.NewLine, "\n");
                            ExecuteScript($"window.$app.setLine({i},\"{lines[i].Text}\")");
                        }
                    }
                }
                if (trace.Count > 0)
                    PushHistory(trace.ToArray());
                return Response.Success(trace.Count);
            }
            return Response.BadRequest;
        }

        [HttpGet(Route = "/app/undo")]
        public ChromelyResponse Undo(ChromelyRequest req)
        {
            var items = PopHistory();
            if (items!=null && items.Length > 0)
            {
                var trace = new List<LineTrace>(items.Length);
                foreach (var item in items)
                {
                    if (item.Index>=0 && item.Index < lines.Length)
                    {
                        trace.Add(new LineTrace(item.Index, lines[item.Index]));
                        lines[item.Index].Text = item.Line.Text;
                        ExecuteScript($"window.$app.setLine({item.Index},\"{lines[item.Index].Text}\")");
                    }
                }
                if (trace.Count>0)
                    PushRedo(trace.ToArray());
            }
            return Response.OK;
        }

        [HttpGet(Route = "/app/redo")]
        public ChromelyResponse Redo(ChromelyRequest req)
        {
            var items = PopRedo();
            if (items != null && items.Length > 0)
            {
                var trace = new List<LineTrace>(items.Length);
                foreach (var item in items)
                {
                    if (item.Index >= 0 && item.Index < lines.Length)
                    {
                        trace.Add(new LineTrace(item.Index, lines[item.Index]));
                        lines[item.Index].Text = item.Line.Text;
                        ExecuteScript($"window.$app.setLine({item.Index},\"{lines[item.Index].Text}\")");
                    }
                }
                if (trace.Count > 0)
                    PushHistory(trace.ToArray());
            }
            return Response.OK;
        }
        [HttpGet(Route = "/app/file-dialog")]
        public ChromelyResponse OpenFileDialog(ChromelyRequest req)
        {
            string textOnly = "Text files (*.txt)|*.txt";
            string locOnly = "Loc files (*.loc)|*.loc";

            string filter = string.Empty;
            bool saveOnly = false;
            if (req.Parameters.ContainsKey("type"))
            {
                var type = req.Parameters["type"];
                if (type.Contains("txt"))
                    filter = textOnly;
                if (type.Contains("loc"))
                {
                    if (!string.IsNullOrEmpty(filter))
                        filter = string.Join("|", textOnly, locOnly);
                    else
                        filter = locOnly;
                }
            }
            else if (req.Parameters.ContainsKey("filter"))
            {
                filter = req.Parameters["filter"];
            }
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            dialog.CheckPathExists = true;
            dialog.CheckFileExists = !saveOnly;
            dialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(dialog.FileName))
                return Response.Success(dialog.FileName.ReplaceAll("\\", "/"));
            return Response.OK;
        }


        [HttpGet(Route = "/app/file-save-dialog")]
        public ChromelyResponse SaveFileDialog(ChromelyRequest req)
        {
            string textOnly = "Text files (*.txt)|*.txt";
            string locOnly = "Loc files (*.loc)|*.loc";

            string filter = string.Empty;
            if (req.Parameters.ContainsKey("type"))
            {
                var type = req.Parameters["type"];
                if (type.Contains("txt"))
                    filter = textOnly;
                if (type.Contains("loc"))
                {
                    if (!string.IsNullOrEmpty(filter))
                        filter = string.Join("|", textOnly, locOnly);
                    else
                        filter = locOnly;
                }
            }
            else if (req.Parameters.ContainsKey("filter"))
            {
                filter = req.Parameters["filter"];
            }
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = filter;
            dialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(dialog.FileName))
                return Response.Success(dialog.FileName.ReplaceAll("\\", "/"));
            return Response.OK;
        }

        [HttpPost(Route = "/app/run-script")]
        public ChromelyResponse RunScript(ChromelyRequest req)
        {
            var body = req.PostData.ToString().ToJson<Dictionary<string, object>>();
            var encrypt = body.ContainsKey("encrypt") && body["encrypt"].ToString().Equals("1");
            var basePath = System.IO.Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);
            var path = System.IO.Path.Combine(basePath, SCRIPT_PATH, encrypt ? ENC_EXE : DEC_EXE);
            ProcessStartInfo info = new ProcessStartInfo(path);
            var source = body["source"].ToString();
            var dest = body["dest"].ToString();
            //var tempSource = System.IO.Path.GetTempFileName();
            //var tempDest = System.IO.Path.GetTempFileName();
            //while (tempSource == tempDest)
            //    tempDest = System.IO.Path.GetTempFileName();
            //File.Copy(source, tempSource, true);
            //if (File.Exists(tempDest))
            //    File.Delete(tempDest);
            //info.ArgumentList.Add(tempSource);
            //info.ArgumentList.Add(tempDest);
            //info.ArgumentList.Add("/c");
            //info.ArgumentList.Add("start");
            //info.ArgumentList.Add(path);
            //info.ArgumentList.Add(source);
            //info.ArgumentList.Add(dest);
            //info.CreateNoWindow = true;
            //info.UseShellExecute = true;
            //info.WindowStyle = ProcessWindowStyle.Hidden;
            //var proc = Process.Start(info);
            //proc.WaitForExit();
            //File.Delete(tempSource);
            //if (File.Exists(dest))
            //    File.Delete(dest);
            //File.Move(tempDest, dest);
            if (!encrypt)
                BDOScript.Decrypt(source, dest);
            else
                BDOScript.Encrypt(source, dest);
            return Response.OK;
        }

        [HttpPost(Route = "/app/run-patcher")]
        public ChromelyResponse RunPatcher(ChromelyRequest req)
        {
            var body = req.PostData.ToString().ToJson<Dictionary<string, object>>();
            var source = body["source"].ToString();
            var dest = body["dest"].ToString();
            var loc1 = new LocalizationFile(source);
            var loc2 = new LocalizationFile(dest);
            var line1 = loc1.Process();
            var line2 = loc2.ProcessWithIndexer(out var indexer);
            long index = 0;
            for (var i = 0; i < line1.Length; i++)
            {
                index = indexer.GetIndex(line1[i]);
                if (index>=0 && index< line2.Length)
                    if (line1[i].HasSameAddress(line2[index]))
                    {
                        if (line1[i].Text != line2[index].Text)
                            line2[index].Text = line1[i].Text;
                    }
            }
            File.WriteAllLines(dest, line2.Select(x => x.ToString()), Encoding.Unicode);
            return Response.OK;
        }
        [HttpPost(Route = "/app/run-gzip")]
        public ChromelyResponse RunGzip(ChromelyRequest req)
        {
            var body = req.PostData.ToString().ToJson<Dictionary<string, object>>();
            var source = body["source"].ToString();
            var dest = body["dest"].ToString();
            if (File.Exists(source)){
                var text = File.ReadAllText(source);
                var bytes = Encoding.Unicode.GetBytes(text);
                using (var fs = new FileStream(dest, FileMode.Create, FileAccess.Write))
                using (var df = new DeflateStream(fs, CompressionLevel.Fastest))
                {
                    df.Write(bytes, 0, bytes.Length);
                }
                return Response.OK;
            }
            return Response.BadRequest;
        }

        [HttpPost(Route = "/app/explorer")]
        public ChromelyResponse OpenExplorer(ChromelyRequest req)
        {
            var p = req.PostData.ToString();
            if (p != null)
            {
                var dir = System.IO.Path.GetDirectoryName(p);
                if (!string.IsNullOrEmpty(dir))
                    Process.Start("explorer.exe", dir);
            }
            return Response.OK;
        }

        [HttpPost(Route = "/app/close")]
        public ChromelyResponse Close(ChromelyRequest req)
        {
            Environment.Exit(0);
            return Response.OK;
        }
    }
}
