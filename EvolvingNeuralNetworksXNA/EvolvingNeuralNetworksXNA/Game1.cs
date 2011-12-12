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
    public class WorldGame : Microsoft.Xna.Framework.Game
    {
        /**
         * CONSTANTES:
         */

        /// <summary>
        /// Cantidad de jugadores:
        /// </summary>
        public static int JUGADORES = 50;

        /// <summary>
        /// Tamaño de los jugadores
        /// </summary>
        static int TAMANO_JUGADOR = 10;

        /// <summary>
        /// Cantidad maxima y minima de comida en el escenario.
        /// </summary>
        public static int COMIDA_MAX = 40, COMIDA_MIN = 20;

        /// <summary>
        /// Tamaño de las particulas de comida
        /// </summary>
        static int TAMANO_COMIDA = 5;

        /// <summary>
        /// Rectangulo que define las dimensiones del escenario
        /// </summary>
        public static Rectangle ESCENARIO = new Rectangle(0, 0, 1200, 800);

        public static float ESCALA;
        /** 
         * CONSTANTES.
         */


        /**
         * Variables del juego
         **/
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont font;
        static Random rnd;
        KeyboardState kbsOld;

        //Arreglo de jugadores:
        public Jugador[] jugadores;

        //Arreglo de comida:
        public Comida[] comidas;

        public int generacion;

        bool Dibujar;

        //Clase controladora de la IA:
        IA ia;
        
        /// <summary>
        /// Indica cuantos ciclos de simulacion se ejecutan por actualizacion, la cual
        /// generalmente ocurre 60 veces por segundo.
        /// </summary>
        public int ciclosPorActualizacion;

        public WorldGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            //Tamaño de la ventana:
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = 600;
            graphics.PreferredBackBufferHeight = 400;
            graphics.ApplyChanges();

            ESCALA = Math.Min(GraphicsDevice.Viewport.Width / (float)ESCENARIO.Width, GraphicsDevice.Viewport.Height / (float)ESCENARIO.Height);


            Dibujar = true;

            jugadores = null;
            comidas = null;

            //Inicializando la IA:
            ia = new IA(this, JUGADORES);
            ia.Initialize();
            Components.Add(ia);

            generacion = 0;
            ciclosPorActualizacion = 1;
        }

        protected override void Initialize()
        {
            if (jugadores != null)  foreach (Jugador j in jugadores) Components.Remove(j);
            if (comidas   != null)  foreach (Comida  c in comidas  ) Components.Remove(c);


            //Al forzar el recolector de basura no importa hacer New aqui:
            rnd = new Random();
            jugadores = new Jugador[JUGADORES];
            comidas = new Comida[rnd.Next(COMIDA_MIN, COMIDA_MAX)];

            //Inicializando los jugadores:
            for (int i = 0; i < jugadores.Length; i++)
            {
                jugadores[i] = new Jugador(this, rnd.Next(ESCENARIO.Center.X, ESCENARIO.Right), rnd.Next(ESCENARIO.Top, ESCENARIO.Bottom), TAMANO_JUGADOR);
                Components.Add(jugadores[i]); //Con esto se grafican y actualizan automaticamente.

                //jugadores[i].moviendose = true; //Esto se puede quitar, pues la IA controlará este parametro.
            }

            //Inicializando la comida:
            for (int i = 0; i < comidas.Length; i++)
            {
                comidas[i] = new Comida(this, rnd.Next(ESCENARIO.Left, ESCENARIO.Center.X), rnd.Next(ESCENARIO.Top, ESCENARIO.Bottom), TAMANO_COMIDA);
                Components.Add(comidas[i]);
            }


            ia.Generation(jugadores, comidas);
            generacion++;

            base.Initialize();

            //Se fuerza el recolector de basura:
            GC.Collect();
        }
        
        /**
         * Funcion para cargar contenido del HD.
         */
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Font");

            Graphics.Initialize(this);
        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            controlDelTeclado();

            /* Este es el tiempo de la partida, se modifica para que el diferencial de tiempo sea fijo de
             * 17 milisegundos, esto disminulle la suavidad grafica de movimiento pero mejora el realismo
             * de la simulacion, sobretodo para varios ciclos por actualizacion.
             */
            GameTime gameTime2 = new GameTime(gameTime.TotalGameTime, new TimeSpan(0, 0, 0, 0, 17 ));

            for (int i = 0; i < ciclosPorActualizacion; i++)
            {
                //Aqui va toda la actualizacion de la simulacion:


                bool JugadorVivo = false;

                #region colision con comidas:
                //Se verifica para cada jugador si este ha tocado comida:
                foreach (Jugador jugador in jugadores)
                {
                    //Console.WriteLine("Updated {0}", jugador.GetHashCode());
                    foreach (Comida comida in comidas)
                    {
                        if (jugador.Enabled) //Si esta vivo:
                        {
                            JugadorVivo = true;
                            //Si el jugador entra en contacto con la comida:
                            if (jugador.probarContacto(comida))
                            {
                                jugador.alimentar(comida.comer());
                            }
                        }
                    }
                }
                #endregion


                //Si no hay jugadores vivos se inicia una nueva poblacion:
                if (!JugadorVivo) Initialize();


                //Se le pasa el gameTime2 para que actualice todo en funcion de el.
                base.Update(gameTime2);

            }
        }

        private void controlDelTeclado()
        {
            KeyboardState kbs = Keyboard.GetState();

            if (kbs.IsKeyDown(Keys.Q))
            {
                ciclosPorActualizacion = Math.Max(1, ciclosPorActualizacion - 1);
            }
            else if (kbs.IsKeyDown(Keys.W))
            {
                ciclosPorActualizacion++;
            }

            if (kbs.IsKeyDown(Keys.D) && kbsOld.IsKeyUp(Keys.D))
            {
                Dibujar = !Dibujar;
            }


            kbsOld = kbs;
        }

        protected override void Draw(GameTime gameTime)
        {
            if (Dibujar)
            {
                GraphicsDevice.Clear(Color.Black);

                //Esto simplemente dibuja todo lo que se halla puesto en el buffer de Graphics
                Graphics.DrawNow();

                //Aqui se llama a Draw en los objetos en Components automaticamente 
                base.Draw(gameTime);

                //Se dibuja el HUD:
                Graphics.ToDraw(font, "Iteraciones: " + ciclosPorActualizacion, new Vector2(10, 10), Color.White);
                Graphics.ToDraw(font, "Generacion: " + generacion, new Vector2(10, 30), Color.White);
                Graphics.ToDraw(font, "Fitness Prom: " + ia.fitnessAvg(), new Vector2(10, 50), Color.White);
                Graphics.ToDraw(font, "Mejor Fitness: " + ia.fitnessMax(), new Vector2(10, 70), Color.White);
            }

        }
    }
}
