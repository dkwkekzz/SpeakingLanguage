using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Logic
{
    public class SubjectAttribute : Attribute
    {
        public List<int> SrcList { get; }

        public SubjectAttribute(params Type[] src)
        {
            SrcList = src.ToKeyList();
            SrcList.Sort();
        }
    }
}
