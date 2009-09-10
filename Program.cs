using System;
using Microsoft.Xna.Framework.Content;

namespace Othello
{
    static class Program
    {
        public static ContentManager Content;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}
