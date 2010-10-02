using System;
using System.IO;
using Alaris.API;
using Alaris.Calculator.analysis;
using Alaris.Calculator.node;

namespace Alaris.Calculator
{
    public sealed class AstPrinter : DepthFirstAdapter, IDisposable, IAlarisComponent
    {
        private int _indent;
        private readonly StreamWriter _writer;
        private readonly Guid _guid;

        public AstPrinter()
        {
            _guid = Guid.NewGuid();
            _writer = new StreamWriter("ast.log", true);
        }

        private void PrintIndent()
        {
            _writer.Write("".PadLeft(_indent, '\t'));
        }

        private void PrintNode(Node node)
        {
            if (node.GetType().ToString().Contains("Alaris.Calculator.node.Start"))
            {
                _writer.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                PrintTime();
            }

            PrintIndent();

            //Console.ForegroundColor = ConsoleColor.White;
            _writer.Write(node.GetType().ToString().Replace("SimpleCalc.node.", ""));

            if (node is ANumberExp)
            {
                //Console.ForegroundColor = ConsoleColor.DarkGray;
                _writer.WriteLine("  " + node);
            }
            else
                _writer.WriteLine();
        }

        private void PrintTime()
        {
            _writer.WriteLine("{0}/{1}/{2} {3}:{4}:{5}:", DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }

        public override void DefaultIn(Node node)
        {
            PrintNode(node);
            _indent++;
        }

        public override void DefaultOut(Node node)
        {
            _indent--;
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public Guid GetGuid()
        {
            return _guid;
        }
    }
}
