using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    static class TestTable
    {
        static unsafe void Main()
        {
            Console.WriteLine($"size: {sizeof(SpeakingLanguage.Table.String16).ToString()}");
            Console.WriteLine($"size: {sizeof(SpeakingLanguage.Table.String32).ToString()}");
            Console.WriteLine($"size: {sizeof(SpeakingLanguage.Table.String64).ToString()}");
            Console.WriteLine($"size: {sizeof(bool).ToString()}"); 
            Console.WriteLine($"size: {sizeof(int).ToString()}");
            Console.WriteLine($"size: {sizeof(SpeakingLanguage.Table.CommonPackage.EventPackage).ToString()}");
            Console.WriteLine($"size: {sizeof(SpeakingLanguage.Table.CommonPackage).ToString()}");
            //var sv32 = SpeakingLanguage.Table.StringHelper.Parse<SpeakingLanguage.Table.String32>("i\'m very very very power archer.");
            //var sv64 = SpeakingLanguage.Table.StringHelper.Parse<SpeakingLanguage.Table.String64>("i\'m very very very power archer. ops! good!!!");

            var ctr = new SpeakingLanguage.Table.CodeGenerator("");
            ctr.GenerateFromCsv("CommonPackage.csv");

            Console.ReadLine();
        }
    }

    struct AddOption
    {
        public string Code;
    }
}
