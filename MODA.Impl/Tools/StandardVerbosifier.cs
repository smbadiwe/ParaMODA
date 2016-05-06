using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODA.Impl.Tools
{
    public class StandardVerbosifier : AbstractVerbosifier
    {
        protected int count = 0;

        protected int printEvery = 0;

        protected long lastTime = 0;

        protected long minTime = 0;

        protected long maxTime = 0;

        protected bool iterPrinted = false;

        public StandardVerbosifier() : this(1)
        {
            setVerbose(false);
        }

        public StandardVerbosifier(int total, double percent) : this(total, percent, 0L, -1L)
        {

        }

        public StandardVerbosifier(int printEvery) : this(printEvery, 0L, -1L)
        {

        }

        public StandardVerbosifier(long maxTime) : this(0L, maxTime)
        {

        }

        public StandardVerbosifier(long minTime, long maxTime) : this(-1, minTime, maxTime)
        {

        }

        public StandardVerbosifier(int printEvery, long maxTime) : this(printEvery, 0L, maxTime)
        {

        }

        public StandardVerbosifier(int total, double percent, long maxTime) : this(total, percent, 0L, maxTime)
        {

        }

        public StandardVerbosifier(int total, double percent, long minTime,
                long maxTime) : this((int)(total * percent), minTime, maxTime)
        {

        }

        public StandardVerbosifier(int printEvery, long minTime, long maxTime)
        {
            this.printEvery = printEvery;
            this.minTime = minTime;
            this.maxTime = maxTime;

            lastTime = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        public override void printIter(string toPrint, TextWriter @out)
        {
            bool printed = false;

            if (verbose)
            {
                long curTime = (int)DateTime.Now.TimeOfDay.TotalMilliseconds - lastTime;

                if (curTime > minTime)
                {
                    if (count % printEvery == 0)
                    {
                        rawPrint("[count=" + count + "] " + toPrint, @out);
                        printed = true;
                    }
                    else if (curTime > maxTime)
                    {
                        rawPrint("[time=" + curTime + "] " + toPrint, @out);
                        printed = true;
                    }
                }

                iterPrinted = printed;
            }
        }

        //public bool iterPrinted()
        //{
        //    return iterPrinted;
        //}

        public override void print(string toPrint, TextWriter @out)
        {
            if (verbose)
                @out.Write(toPrint);
        }

        public long getMaxTime()
        {
            return maxTime;
        }

        public void setMaxTime(long maxTime)
        {
            this.maxTime = maxTime;
        }

        public long getMinTime()
        {
            return minTime;
        }

        public void setMinTime(long minTime)
        {
            this.minTime = minTime;
        }

        public int getPrintEvery()
        {
            return printEvery;
        }

        public void setPrintEvery(int printEvery)
        {
            this.printEvery = printEvery;
        }

        public int getCount()
        {
            return count;
        }

        public void incrementCount(int inc)
        {
            count += inc;
        }

        public long getLastTime()
        {
            return lastTime;
        }

        public void reset()
        {
            count = 0;
            lastTime = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        protected void rawPrint(string toPrint, TextWriter @out)
        {
            @out.Write(toPrint);
            lastTime = (int)DateTime.Now.TimeOfDay.TotalMilliseconds;
        }
    }
}
