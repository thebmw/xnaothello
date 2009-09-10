using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Othello.Menu
{
    public class Menu : List<MenuItem>
    {
        public Menu()
        {

        }
        public Int32 selectedIndex
        {
            get
            {
                return this.FindIndex(Search);
            }
        }
        private bool Search(MenuItem item)
        {
            return item.Selected;
        }
        public void Up()
        {
            int i = selectedIndex;
            
            if (i != 0)
            {
                this[i].Selected = false;
                this[i].Color = Color.White;
                this[i - 1].Color = Color.Red;
                this[i - 1].Selected = true;
            }
        }
        public void Down()
        {
            int i = selectedIndex;
            if (i != this.Count - 1)
            {
                this[i].Selected = false;
                this[i].Color = Color.White;
                this[i + 1].Color = Color.Red;
                this[i + 1].Selected = true;
            }
        }
    }
}