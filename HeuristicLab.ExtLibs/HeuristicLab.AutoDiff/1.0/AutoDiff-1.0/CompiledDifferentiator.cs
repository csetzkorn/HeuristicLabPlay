﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace AutoDiff
{
    /// <summary>
    /// Compiles the terms tree to a more efficient form for differentiation.
    /// </summary>
    internal partial class CompiledDifferentiator<T> : ICompiledTerm
        where T : IList<Variable>
    {
        private readonly Compiled.TapeElement[] tape;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledDifferentiator"/> class.
        /// </summary>
        /// <param name="function">The function.</param>
        /// <param name="variables">The variables.</param>
        public CompiledDifferentiator(Term function, T variables)
        {
            Contract.Requires(function != null);
            Contract.Requires(variables != null);
            Contract.Requires(Contract.ForAll(variables, variable => variable != null));
            Contract.Ensures(Dimension == variables.Count);

            if (function is Variable)
                function = new ConstPower(function, 1);

            var tapeList = new List<Compiled.TapeElement>();
            new Compiler(variables, tapeList).Compile(function);
            tape = tapeList.ToArray();

            Dimension = variables.Count;
            Variables = new ReadOnlyCollection<Variable>(variables);
        }

        public int Dimension { get; private set; }

        public double Evaluate(double[] arg)
        {
            EvaluateTape(arg);
            return tape.Last().Value;
        }

        public Tuple<double[], double> Differentiate<S>(S arg)
            where S : IList<double>
        {
            ForwardSweep(arg);
            ReverseSweep();

            var gradient = new double[Dimension];
            for (int i = 0; i < Dimension; i++)
               gradient[i] = tape[i].Adjoint;
            var value = tape.Last().Value;            

            return Tuple.Create(gradient, value);
        }

        public Tuple<double[], double> Differentiate(params double[] arg)
        {
            return Differentiate<double[]>(arg);
        }

        private void ReverseSweep()
        {
            tape.Last().Adjoint = 1;
            
            // initialize adjoints
            for (int i = 0; i < tape.Length - 1; ++i)
                tape[i].Adjoint = 0;

            // accumulate adjoints
            for (int i = tape.Length - 1; i >= Dimension; --i)
            {
                var inputs = tape[i].Inputs;
                var adjoint = tape[i].Adjoint;
                
                for(int j = 0; j < inputs.Length; ++j)
                    tape[inputs[j].Index].Adjoint += adjoint * inputs[j].Weight;
            }
        }

        private void ForwardSweep<S>(S arg)
            where S : IList<double>
        {
            for (int i = 0; i < Dimension; ++i)
                tape[i].Value = arg[i];

            var forwardDiffVisitor = new ForwardSweepVisitor(tape);
            for (int i = Dimension; i < tape.Length; ++i)
                tape[i].Accept(forwardDiffVisitor);
        }

        private void EvaluateTape(double[] arg)
        {
            for(int i = 0; i < Dimension; ++i)
                tape[i].Value = arg[i];
            var evalVisitor = new EvalVisitor(tape);
            for (int i = Dimension; i < tape.Length; ++i )
                tape[i].Accept(evalVisitor);
        }

        private double ValueOf(int index)
        {
            return tape[index].Value;
        }

        public ReadOnlyCollection<Variable> Variables { get; private set; }


    }
}
