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
    /// Es una clase Singlenton que facilita el dibujo de sprites en el juego.
    /// </summary>
    public class Graphics
    {
        public static void Initialize(Game game)
        {
            if (instance == null) instance = new Graphics(game);
        }

        /// <summary>
        /// Se dibuja todo lo que este en el buffer.
        /// </summary>
        public static void DrawNow()
        {
            instance.sb.End();
            instance.sb.Begin();
        }

        /// <summary>
        /// Dibuja una textura en pantalla
        /// </summary>
        /// <param name="t">Textura a dibujar</param>
        /// <param name="rect">Area en el que se dibujara</param>
        /// <param name="c">Tinta para la textura (Blanco no la cambia)</param>
        public static void ToDraw(Texture2D t, Rectangle rect, Color c) {
            instance.sb.Draw(t, rect, c);
        }

        /// <summary>
        /// Dibuja una textura en pantalla rotandolo
        /// </summary>
        /// <param name="t">Textura a dibujar</param>
        /// <param name="rect">Area en el que se dibujara</param>
        /// <param name="c">Tinta para la textura (Blanco no la cambia)</param>
        /// <param name="angulo"> Angulo en que se rotara</param>
        public static void ToDraw(Texture2D t, Rectangle rect, Color c, float angulo)
        {
            Rectangle rect2 = rect;
            rect2.X += rect.Width / 2;
            rect2.Y += rect.Height / 2;
            instance.sb.Draw(t, rect2, null, c, angulo, new Vector2(t.Width/2, t.Height/2), SpriteEffects.None, 0f);
        }


        /// <summary>
        /// Dibuja un texto en pantalla
        /// </summary>
        public static void ToDraw(SpriteFont font, String str, Vector2 pos, Color c)
        {
            instance.sb.DrawString(font, str, pos, c);
        }

        /// <summary>
        /// Una textura de metida 1x1 de color blanco para dibujar rectangulos monocolor.
        /// </summary>
        public static Texture2D Pixel { get { return instance.pixel; } }
        /// <summary>
        /// Una textura que contiene un circulo de color blanco y transparente en el resto.
        /// </summary>
        public static Texture2D Circulo { get { return instance.circulo; } }



        #region Privado
        private static Graphics instance = null;
        private Game game;
        private SpriteBatch sb;
        private Texture2D pixel;
        private Texture2D circulo;

        private Graphics(Game game)
        {
            this.game = game;
            sb = new SpriteBatch(game.GraphicsDevice);
            sb.Begin();

            pixel = new Texture2D(game.GraphicsDevice, 1, 1);
            pixel.SetData<Color>(new Color[] {Color.White});

            circulo = game.Content.Load<Texture2D>("Circulo");
        }
        #endregion
        

    }
}
