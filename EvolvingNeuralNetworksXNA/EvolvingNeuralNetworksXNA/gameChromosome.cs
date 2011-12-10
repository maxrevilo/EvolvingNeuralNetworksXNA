using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AForge.Genetic;
using AForge.Math.Random;

namespace EvolvingNeuralNetworksXNA
{
    class gameChromosome : DoubleArrayChromosome
    {
        private double fitness;

        public double Fitness
        {
            get { return fitness; }
            set { fitness = value; }
        }

        public gameChromosome(gameChromosome source)
            : base(source)
        {
            this.fitness = source.fitness;
        }

        public gameChromosome(IRandomNumberGenerator chromosomeGenerator,
            IRandomNumberGenerator mutationMultiplierGenerator,
            IRandomNumberGenerator mutationAdditionGenerator, int length)
            : base(chromosomeGenerator, mutationMultiplierGenerator, mutationMultiplierGenerator, length)
        {
            this.fitness = 0.000000001; //Fitness muy bajo pero mayor que 0 (como lo requiere la libreria AForge).
        }

        public override IChromosome CreateNew()
        {
            gameChromosome nuevo = new gameChromosome(chromosomeGenerator, mutationMultiplierGenerator, mutationAdditionGenerator, Length);
            nuevo.Generate();
            nuevo.fitness = 0.0000000001;
            return nuevo;
        }

        public override IChromosome Clone()
        {
            return new gameChromosome(this);
        }
    }
}
