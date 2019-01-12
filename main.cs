using System;
using System.IO;
using System.Collections.Generic;
#pragma warning disable 0649
namespace AI
{
    class App
    {
        public static void Main(string[] args)
        {

            network.applyWeights(1, 3, 9, 9);
            Server.start();
        }
    }
    static class network
    {
        public static List<List<List<decimal>>> weights = new List<List<List<decimal>>>();
        public static List<List<List<decimal>>> derivatives = new List<List<List<decimal>>>();
        public static List<List<decimal>> values = new List<List<decimal>>();
        public static List<decimal> _temp = new List<decimal>();

        public static decimal _output;
        public static decimal _error;

        public static void train(List<decimal[]> inputs, decimal[] outputs, int iterations)
        {
            for (int j = 0; j < iterations; j++) {
                for (int q = 0; q < inputs.Count; q++)
                {
                    _output=(run(inputs[q]));
                    _error=(outputs[q]-_output);
                    _error*=d_sigmoid(_output);
                    for (int i = 0; i < weights[weights.Count - 1][0].Count; i++)
                    {
                        derivatives[weights.Count - 1][0][i] = values[weights.Count - 2][i];
                        weights[weights.Count - 1][0][i] += values[weights.Count-2][i] * _error;
                    }
                    if (weights.Count > 2)
                    {
                        for (int b = 0; b < weights[weights.Count - 2].Count; b++)
                        {
                            for (int i = 0; i < weights[weights.Count - 2][b].Count; i++)
                            {

                                derivatives[weights.Count - 2][b][i] = weights[weights.Count - 1][0][b] * values[weights.Count - 3][i];
                                weights[weights.Count - 2][b][i] += weights[weights.Count - 1][0][b] * values[weights.Count - 3][i] * _error;
                            }
                        }
                    }
                }
            }

        }
        public static decimal run(decimal[] inputs)
        {
            values = new List<List<decimal>>();
            decimal value;

            values.Add(new List<decimal>());
            for (int b = 0; b < weights[0].Count; b++)
            {
                value = 0;
                for (int i = 0; i < inputs.Length; i++)
                {
                    //change
                    value += weights[0][b][i] * inputs[i];
                }
                values[0].Add(value);
            }
            for (int j = 1; j < weights.Count-1; j++)
            {

                values.Add(new List<decimal>());
                for (int b = 0; b < weights[j].Count; b++)
                {
                    value = 0;
                    for (int i = 0; i < weights[j][b].Count; i++)
                    {
                        //change (out of range)
                        value += weights[j][b][i] * values[j-1][i];
                    }
                    values[j].Add(value);
                }
            }

            value = 0;
            for (int i = 0; i < weights[weights.Count-1][0].Count; i++) {
                value += weights[weights.Count-1][0][i] * values[weights.Count-2][i];
            }
            return sigmoid(value);
        }
        public static void applyWeights(int numOuts, int hiddenLayers, int width, int numIns) {
            weights = new List<List<List<decimal>>>();
            derivatives = new List<List<List<decimal>>>();
            for (int q = -1; q < hiddenLayers; q++) {
                weights.Add(new List<List<decimal>>());
                derivatives.Add(new List<List<decimal>>());
            }

            derivatives[derivatives.Count - 1].Add(new List<decimal>());
            weights[weights.Count-1].Add(new List<decimal>());

            Random r = new Random();
            
            for (int q = 0; q < width; q++)
            {
                weights[0].Add(new List<decimal>());
                derivatives[0].Add(new List<decimal>());
                for (int i = 0; i < numIns; i++)
                {
                    weights[0][q].Add((decimal)r.NextDouble() - (decimal).5);
                    derivatives[0][q].Add(0);
                }
            }
            for (int b = 1; b < hiddenLayers; b++)
            {
                for (int q = 0; q < width; q++)
                {
                    derivatives[b].Add(new List<decimal>());
                    weights[b].Add(new List<decimal>());
                    for (int i = 0; i < width; i++)
                    {

                        derivatives[b][q].Add(0);
                        weights[b][q].Add((decimal)r.NextDouble() - (decimal).5);
                    }
                }
            }
            for (int i = 0; i < width; i++)
            {

                derivatives[weights.Count - 1][0].Add(0);
                weights[weights.Count-1][0].Add((decimal)r.NextDouble() - (decimal).5);
            }
        }
        public static decimal sigmoid(decimal x)
        {
            return (decimal)(1 / (1 + Math.Pow(Math.E, (double)-x)));
        }
        public static decimal d_sigmoid(decimal x)
        {
            return (x) * (1 - (x));
        }
    }
}
