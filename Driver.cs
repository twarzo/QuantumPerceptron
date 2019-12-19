using System;
using System.Collections.Generic;
using System.IO;

using Microsoft.Quantum.Simulation.Core;
using Microsoft.Quantum.Simulation.Simulators;
using QuantumPerceptron;

namespace Quantum.QuantumPerceptron
{
    class Driver
    {
        internal const int DEFAULT_ITERATIONS = 2048;
        static void Main(string[] args)
        {
            string[] lines = File.ReadAllLines(@"input.txt");
            string numInput = lines[0];
            string inputVectorStr = lines[1];
            string weightVectorStr = lines[2];
            string iterationInput = lines[3];
            int qubitCount = Convert.ToInt32(numInput);
            int iterations = (!string.IsNullOrEmpty(iterationInput.Trim())) ? Convert.ToInt32(iterationInput) : DEFAULT_ITERATIONS;
            Console.WriteLine("Reading Inputs from file: input.txt as below");
            Console.WriteLine($"Input Vector: {inputVectorStr}");
            Console.WriteLine($"Weight Vector: {weightVectorStr}");
            Console.WriteLine($"Running with {qubitCount} Qubits for {iterations} iterations...");
            Console.WriteLine("------------------------------------");

            Dictionary<long, Dictionary<long, List<long>>> preComputedOnesListDictionary = Helper.CreateBitSetListForNumbers(qubitCount);

            long[] inputVector = Helper.ConvertToLongArray(inputVectorStr);
            long[] weightVector = Helper.ConvertToLongArray(weightVectorStr);
            List<List<long>> listInputCPZ = Helper.CalculateInputZsList(inputVector, preComputedOnesListDictionary);
            List<List<long>> listWeightCPZ = Helper.CalculateInputZsList(weightVector, preComputedOnesListDictionary);

            bool[] inputHyperGraph = Helper.ConstructHyperGraph(listInputCPZ, qubitCount);
            bool[] weightHyperGraph = Helper.ConstructHyperGraph(listWeightCPZ, qubitCount);

            using (var qsim = new QuantumSimulator())
            {
                long countOne = Perceptron.Run(
                    qsim,
                    new QArray<bool>(inputHyperGraph),
                    new QArray<bool>(weightHyperGraph),
                    qubitCount,
                    iterations).Result;

                double cm = Math.Sqrt((double)countOne / (double)iterations);
                double dotproduct = (1 << qubitCount) * cm;
                Console.WriteLine("Quantum DotProduct:" + dotproduct);
                Console.WriteLine("Classical DotProduct:" + Helper.DotProduct(inputVector, weightVector));
            }
        }
    }
}