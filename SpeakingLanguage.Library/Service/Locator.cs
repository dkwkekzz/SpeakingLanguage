using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Library
{
    public class Locator
    {
        public static BufferPool BufferPool { get; } = new BufferPool();
    }
}
