using System;

using GraphicCanvas.Graphic;
using GraphicCanvas.VM;
using System.Threading;
using OpenTK.Graphics;
using System.Collections.Generic;

namespace GraphicCanvas
{
    class REPL
    {
        private Canvas canvas;
        private Runtime runtime;
        private VirtualMachine vm;
        private CanvasWindow window;

        #region Constructors

        public REPL(int width, int height, string title)
        {
            canvas = new Canvas(width, height);
            window = new CanvasWindow(canvas, title);
            runtime = new Runtime(window, canvas, 99);
            vm = new VirtualMachine();

            Run();
        }

        public REPL(Canvas canvas, CanvasWindow window)
        {
            this.canvas = canvas;
            this.window = window;
            runtime = new Runtime(window, canvas, 99);
            vm = new VirtualMachine();

            Run();
        }

        #endregion
        
        private void Evaluate(string source)
        {
            vm.Run(source, runtime);
        }

        private void Run()
        {
#if !DEBUG
            Program.AllocConsole();
            Console.Title = "Graphic Canvas - Interactive";
#endif
            Console.WriteLine("# GRAPHIC CANVAS INTERACTIVE #\nEnter any command in this command line and press enter.\n");

            Thread consoleThread = new Thread(new ThreadStart(ConsoleWorker));
            
            consoleThread.Start();
            window.Run(1.0 / 60.0);

            consoleThread.Abort();
            Program.FreeConsole();

            window.Dispose();
            canvas.Dispose();
            runtime.Dispose();
        }

        private void ConsoleWorker()
        {
            string code = String.Empty;
            int index = 0;
            List<string> history = new List<string>();

            Console.Write("> ");

            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey();

                if (key.Key == ConsoleKey.Enter)
                {
                    Console.Write('\n');
                    Evaluate(code);

                    history.Add(code);
                    code = String.Empty;
                    index = history.Count;
                    Console.Write("> ");
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (index - 1 >= 0)
                    {
                        Console.CursorLeft = 2;
                        Console.WriteLine("                                        ");
                        Console.CursorTop -= 1;
                        Console.CursorLeft = 2;

                        code = history[--index];
                        Console.Write(code);
                    }
                    else
                    {
                        Console.CursorLeft -= 1;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (history.Count > index + 1)
                    {
                        Console.CursorLeft = 2;
                        Console.WriteLine("                                        ");
                        Console.CursorTop -= 1;
                        Console.CursorLeft = 2;

                        code = history[++index];
                        Console.Write(code);
                    }
                    else
                    {
                        Console.CursorLeft -= 1;
                    }
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (code.Length > 0 && Console.CursorLeft >= 2)
                    {
                        code = code.Substring(0, code.Length - 1);
                    }
                    if (Console.CursorLeft >= 2)
                    {
                        Console.Write(' ');
                        Console.CursorLeft -= 1;
                    }
                    else
                    {
                        Console.CursorLeft += 1;
                    }
                }
                else if (key.Key == ConsoleKey.Backspace && key.Modifiers == ConsoleModifiers.Control)
                {
                    code = String.Empty;
                    Console.CursorLeft = 0;
                    Console.WriteLine("                                        ");
                    Console.CursorTop -= 1;
                }
                else
                {
                    code += key.KeyChar;
                }
            }
        }
    }
}
