using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace Hoard.MVC
{
    /// <summary>
    /// It's simple manager, which whole purpose is to
    /// bring back the task results to the game play thread.
    /// </summary>
    public class TaskRunner
    {
        private static TaskRunner instance;


        public static TaskRunner Instance
        {
            get
            {
                if (instance == null) instance = new TaskRunner();
                return instance;
            }
        }
        private static Queue<Action> executeDuringNextPool = new Queue<Action>();

        /// <summary>
        ///   Executes the Action during the next GUI execution
        /// </summary>
        public static void ExecuteDuringNextPool(Action action)
        {
            if (action != null) executeDuringNextPool.Enqueue(action);
        }

        /// <summary>
        ///   Observe the task and execute the callback on the GUI update with provided callback
        /// </summary>
        public static void RunWithCallback<T>(Task<T> task, Action<Task<T>> callback, bool autoactivate = true)
        {
            Instance.tasks.Add(new ComplitionGuard<T>(callback, task));
            if (task.Status == (TaskStatus.Created) && autoactivate)
            {
                task = Task.Run<T>(async () => await task);
            }
        }

        /// <summary>
        ///   Observe the task and execute the callback on the GUI update with provided callback
        /// </summary>
        public static void RunWithCallback(Task task, Action<Task> callback, bool autoactivate = true)
        {
            Instance.tasks.Add(new CompletionGuard(callback, task));
            if (task.Status == (TaskStatus.Created) && autoactivate)
            {
                task = Task.Run(async () => await task);
            }
        }

        private List<IExecuteWhenFinished> tasks = new List<IExecuteWhenFinished>();

        /// <summary>
        ///   Current Task Count
        /// </summary>
        public static int TaskCount => Instance.tasks.Count;

        /// <summary>
        ///   Pool and check all the Task handles
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This method has to be called from outside during the GUI update.
        ///   </para>
        /// </remarks>
        public static void Pool()
        {
            while (executeDuringNextPool.Count > 0)
            {
                executeDuringNextPool.Dequeue()?.Invoke();
            }
            var completed = Instance.tasks.FindAll(x => x.IsCompleted);
            completed.ForEach(
                x =>
                {
                    if (x.IsCanceled) return;
                    x.Execute();
                    Instance.tasks.Remove(x);
                });
        }
    }

    /// <summary>
    ///   Extensions that allows easy task controll during GUI/Update. 
    /// </summary>
    public static class TaskExtension
    {
        public static void ContinueGUISynch<TResult>(this Task<TResult> task, Action<Task<TResult>> continuation)
        {
            TaskRunner.RunWithCallback(task, continuation);
        }

        public static void ContinueGUISynch(this Task task, Action<Task> continuation)
        {
            TaskRunner.RunWithCallback(task, continuation);
        }
    }

    internal interface IExecuteWhenFinished
    {
        void Execute();

        bool IsCompleted { get; }
        bool IsFaultedOrCanceled {get;}
        bool IsCanceled {get;}
    }

    internal class CompletionGuard : IExecuteWhenFinished
    {
        public bool IsCompleted => task.IsCompleted;
        public bool IsCanceled => task.IsCanceled;
        public bool IsFaultedOrCanceled => task.IsCanceled || task.IsFaulted;

        private readonly Action<Task> callback;
        private readonly Task task;

        public CompletionGuard(Action<Task> callback, Task task)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.task = task ?? throw new ArgumentNullException(nameof(task));
        }

        public void Execute() =>
            callback.Invoke(task);
    }

    internal class ComplitionGuard<TResult> : IExecuteWhenFinished
    {
        public bool IsCompleted => task.IsCompleted;
        public bool IsFaultedOrCanceled => task.IsCanceled || task.IsFaulted;
        public bool IsCanceled => task.IsCanceled;

        private readonly Action<Task<TResult>> callback;
        private readonly Task<TResult> task;

        public ComplitionGuard(Action<Task<TResult>> callback, Task<TResult> task)
        {
            this.callback = callback ?? throw new ArgumentNullException(nameof(callback));
            this.task = task ?? throw new ArgumentNullException(nameof(task));
        }

        public void Execute() =>
            callback.Invoke(task);
    }
}
