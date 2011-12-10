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

namespace EvolvingNeuralNetworksXNA
{
    public class Jugador : Posicionable
    {

        /// <summary>
        /// La llenura con la que se crean los jugadores.
        /// </summary>
        public static float LLENURA_POR_DEFECTO = 0.5f;
        
        /// <summary>
        /// La taza de ambruna con la que se crean los jugadores.
        /// </summary>
        public static float TAZA_DE_AMBRUNA_POR_DEFECTO = 0.01f;

        /// <summary>
        /// La velocidad con la que se crean los jugadores.
        /// </summary>
        public static float VELOCIDAD_POR_DEFECTO = 25f;

        /// <summary>
        /// Indica si el jugador esta avanzando
        /// </summary>
        public bool moviendose;
        
        /// <summary>
        /// Direccion en la que apunta el jugador.
        /// </summary>
        public float direccion;

        /// <summary>
        ///  Taza de movimiento por segundo.
        /// </summary>
        public float velocidad;
        
        //Cantidad de llenura consumida por segundo de actividad.
        private float tazaDeAmbruna;
        
        
        //Indica si el jugador esta vivo.
        private bool vivo;
        /// <summary>
        /// Indica si el jugador esta vivo.
        /// </summary>
        public bool Vivo
        {
            get { return vivo; }
            set { vivo = value; Enabled = vivo; /*Visible = vivo;*/ }
        }


        //Parametro entre 0 y 1 que indica cuando lleno o muerto de hambre esta el jugador.
        private float llenura;
        /// <summary>
        /// Indica que tan lleno (de comida) esta el jugador.
        /// </summary>
        public float Llenura
        {
            get { return llenura; }
        }

        private WorldGame worldGame;

        public Jugador(WorldGame game, float X, float Y, float tamano)
            : base(game, X, Y, tamano)
        {
            Vivo = true;
            llenura = LLENURA_POR_DEFECTO;
            tazaDeAmbruna = TAZA_DE_AMBRUNA_POR_DEFECTO;
            velocidad = VELOCIDAD_POR_DEFECTO;
            worldGame = game;

            moviendose = false;
            direccion = 0f;
        }

        override public void Update(GameTime gameTime)
        {
            float seg = (float) gameTime.ElapsedGameTime.TotalSeconds;

            if (moviendose)
            {
                alimentar(- tazaDeAmbruna * seg );

                X += (float) Math.Cos(direccion) * velocidad * seg;
                Y += (float) Math.Sin(direccion) * velocidad * seg;
            }
        }

        /// <summary>
        /// Aumenta la llenura del jugador
        /// </summary>
        /// <param name="cantidad">Cantidad de llenura a aumentar</param>
        public void alimentar(float cantidad) {
            llenura += cantidad;

            if(llenura > 1f || llenura < 0f) {
                Vivo = false;
            }

        }

        /// <summary>
        /// Controla al jugador dandole orden de avanzar y/o jirar.
        /// </summary>
        /// <param name="Avanzar">Indica si el jugador se va a mover o no</param>
        /// <param name="sumarAngulo">Le suma este valor a la direccion del jugador</param>
        public void controlar(bool Avanzar, float sumarAngulo)
        {
            moviendose = Avanzar;
            direccion += sumarAngulo;
        }

        override public void Draw(GameTime gameTime)
        {
            Graphics.ToDraw(Graphics.Circulo, new Rectangle((int)(X - R), (int)(Y - R), (int)(2 * R), (int)(2 * R)), Color.Lerp(Color.Red, Color.Green, llenura), direccion);
        }
    }
}
