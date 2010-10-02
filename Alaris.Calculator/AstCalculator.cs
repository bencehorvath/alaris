using System;
using System.Collections.Generic;
using System.Globalization;
using Alaris.API;
using Alaris.Calculator.analysis;
using Alaris.Calculator.node;
using Emil.GMP;

namespace Alaris.Calculator
{
    public sealed class AstCalculator : DepthFirstAdapter, IAlarisComponent
    {
        private double? _result;
        private readonly Stack<double> _stack = new Stack<double>();
        private readonly Guid _guid;

        public AstCalculator() {
            _guid = Guid.NewGuid(); }

        public double CalculatedResult
        {
            get
            {
                if (_result == null)
                    throw new InvalidOperationException("Must apply AstCalculator to the AST first.");

                return _result.Value;
            }
        }

        public override void OutStart(Start node)
        {
            if (_stack.Count != 1)
                throw new Exception("Stack should contain only one element at end.");

            _result = _stack.Pop();
        }

        // Associative operators
        public override void OutAMulExp(AMulExp node)
        {
            _stack.Push(_stack.Pop() * _stack.Pop());
        }

        public override void OutAAddExp(AAddExp node)
        {
            _stack.Push(_stack.Pop() + _stack.Pop());
        }

        // Non associative operators
        public override void OutASubExp(ASubExp node)
        {
            double numB = _stack.Pop();

            _stack.Push(_stack.Pop() - numB);
        }

        public override void OutAModExp(AModExp node)
        {
            double numB = _stack.Pop();

            _stack.Push(_stack.Pop() % numB);
        }

        public override void OutAAbsExp(AAbsExp node)
        {
            _stack.Push(Math.Abs(_stack.Pop()));
        }

        public override void OutAPowExp(APowExp node)
        {
            double hv = _stack.Pop();

            _stack.Push(Math.Pow(_stack.Pop(), hv));
        }

        public override void OutADivExp(ADivExp node)
        {
            double numB = _stack.Pop();

            _stack.Push(_stack.Pop() / numB);
        }

        // Unary
        public override void OutASqrtExp(ASqrtExp node)
        {
            _stack.Push(Math.Sqrt(_stack.Pop()));
            

        }

        public override void OutACosExp(ACosExp node)
        {
            _stack.Push(Math.Cos(_stack.Pop()));
        }

        public override void OutASinExp(ASinExp node)
        {
            _stack.Push(Math.Sin(_stack.Pop()));
        }

        public override void InANumberExp(ANumberExp node)
        {
            _stack.Push(Convert.ToDouble(node.GetNumber().Text.Trim(), new CultureInfo("en-us")));
        }

        public Guid GetGuid()
        {
            return _guid;
        }
    }
}
