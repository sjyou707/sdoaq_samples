using System;
using System.Collections.Concurrent;
using System.Threading;

namespace SDOAQCSharp.Tool
{
    public class MyQueue<T> : IDisposable
    {
        public delegate void MsgLoopCallBack(T item);
        public MsgLoopCallBack CallBackMsgLoop;

        private ConcurrentQueue<QueueMsg> _queue = new ConcurrentQueue<QueueMsg>();
        private Thread _thrQueue;
        private AutoResetEvent _evt = new AutoResetEvent(false);
        private bool _bThreadStop = false;

        private class QueueMsg : IDisposable
        {
            public T Item;
            public ManualResetEventSlim WaitMsgLoop;
            public QueueMsg(T item, bool bWaitMsgLoop = false)
            {
                Item = item;
                if (bWaitMsgLoop)
                {
                    WaitMsgLoop = new ManualResetEventSlim(false);
                }
            }

            public void Dispose()
            {
                Item = default(T);
                WaitMsgLoop?.Dispose();
            }
        }

        private const int DFLT_WAIT_EVENT_TIME = 1000;

        public MyQueue(bool bQueueStart = true)
        {
            if (bQueueStart)
            {
                _thrQueue = new Thread(ThreadQueueMain);
                _thrQueue.IsBackground = true;
                _thrQueue.Start();
            }
        }

        public void Start()
        {
            if (_thrQueue == null)
            {
                _thrQueue = new Thread(ThreadQueueMain);
                _thrQueue.IsBackground = true;
            }

            if (_thrQueue.IsAlive == false)
            {
                _thrQueue.Start();
            }
        }

        public void Stop()
        {
            if (_thrQueue != null && _thrQueue.IsAlive)
            {
                _bThreadStop = true;
                _evt.Set();

                _thrQueue.Join();
                _thrQueue = null;
            }
        }

        public void Enq_Msg(T item)
        {
            Push_Msg(new QueueMsg(item));
        }

        public void Run_Msg(T item)
        {
            var msg = new QueueMsg(item, true);
            Push_Msg(msg);

            msg.WaitMsgLoop.Wait();
        }

        private void Push_Msg(QueueMsg msg)
        {
            _queue.Enqueue(msg);
            _evt.Set();
        }

        protected virtual void Msg_Loop(object obj)
        {

        }

        private void ThreadQueueMain()
        {
            QueueMsg msg = null;
            bool bAlreadyDequeue = false;
            while (true)
            {
                if (_evt.WaitOne(DFLT_WAIT_EVENT_TIME) == false)
                {
                    if (_queue.TryDequeue(out msg))
                    {
                        bAlreadyDequeue = true;
                    }
                    else if (_bThreadStop)
                    {
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }

                while (true)
                {
                    if (bAlreadyDequeue)
                    {
                        bAlreadyDequeue = false;
                    }
                    else if (_queue.TryDequeue(out msg) == false)
                    {
                        break;
                    }

                    if (_bThreadStop)
                    {
                        msg.WaitMsgLoop?.Set();
                    }
                    else if (CallBackMsgLoop != null)
                    {
                        CallBackMsgLoop(msg.Item);
                    }
                    else
                    {
                        Msg_Loop(msg.Item);
                    }

                    msg.WaitMsgLoop?.Set();
                    msg.Dispose();
                    msg = null;
                }

                if (_bThreadStop)
                {
                    return;
                }
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
