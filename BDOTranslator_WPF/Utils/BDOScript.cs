using BDOTranslator.Utils;
using ComponentAce.Compression.Libs.zlib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace BDOTranslator_WPF.Utils
{
    public class BDOScript
    {
        const char CHAR_CR = (char) 0x000D;
        const char CHAR_LF = (char) 0x000A;
        const char BOM_UTF16LE = (char) 0xFEFF;
        const ulong MAX_BUFF_SIZE = 4096;
        const char CHAR_NULL = (char) 0x0000;

        public static void Decrypt(string sourcePath, string destPath)
        {

            var (tmpName, tmp) = CreateTempFile();
            using (var fs = new FileStream(sourcePath, FileMode.Open, FileAccess.ReadWrite))
            using (var ofs = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            using (var br = new BinaryReader(fs))
            {
                ulong compressedSize = 0;
                ulong decompressedSize = 0;
                fs.Seek(0, SeekOrigin.End);
                compressedSize = (ulong) fs.Position - 4;
                fs.Seek(0, SeekOrigin.Begin);
                decompressedSize = br.ReadUInt32();
                //decompressedSize = BitConverter.ToUInt64(bytes);
                //var bytes = new byte[compressedSize];
                //fs.Read(bytes, 4, (int) compressedSize);
                var chars = br.ReadBytes((int)compressedSize);
                    //.Select(x=> (byte)x).ToArray();
                Span<byte> uncompressedData;
                using (var output = new MemoryStream())
                {
                    using (var input = new MemoryStream(chars))
                    using (var zlib = new InflaterInputStream(input))
                    {
                        zlib.CopyTo(output);
                    }
                    uncompressedData = output.ToArray().AsSpan();
                }
                if (uncompressedData.Length == (int) decompressedSize)
                {
                    tmp.Write(uncompressedData);
                    tmp.Seek(0, SeekOrigin.Begin);
                    ProcessUncompressedData(tmp, ofs);
                }
            }
            tmp.Dispose();
            File.Delete(tmpName);
        }

        public static void Encrypt(string sourceFile, string destFile)
        {
            ulong size;
            ulong type;
            ulong id1;
            ulong id2;
            byte id3;
            byte id4;
            string temp;
            var (tmpName, tmp) = CreateTempFile();
            using (var bw = new BinaryWriter(tmp))
            {
                int a;
                LocalizationFile file = new LocalizationFile(sourceFile);
                var lines = file.Process();
                List<short> span = new List<short>((int)MAX_BUFF_SIZE);
                var index = 0;
                foreach (var line in lines)
                {
                    //strBuff.Clear();
                    span.Clear();
                    type = (ulong)line.Type;
                    id1 = (ulong)line.Addr1;
                    id2 = (ulong)line.Addr2;
                    id3 = (byte)line.Addr3;
                    id4 = (byte)line.Addr4;
                    temp = line.Text; //string.Concat(line.Text,"\r\n");
                    for (a = 0; a < temp.Length; a++)
                    {
                        index = span.Count;
                        span.Add((short)temp[a]);
                        //strBuff[a] = temp[a];
                        if (a > 0)
                        {
                            if (temp[a] == 'n' && temp[a - 1].ToString() == "\\")
                            {
                                span[index-1] = (short)CHAR_LF;
                                span.RemoveAt(index);
                            }
                        }
                    }
                    size = (ulong) span.Count;
                    bw.Write((UInt32)size); // strSize - 4
                    bw.Write((UInt32)type); //strType - 4
                    bw.Write((UInt32)id1); // Id1 - 4
                    bw.Write((UInt16)id2); // id2 -2
                    bw.Write((byte)id3); //id3 -1 
                    bw.Write((byte)id4); //id4 -1
                    foreach (var c in span)// tempSpan)
                        bw.Write(c); // -size
                    bw.Write((short) CHAR_NULL);
                    bw.Write((short) CHAR_NULL);
                }
                tmp.Seek(0, SeekOrigin.End);
                ulong uncompressedSize = (ulong)tmp.Position;
                ulong compressedSize;
                Span<byte> uncompressedData = new byte[uncompressedSize];
                Span<byte> compressedData;
                tmp.Seek(0, SeekOrigin.Begin);
                using (var br = new BinaryReader(tmp))
                {
                    uncompressedData = br.ReadBytes((int)uncompressedSize);

                    //using (var ms = new MemoryStream(uncompressedData.ToArray()))
                    using (var output = new MemoryStream())
                    {
                        using (var zlib = new DeflaterOutputStream(output, new Deflater(Deflater.BEST_SPEED)))
                        {
                            zlib.Write(uncompressedData.ToArray());
                            zlib.Finish();
                            compressedData = new byte[output.Length];
                            compressedSize = (ulong)output.Length;
                            compressedData = output.ToArray().AsSpan();
                        }
                    }
                }
                using (var dfs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                using (var writer = new BinaryWriter(dfs, Encoding.Unicode))
                {
                    writer.Write((UInt32)uncompressedSize);
                    writer.Write(compressedData);
                }
            }
            tmp.Dispose();
            File.Delete(tmpName);
        }

        private static void ProcessUncompressedData(Stream stream, Stream outputStream)
        {
            ulong size;
            ulong type;
            ulong id1;
            ulong id2;
            byte id3;
            byte id4;
            char tempChar;
            Span<char> strBuff = new char[MAX_BUFF_SIZE];
            ulong a;
            int b;
            using (BufferedStream bs = new BufferedStream(stream))
            using (StreamWriter sw= new StreamWriter(outputStream, Encoding.Unicode))
            using (var tmpBr = new BinaryReader(bs))
            {
                long length= tmpBr.BaseStream.Length;
                while (tmpBr.BaseStream.Position < length)
                {
                    if (tmpBr.BaseStream.Position >= length)
                        break;
                    size = tmpBr.ReadUInt32();
                    if (tmpBr.BaseStream.Position >= length)
                        break;
                    type = tmpBr.ReadUInt32();
                    if (tmpBr.BaseStream.Position >= length)
                        break;
                    id1 = tmpBr.ReadUInt32();
                    if (tmpBr.BaseStream.Position >= length)
                        break;
                    id2 = tmpBr.ReadUInt16();
                    if (tmpBr.BaseStream.Position >= length)
                        break;
                    id3 = tmpBr.ReadByte();
                    if (tmpBr.BaseStream.Position >= length)
                        break;
                    id4 = tmpBr.ReadByte();
                    b = 0;
                    strBuff.Clear();
                    for (a = 0; a < size + 2; a++)
                    {
                        if (tmpBr.BaseStream.Position >= length)
                            break;
                        tempChar = (char)tmpBr.ReadInt16();
                        if (tempChar == CHAR_LF)
                        {
                            strBuff[b] = '\\';
                            b++;
                            strBuff[b] = 'n';
                            b++;
                        }
                        else
                        {
                            strBuff[b] = tempChar;
                            b++;
                        }
                    }
                    sw.WriteLine(string.Join("\t", type, id1, id2, id3, id4, string.Format("\"{0}\"", (strBuff).ToString().TrimStart('\0').Trim('\0').TrimEnd('\0'))));
                }
            }
        }

        private static void CopyStream(System.IO.Stream input, System.IO.Stream output)
        {
            byte[] buffer = new byte[MAX_BUFF_SIZE];
            int len;
            while ((len = input.Read(buffer, 0, (int) MAX_BUFF_SIZE)) > 0)
            {
                output.Write(buffer, 0, len);
            }
            output.Flush();
        }

        private static (string, FileStream) CreateTempFile()
        {
            var fileName = Path.GetTempFileName();
            return (fileName, new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite));
        }
    }
}
