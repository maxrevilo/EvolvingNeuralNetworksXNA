using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;

namespace EvolvingNeuralNetworksXNA
{
    class GameFitnessFunction : IFitnessFunction
    {
        public double Evaluate(IChromosome chromosome)
        {
            return ((gameChromosome)chromosome).Fitness;
        }
    }
}
