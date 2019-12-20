using System.Collections.Generic;
using System.Linq;

namespace Hoard.MVC
{
    /// <summary>
    ///   Organises the way the user can open close the views
    /// </summary>
    public static class Navigation
    {
        /// <summary>
        /// Controler provider can be used to perform side operations
        /// (decorate bind etc) before returning useful HoardViewModel
        /// </summary>
        public static IHoardViewModelProvide controlerProvider;

        private static IHoardViewController GetCompleteControler(IHoardViewController c)
            => controlerProvider != null ? controlerProvider.ProvideFor(c) : c;

        private static Stack<IHoardViewController> stackedControlers = new Stack<IHoardViewController>();

        /// <summary>
        /// In modal mode only the view that was opened in this mode can go back or open next controller
        /// </summary>
        public static bool ModalView { get => modalControler != null; }

        private static IHoardViewController modalControler;

        private static bool CanPassModalLock(IHoardViewController controler)
            => ModalView ? controler == modalControler : true;

        /// <summary>
        ///   Currently opened controller at the top of the controller stack
        /// </summary>
        public static IHoardViewController CurrentController => stackedControlers.SafePeek();

        /// <summary>
        ///   Open controller in the modal mode
        /// </summary>
        public static void OpenControlerModal(IHoardViewController controler)
        {
            controler = GetCompleteControler(controler);
            Open(controler);
            modalControler = controler;
        }

        /// <summary>
        ///   Open and show new controller to the user
        /// </summary>
        public static void Open(IHoardViewController controler)
        {
            controler = GetCompleteControler(controler);
            controler.Open();
            controler.Enable();
            var top = stackedControlers.SafePeek();
            if (top != null)
            {
                top.Disable();
            }
            stackedControlers.Push(controler);
        }

        /// <summary>
        ///   Opens new controller and clears the current controller stack. New controller will be the base
        /// for the new navigation stack
        /// </summary>
        public static void OpenControlerWithNewRoot(IHoardViewController rootControler)
        {
            if (stackedControlers == null) stackedControlers = new Stack<IHoardViewController>();
            else
            {
                foreach (var v in stackedControlers) v.CloseAndDisable();
                stackedControlers.Clear();
            }
            if (rootControler != null)
            {
                Open(rootControler);
            }
        }

        /// <summary>
        ///   Closes top view but not go back to previous view.
        /// </summary>
        public static void CloseTopView()
        {
            if (stackedControlers.Count == 0)
            {
                ErrorCallbackProvider.ReportWarning(string.Format("Attempt to call {0} when controllers count is 0. Check code logic"));
                return;
            }
            var view = stackedControlers.Pop();
            view.Disable();
            view.Close();
        }

        /// <summary>
        /// Folds the view stack until it finds the type that it looks for
        /// If the view is not available on the stack it throws
        /// </summary>
        /// <typeparam name="T">Type of view for navigation to go back to</typeparam>
        /// <returns></returns>
        public static IHoardViewController GoBackToView<T>() where T : IHoardViewController
        {
            if (!stackedControlers.Any(x => x is T))
            {
                throw new System.ArgumentException("Type " + typeof(T) + " was not found on the navigation stack");
            }

            while (stackedControlers.Count > 0)
            {
                var backView = GoBack();
                if (backView is T) return backView;
            }
            return null;
        }

        /// <summary>
        /// Moves back on the view stack and closes the current view
        /// </summary>
        /// <returns></returns>
        public static IHoardViewController GoBack()
        {
            var current = stackedControlers.SafePop();
            if (current == null) return null;
            current.CloseAndDisable();

            var next = stackedControlers.SafePeek();
            next?.Enable();
            return next;
        }

        /// <summary>
        /// Helper extension for concise syntax
        /// </summary>
        private static void CloseAndDisable(this IHoardViewController controler)
        {
            if (controler == null) return;
            controler.Disable();
            controler.Close();
        }

        /// <summary>
        ///   Go back more and close more than one controler
        /// </summary>
        public static IHoardViewController GoBackTimes(int times)
        {
            for (int i = 0; i < times - 1; i++)
            {
                GoBack();
            }
            return GoBack();
        }

        /// <summary>
        ///   Clear the controllers from the navigation stack
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     This call does NOT close opened controller. Don't use without good reason.
        ///   </para>
        /// </remarks>
        public static void Clear()
        {
            stackedControlers.Clear();
        }
    }

    /// <summary>
    /// If controler needs some context or be decorated
    /// user can create the provider that will do all the side effects necessary
    /// for use. See CanvasViewControlerProvider for example
    /// </summary>
    public interface IHoardViewModelProvide
    {
        IHoardViewController ProvideFor(IHoardViewController controler);
    }
}
