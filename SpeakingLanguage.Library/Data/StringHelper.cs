using System;
using System.Collections.Generic;
using System.Text;

namespace SpeakingLanguage.Library
{
    public static class StringHelper
    {
        public static unsafe string ToManagedString<TString>(TString value) where TString : unmanaged
        {
            var sb = new StringBuilder();
            var ptr = (char*)(&value);
            for (int i = 0; i != sizeof(TString) / sizeof(char); i++)
            {
                var c = *ptr;
                if (c == '\0')
                    break;
                sb.Append(c);
                ptr++;
            }
            
            return sb.ToString();
        }

        public static unsafe TString Parse<TString>(string value) where TString : unmanaged
        {
            var ret = new TString();
            var ptr = (char*)(&ret);
            for (int i = 0; i != value.Length; i++)
            {
                *ptr = value[i];
                ptr++;
            }

            return ret;
        }

        public static unsafe bool TryParse<TString>(string value, out TString ret) where TString : unmanaged
        {
            if (value.Length > (sizeof(TString) / sizeof(char)))
            {
                ret = default(TString);
                return false;
            }

            ret = new TString();
            fixed (TString* ptr = &ret)
            {
                var chPtr = (char*)ptr;
                for (int i = 0; i != value.Length; i++)
                {
                    *chPtr = value[i];
                    chPtr++;
                }
            }

            return true;
        }
    }
}
