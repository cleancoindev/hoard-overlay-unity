using System.Collections.Generic;
using System.Linq;

namespace Hoard.MVC
{
    public static class StackExtensions
    {
        /// <summary>
        /// Takes a look at the top of the stack. Does not throw if stack is empty
        /// </summary>
        public static T SafePeek<T>(this Stack<T> stack)
            => stack.Count == 0 ? default : stack.Peek();

        /// <summary>
        /// Pops top of the stack. Does not throw if stack is empty
        /// </summary>
        public static T SafePop<T>(this Stack<T> stack)
            => stack.Count == 0 ? default : stack.Pop();

        /// <summary>
        /// Checks if element is in the stack. If it's true it pops stack until it
        /// removes the required variable.
        /// If element is not there stack remains the same
        /// If element is found stack top is the last element after the required one
        /// </summary>
        public static T PopUntil<T>(this Stack<T> stack, System.Predicate<T> predicate)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                if (predicate(stack.ElementAt(i)))
                {
                    for (int j = 0; j < i; j++)
                    {
                        stack.Pop();
                    }
                    return stack.Pop();
                }
            }
            return default;
        }

        /// <summary>
        /// Checks if element is in the stack. If it's true it pops stack until it
        /// removes the required variable.
        /// If element is not there stack remains the same
        /// If element is found stack top is the last element after the required one
        /// Similar to take while but changes the state of the input stack
        /// </summary>
        public static T PopUntil<T>(this Stack<T> stack, T toFind)
        {
            return stack.PopUntil(x => x.Equals(toFind));
        }
    }
}
