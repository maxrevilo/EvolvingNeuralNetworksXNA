using System;
using System.Collections.Generic;
using System.Threading;
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
        //Indica si se desea control por IA, o movimiento aleatorio.
        private const bool RANDOM = false;

        //Topologia de las redes
        private const int HIDDEN_UNITS0 = 6;
        private const int HIDDEN_UNITS1 = 6;
        private const int OUTPUT_UNITS = 3;
        private const int INPUT_UNITS = 3;
        private int numWeights;

        //Diferencial de angulo que se aplicara a la direccion
        //del jugador, de acuerdo a la salida de la red neuronal
        private const float ANGLE_DIFF = 0.35f;

        //Si la salida de la red es mayor que esta constante, el jugador avanza. Caso contrario se detiene.
        private const float MOVEMENT_THRESHOLD = 0f;

        private Comida[] comidas;
        private Jugador[] jugadores;
        private ActivationNetwork[] redes;
        private double[] inputVector;
        private double[] outputVector;
        private ManualResetEvent[] doneEvents;

        //Campos para la logica de evolucion.
        private Population poblacion;
        private IFitnessFunction fitnessFunction;
        private IChromosome padre;
        private static IRandomNumberGenerator chromosomeGenerator;
        private static IRandomNumberGenerator mutationMultiplierGenerator;
        private static IRandomNumberGenerator mutationAdditionGenerator;
        private ISelectionMethod selectionMethod;

        //Este generador aleatorio se usara para generar un seed para los otros generadores aleatorios.
        private static Random rndSeedGen;

        //Este generador aleatorio se usara para generar movimiento aleatorio de los jugadores, sin IA.
        private static Random rndControl;
        private static Random rndMovControl;

        public IA(Game game, int players)
            : base(game)
        {
            rndSeedGen = new Random();
            rndControl = new Random();
            rndMovControl = new Random();
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
            doneEvents = new ManualResetEvent[players];
            for (int i = 0; i < players; i++) doneEvents[i] = new ManualResetEvent(false);

            //Se puede jugar con los parametros de los rangos para modificar la evolucion de las redes
            //Tambien se puede modificar el metodo de seleccion.
            chromosomeGenerator = new UniformGenerator(new Range(-10f, 10f), rndSeedGen.Next(-100, 100));
            mutationAdditionGenerator = new UniformGenerator(new Range(-8f, 8f), rndSeedGen.Next(-100, 100));
            mutationMultiplierGenerator = new UniformGenerator(new Range(-8f, 8f), rndSeedGen.Next(-100, 100));
            fitnessFunction = new GameFitnessFunction();
            selectionMethod = new EliteSelection();
            padre = new gameChromosome(chromosomeGenerator, mutationMultiplierGenerator, mutationAdditionGenerator, numWeights);
            poblacion = new Population(WorldGame.JUGADORES, padre, fitnessFunction, selectionMethod);
        }

        public float fitnessAvg() { return (float)poblacion.FitnessAvg; }
        public float fitnessMax() { return (float)poblacion.FitnessMax; }

        public void Generation(Jugador[] jugadores, Comida[] comidas)
        {
            //Actualizar los fitness de la poblacion actual y correr una epoca evolutiva en la poblacion
            if (this.comidas != null && this.jugadores != null) //Generacion posterior a la primera
            {
                for (int i = 0; i < jugadores.Length; i++)
                    ((gameChromosome)poblacion[i]).chromoFitness = this.jugadores[i].Fitness();
                poblacion.RunEpoch();
            }
            //Transformar la poblacion actual a redes neuronales controladoras            
            for (int i = 0; i < poblacion.Size; i++)
                ChromosomeNetworkMapper.ChromosomeToNetwork((gameChromosome)poblacion[i], redes[i]);
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
            for (int j = 0; j < jugadores.Length; j++)
            {
                doneEvents[j].Reset();
                ThreadPool.QueueUserWorkItem(ParrallelUpdate, new KeyValuePair<int, ManualResetEvent>(j, doneEvents[j]));
            }

            WaitHandle.WaitAll(doneEvents);

            base.Update(gameTime);
        }

        private void ParrallelUpdate(object param)
        {
            Thread.Sleep(0);
            KeyValuePair<int, ManualResetEvent> lim = (KeyValuePair<int, ManualResetEvent>)param;
            int i = lim.Key;
            ManualResetEvent e = lim.Value;

            float a1, a2;
            if (jugadores[i].Vivo)
            {
                a1 = jugadores[i].antenaInfo(0);
                a2 = jugadores[i].antenaInfo(1);

                inputVector[0] = 10f * (a1 - a2); //Se le pasa la informacion de diferencia de distancias
                inputVector[1] = (a1 + a2) / 10f; //una especie de promedio de distancias
                inputVector[2] = 100f * jugadores[i].Llenura - 50f; //La llenura que puede ser + o -

                outputVector = redes[i].Compute(inputVector);
                applyNetworkOutput(outputVector, jugadores[i]);
            }
            e.Set();
        }


        /// <summary>
        /// Aplica la salida de la red neuronal a cada jugador,
        /// modificando la direccion en la cual debe moverse.
        /// </summary>
        /// <param name="outputVector">Salida de la red neuronal que controla al jugador</param>
        /// <param name="j">Jugador para el cual se quiere modificar su cinematica</param>
        private void applyNetworkOutput(double[] outputVector, Jugador j)
        {
            //Preliminar, el codigo final esta sujeto a nuestra interpretacion de la salida y lo que esta modifica.
            float diffDireccionReal = ANGLE_DIFF;
            //Cambiar la direccion de acuerdo a la salida de la red.
            if (0.05 < Math.Abs(outputVector[1] - outputVector[0])) //Si hay duda no se voltea.
            {
                if (outputVector[0] > outputVector[1])
                    diffDireccionReal *= -1f;
            }
            else
            {
                diffDireccionReal = 0f;
            }
            //Enviar las ordenes de la red Neural al jugador.
            if (!RANDOM)
                j.controlar(outputVector[2] > MOVEMENT_THRESHOLD, diffDireccionReal);
            else
                j.controlar(rndMovControl.NextDouble() > 0d, 2f * (float)rndControl.NextDouble() - 1f);            
        }
    }
}
