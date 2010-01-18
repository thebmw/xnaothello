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

namespace Othello
{
    public class Sound
    {
        private SoundEffect soundeff;
        public SoundEffect Sound_Effect
        {
            get { return soundeff; }
            set { soundeff = value; }
        }
        private String name;
        public String Name
        {
            get { return name; }
            set { name = value; }
        }
        public Sound()
        {

        }
        public void Play()
        {
            try
            {

                soundeff.Play();
            }
            catch (Exception)
            {

            }
        }
        public Sound(SoundEffect sound, String iname)
        {
            soundeff = sound;
            name = iname;
        }
    }
    public class SoundManager : List<Sound>
    {
        private String searchterm;
        public Sound this[String Name]
        {
            get
            {
                searchterm = Name;
                return this.Find(Search);
            }
        }
        public void Add(SoundEffect se, String name)
        {
            this.Add(new Sound(se, name));
        }
        public bool Search(Sound sound)
        {
            if (sound.Name == searchterm)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}