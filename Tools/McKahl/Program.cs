using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mc.Core;

namespace McKahl
{
    class Program
    {
        static int Main(string[] args)
        {
            // Create a new command line interpreter class
            // and use it to pass parameters to tools classes
            int result=0;
            var cli = new CLI(args);
            if (cli.HasInstruction("I2P"))
            {
                
            }
            return result;
        }
    }
}
