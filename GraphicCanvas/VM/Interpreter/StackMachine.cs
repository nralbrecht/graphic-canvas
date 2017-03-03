using System;

namespace GraphicCanvas.VM
{
    class StackMachine<T>
    {
        private T[] stack;
        private int maxStack;
        private int stackSize;

        public bool IsEmpty { get { return stackSize < 1; } }

        public StackMachine(int maxStack)
        {
            stackSize = 0;
            stack = new T[maxStack];
            this.maxStack = maxStack;
        }

        public void Push(T value)
        {
            if (stackSize < maxStack)
            {
                stack[stackSize++] = value;
            }
            else
            {
                throw new StackOverflowException();
            }
        }

        public T Pop()
        {
            if (IsEmpty)
            {
                throw new Exception("Empty Stack.");
            }
            else
            {
                return stack[--stackSize];
            }
        }

        public T Peek()
        {
            if (IsEmpty)
            {
                throw new Exception("Empty Stack.");
            }
            else
            {
                return stack[stackSize - 1];
            }
        }
    }
}
