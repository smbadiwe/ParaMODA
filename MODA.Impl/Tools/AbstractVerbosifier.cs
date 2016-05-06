using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Tools
{
    public abstract class AbstractVerbosifier
    {
        protected bool verbose = true;
        protected TextWriter defaultPrintStream = Console.Out;

        public TextWriter getDefaultPrintStream()
        {
            return defaultPrintStream;
        }

        public TextWriter setDefaultPrintStream(TextWriter defaultPrintStream)
        {
            TextWriter old = this.defaultPrintStream;
            this.defaultPrintStream = defaultPrintStream;
            return old;
        }

        public void print(string toPrint)
        {
            print(toPrint, defaultPrintStream);
        }

        public void printIter(string toPrint)
        {
            printIter(toPrint, defaultPrintStream);
        }

        public void println(string toPrint)
        {
            println(toPrint, defaultPrintStream);
        }

        public void printlnIter(string toPrint)
        {
            printlnIter(toPrint, defaultPrintStream);
        }

        public void println(string toPrint, TextWriter stream)
        {
            print(toPrint + "\n", stream);
        }

        public void printlnIter(string toPrint, TextWriter stream)
        {
            printIter(toPrint + "\n", stream);
        }

        public bool isVerbose()
        {
            return verbose;
        }

        public void setVerbose(bool verbose)
        {
            this.verbose = verbose;
        }

        public abstract void printIter(string toPrint, TextWriter stream);

        public abstract void print(string toPrint, TextWriter stream);

    }
}
