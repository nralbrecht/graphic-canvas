using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphicCanvas.Graphic;
using GraphicCanvas.VM;
using System.IO;

namespace GraphicCanvas
{
    class PlainViewer
    {
        private string source;
        private Canvas canvas;
        private Runtime runtime;
        private VirtualMachine vm;
        private CanvasWindow window;

        public PlainViewer(string sourceFile, string title)
        {
            canvas = new Canvas(512, 512);
            window = new CanvasWindow(canvas, title);
            runtime = new Runtime(window, canvas, 99);
            vm = new VirtualMachine();

            if (File.Exists(sourceFile))
            {
                source = File.ReadAllText(sourceFile);
            }
            else
            {
                Program.MessageBox(IntPtr.Zero, "The given source code file(" + sourceFile + " could not be found.", "Error!", 0);
                return;
            }

            window.Load += OnLoad;
            window.Run(1 / 60.0);

            window.Dispose();
            canvas.Dispose();
            runtime.Dispose();
        }

        public PlainViewer(Canvas canvas, CanvasWindow window)
        {
            this.canvas = canvas;
            this.window = window;
            runtime = new Runtime(window, canvas, 99);
            vm = new VirtualMachine();

            window.Load += OnLoad;
        }
        
        private void OnLoad(object sender, EventArgs e)
        {
#if DEBUG
            DateTime start = DateTime.Now;
            Console.WriteLine("Compiling...");
#endif
            vm.Run(source, runtime);
#if DEBUG
            Console.WriteLine("Compiled in " + (DateTime.Now - start).TotalMilliseconds + " milliseconds");
#endif
        }
    }
}
