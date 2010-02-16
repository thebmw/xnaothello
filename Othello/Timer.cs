using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Othello
{
    public class Timer
    {
        private DateTime end;
        /// <summary>
        /// Interval in Miliseconds
        /// </summary>
        private Int32 interval = 0;
        public Boolean running = false;
        public Timer()
        { 
        }
        public Timer(Int32 inter)
        {
            interval = inter;
        }
        public void Start()
        {
            
            DateTime n = DateTime.Now;
            end = DateTime.Now.AddMilliseconds(Double.Parse(interval.ToString()));
            running = true;
            
            //Tick("Timer", new EventArgs());

        }
        public void Check()
        {
            
            if (running)
            {
                if (DateTime.Now == end)
                {
                    Tick("Timer", new EventArgs());
                    running = false;
                }
            }
            
        }
        public event EventHandler<EventArgs> Tick;
    }
}