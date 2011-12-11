using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using AForge.Genetic;
using AForge.Neuro;
using AForge.Math.Random;
using AForge;


namespace EvolvingNeuralNetworksXNA
{
    public class IA : GameComponent
    {
        //Topologia de las redes
        private const int HIDDEN_UNITS0 = 4;
        private const int HIDDEN_UNITS1 = 4;
        private const int OUTPUT_UNITS = 3;
        private const int INPUT_UNITS = 3;
        private int numWeights;

        //Diferencial de angulo que se aplicara a la direccion
        //del jugador, de acuerdo a la salida de la red neuronal
        private const float ANGLE_DIFF = 0.02f;

        //Si la salida de la red es mayor que esta constante, el jugador avanza. Caso contrario se detiene.
        private const float MOVEMENT_THRESHOLD = 0.5f;

        private Comida[] comidas;
        private Jugador[] jugadores;
        private ActivationNetwork[] redes;
        private double[] inputVector;
        private double[] outputVector;

        //Campos para la logica de evolucion
        private Population poblacion;
        private IFitnessFunction fitnessFunction;
        private IChromosome padre;
        private static IRandomNumberGenerator chromosomeGenerator;
        private static IRandomNumberGenerator mutationMultiplierGenerator;
        private static IRandomNumberGenerator mutationAdditionGenerator;
        private ISelectionMethod selectionMethod;

        public IA(Game game, int players)
            : base(game)
        {
            this.comidas = null;
            this.jugadores = null;
            this.numWeights = HIDDEN_UNITS0 * (INPUT_UNITS + 1) + HIDDEN_UNITS1 * (HIDDEN_UNITS0 + 1) + OUTPUT_UNITS * (HIDDEN_UNITS1 + 1);
            redes = new ActivationNetwork[players];
            for (int i = 0; i < redes.Length; i++)
            {
                redes[i] = new ActivationNetwork(new SigmoidFunction(400), INPUT_UNITS, HIDDEN_UNITS0, HIDDEN_UNITS1, OUTPUT_UNITS);
            }
            inputVector = new double[INPUT_UNITS];
            outputVector = new double[OUTPUT_UNITS];

            //Se puede jugar con los parametros de los rangos para modificar la evolucion de las redes
            //Tambien se puede modificar el metodo de seleccion.
            chromosomeGenerator = new UniformGenerator(new Range(-1f, 1f));
            mutationAdditionGenerator = new UniformGenerator(new Range(-3f, 3f));
            mutationMultiplierGenerator = new UniformGenerator(new Range(-2f, 2f));
            fitnessFunction = new GameFitnessFunction();
            selectionMethod = new EliteSelection();
            padre = new gameChromosome(chromosomeGenerator, mutationMultiplierGenerator, mutationAdditionGenerator, numWeights);
            poblacion = new Population(WorldGame.JUGADORES, padre, fitnessFunction, selectionMethod);
            
        }

        //Estas funciones no sirve todavia:
        public float fitnessAvg() { return (float) poblacion.FitnessAvg; }
        public float fitnessMax() { return (float) poblacion.FitnessMax; }

        public void Generation(Jugador[] jugadores, Comida[] comidas)
        {
            //Actualizar los fitness de la poblacion actual y correr una epoca evolutiva en la poblacion
            if (this.comidas != null && this.jugadores != null) //Generacion posterior a la primera
            {
                for (int i = 0; i < jugadores.Length; i++)
                    ((gameChromosome)poblacion[i]).chromoFitness = this.jugadores[i].Fitness();

                poblacion.RunEpoch();

                //Transformar la poblacion actual a redes neuronales controladoras            
                for (int i = 0; i < poblacion.Size; i++)
                {
                    ChromosomeNetworkMapper.ChromosomeToNetwork((gameChromosome)poblacion[i], redes[i]);
                }
            }

            
            //Simon: Antes de estas asignaciones tienes las generaciones anteriores apuntadas (o null) para que hagas los calculos necesarios
            this.jugadores = jugadores;
            this.comidas = comidas;
        }


        public override void Initialize()
        {
            Enabled = true;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float a1, a2;
            for (int i = 0; i < jugadores.Length; i++)
            {
                if (jugadores[i].Enabled)
                {
                    a1 = jugadores[i].antenaInfo(0); a2 = jugadores[i].antenaInfo(1);
                    inputVector[0] = 10f * (a1-a2); //Se le pasa la informacion de diferencia de distancias
                    inputVector[1] = (a1+a2) / 10f; //un especie de promedio de distancias
                    inputVector[2] = 100f * jugadores[i].Llenura - 50f; //La llenura que puede ser + o -
                    outputVector = redes[i].Compute(inputVector);
                    applyNetworkOutput(outputVector, jugadores[i]);
                }
            }
            base.Update(gameTime);
        }

        public double maxExit = 0;

        /// <summary>
        /// Aplica la salida de la red neuronal a cada jugador,
        /// modificando la direccion en la cual debe moverse.
        /// </summary>
        /// <param name="outputVector">Salida de la red neuronal que controla al jugador</param>
        /// <param name="j">Jugador para el cual se quiere modificar su cinebmatica</param>
        private void applyNetworkOutput(double[] outputVector, Jugador j)
        {
            //Preliminar, el codigo final esta sujeto a nuestra interpretacion de la salida y lo que esta modifica.
            float diffDireccionReal = ANGLE_DIFF;
            //Cambiar la direccion de acuerdo a la salida de la red.
            if (0.01 < Math.Abs(outputVector[1] - outputVector[0])) //Si hay duda no se voltea.
            {
                if (outputVector[0] > outputVector[1]) diffDireccionReal *= -1f;
            }
            else
            {
                diffDireccionReal = 0f;
            }

            maxExit = Math.Max(maxExit, outputVector[2]);
            //Enviar las ordenes de la red Neural al jugador.
            j.controlar(outputVector[2] > MOVEMENT_THRESHOLD, diffDireccionReal);
        }
    }
}
