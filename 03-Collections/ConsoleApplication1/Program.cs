using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Collections.Tasks;
using System.IO;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TextReader reader = new StringReader("TextReader, is, the, abstract, base, class, of, StreamReader, and, StringReader, which"))
            {
                var list = Collections.Tasks.Task.Tokenize(reader);
            }
        }
    }
}