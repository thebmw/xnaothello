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
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace Othello
{
    public class Piece
    {
        public Texture2D image;
        public Point boardplace;
        public Point screenplace;
        public Int32 col;
        public Piece()
        {

        }
        public Piece(Texture2D pic, Point bp, Int32 icol)
        {
            image = pic;
            boardplace = bp;
            screenplace = new Point(bp.X * 30, bp.Y * 30);
            col = icol;
        }
    }
    public class Texture2DColl
    {
        public Piece[,] spots = new Piece[8, 8];
        Texture2D b;
        Texture2D w;
        Texture2D bl;
        public Texture2DColl()
        {
            
        }
        public Texture2DColl(Texture2D black, Texture2D white, Texture2D blank)
        {
            b = black;
            bl = blank;
            w = white;
            
            #region Pieses
            #region row 1
            spots[0, 0] = new Piece(blank, new Point(0, 0), 0);
            spots[0, 1] = new Piece(blank, new Point(0, 1), 0);
            spots[0, 2] = new Piece(blank, new Point(0, 2), 0);
            spots[0, 3] = new Piece(blank, new Point(0, 3), 0);
            spots[0, 4] = new Piece(blank, new Point(0, 4), 0);
            spots[0, 5] = new Piece(blank, new Point(0, 5), 0);
            spots[0, 6] = new Piece(blank, new Point(0, 6), 0);
            spots[0, 7] = new Piece(blank, new Point(0, 7), 0);
            
            #endregion
            #region row 2
            spots[1, 0] = new Piece(blank, new Point(1, 0), 0);
            spots[1, 1] = new Piece(blank, new Point(1, 1), 0);
            spots[1, 2] = new Piece(blank, new Point(1, 2), 0);
            spots[1, 3] = new Piece(blank, new Point(1, 3), 0);
            spots[1, 4] = new Piece(blank, new Point(1, 4), 0);
            spots[1, 5] = new Piece(blank, new Point(1, 5), 0);
            spots[1, 6] = new Piece(blank, new Point(1, 6), 0);
            spots[1, 7] = new Piece(blank, new Point(1, 7), 0);
            #endregion
            #region row 3
            spots[2, 0] = new Piece(blank, new Point(2, 0), 0);
            spots[2, 1] = new Piece(blank, new Point(2, 1), 0);
            spots[2, 2] = new Piece(blank, new Point(2, 2), 0);
            spots[2, 3] = new Piece(blank, new Point(2, 3), 0);
            spots[2, 4] = new Piece(blank, new Point(2, 4), 0);
            spots[2, 5] = new Piece(blank, new Point(2, 5), 0);
            spots[2, 6] = new Piece(blank, new Point(2, 6), 0);
            spots[2, 7] = new Piece(blank, new Point(2, 7), 0);
            #endregion
            #region row 4
            spots[3, 0] = new Piece(blank, new Point(3, 0), 0);
            spots[3, 1] = new Piece(blank, new Point(3, 1), 0);
            spots[3, 2] = new Piece(blank, new Point(3, 2), 0);
            spots[3, 3] = new Piece(black, new Point(3, 3), -1);
            spots[3, 4] = new Piece(white, new Point(3, 4), 1);
            spots[3, 5] = new Piece(blank, new Point(3, 5), 0);
            spots[3, 6] = new Piece(blank, new Point(3, 6), 0);
            spots[3, 7] = new Piece(blank, new Point(3, 7), 0);
            #endregion
            #region row 5
            spots[4, 0] = new Piece(blank, new Point(4, 0), 0);
            spots[4, 1] = new Piece(blank, new Point(4, 1), 0);
            spots[4, 2] = new Piece(blank, new Point(4, 2), 0);
            spots[4, 3] = new Piece(white, new Point(4, 3), 1);
            spots[4, 4] = new Piece(black, new Point(4, 4), -1);
            spots[4, 5] = new Piece(blank, new Point(4, 5), 0);
            spots[4, 6] = new Piece(blank, new Point(4, 6), 0);
            spots[4, 7] = new Piece(blank, new Point(4, 7), 0);
            #endregion
            #region row 6
            spots[5, 0] = new Piece(blank, new Point(5, 0), 0);
            spots[5, 1] = new Piece(blank, new Point(5, 1), 0);
            spots[5, 2] = new Piece(blank, new Point(5, 2), 0);
            spots[5, 3] = new Piece(blank, new Point(5, 3), 0);
            spots[5, 4] = new Piece(blank, new Point(5, 4), 0);
            spots[5, 5] = new Piece(blank, new Point(5, 5), 0);
            spots[5, 6] = new Piece(blank, new Point(5, 6), 0);
            spots[5, 7] = new Piece(blank, new Point(5, 7), 0);
            #endregion
            #region row 7
            spots[6, 0] = new Piece(blank, new Point(6, 0), 0);
            spots[6, 1] = new Piece(blank, new Point(6, 1), 0);
            spots[6, 2] = new Piece(blank, new Point(6, 2), 0);
            spots[6, 3] = new Piece(blank, new Point(6, 3), 0);
            spots[6, 4] = new Piece(blank, new Point(6, 4), 0);
            spots[6, 5] = new Piece(blank, new Point(6, 5), 0);
            spots[6, 6] = new Piece(blank, new Point(6, 6), 0);
            spots[6, 7] = new Piece(blank, new Point(6, 7), 0);
            #endregion
            #region row 8
            spots[7, 0] = new Piece(blank, new Point(7, 0), 0);
            spots[7, 1] = new Piece(blank, new Point(7, 1), 0);
            spots[7, 2] = new Piece(blank, new Point(7, 2), 0);
            spots[7, 3] = new Piece(blank, new Point(7, 3), 0);
            spots[7, 4] = new Piece(blank, new Point(7, 4), 0);
            spots[7, 5] = new Piece(blank, new Point(7, 5), 0);
            spots[7, 6] = new Piece(blank, new Point(7, 6), 0);
            spots[7, 7] = new Piece(blank, new Point(7, 7), 0);
            #endregion
            #endregion
        }
        public Piece getp(Int32 x, Int32 y)
        {
            return spots[x, y];
        }
        public void setp(Int32 x, Int32 y, Int32 s)
        {
            if (s == -1)
            {
                spots[x, y].image = b;
            }
            else if (s == 0)
            {
                spots[x, y].image = bl;
            }
            else
            {
                spots[x, y].image = w;
            }
            spots[x, y].col = s;
        }
        

    }
}