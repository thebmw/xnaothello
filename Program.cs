using System;
using Microsoft.Xna.Framework.Content;

namespace Othello
{
    static class Program
    {
        static public Boolean HD = false;
        public static ContentManager Content;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length != 0)
            {
                if (args[0] == "ZuneHD")
                {
                    HD = true;
                }
            }
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
}
