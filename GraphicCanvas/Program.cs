using System;
using System.Runtime.InteropServices;

namespace GraphicCanvas
{
    class Program
    {
        private static readonly string version = "beta 2.1";

        static void Main(string[] args)
        {
#if DEBUG
            Program.AllocConsole();
            Console.Title = "Graphic Canvas - Debug";
#endif
            if (args.Length > 0)
            {
                new PlainViewer(args[0], "Graphic Canvas [" + version + "]");
            }
            else
            {
                new REPL(512, 512, "Graphic Canvas [" + version + "]");
            }
        }

        #region Imports

        [DllImport("kernel32.dll")]
        public static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern bool FreeConsole();

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        #endregion
    }
}
