using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace GraphicCanvas.VM
{
    class Interpreter
    {
        private StackMachine<Value> stack;
        private Dictionary<int, Value> variables;

        public Interpreter()
        {
            Reset();
        }

        public void Reset()
        {
            stack = new StackMachine<Value>(256);
            variables = new Dictionary<int, Value>();
        }

        public void Interpret(byte[] bytecode, Runtime runtime)
        {
            // TODO: Error Handling

            for (int i = 0; i < bytecode.Count(); i++)
            {
                try
                {
                    #region Interpret Tokens

                    byte instruction = bytecode[i];

                    switch ((Instruction)instruction)
                    {
                        case Instruction.READ:
                            stack.Push(variables[stack.Pop().GetAs<int>()]);
                            break;
                        case Instruction.WRITE:
                            Value val = stack.Pop();
                            int key = stack.Pop().GetAs<int>();

                            variables[key] = val;
                            break;
                        case Instruction.DUP:
                            stack.Push(stack.Peek());
                            break;
                        case Instruction.INTEGER_LITERAL:
                            stack.Push(new Value(MyValueType.INTEGER, bytecode, i + 1));
                            i += 4;
                            break;
                        case Instruction.COLOR_LITERAL:
                            stack.Push(new Value(MyValueType.COLOR, new ArraySegment<byte>(bytecode, i + 1, 4).Reverse().ToArray()));
                            i += 4;
                            break;
                        case Instruction.STRING_LITERAL:
                            stack.Push(new Value(MyValueType.STRING, new ArraySegment<byte>(bytecode, i + 2, bytecode[i + 1]).ToArray()));
                            i += 1 + bytecode[i + 1];
                            break;
                        case Instruction.FILL:
                            runtime.Fill(stack.Pop().GetAs<Color>());
                            break;
                        case Instruction.SET_PIXEL:
                            Color c1 = stack.Pop().GetAs<Color>();
                            int y2 = stack.Pop().GetAs<int>();
                            int x2 = stack.Pop().GetAs<int>();

                            runtime.DrawPixel(x2, y2, c1);
                            break;
                        case Instruction.SET_RECT:
                            Color c2 = stack.Pop().GetAs<Color>();
                            int h = stack.Pop().GetAs<int>();
                            int w = stack.Pop().GetAs<int>();
                            int y3 = stack.Pop().GetAs<int>();
                            int x3 = stack.Pop().GetAs<int>();

                            runtime.DrawRect(x3, y3, w, h, c2);
                            break;
                        case Instruction.WRITE_TEXT:
                            Color c3 = stack.Pop().GetAs<Color>();
                            string text = stack.Pop().GetAs<string>();
                            int size = stack.Pop().GetAs<int>();
                            int y4 = stack.Pop().GetAs<int>();
                            int x4 = stack.Pop().GetAs<int>();

                            runtime.Write(x4, y4, size, text, c3);
                            break;
                        case Instruction.RGB:
                        case Instruction.CMYK:
                        case Instruction.HSV:
                        case Instruction.LAB:
                            throw new NotImplementedException();
                        case Instruction.GET_PIXEL:
                            int y = stack.Pop().GetAs<int>();
                            int x = stack.Pop().GetAs<int>();

                            stack.Push(new Value(runtime.GetPixel(x, y)));
                            break;
                        case Instruction.GET_NOISE:
                            int y1 = stack.Pop().GetAs<int>();
                            int x1 = stack.Pop().GetAs<int>();

                            stack.Push(new Value(runtime.Noise(x1, y1)));
                            break;
                        case Instruction.GET_WIDTH:
                            stack.Push(new Value(runtime.CanvasWidth));
                            break;
                        case Instruction.GET_HEIGHT:
                            stack.Push(new Value(runtime.CanvasHeight));
                            break;
                        case Instruction.GET_RAND:
                            int max = stack.Pop().GetAs<int>();
                            int min = stack.Pop().GetAs<int>();

                            stack.Push(new Value(runtime.Random(min, max)));
                            break;
                        default:
                            throw new Exception("Unexspected Instruction.");
                    }
                    
                    #endregion
                }
                catch (Exception e)
                {
                    VirtualMachine.RuntimeError(i, (Instruction)bytecode[i], e.Message);
                    System.IO.File.WriteAllBytes("bytecode.log", bytecode);
                    break;
                }
            }

            runtime.Reload();
        }
    }
}
