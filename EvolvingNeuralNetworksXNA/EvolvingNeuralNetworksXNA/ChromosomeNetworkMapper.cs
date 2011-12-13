using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;
using AForge.Neuro;

namespace EvolvingNeuralNetworksXNA
{
    static class ChromosomeNetworkMapper
    {
        public static void ChromosomeToNetwork(gameChromosome chromosome, ActivationNetwork network)
        {
            double[] values = chromosome.Value;
            int l = 0;
            for (int i = 0; i < network.LayersCount; i++)
                for (int j = 0; j < network[i].NeuronsCount; j++)
                    for (int k = 0; k <= network[i][j].InputsCount; k++)
                    {
                        if (k == 0)
                            network[i][j].Threshold = values[l];
                        else
                            network[i][j][k - 1] = values[l];
                        l++;
                    }
        }
    }
}
