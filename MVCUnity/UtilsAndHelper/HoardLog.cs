using UnityEngine;
using System.Collections.Generic;
using System;

namespace Hoard.MVC.Unity
{
    /// <summary>
    ///   Pools the error stream from the hoard sdk and pushes it to the unity log
    /// in the safe way. Avoids 'call from the other thread' errors
    /// </summary>
    public class HoardLog : MonoBehaviour
    {
        Queue<Action> messageCallbacks = new Queue<Action>();

        public void Awake()
        {
            ErrorCallbackProvider.OnReportError += ReportError;
            ErrorCallbackProvider.OnReportWarning += ReportWarning;
            ErrorCallbackProvider.OnReportInfo += ReportMessage;
        }

        private void ReportWarning(string msg)
        {
            messageCallbacks.Enqueue(() => Debug.LogWarning(msg));
        }
        private void ReportMessage(string msg)
        {
            messageCallbacks.Enqueue(() => Debug.Log(msg));
        }
        private void ReportError(string msg)
        {
            messageCallbacks.Enqueue(() => Debug.LogError(msg));
        }

        private void Update()
        {
            // Pools messages during update
            while (messageCallbacks.Count > 0)
                messageCallbacks.Dequeue()?.Invoke();
        }
    }

}
