using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDOAQCSharpTool
{
    public class LoggerEventArgs : EventArgs
    {
        public string Data;

        public LoggerEventArgs(string data)
        {
            Data = data;
        }
    }

    public class Logger : IDisposable
    {
        public event EventHandler<LoggerEventArgs> DataReceived;

        private static MyQueue<string> _queue;


        public Logger()
        {
            _queue = new MyQueue<string>();

            _queue.CallBackMsgLoop += CallBackMsgLoop;
        }

        public void WriteLog(string log)
        {
            _queue.Enq_Msg($"[{DateTime.Now.ToString("HH:mm:ss.fff")}]{log}{Environment.NewLine}");
        }
        

        private void CallBackMsgLoop(string log)
        {
            DataReceived?.Invoke(this, new LoggerEventArgs(log));
        }

        public void Dispose()
        {
            _queue.Stop();
        }
    }
}
