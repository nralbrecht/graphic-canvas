using System;
using System.Collections.Generic;
using System.IO;

namespace GraphicCanvas.VM
{
    class VirtualMachine
    {
        private Interpreter interpreter;
        private static bool hadError = false;
        
        public VirtualMachine()
        {
            interpreter = new Interpreter();
        }

        #region Controlls

        public void Run(string code, Runtime runtime)
        {
            Lexer lexer = new Lexer(code);
            List<Token> tokens = lexer.ScanTokens();

#if DEBUG
            StreamWriter sw = File.CreateText("tokens.log");
            foreach (var item in tokens)
            {
                sw.WriteLine(item);
            }
            sw.Close();
#endif
            
            Parser parser = new Parser(tokens);
            byte[] bytecode = parser.ParseTokens();

#if DEBUG
            FileStream fs = File.Create("bytecode.bin");
            fs.Write(bytecode, 0, bytecode.Length);
            fs.Close();
#endif
            
            interpreter.Interpret(bytecode, runtime);
        }

        public byte[] Compile(string code)
        {
            Lexer lexer = new Lexer(code);
            List<Token> tokens = lexer.ScanTokens();
            
            Parser parser = new Parser(tokens);
            return parser.ParseTokens();
        }

        public void Reset()
        {
            interpreter.Reset();
        }

        #endregion

        #region Error Handling

        public static void Error(int line, string message)
        {
            ReportError(line, "", message);
        }

        public static void RuntimeError(int position, Instruction instruction, string message)
        {
            ReportRuntimeError(position, instruction, message);
        }

        private static void ReportError(int line, string where, string message)
        {
            Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }

        private static void ReportRuntimeError(int position, Instruction instruction, string message)
        {
            Console.WriteLine("[byte " + position + "] " + instruction + " Runtime Error: " + message);
            hadError = true;
        }
        
        #endregion
    }
}
