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


namespace EvolvingNeuralNetworksXNA
{
    public class IA : GameComponent
    {
        //Topologia de las redes
        private const int HIDDEN_UNITS0 = 10;
        private const int HIDDEN_UNITS1 = 10;
        private const int OUTPUT_UNITS = 3;
        private const int INPUT_UNITS = 3;

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
        private Population poblacion;
        private IFitnessFunction fitnessFunction;
        private IChromosome padre;
        private 


        private static Random rnd; //TEMPORAL, solo para pruebas.

        public IA(Game game, int players)
            : base(game)
        {
            this.comidas = null;
            this.jugadores = null;
            rnd = new Random();
            redes = new ActivationNetwork[players];
            for (int i = 0; i < redes.Length; i++)
            {
                redes[i] = new ActivationNetwork(new SigmoidFunction(0.5), INPUT_UNITS, HIDDEN_UNITS0, HIDDEN_UNITS1, OUTPUT_UNITS);                
            }
            inputVector = new double[INPUT_UNITS];
            outputVector = new double[OUTPUT_UNITS];
            poblacion = new Population(5, padre, fitnessFunction, selectionMethodMethod);
        }

        public void Generation(Jugador[] jugadores, Comida[] comidas)
        {
            //Simon: Antes de estas asignaciones tienes las generaciones anteriores apuntadas (o null) para que hagas los calculos nec 
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
            for (int i = 0; i < jugadores.Length; i++)
            {
                inputVector[0] = jugadores[i].antenaInfo(0);
                inputVector[1] = jugadores[i].antenaInfo(1);
                inputVector[2] = jugadores[i].Llenura;               
                outputVector = redes[i].Compute(inputVector);
                applyNetworkOutput(outputVector, jugadores[i]);
            }
            base.Update(gameTime);
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
            if (outputVector[0] > outputVector[1])
                diffDireccionReal *= -1;
            
            //Enviar las ordenes de la red Neural al jugador.
            j.controlar(outputVector[2] > MOVEMENT_THRESHOLD, diffDireccionReal);
        }
    }
}
