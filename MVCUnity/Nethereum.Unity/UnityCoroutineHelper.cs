using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Hoard.MVC.Unity
{
    public class UnityCoroutineHandler : MonoBehaviour
    {
        static private UnityCoroutineHandler m_Instance;
        private readonly ConcurrentQueue<Action> ActionQueue = new ConcurrentQueue<Action>();
        static public UnityCoroutineHandler Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    GameObject o = new GameObject("UnityCoroutineHandler");
                    DontDestroyOnLoad(o);
                    m_Instance = o.AddComponent<UnityCoroutineHandler>();
                }

                return m_Instance;
            }
        }

        public static void executeCoroutine(IEnumerator enumerator)
        {
            if (enumerator == null)
                return;
            while (enumerator.MoveNext())
            {
                var nested = enumerator.Current as IEnumerable;
                if (nested != null)
                    executeCoroutine(nested.GetEnumerator());

                var asyncOp = enumerator.Current as AsyncOperation;
                if (asyncOp != null)
                {
                    while (!asyncOp.isDone) { }
                }
            }
        }

        public static async Task<TResult> ExecuteCoroutineOnMainThread<TResult>(IEnumerator<TResult> coroutine)
        {
            var tcs = new TaskCompletionSource<TResult>();

            Instance.ActionQueue.Enqueue(() =>
            {
            Instance.StartCoroutine(
                RunThrowingIterator(
                    //tempCoroutine(
                    coroutine,
                    (ret,ex) => {
                        if (ex != null)
                            tcs.SetException(ex);
                        else
                            tcs.SetResult(ret);
                        }
                    ));
            });

            return await tcs.Task;
        }

        /// <summary>
        /// Run an iterator function that might throw an exception. Call the callback with the exception
        /// if it does or null if it finishes without throwing an exception.
        /// </summary>
        /// <param name="enumerator">Iterator function to run</param>
        /// <param name="done">Callback to call when the iterator has thrown an exception or finished.
        /// The thrown exception or null is passed as the parameter.</param>
        /// <returns>An enumerator that runs the given enumerator</returns>
        public static IEnumerator RunThrowingIterator<TResult>(
            IEnumerator<TResult> enumerator,
            Action<TResult, Exception> done
        )
        {
            TResult current = default(TResult);
            while (true)
            {
                try
                {
                    if (enumerator.MoveNext() == false)
                    {
                        break;
                    }
                    current = enumerator.Current;
                }
                catch (Exception ex)
                {
                    done(default(TResult), ex);
                    yield break;
                }
                yield return current;
            }
            done(current,null);
        }

        private static IEnumerator tempCoroutine<TResult>(IEnumerator<TResult> cor, Action<TResult> afterExecutionCallback)
        {
            yield return cor;
            afterExecutionCallback(cor.Current);
        }

        public virtual void Update()
        {
            // dispatch actions
            while (ActionQueue.Count>0)
            {
                Action action;
                if (ActionQueue.TryDequeue(out action))
                    action.Invoke(); 
            }
        }

        public void OnDisable()
        {
            if (m_Instance)
                Destroy(m_Instance.gameObject);
        }
    }
}
