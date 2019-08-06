using System;
using System.Collections.Generic;
using System.Text;

namespace SpeakingLanguage.Table
{
    internal class ColumnConstructor
    {
        private const string namespaceTemplate = "namespace SpeakingLanguage.Table\r\n{\r\n__INNER__}\r\n";
        private const string classTemplate = "public struct __CLASS_NAME__\r\n{\r\n__INNER__}\r\n";
        private const string fieldTemplate = "public __TYPE_NAME__ __FIELD_NAME__;";

        struct Column
        {
            public string type;
            public string name;
            public int maxLen;
        }

        class ClassInfo
        {
            private int _length = 1;
            private Dictionary<string, ClassInfo> _dicCls = new Dictionary<string, ClassInfo>();
            private Dictionary<string, Column> _dicClm = new Dictionary<string, Column>();

            public int Depth { get; }

            public ClassInfo(int dep)
            {
                Depth = dep;
            }

            public void IncreaseLength()
            {
                _length++;
            }

            public void Insert(string type, string column, int maxLen, int begin)
            {
                int end = 0;
                if ((end = column.IndexOf('.', begin)) < 0)
                {
                    var columnName = column.Substring(begin, column.Length - begin);
                    if (!_dicClm.ContainsKey(columnName))
                        _dicClm.Add(columnName, new Column { type = type, name = columnName, maxLen = maxLen });
                    return;
                }

                int index = 0;
                var lenClm = end - begin;
                if (column[end - 1] == ']')
                {
                    var bucketBegin = column.LastIndexOf('[', end);
                    var strNum = column.Substring(bucketBegin + 1, end - bucketBegin - 2);
                    index = int.Parse(strNum);
                    lenClm = bucketBegin - begin;
                }

                var clsName = column.Substring(begin, lenClm);
                if (!_dicCls.TryGetValue(clsName, out ClassInfo info))
                {
                    info = new ClassInfo(Depth + 1);
                    _dicCls.Add(clsName, info);
                }

                info.Insert(type, column, maxLen, end + 1);

                var clsClmName = $"{clsName}_{index.ToString("00")}";
                if (!_dicClm.ContainsKey(clsClmName))
                    _dicClm.Add(clsClmName, new Column { type = clsName, name = clsClmName, maxLen = maxLen });
            }

            public override string ToString()
            {
                var sb = new StringBuilder();

                var clsIter = _dicCls.GetEnumerator();
                while (clsIter.MoveNext())
                {
                    var info = clsIter.Current.Value;
                    var txtCls = info.ToString().Replace("__CLASS_NAME__", clsIter.Current.Key);
                    sb.AppendLine(txtCls);
                }

                var clmIter = _dicClm.GetEnumerator();
                while (clmIter.MoveNext())
                {
                    string column;
                    var clm = clmIter.Current.Value;
                    if (clm.type == "Text")
                    {
                        if (clm.maxLen <= 16)
                            column = fieldTemplate.Replace("__TYPE_NAME__", "String16");
                        else if (clm.maxLen <= 32)
                            column = fieldTemplate.Replace("__TYPE_NAME__", "String32");
                        else if (clm.maxLen <= 64)
                            column = fieldTemplate.Replace("__TYPE_NAME__", "String64");
                        else
                            column = fieldTemplate.Replace("__TYPE_NAME__", "String128");
                    }
                    else if (clm.type == "Integer")
                        column = fieldTemplate.Replace("__TYPE_NAME__", "int");
                    else if (clm.type == "Boolean")
                        column = fieldTemplate.Replace("__TYPE_NAME__", "bool");
                    else
                        column = fieldTemplate.Replace("__TYPE_NAME__", clm.type);

                    column = column.Replace("__FIELD_NAME__", clm.name);

                    sb.AppendLine(column);
                }

                return classTemplate.Replace("__INNER__", sb.ToString());
            }
        }

        private string _rootName;
        private ClassInfo _rootInfo;

        public ColumnConstructor(string rootName)
        {
            _rootName = rootName;
            _rootInfo = new ClassInfo(0);
        }

        public void Insert(string type, string name, int maxLen)
        {
            _rootInfo.Insert(type, name, maxLen, 0);
        }

        public override string ToString()
        {
            return namespaceTemplate.Replace("__INNER__", _rootInfo.ToString().Replace("__CLASS_NAME__", _rootName));
        }
    }
}
