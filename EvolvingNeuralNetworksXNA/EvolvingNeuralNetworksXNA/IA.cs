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
        private const int HIDDEN_UNITS = 10;
        private const int OUTPUT_UNITS = 2;
        private const int INPUT_UNITS = 3;

        //Diferencial de angulo que se aplicara a la direccion
        //del jugador, de acuerdo a la salida de la red neuronal
        private const float ANGLE_DIFF = 0.01f;

        private Comida[] comidas;
        private Jugador[] jugadores;
        private ActivationNetwork[] redes;
        private double[] inputVector;
        private double[] outputVector;

        private static Random rnd; //TEMPORAL, solo para pruebas.

        public IA(Game game, Comida[] comidas, Jugador[] jugadores)
            : base(game)
        {
            this.comidas = comidas;
            this.jugadores = jugadores;
            rnd = new Random();
        }

        public override void Initialize()
        {
            redes = new ActivationNetwork[jugadores.Length];
            inputVector = new double[INPUT_UNITS];
            outputVector = new double[OUTPUT_UNITS];

            for (int i = 0; i < redes.Length; i++)
            {
                redes[i] = new ActivationNetwork(new SigmoidFunction(0.1), INPUT_UNITS, HIDDEN_UNITS, OUTPUT_UNITS);
                redes[i].Randomize();
            }
            Enabled = true;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            for (int i = 0; i < jugadores.Length; i++)
            {
                inputVector[0] = rnd.NextDouble() - 0.5; //TODO: el inputVector debe ser funcion de cada Jugador.
                inputVector[1] = rnd.NextDouble() - 0.5; //TODO: el inputVector debe ser funcion de cada Jugador.
                inputVector[2] = rnd.NextDouble() - 0.5; //TODO: el inputVector debe ser funcion de cada Jugador.                
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
            //Preliminar, el codigo final esta sujeto a nuestra interpretacion de la salida y lo que esta modifica
            float diffDireccionReal = ANGLE_DIFF;
            if (outputVector[0] > outputVector[1])
                diffDireccionReal *= -1;
            j.direccion += diffDireccionReal;
        }
    }
}
