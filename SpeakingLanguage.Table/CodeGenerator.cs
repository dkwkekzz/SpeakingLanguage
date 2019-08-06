using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SpeakingLanguage.Table
{
    public class CodeGenerator
    {
        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory", SetLastError = false)]
        private static unsafe extern void MoveMemory(void* dest, void* src, int size);

        private static unsafe void MoveMemory<T>(IntPtr dest, void* src) where T : unmanaged
        {
            var sz = sizeof(T);
            MoveMemory(dest.ToPointer(), src, sz);
            dest += sz;
        }

        private readonly string _folderPath;

        public CodeGenerator(string folderPath)
        {
            _folderPath = folderPath;
        }

        public void GenerateFromCsv(string filePath)
        {
            using (var file = new System.IO.StreamReader(filePath))
            {
                string line;
                // date time
                line = file.ReadLine();
                // type
                line = file.ReadLine();
                var arrType = line.Split(',', '\t');
                // name
                line = file.ReadLine();
                var arrName = line.Split(',', '\t');

                var arrLine = new List<string[]>();
                while ((line = file.ReadLine()) != null)
                {
                    var arrValue = line.Split(',', '\t');
                    arrLine.Add(arrValue);
                }

                var arrMaxLen = new int[arrType.Length];
                for (int lineIdx = 0; lineIdx != arrLine.Count; lineIdx++)
                {
                    var arrValue = arrLine[lineIdx];
                    for (int i = 0; i != arrValue.Length; i++)
                    {
                        arrMaxLen[i] = Math.Max(arrMaxLen[i], arrValue[i].Length);
                    }
                }

                // write script
                var bi = filePath.LastIndexOf('/');
                if (bi < 0)
                    bi = 0;
                var ei = filePath.LastIndexOf('.');

                var rootName = filePath.Substring(bi, ei - bi);
                var ctr = new ColumnConstructor(rootName);
                for (int i = 0; i != arrType.Length; i++)
                {
                    ctr.Insert(arrType[i], arrName[i], arrMaxLen[i]);
                }

                var scriptPath = Path.Combine(_folderPath, $"{rootName}.cs");
                var text = ctr.ToString();
                File.WriteAllText(scriptPath, text);

                // write raw data
                var root = Marshal.AllocHGlobal(1000000);
                var head = root;

                unsafe
                {
                    for (int lineIdx = 0; lineIdx != arrLine.Count; lineIdx++)
                    {
                        var begin = head;
                        var arrValue = arrLine[lineIdx];
                        for (int i = 0; i != arrType.Length; i++)
                        {
                            var type = arrType[i];
                            var value = arrValue[i];
                            if (string.IsNullOrEmpty(value))
                            {
                                if (type == "Text")
                                {
                                    var maxLen = arrMaxLen[i];
                                    if (maxLen <= 16)
                                        head += sizeof(String16);
                                    else if (maxLen <= 32)
                                        head += sizeof(String32);
                                    else if (maxLen <= 64)
                                        head += sizeof(String64);
                                    else
                                        head += sizeof(String128);
                                }
                                else if (type == "Integer")
                                    head += sizeof(int);
                                else if (type == "Integer64")
                                    head += sizeof(long);
                                else if (type == "Boolean")
                                    head += sizeof(bool);
                                continue;
                            }

                            if (type == "Text")
                            {
                                var maxLen = arrMaxLen[i];
                                if (maxLen <= 16)
                                {
                                    var sv = StringHelper.Parse<String16>(value);
                                    MoveMemory<String16>(head, &sv);
                                }
                                else if (maxLen <= 32)
                                {
                                    var sv = StringHelper.Parse<String32>(value);
                                    MoveMemory<String32>(head, &sv);
                                }
                                else if (maxLen <= 64)
                                {
                                    var sv = StringHelper.Parse<String64>(value);
                                    MoveMemory<String64>(head, &sv);
                                }
                                else
                                {
                                    var sv = StringHelper.Parse<String128>(value);
                                    MoveMemory<String128>(head, &sv);
                                }
                            }
                            else if (type == "Integer")
                            {
                                var iv = int.Parse(value);
                                MoveMemory<int>(head, &iv);
                            }
                            else if (type == "Integer64")
                            {
                                var lv = long.Parse(value);
                                MoveMemory<long>(head, &lv);
                            }
                            else if (type == "Boolean")
                            {
                                var bv = bool.Parse(value);
                                MoveMemory<bool>(head, &bv);
                            }
                        }

                        // for debug
                        var cls = new CommonPackage();
                        var destPtr = (IntPtr)(&cls);
                        MoveMemory<CommonPackage>(destPtr, begin.ToPointer());
                        Console.WriteLine($"done: {lineIdx.ToString()}");
                    }
                }
                
                Marshal.FreeHGlobal(root);
            }
        }
    }
}
