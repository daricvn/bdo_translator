using BDOTranslator.Models;
using BDOTranslator_WPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BDOTranslator.Utils
{
    public class LocalizationFile
    {
        private readonly string path;

        public LocalizationFile(string path)
        {
            this.path = path;
        }

        public TextLine[] Process()
        {
            using (var fs= new FileStream(path,FileMode.Open, FileAccess.Read))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
            {
                List<TextLine> lines= new List<TextLine>();
                string str = null;
                while (!string.IsNullOrWhiteSpace((str = sr.ReadLine()))){
                    var line = str.ToLine();
                    if (line != null)
                        lines.Add(line.Value);
                }
                return lines.ToArray();
            }
        }


        public TextLine[] ProcessWithIndexer(out LocIndexer indexer)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var bs = new BufferedStream(fs))
            using (var sr = new StreamReader(bs))
            {
                List<TextLine> lines = new List<TextLine>();
                indexer = new LocIndexer();
                string str = null;
                int count = 0;
                while (!string.IsNullOrWhiteSpace((str = sr.ReadLine())))
                {
                    var line = str.ToLine();
                    if (line != null)
                    {
                        lines.Add(line.Value);
                        indexer.CreateIndex(line.Value, count);
                        count++;
                    }
                }
                return lines.ToArray();
            }
        }

    }
}
