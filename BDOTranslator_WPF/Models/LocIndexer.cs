using BDOTranslator.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BDOTranslator_WPF.Models
{
    public class LocIndexer
    {
        private readonly Dictionary<string, long> _index;
        public LocIndexer()
        {
            _index = new Dictionary<string, long>();
        }

        public void CreateIndex(TextLine line, long index)
        {
            var key = GetKey(line);
            if (!_index.ContainsKey(key)){
                _index.Add(key, index);
            }
        }

        public long GetIndex(TextLine line)
        {
            var key = GetKey(line);
            if (_index.ContainsKey(key))
                return _index[key];
            return -1;
        }

        private string GetKey(TextLine line)
        {
            return string.Join(":", line.Type, line.Addr1, line.Addr2, line.Addr3, line.Addr4);
        }
    }
}
