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
    /// <summary>
    /// Clase base para todo lo que tenga una posicion y un area (Circular) en el escenario.
    /// </summary>
    public class Posicionable : DrawableGameComponent
    {
        private Vector2 pos;

        public float X
        {
            get { return pos.X; }
            set { pos.X = value; }
        }

        public float Y
        {
            get { return pos.Y; }
            set { pos.Y = value; }
        }

        public float R;


        public Posicionable(Game game, float X, float Y, float R)
            : base(game)
        {
            this.X = X;
            this.Y = Y;
            this.R = R;
        }

        public Vector2 toVector2()
        {
            return pos;
        }

        public void toVector2(out Vector2 vec)
        {
            vec = pos;
        }

        /// <summary>
        /// Detecta si este Posicionable hace contacto con otro.
        /// </summary>
        /// <param name="otro">El objeto a probar colision</param>
        public bool probarContacto(Posicionable otro)
        {
            float dX = X - otro.X;
            float dY = Y - otro.Y;
            float RR = otro.R + R;
            return dX * dX + dY * dY <= RR * RR;
        }
    }
}
