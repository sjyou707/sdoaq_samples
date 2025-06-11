using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using SDOAQ;
using SDOAQCSharp.Tool;

namespace SDOAQCSharp
{
    partial class MySdoaq
    {
        private MyManualResetEvent<bool> _evtContinuosAcq_FocusStack = new MyManualResetEvent<bool>(false);
        private MyManualResetEvent<bool> _evtContinuosAcq_Af = new MyManualResetEvent<bool>(false);
        private MyManualResetEvent<bool> _evtContinuosAcq_Edof = new MyManualResetEvent<bool>(false);

        private System.Threading.Thread _thrContinuosAcq_FocusStack;
        private System.Threading.Thread _thrContinuosAcq_Af;
        private System.Threading.Thread _thrContinuosAcq_Edof;

        private void CreateContinuosAcqThread()
        {
            _thrContinuosAcq_FocusStack = new System.Threading.Thread(ContinuosAcq_FocusStack);
            _thrContinuosAcq_Af = new System.Threading.Thread(ContinuosAcq_Af);
            _thrContinuosAcq_Edof = new System.Threading.Thread(ContinuosAcq_Edof);

            _thrContinuosAcq_FocusStack.Start();
            _thrContinuosAcq_Af.Start();
            _thrContinuosAcq_Edof.Start();
        }

        private void DisposeContinuosAcqThread()
        {
            int maxWaitTime = 1000;

            _evtContinuosAcq_FocusStack.Abort(false);
            _evtContinuosAcq_Af.Abort(false);
            _evtContinuosAcq_Edof.Abort(false);

            if (_thrContinuosAcq_FocusStack.IsAlive && _thrContinuosAcq_FocusStack.Join(maxWaitTime) == false)
            {
                _thrContinuosAcq_FocusStack.Abort();
            }

            if (_thrContinuosAcq_Af.IsAlive && _thrContinuosAcq_Af.Join(maxWaitTime) == false)
            {
                _thrContinuosAcq_Af.Abort();
            }

            if (_thrContinuosAcq_Edof.IsAlive && _thrContinuosAcq_Edof.Join(maxWaitTime) == false)
            {
                _thrContinuosAcq_Edof.Abort();
            }

            _evtContinuosAcq_FocusStack.Dispose();
            _evtContinuosAcq_Af.Dispose();
            _evtContinuosAcq_Edof.Dispose();
        }

        #region Focus Stack

        private void ContinuosAcq_FocusStack()
        {
            while(true)
            {
                _evtContinuosAcq_FocusStack.WaitOne();

                if (_evtContinuosAcq_FocusStack.IsAbort)
                {
                    return;
                }
                
                var focusList = FocusList.GetStepList();

                while (_evtContinuosAcq_FocusStack.IsWaitSet == false)
                {
                    Acq_FocusStack(CamInfo.AcqParam, focusList, false);
                }

                if (_evtContinuosAcq_FocusStack.IsAbort)
                {
                    return;
                }
            }
        }
        #endregion

        #region Af
        private void ContinuosAcq_Af()
        {
            while (true)
            {
                _evtContinuosAcq_Af.WaitOne();

                if (_evtContinuosAcq_Af.IsAbort)
                {
                    return;
                }
                
                var focusList = FocusList.GetStepList();

                while (_evtContinuosAcq_Af.IsWaitSet == false)
                {
                    Acq_Af(CamInfo.AcqParam, focusList, false);
                }

                if (_evtContinuosAcq_Af.IsAbort)
                {
                    return;
                }
            }
        }
        #endregion

        #region Edof
        private void ContinuosAcq_Edof()
        {
            while (true)
            {
                _evtContinuosAcq_Edof.WaitOne();

                if (_evtContinuosAcq_Edof.IsAbort)
                {
                    return;
                }
                
                var focusList = FocusList.GetStepList();

                while (_evtContinuosAcq_Edof.IsWaitSet == false)
                {
                    Acq_Edof(CamInfo.AcqParam, focusList, _edofImageList, false);
                }

                if (_evtContinuosAcq_Edof.IsAbort)
                {
                    return;
                }
            }
        }
        #endregion
        
    }
}
