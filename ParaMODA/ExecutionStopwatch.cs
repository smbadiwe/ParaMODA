using System;
using System.Runtime.InteropServices;

namespace ParaMODA
{
    public class ExecutionStopwatch
    {
        [DllImport("kernel32.dll")]
        private static extern long GetThreadTimes
            (IntPtr threadHandle, out long createionTime,
             out long exitTime, out long kernelTime, out long userTime);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetCurrentThread();

        private long _endTimeStamp;
        private long _startTimeStamp;

        private bool _isRunning;

        public void Start()
        {
            _isRunning = true;
            _startTimeStamp = GetThreadTimes();
        }

        public void Stop()
        {
            _isRunning = false;
            _endTimeStamp = GetThreadTimes();
        }

        public void Reset()
        {
            _startTimeStamp = 0;
            _endTimeStamp = 0;
        }

        public TimeSpan Elapsed
        {
            get
            {
                return TimeSpan.FromMilliseconds(ElapsedMilliseconds);
            }
        }

        public long ElapsedMilliseconds
        {
            get
            {
                return (_endTimeStamp - _startTimeStamp) / 10000;
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        private long GetThreadTimes()
        {
            IntPtr _threadHandle = GetCurrentThread();

            long notIntersting;
            long kernelTime, userTime;

            long retcode = GetThreadTimes(_threadHandle, out notIntersting, out notIntersting, out kernelTime, out userTime);
            bool success = Convert.ToBoolean(retcode); // => retcode != 0
            if (!success) throw new Exception(string.Format("failed to get timestamp. error code: {0}", retcode));
            
            return kernelTime + userTime;
        }
    }
}
