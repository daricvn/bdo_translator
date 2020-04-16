using BDOTranslator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace BDOTranslator.Utils
{
    public static class StringUtils
    {
        private static JsonSerializerSettings _settings;
        public static JsonSerializerSettings JsonSettings
        {
            get
            {
                if (_settings == null)
                    _settings = new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };
                return _settings;
            }
        }
        public static TextLine? ToLine(this string source)
        {
            var arr = source.Split("\t");
            if (arr.Length > 0)
            {
                return new TextLine(arr[0].ToUint(), arr[1].ToUint(), arr[2].ToUint(), arr[3].ToUint(), arr[4].ToUint(), arr[5].TrimQuotes());
            }
            return null;
        }

        public static uint ToUint(this string source)
        {
            return uint.Parse(source);
        }

        public static string TrimQuotes(this string source)
        {
            var str = source.AsSpan().TrimEnd(new char[] { '\r','\n' });
            if (str.StartsWith("\"") && str.EndsWith("\""))
            {
                return str.Slice(1, str.Length - 2).ToString();
            }
            return str.ToString();
        }

        public static T ToJson<T>(this string source)
        {
            if (source!=null)
                return JsonConvert.DeserializeObject<T>(source, JsonSettings);
            return default(T);
        }

        public static string Stringify(this object source)
        {
            if (source != null)
                return JsonConvert.SerializeObject(source, JsonSettings
                    );
            return string.Empty;
        }

        public static string ReplaceAll(this string source, string pattern, string to, bool ignoreCase= false)
        {
            var str = source;
            var index = -to.Length;
            var lastIndex = -1;
            var compareType = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
            do
            {
                lastIndex = index + to.Length;
                index = str.IndexOf(pattern, compareType);
                if (index >= 0 && index >= lastIndex)
                    str = str.Replace(pattern, to, compareType);
            }
            while (index >= 0 && index>= lastIndex);
            return str;
        }
    }
}
