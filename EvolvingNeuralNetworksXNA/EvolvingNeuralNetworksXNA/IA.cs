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
    public class IA : GameComponent
    {
        private Comida[] comidas;
        private Jugador[] jugadores;

        public IA(Game game, Comida[] comidas, Jugador[] jugadores)
            : base(game)
        {
            this.comidas = comidas;
            this.jugadores = jugadores;
        }

        public override void Initialize()
        {
            //CODIGO DE INICIALIZACION DE LA IA.

            Enabled = true;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            //CODIGO EN CADA ITERACION DEL JUEGO.

            base.Update(gameTime);
        }
    }
}
