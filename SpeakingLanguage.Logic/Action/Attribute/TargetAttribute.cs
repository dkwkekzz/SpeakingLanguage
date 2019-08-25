using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class TargetAttribute : Attribute
    {
        public List<int> SrcList { get; }

        public TargetAttribute(params Type[] src)
        {
            SrcList = src.ToKeyList();
            SrcList.Sort();
        }
    }
}
