using System;

namespace EvolvingNeuralNetworksXNA
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (WorldGame game = new WorldGame())
            {
                game.Run();
            }
        }
    }
#endif
}

