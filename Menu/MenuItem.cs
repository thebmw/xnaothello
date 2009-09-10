using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace Othello.Menu
{
    public class MenuItem
    {
        private String text;
        private Vector2 position;
        private Boolean selected;
        private Microsoft.Xna.Framework.Graphics.Color color;
        public String Text
        {
            get { return text; }
            set { text = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Boolean Selected
        {
            get { return selected; }
            set { selected = value; }
        }
        public Microsoft.Xna.Framework.Graphics.Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public MenuItem()
        {

        }
        public MenuItem(String _text, Vector2 _position, Boolean _selected, Microsoft.Xna.Framework.Graphics.Color _color)
        {
            text = _text;
            position = _position;
            selected = _selected;
            color = _color;
        }
    }
}