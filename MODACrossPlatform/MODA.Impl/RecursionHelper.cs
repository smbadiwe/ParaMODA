using System;
using System.Collections.Generic;

namespace MODA.Impl
{
    public class RecursionHelper<T>
    {
        private readonly List<Tuple<Func<T, T, T>, bool>> _steps = new List<Tuple<Func<T, T, T>, bool>>();
        private readonly Stack<Tuple<T, int>> _stack = new Stack<Tuple<T, int>>();
        private Func<T, bool> _terminationCondition;
        private Func<T, T> _terminationOperation;

        /// <summary>
        /// Creates a single stack recursion manager.
        /// </summary>
        /// <typeparam name="TR">Type of item to recurse for</typeparam>
        /// <param name="terminateCondition">The terminate condition.</param>
        /// <param name="terminationOperation">Operation to run in case termination was true.</param>
        /// <returns></returns>
        public static RecursionHelper<T> CreateSingular(Func<T, bool> terminateCondition, Func<T, T> terminationOperation = null)
        {
            var rv = new RecursionHelper<T>
            {
                _terminationCondition = terminateCondition,
                _terminationOperation = terminationOperation
            };
            return rv;
        }

        public RecursionHelper<T> RecursiveCall(Func<T, T, T> func)
        {
            addStep(func, true);
            return this;
        }

        public RecursionHelper<T> Do(Func<T, T, T> func)
        {
            addStep(func, false);
            return this;
        }

        public RecursionHelper<T> Do(Action<T, T> action)
        {
            addStep((i, o) =>
            {
                action(i, o);
                return o;
            }, false);
            return this;
        }

        private void addStep(Func<T, T, T> func, bool isRecursive)
        {
            _steps.Add(Tuple.Create(func, isRecursive));
        }

        public T Execute(T initialItem)
        {
            var currentItem = initialItem;
            var currentResult = default(T);
            var currentStep = 0;
            while (true)
            {
                var recursiveContinue = false;
                if (currentStep == 0 && _terminationCondition(currentItem))
                {
                    currentResult = _terminationOperation(currentItem);
                }
                else
                {
                    for (int index = currentStep; index < _steps.Count; index++)
                    {
                        var step = _steps[index];
                        if (step.Item2) //Step is recursive
                        {
                            _stack.Push(Tuple.Create(currentItem, index + 1)); //Push the current position and value
                            currentItem = step.Item1(currentItem, currentResult);
                            recursiveContinue = true;
                            break;
                        }
                        currentResult = step.Item1(currentItem, currentResult);
                        recursiveContinue = false;
                    }
                }
                currentStep = 0;
                if (!recursiveContinue)
                {
                    //Once a function has finished it works, pop the stack and continue from where it was before
                    if (_stack.Count == 0)
                    {
                        return currentResult;
                    }
                    var stackPopped = _stack.Pop();
                    currentItem = stackPopped.Item1;
                    currentStep = stackPopped.Item2;
                }
            }
        }
    }
}
