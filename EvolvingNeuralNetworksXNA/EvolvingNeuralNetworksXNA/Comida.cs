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
    /// Representa un cumulo de comida en algun punto del escenario.
    /// </summary>
    public class Comida : Posicionable
    {
        //La cantidad con la que se crean las comidas.
        public static float CANTIDAD_POR_DEFECTO = 0.3f;

        private float cantidad;
        public float Cantidad { 
            get { return cantidad; }
            set { cantidad = value; Visible = cantidad > 0f; } 
        }

        public Comida(Game game, float X, float Y, float R)
            : base(game, X, Y, R)
        {
            Cantidad = CANTIDAD_POR_DEFECTO;
        }

        /// <summary>
        /// Consume la comida.
        /// </summary>
        /// <returns>retorna la cantidad de comida que estaba en el objeto</returns>
        public float comer()
        {
            float r = Cantidad;
            Cantidad = 0f;
            return r;
        }

        /// <summary>
        /// Consume parte de la comida.
        /// </summary>
        /// <param name="cantidad">cantidad de comida a consumir</param>
        /// <returns>returona la cantidad de comida que efectivamente se pudo consumir del objeto</returns>
        public float comer(float cantidad)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Dibuja la comida en su posicion
        /// </summary>
        /// <param name="gameTime"></param>
        override public void Draw(GameTime gameTime)
        {
            Graphics.ToDraw(Graphics.Pixel, new Rectangle((int)(X - R), (int)(Y - R), (int)(2 * R), (int)(2 * R)), Color.Pink);
        }
    }
}
