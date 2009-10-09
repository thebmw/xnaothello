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
    enum MultiplayerRole
    {
        None,
        Server,
        Client
    }
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //System.Windows.Forms.Timer t = new System.Windows.Forms.Timer();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D black;
        Texture2D white;
        Texture2D blank;
        Texture2D sel;
        Texture2D selW;
        Texture2D selB;
        Texture2D selected;
        Texture2D pop;
        Texture2D StartScreen;
        Texture2D StartScreenHD;
        Texture2DColl tx2dcol;
        Int32 turn = -1;
        Point selectedscreen;
        Point selectedboard = new Point(3, 4);
        Int32 rise;
        Int32 run;
        SpriteFont font1;
        SpriteFont menufont;
        Mode mode = Mode.startscreen;
        Menu.Menu menu = new Othello.Menu.Menu();
        Menu.Menu wierless = new Othello.Menu.Menu();
        Menu.Menu gameselect = new Othello.Menu.Menu();
        Menu.Menu newgame = new Othello.Menu.Menu();
        Timer start = new Timer(6000);
        int sid = 0;
        Boolean startscreen = true;
        private int blackCount;
        private int whiteCount;
        private int emptyCount;
        private int blackFrontierCount;
        private int whiteFrontierCount;
        private int blackSafeCount;
        private int whiteSafeCount;
        Boolean isZuneHD
        {
            get
            {
                if (Program.HD)
                {
                    return true;
                }

                else if (new AccelerometerCapabilities().IsConnected)
                {
                    return true;
                }
                
                return false;
            }
        }
        bool isExistingGame = false;
        Timer t = new Timer(1000);
        MultiplayerRole multiplayerRole;
        GamerServicesComponent gs;
        NetworkSession networkSession;

        PacketWriter packetWriter;
        PacketReader packetReader;

        SoundManager SM = new SoundManager();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gs = new GamerServicesComponent(this);
            Components.Add(new GamerServicesComponent(this));
            // Frame rate is 30 fps by default for Zune.
            //t.Interval = 2000;
            //t.Tick += new EventHandler(t_Tick);
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
        }

        

        public enum Mode
        {
            start,
            single,
            multi,
            wireless,
            new_continue,
            waiting,
            gameselect,
            game,
            startscreen
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Program.Content = Content;
            //Othello.Help.Pro.Content = Content;
            packetWriter = new PacketWriter();
            packetReader = new PacketReader();
            // TODO: Add your initialization logic here
            setsel();
            t.Tick += new EventHandler<EventArgs>(t_Tick);
            start.Tick += new EventHandler<EventArgs>(start_Tick);
            
            menu.Add(new Othello.Menu.MenuItem("Single Player", new Vector2(5, 5), true, Color.Red));
            menu.Add(new Othello.Menu.MenuItem("Multiplayer", new Vector2(5, 35), false, Color.White));
            menu.Add(new Othello.Menu.MenuItem("Wireless", new Vector2(5, 65), false, Color.White));
            menu.Add(new Othello.Menu.MenuItem("Exit", new Vector2(5, 95), false, Color.White));
            wierless.Add(new Othello.Menu.MenuItem("Host", new Vector2(5, 5), true, Color.Red));
            wierless.Add(new Othello.Menu.MenuItem("Join", new Vector2(5, 35), false, Color.White));
            wierless.Add(new Othello.Menu.MenuItem("Back", new Vector2(5, 65), false, Color.White));
            newgame.Add(new Othello.Menu.MenuItem("New Game", new Vector2(5, 5), true, Color.Red));
            newgame.Add(new Othello.Menu.MenuItem("Continue Game", new Vector2(5, 35), false, Color.White));
            base.Initialize();
        }

        void start_Tick(object sender, EventArgs e)
        {
            startscreen = false;
            mode = Mode.start;
        }

        void t_Tick(object sender, EventArgs e)
        {
            MakeComputerMove();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if (!isZuneHD)
            {
                black = Content.Load<Texture2D>("Pieces\\Black");
                white = Content.Load<Texture2D>("Pieces\\White");
                blank = Content.Load<Texture2D>("Pieces\\Blank");
                sel = Content.Load<Texture2D>("Pieces\\Sblank");
                selB = Content.Load<Texture2D>("Pieces\\Sblack");
                selW = Content.Load<Texture2D>("Pieces\\Swhite");
            
            }
            else
            {
                black = Content.Load<Texture2D>("Pieces\\HD\\BlackPiece");
                white = Content.Load<Texture2D>("Pieces\\HD\\WhitePiece");
                blank = Content.Load<Texture2D>("Pieces\\HD\\Empty");
                sel = Content.Load<Texture2D>("Pieces\\HD\\Selected");
                selB = Content.Load<Texture2D>("Pieces\\HD\\SelW");
                selW = Content.Load<Texture2D>("Pieces\\HD\\SelB");
            }
           
            pop = Content.Load<Texture2D>("BlackOverlay");
            font1 = Content.Load<SpriteFont>("Arial");
            menufont = Content.Load<SpriteFont>("Menu");
            selected = selW;
            SM.Add(Content.Load<SoundEffect>("Sounds\\Ding"), "Ding");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Click"), "Click");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Error"), "Error");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Merr"), "Turn");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Mstart"), "Wifi Start");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Mstop"), "Wifi End");
            SM.Add(Content.Load<SoundEffect>("Sounds\\GameStart"), "Start");
            SM.Add(Content.Load<SoundEffect>("Sounds\\GameExit"), "Stop");
            SM.Add(Content.Load<SoundEffect>("Sounds\\ModeStart"), "Mode Start");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Turn"), "Change Turn");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Win"), "Win");
            SM.Add(Content.Load<SoundEffect>("Sounds\\Win"), "Lose");
            StartScreen = Content.Load<Texture2D>("StartScreen");
            StartScreenHD = Content.Load<Texture2D>("StartScreenHD");
            //Controls = Content.Load<Help>("Help\\Controls");
            SM["Start"].Play();
            // TODO: use this.Content to load your game content here
            tx2dcol = new Texture2DColl(black, white, blank);
            start.Start();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
           
        }
        Boolean up = false;
        Boolean down = false;
        Boolean left = false;
        Boolean right = false;
        Boolean click = false;
        Boolean back = false;
        Boolean startw = false;
        Boolean clickA = false;
        Mode Existing_game_mode = Mode.new_continue;
        Mode starting = Mode.new_continue;
        //Boolean startmulti = false;
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            TouchCollection curTouches = TouchPanel.GetState();
            

            GamePadState gps = GamePad.GetState(PlayerIndex.One);
            // Allows the game to exit
            Vector2 loca = new Vector2();
            bool back = false;
            #region Multi
            if (mode == Mode.multi)
            {
                foreach (TouchLocation location in curTouches)
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:
                            loca = location.Position;
                            //Start tracking a particular touch location
                            //In this example, you start the stroke from a special
                            //area of the screen.
                            if (location.Position.X < 272 && location.Position.Y < 272)
                            {

                                int x = int.Parse(Math.Round(decimal.Parse((location.Position.X / 34).ToString()), 0).ToString());
                                int y = int.Parse(Math.Round(decimal.Parse((location.Position.Y / 34).ToString()), 0).ToString());
                                if (IsValidMove(turn, x, y))
                                {
                                    MakeMove(turn, x, y);
                                    changeturn();
                                }
                            }
                            break;
                        case TouchLocationState.Moved:
                            
                            

                            break;
                    }
                }
                if (!up && gps.IsButtonDown(Buttons.DPadUp))
                {
                    movesel_up();
                    up = true;
                }
                if (up && gps.IsButtonUp(Buttons.DPadUp))
                {
                    up = false;
                }
                if (!down && gps.IsButtonDown(Buttons.DPadDown))
                {
                    movesel_down();
                    down = true;
                }
                if (down && gps.IsButtonUp(Buttons.DPadDown))
                {
                    down = false;
                }
                if (!left && gps.IsButtonDown(Buttons.DPadLeft))
                {
                    movesel_left();
                    left = true;
                }
                if (left && gps.IsButtonUp(Buttons.DPadLeft))
                {
                    left = false;
                }
                if (!right && gps.IsButtonDown(Buttons.DPadRight))
                {
                    movesel_right();
                    right = true;
                }
                if (right && gps.IsButtonUp(Buttons.DPadRight))
                {
                    right = false;
                }
                if (!click && gps.IsButtonDown(Buttons.B))
                {
                    click = true;

                    if (IsValidMove(turn, selectedboard.X, selectedboard.Y))
                    {
                        //tx2dcol.setp(selectedboard.X, selectedboard.Y, turn);
                        MakeMove(turn, selectedboard.X, selectedboard.Y);

                        //Flip(turn, selectedboard.X, selectedboard.Y, rise, run);

                        System.Diagnostics.Debug.WriteLine("Placed Piece");
                        changeturn();
                    }
                    else
                    {
                        SM["Error"].Play();
                    }

                }
                if (click && gps.IsButtonUp(Buttons.B))
                {
                    click = false;
                }
                // TODO: Add your update logic here
                try
                {
                    if (tx2dcol.spots[selectedboard.X, selectedboard.Y].col == -1)
                    {
                        selected = selB;
                    }
                    else if (tx2dcol.spots[selectedboard.X, selectedboard.Y].col == 1)
                    {
                        selected = selW;
                    }
                    else
                    {
                        selected = sel;
                    }
                }
                catch (Exception)
                {
                    selected = sel;
                }
                if (!back && gps.IsButtonDown(Buttons.Back))
                {
                    mode = Mode.start;
                    if (gameover)
                    {
                        //tx2dcol = null;
                        //tx2dcol = new Texture2DColl(black, white, blank);
                       
                        tx2dcol.reset();
                        gameover = false;
                    }
                    back = true;
                }
                if (back && gps.IsButtonUp(Buttons.Back))
                {
                    back = false;

                }
                count();
            }
            #endregion
            #region Start
            if (mode == Mode.start)
            {
                foreach (TouchLocation location in curTouches)
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:
                            //Start tracking a particular touch location
                            //In this example, you start the stroke from a special
                            //area of the screen.
                            if ((location.Position.Y - 5) < 30 && (location.Position.Y - 5) > 0)
                            {
                                menu.selectedIndex = 0;
                                mode = Mode.single;
                            }
                            else if ((location.Position.Y - 5) < 60 && (location.Position.Y - 5) > 30)
                            {
                                menu.selectedIndex = 1;
                                mode = Mode.multi;
                            }
                            else if ((location.Position.Y - 5) < 90 && (location.Position.Y - 5) > 60)
                            {
                                menu.selectedIndex = 2;
                                mode = Mode.game;
                            }
                            else if ((location.Position.Y - 5) < 120 && (location.Position.Y - 5) > 90)
                            {
                                menu.selectedIndex = 3;
                                Exit();
                            }
                            break;
                        case TouchLocationState.Released:



                            break;
                    }
                }
                if (!clickA && gps.IsButtonDown(Buttons.A))
                {
                    if (menu.selectedIndex == 0)
                    {
                        mode = Mode.single;
                    }
                    else if (menu.selectedIndex == 1)
                    {
                        mode = Mode.multi;
                    }
                    else if (menu.selectedIndex == 2)
                    {
                        mode = Mode.game;
                        //Client();
                    }
                    else
                    {
                        Exit();
                    }
                    //mode = Mode.multi;
                    SM["Mode Start"].Play();
                    clickA = true;
                }
                if (clickA && gps.IsButtonUp(Buttons.A))
                {
                    clickA = false;
                }
                if (!back && gps.IsButtonDown(Buttons.Back))
                {
                    //SM["Stop"].Play();
                    //Exit();
                }
                if (back && gps.IsButtonUp(Buttons.Back))
                {
                    back = false;
                }
                if (!startw && gps.IsButtonDown(Buttons.B))
                {
                    //SM["Mode Start"].Play();
                    //mode = Mode.waiting;
                    //Client();
                    //startw = true;
                    click = true;
                }
                if (!up && gps.IsButtonDown(Buttons.DPadUp))
                {
                    SM["Click"].Play();
                    menu.Up();
                    up = true;
                }
                if (up && gps.IsButtonUp(Buttons.DPadUp))
                {
                    up = false;
                }
                if (!down && gps.IsButtonDown(Buttons.DPadDown))
                {
                    SM["Click"].Play();
                    menu.Down();
                    down = true;
                }
                if (down && gps.IsButtonUp(Buttons.DPadDown))
                {
                    down = false;
                }
            }
            #endregion
            #region Wireless
            if (mode == Mode.wireless)
            {
                foreach (TouchLocation location in curTouches)
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:
                            //Start tracking a particular touch location
                            //In this example, you start the stroke from a special
                            //area of the screen.
                            if (location.Position.X < 272 && location.Position.Y < 272)
                            {
                                int x = int.Parse(Math.Round(decimal.Parse((location.Position.X / 34).ToString()), 0).ToString());
                                int y = int.Parse(Math.Round(decimal.Parse((location.Position.Y / 34).ToString()), 0).ToString());
                                if (IsValidMove(turn, x, y))
                                {
                                    MakeMove(turn, x, y);
                                    sendMessage(new Point(x, y));
                                    changeturn();
                                }
                            }
                            break;
                        case TouchLocationState.Released:



                            break;
                    }
                }
                if (this.networkSession != null)
                {
                    this.networkSession.Update();
                }

                if (networkSession != null)
                    recieveMessage();

                if (!up && gps.IsButtonDown(Buttons.DPadUp))
                {
                    movesel_up();
                    up = true;
                }
                if (up && gps.IsButtonUp(Buttons.DPadUp))
                {
                    up = false;
                }
                if (!down && gps.IsButtonDown(Buttons.DPadDown))
                {
                    movesel_down();
                    down = true;
                }
                if (down && gps.IsButtonUp(Buttons.DPadDown))
                {
                    down = false;
                }
                if (!left && gps.IsButtonDown(Buttons.DPadLeft))
                {
                    movesel_left();
                    left = true;
                }
                if (left && gps.IsButtonUp(Buttons.DPadLeft))
                {
                    left = false;
                }
                if (!right && gps.IsButtonDown(Buttons.DPadRight))
                {
                    movesel_right();
                    right = true;
                }
                if (right && gps.IsButtonUp(Buttons.DPadRight))
                {
                    right = false;
                }
                if (!click && gps.IsButtonDown(Buttons.B))
                {
                    click = true;
                    if (multiplayerRole == MultiplayerRole.Server)
                    {
                        if (turn == -1)
                        {
                            if (IsValidMove(turn, selectedboard.X, selectedboard.Y))
                            {

                                //tx2dcol.setp(selectedboard.X, selectedboard.Y, turn);
                                MakeMove(turn, selectedboard.X, selectedboard.Y);
                                sendMessage(selectedboard);
                                //Flip(turn, selectedboard.X, selectedboard.Y, rise, run);

                                System.Diagnostics.Debug.WriteLine("Placed Piece");

                                changeturn();


                            }
                            else
                            {
                                SM["Error"].Play();
                            }
                        }
                    }
                    else
                    {
                        if (turn == 1)
                        {
                            if (IsValidMove(turn, selectedboard.X, selectedboard.Y))
                            {

                                //tx2dcol.setp(selectedboard.X, selectedboard.Y, turn);
                                MakeMove(turn, selectedboard.X, selectedboard.Y);
                                sendMessage(selectedboard);
                                //Flip(turn, selectedboard.X, selectedboard.Y, rise, run);

                                System.Diagnostics.Debug.WriteLine("Placed Piece");

                                changeturn();


                            }
                            else
                            {
                                SM["Error"].Play();
                            }
                        }
                    }

                }
                if (click && gps.IsButtonUp(Buttons.B))
                {
                    click = false;

                }
                if (startw && gps.IsButtonUp(Buttons.B))
                {
                    startw = false;
                }
                // TODO: Add your update logic here
                try
                {
                    if (tx2dcol.spots[selectedboard.X, selectedboard.Y].col == -1)
                    {
                        selected = selB;
                    }
                    else if (tx2dcol.spots[selectedboard.X, selectedboard.Y].col == 1)
                    {
                        selected = selW;
                    }
                    else
                    {
                        selected = sel;
                    }
                }
                catch (Exception)
                {
                    selected = sel;
                }
                if (!back && gps.IsButtonDown(Buttons.Back))
                {
                    mode = Mode.start;
                    if (gameover)
                    {
                        //tx2dcol = new Texture2DColl(black, white, blank);
                        tx2dcol.reset();
                        gameover = false;
                    }
                    back = true;
                }
                if (back && gps.IsButtonUp(Buttons.Back))
                {
                    back = false;

                }
                count();
            }
            #endregion
            #region Single
            if (mode == Mode.single)
            {
                foreach (TouchLocation location in curTouches)
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:
                            //Start tracking a particular touch location
                            //In this example, you start the stroke from a special
                            //area of the screen.
                            if (location.Position.X < 272 && location.Position.Y < 272)
                            {
                                int x = int.Parse(Math.Round(decimal.Parse((location.Position.X / 34).ToString()), 0).ToString());
                                int y = int.Parse(Math.Round(decimal.Parse((location.Position.Y / 34).ToString()), 0).ToString());
                                if (IsValidMove(turn, x, y))
                                {
                                    MakeMove(turn, x, y);
                                    changeturn();
                                }
                            }
                            break;
                        case TouchLocationState.Released:



                            break;
                    }
                }
                if (gps.IsButtonDown(Buttons.A) && gps.IsButtonDown(Buttons.B))
                {
                    Win(-1);
                }
                if (!up && gps.IsButtonDown(Buttons.DPadUp))
                {
                    movesel_up();
                    up = true;
                }
                if (up && gps.IsButtonUp(Buttons.DPadUp))
                {
                    up = false;
                }
                if (!down && gps.IsButtonDown(Buttons.DPadDown))
                {
                    movesel_down();
                    down = true;
                }
                if (down && gps.IsButtonUp(Buttons.DPadDown))
                {
                    down = false;
                }
                if (!left && gps.IsButtonDown(Buttons.DPadLeft))
                {
                    movesel_left();
                    left = true;
                }
                if (left && gps.IsButtonUp(Buttons.DPadLeft))
                {
                    left = false;
                }
                if (!right && gps.IsButtonDown(Buttons.DPadRight))
                {
                    movesel_right();
                    right = true;
                }
                if (right && gps.IsButtonUp(Buttons.DPadRight))
                {
                    right = false;
                }
                if (!click && gps.IsButtonDown(Buttons.B))
                {
                    click = true;
                    if (turn == -1)
                    {
                        if (IsValidMove(turn, selectedboard.X, selectedboard.Y))
                        {
                            Vector2 vep = vec2frompoint(selectedboard);
                            //tx2dcol.setp(selectedboard.X, selectedboard.Y, turn);
                            MakeMove(turn, selectedboard.X, selectedboard.Y);
                            AudioListener al = new AudioListener();
                            al.Position = new Vector3(0, 0, 0);
                            AudioEmitter ae = new AudioEmitter();
                            ae.Position = new Vector3(vep.X - 4, vep.Y - 4, 0);

                            //SM["Ding"].Sound_Effect.Play3D(al, ae, 1.0f, 0.0f, false);
                            SM["Ding"].Play();
                            
                            //Flip(turn, selectedboard.X, selectedboard.Y, rise, run);

                            System.Diagnostics.Debug.WriteLine("Placed Piece");
                            changeturn();
                        }
                        else
                        {
                            SM["Error"].Play();
                        }

                    }

                }
                if (click && gps.IsButtonUp(Buttons.B))
                {
                    click = false;
                }
                // TODO: Add your update logic here
                try
                {
                    if (tx2dcol.spots[selectedboard.X, selectedboard.Y].col == -1)
                    {
                        selected = selB;
                    }
                    else if (tx2dcol.spots[selectedboard.X, selectedboard.Y].col == 1)
                    {
                        selected = selW;
                    }
                    else
                    {
                        selected = sel;
                    }
                }
                catch (Exception)
                {
                    selected = sel;
                }
                if (!back && gps.IsButtonDown(Buttons.Back))
                {
                    mode = Mode.start;
                    back = true;
                    if (gameover)
                    {
                        gameover = false;
                        //tx2dcol = null;
                        //tx2dcol = new Texture2DColl(black, white, blank);
                        try
                        {
                            tx2dcol.reset();
                        }
                        catch (Exception e)
                        {

                        }
                        
                    }
                    
                }
                if (back && gps.IsButtonUp(Buttons.Back))
                {
                    back = false;

                }
                count();
            }
            #endregion
            #region Menus
            #region New Continue
            if (mode == Mode.new_continue)
            {

                if (!clickA && gps.IsButtonDown(Buttons.A))
                {
                    if (newgame.selectedIndex == 0)
                    {
                        tx2dcol = new Texture2DColl(black, white, blank);
                    }
                    mode = starting;
                    isExistingGame = false;
                    clickA = true;
                }
                if (clickA && gps.IsButtonUp(Buttons.A))
                {
                    clickA = false;
                }
                if (!click && gps.IsButtonDown(Buttons.B))
                {

                    click = true;
                }
            }
            #endregion
            #region Waiting
            if (mode == Mode.waiting)
            {
                if (this.networkSession != null)
                {
                    this.networkSession.Update();
                }
                if (!back && gps.IsButtonDown(Buttons.Back))
                {
                    mode = Mode.start;

                    back = true;
                }
                if (back && gps.IsButtonUp(Buttons.Back))
                {
                    back = false;

                }
            }
            #endregion
            #endregion
            #region gameselect
            {
                if (mode == Mode.gameselect)
                {
                    bool moved = false;
                    foreach (TouchLocation location in curTouches)
                    {
                       
                        switch (location.State)
                        {
                            case TouchLocationState.Moved:
                                //Start tracking a particular touch location
                                //In this example, you start the stroke from a special
                                //area of the screen.
                                moved = true;
                                break;
                            case TouchLocationState.Released:
                                if (!moved)
                                {
                                    gameselect.selectedIndex = int.Parse(Math.Round(decimal.Parse(((location.Position.Y - 5) / 30).ToString())).ToString());
                                    Connect(gameselect.selectedIndex);
                                }

                                break;
                        }
                    }
                    if (!clickA && gps.IsButtonDown(Buttons.A))
                    {
                        Connect(gameselect.selectedIndex);
                        
                        //mode = Mode.multi;
                        SM["Mode Start"].Play();
                        clickA = true;
                    }
                    if (clickA && gps.IsButtonUp(Buttons.A))
                    {
                        clickA = false;
                    }
                    if (!up && gps.IsButtonDown(Buttons.DPadUp))
                    {
                        SM["Click"].Play();
                        gameselect.Up();
                        up = true;
                    }
                    if (up && gps.IsButtonUp(Buttons.DPadUp))
                    {
                        up = false;
                    }
                    if (!down && gps.IsButtonDown(Buttons.DPadDown))
                    {
                        SM["Click"].Play();
                        gameselect.Down();
                        down = true;
                    }
                    if (down && gps.IsButtonUp(Buttons.DPadDown))
                    {
                        down = false;
                    }
                }
            }
            #endregion
            #region Host Or Join
            {
                if (mode == Mode.game)
                {
                    if (!clickA && gps.IsButtonDown(Buttons.A))
                    {
                        if (wierless.selectedIndex == 0)
                        {
                            mode = Mode.waiting;
                            createSession();
                            
                        }
                        else if (wierless.selectedIndex == 1)
                        {
                            Client();
                        }
                        else if (wierless.selectedIndex == 2)
                        {
                            mode = Mode.start;
                        }
                        //mode = Mode.multi;
                        SM["Mode Start"].Play();
                        clickA = true;
                    }
                    if (clickA && gps.IsButtonUp(Buttons.A))
                    {
                        clickA = false;
                    }
                    if (!up && gps.IsButtonDown(Buttons.DPadUp))
                    {
                        SM["Click"].Play();
                        wierless.Up();
                        up = true;
                    }
                    if (up && gps.IsButtonUp(Buttons.DPadUp))
                    {
                        up = false;
                    }
                    if (!down && gps.IsButtonDown(Buttons.DPadDown))
                    {
                        SM["Click"].Play();
                        wierless.Down();
                        down = true;
                    }
                    if (down && gps.IsButtonUp(Buttons.DPadDown))
                    {
                        down = false;
                    }
                }
            }
            #endregion
            t.Check();
            start.Check();
            if (PowerStatus.BatteryChargeStatus == BatteryChargeStatus.Critical)
            {
                this.Exit();
            }
            base.Update(gameTime);
        }
       
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            spriteBatch.Begin();
            if (startscreen)
            {
                GraphicsDevice.Clear(Color.Black);
                spriteBatch.Draw(isZuneHD ? StartScreenHD : StartScreen, new Vector2(0, 0), Color.White);
            }
            else
            {
                GraphicsDevice.Clear(Color.Green);
            }
            #region Multi
            if (mode == Mode.multi)
            {
                for (Int32 x = 0; x < 8; x++)
                {
                    for (Int32 y = 0; y < 8; y++)
                    {
                        spriteBatch.Draw(tx2dcol.spots[x, y].image, vec2frompoint(tx2dcol.spots[x, y].screenplace), Color.White);
                    }

                }
                spriteBatch.Draw(selected, vec2frompoint(selectedscreen), Color.White);
                String[] line = new String[4];
                Color col = Color.White;
                if (turn == -1)
                {
                    line[0] = "Black";
                    col = Color.Black;
                }
                else
                {
                    line[0] = "White";
                    col = Color.White;
                }
                if (emptyCount == 0 || !HasAnyValidMove(1) && !HasAnyValidMove(-1))
                {
                    if (blackCount > whiteCount)
                    {
                        Vector2 timePosition = font1.MeasureString("Black Wins");
                        spriteBatch.DrawString(font1, "Black Wins", timePosition, Color.Black);
                    }
                    else
                    {
                        Vector2 timePosition = font1.MeasureString("White Wins");
                        spriteBatch.DrawString(font1, "White Wins", timePosition, Color.White);
                    }
                }
                line[1] = "Empty: " + emptyCount.ToString();
                line[2] = "Black: " + blackCount.ToString();
                line[3] = "White: " + whiteCount.ToString();
                if (isZuneHD)
                {
                    spriteBatch.DrawString(font1, line[0], vec2frompoint(new Point(0, 270)), col);
                    spriteBatch.DrawString(font1, line[1], vec2frompoint(new Point(120, 270)), Color.Green);
                    spriteBatch.DrawString(font1, line[2], vec2frompoint(new Point(0, 300)), Color.Black);
                    spriteBatch.DrawString(font1, line[3], vec2frompoint(new Point(120, 300)), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font1, line[0], vec2frompoint(new Point(0, 240)), col);
                    spriteBatch.DrawString(font1, line[1], vec2frompoint(new Point(120, 240)), Color.Green);
                    spriteBatch.DrawString(font1, line[2], vec2frompoint(new Point(0, 270)), Color.Black);
                    spriteBatch.DrawString(font1, line[3], vec2frompoint(new Point(120, 270)), Color.White);
                }
            }
            #endregion
            #region Start
            if (mode == Mode.start)
            {
                /*
                spriteBatch.DrawString(font1, "Press A to start Multi", vec2frompoint(new Point(5, 5)), Color.White);
                spriteBatch.DrawString(font1, "Press B to Start Wireless", vec2frompoint(new Point(5, 35)), Color.White);
                spriteBatch.DrawString(font1, "Press up to Start Single", vec2frompoint(new Point(5, 65)), Color.White);
                spriteBatch.DrawString(font1, "Press Back to Exit", vec2frompoint(new Point(5, 95)), Color.White);
                */
                foreach (Menu.MenuItem item in menu)
                {
                    spriteBatch.DrawString(menufont, item.Text, item.Position, item.Color);
                }
                spriteBatch.Draw(Content.Load<Texture2D>("GameThumbnail"), new Vector2(5, 125), Color.White);
                spriteBatch.DrawString(menufont, String.Format("Battery: {0}%", PowerStatus.BatteryLifePercent.ToString()), vec2frompoint(new Point(5, 290)), Color.White);
            }
            #endregion
            #region Wireless
            if (mode == Mode.wireless)
            {
                for (Int32 x = 0; x < 8; x++)
                {
                    for (Int32 y = 0; y < 8; y++)
                    {
                        spriteBatch.Draw(tx2dcol.spots[x, y].image, vec2frompoint(tx2dcol.spots[x, y].screenplace), Color.White);
                    }

                }
                spriteBatch.Draw(selected, vec2frompoint(selectedscreen), Color.White);
                String[] line = new String[4];
                Color col = Color.White;
                if (turn == -1)
                {
                    line[0] = "Black";
                    col = Color.Black;
                }
                else
                {
                    line[0] = "White";
                    col = Color.White;
                }
                if (emptyCount == 0 || !HasAnyValidMove(1) && !HasAnyValidMove(-1))
                {
                    if (blackCount > whiteCount)
                    {
                        Vector2 timePosition = font1.MeasureString("Black Wins");
                        Win(-1);
                        spriteBatch.DrawString(font1, "Black Wins", timePosition, Color.Black);
                    }
                    else
                    {
                        Vector2 timePosition = font1.MeasureString("White Wins");
                        Win(1);
                        spriteBatch.DrawString(font1, "White Wins", timePosition, Color.White);
                    }
                }
                line[1] = "";
                line[2] = "Black: " + blackCount.ToString();
                line[3] = "White: " + whiteCount.ToString();
                if (isZuneHD)
                {
                    spriteBatch.DrawString(font1, line[0], vec2frompoint(new Point(0, 270)), col);
                    spriteBatch.DrawString(font1, line[1], vec2frompoint(new Point(120, 270)), Color.Red);
                    spriteBatch.DrawString(font1, line[2], vec2frompoint(new Point(0, 300)), Color.Black);
                    spriteBatch.DrawString(font1, line[3], vec2frompoint(new Point(120, 300)), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font1, line[0], vec2frompoint(new Point(0, 240)), col);
                    spriteBatch.DrawString(font1, line[1], vec2frompoint(new Point(120, 240)), Color.Red);
                    spriteBatch.DrawString(font1, line[2], vec2frompoint(new Point(0, 270)), Color.Black);
                    spriteBatch.DrawString(font1, line[3], vec2frompoint(new Point(120, 270)), Color.White);
                }
                if (networkSession != null && networkSession.RemoteGamers.Count != 0)
                {
                    spriteBatch.DrawString(font1, networkSession.RemoteGamers[0].Gamertag, vec2frompoint(new Point(0, 300)), Color.White);
                }
                if (multiplayerRole == MultiplayerRole.Client)
                {
                    spriteBatch.DrawString(font1, "Client", vec2frompoint(new Point(120, 300)), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font1, "Server", vec2frompoint(new Point(120, 300)), Color.Black);
                }
            }
            #endregion
            #region Single
            if (mode == Mode.single)
            {
                for (Int32 x = 0; x < 8; x++)
                {
                    for (Int32 y = 0; y < 8; y++)
                    {
                        spriteBatch.Draw(tx2dcol.spots[x, y].image, vec2frompoint(tx2dcol.spots[x, y].screenplace), Color.White);
                    }

                }
                spriteBatch.Draw(selected, vec2frompoint(selectedscreen), Color.White);
                String[] line = new String[4];
                Color col = Color.White;
                if (turn == -1)
                {
                    line[0] = "Black";
                    col = Color.Black;
                }
                else
                {
                    line[0] = "White";
                    col = Color.White;
                }
                if (emptyCount == 0 || !HasAnyValidMove(1) && !HasAnyValidMove(-1))
                {
                    if (blackCount > whiteCount)
                    {
                        spriteBatch.Draw(pop, new Vector2(0, 0), transparent(200));
                        Vector2 timePosition = font1.MeasureString("Black Wins");
                        //Win(1);
                        spriteBatch.DrawString(font1, "Black Wins", new Vector2(30, 120), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(pop, new Vector2(0, 0), transparent(200));
                        Vector2 timePosition = font1.MeasureString("White Wins");
                        //Win(1);
                        spriteBatch.DrawString(font1, "White Wins", new Vector2(30, 120), Color.White);
                    }
                }
                line[1] = "Empty: " + emptyCount.ToString();
                line[2] = "Black: " + blackCount.ToString();
                line[3] = "White: " + whiteCount.ToString();
                if (isZuneHD)
                {
                    spriteBatch.DrawString(font1, line[0], vec2frompoint(new Point(0, 270)), col);
                    //spriteBatch.DrawString(font1, line[1], vec2frompoint(new Point(120, 240)), Color.Green);
                    spriteBatch.DrawString(font1, line[2], vec2frompoint(new Point(0, 300)), Color.Black);
                    spriteBatch.DrawString(font1, line[3], vec2frompoint(new Point(120, 300)), Color.White);
                }
                else
                {

                    spriteBatch.DrawString(font1, line[0], vec2frompoint(new Point(0, 240)), col);
                    //spriteBatch.DrawString(font1, line[1], vec2frompoint(new Point(120, 240)), Color.Green);
                    spriteBatch.DrawString(font1, line[2], vec2frompoint(new Point(0, 270)), Color.Black);
                    spriteBatch.DrawString(font1, line[3], vec2frompoint(new Point(120, 270)), Color.White);
                }
            }
            #endregion
            #region Waiting
            if (mode == Mode.waiting)
            {
                for (Int32 x = 0; x < 8; x++)
                {
                    for (Int32 y = 0; y < 8; y++)
                    {
                        spriteBatch.Draw(tx2dcol.spots[x, y].image, vec2frompoint(tx2dcol.spots[x, y].screenplace), Color.White);
                    }

                }
                spriteBatch.Draw(selected, vec2frompoint(selectedscreen), Color.White);
                String[] line = new String[4];
                Color col = Color.White;
                if (turn == -1)
                {
                    line[0] = "Black";
                    col = Color.Black;
                }
                else
                {
                    line[0] = "White";
                    col = Color.White;
                }
                if (emptyCount == 0 || !HasAnyValidMove(1) && !HasAnyValidMove(-1))
                {
                    if (blackCount > whiteCount)
                    {
                        Vector2 timePosition = font1.MeasureString("Black Wins");
                        Win(-1);
                        spriteBatch.DrawString(font1, "Black Wins", timePosition, Color.Black);
                    }
                    else
                    {
                        Vector2 timePosition = font1.MeasureString("White Wins");
                        Win(1);
                        spriteBatch.DrawString(font1, "White Wins", timePosition, Color.White);
                    }
                }
                line[1] = "Empty: " + emptyCount.ToString();
                line[2] = "Black: " + blackCount.ToString();
                line[3] = "White: " + whiteCount.ToString();
                spriteBatch.DrawString(font1, line[0], vec2frompoint(new Point(0, 240)), col);
                spriteBatch.DrawString(font1, line[1], vec2frompoint(new Point(120, 240)), Color.Green);
                spriteBatch.DrawString(font1, line[2], vec2frompoint(new Point(0, 270)), Color.Black);
                spriteBatch.DrawString(font1, line[3], vec2frompoint(new Point(120, 270)), Color.White);
                if (networkSession != null && networkSession.RemoteGamers.Count != 0)
                {
                    spriteBatch.DrawString(font1, networkSession.RemoteGamers[0].Gamertag, vec2frompoint(new Point(0, 300)), Color.White);
                }
                if (multiplayerRole == MultiplayerRole.Client)
                {
                    spriteBatch.DrawString(font1, "Client", vec2frompoint(new Point(120, 300)), Color.White);
                }
                else
                {
                    spriteBatch.DrawString(font1, "Server", vec2frompoint(new Point(120, 300)), Color.Black);
                }
                spriteBatch.Draw(pop, new Vector2(0, 0), transparent(200));
                spriteBatch.DrawString(font1, multiplayerRole == MultiplayerRole.Server ? "Waiting for player\nto join." : "Looking for games\nto join.", new Vector2(30, 160), Color.Black);
                spriteBatch.DrawString(font1, String.Format("Current Mode: {0}", multiplayerRole == MultiplayerRole.Server ? "Server" : "Client"), new Vector2(30, 130), Color.Black);
            }
            #endregion
            #region gameselect
            if (mode == Mode.gameselect)
            {
                foreach (Menu.MenuItem item in gameselect)
                {
                    spriteBatch.DrawString(menufont, item.Text, item.Position, item.Color);
                }
            }
            #endregion
            #region Host or Join
            if (mode == Mode.game)
            {
                foreach (Menu.MenuItem item in wierless)
                {
                    spriteBatch.DrawString(menufont, item.Text, item.Position, item.Color);
                }
            }
            #endregion
            if (!this.IsActive)
            {
                spriteBatch.Draw(pop, new Vector2(0, 0), transparent(200));
                spriteBatch.DrawString(font1, "The hold switch is on.", new Vector2(30, 120), Color.Black);
                
            }
            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
        public Color transparent(byte persent)
        {
            Color c = new Color();
            c.A = persent;
            c.B = 255;
            c.G = 255;
            c.R = 255;
            return c;
        }
        #region Move code
        public Vector2 vec2frompoint(Point point)
        {
            return new Vector2(float.Parse(point.X.ToString()), float.Parse(point.Y.ToString()));
        }
        public bool winstart = false;
        bool gameover = false;
        public void Win(int col)
        {
            gameover = true;
            if (!winstart)
            {
                winstart = true;
                if (mode == Mode.single)
                {
                    
                    try
                    {
                        
                        SM[col == -1 ? "Win" : "Lose"].Play();
                    }
                    catch(Exception)
                    {

                    }
                }
                if (mode == Mode.wireless)
                {
                    if (multiplayerRole == MultiplayerRole.Server)
                    {
                        SM[col == -1 ? "Win" : "Lose"].Play();
                    }
                    else
                    {
                        SM[col == 1 ? "Win" : "Lose"].Play();
                    }
                }
            }
        }

        //
        // Determines if the player can make any valid move on the board.
        //
        public bool HasAnyValidMove(Int32 color)
        {
            // Check all board positions for a valid move.
            int r, c;
            for (r = 0; r < 8; r++)
                for (c = 0; c < 8; c++)
                    if (this.IsValidMove(color, r, c))
                        return true;

            // None found.
            return false;
        }

        //
        // Determines if a specific move is valid for the player.
        //
        public bool IsValidMove(Int32 color, int row, int col)
        {
            // The square must be empty.
            if (tx2dcol.spots[row, col].col != 0)
                return false;

            // Must be able to flip at least one opponent disc.
            int dr, dc;
            for (dr = -1; dr <= 1; dr++)
                for (dc = -1; dc <= 1; dc++)
                    if (!(dr == 0 && dc == 0) && this.IsOutflanking(color, row, col, dr, dc))
                    {
                        rise = dr;
                        run = dc;
                        return true;
                    }
            
            // No opponents could be flipped.
            
            return false;
        }

        private bool IsOutflanking(Int32 color, int row, int col, int dr, int dc)
        {
            // Move in the given direction as long as we stay on the board and
            // land on a disc of the opposite color.
            int r = row + dr;
            int c = col + dc;
            while (r >= 0 && r < 8 && c >= 0 && c < 8 && tx2dcol.spots[r, c].col == -color)
            {
                
                
                    r += dr;
                    c += dc;
               
            }

            // If we ran off the board, only moved one space or didn't land on
            // a disc of the same color, return false.
            if (r < 0 || r > 7 || c < 0 || c > 7 || (r - dr == row && c - dc == col) || tx2dcol.spots[r, c].col != color)
                return false;

            // Otherwise, return true;
            return true;
        }

        private void Flip(Int32 color, int row, int col, int dr, int dc)
        {
            // Move in the given direction as long as we stay on the board and
            // land on a disc of the opposite color.
            int r = row + dr;
            int c = col + dc;
            while (r >= 0 && r < 8 && c >= 0 && c < 8 && tx2dcol.spots[r, c].col == -color)
            {

                tx2dcol.setp(r, c, turn);
                r += dr;
                c += dc;

            }

            
        }

        public void movesel_left()
        {
            if (selectedboard.X > 0 && selectedboard.X < 8)
            {
                selectedboard.X = selectedboard.X - 1;
                SM["Click"].Play();
                
                setsel();
            }
        }
        public void movesel_right()
        {
            if (selectedboard.X > -1 && selectedboard.X < 7)
            {
                selectedboard.X = selectedboard.X + 1;
                SM["Click"].Play();
                setsel();
            }
        }
        public void movesel_up()
        {
            if (selectedboard.Y > 0 && selectedboard.Y < 8)
            {
                selectedboard.Y = selectedboard.Y - 1;
                SM["Click"].Play();
                setsel();
            }
        }
        public void movesel_down()
        {
            if (selectedboard.Y > -1 && selectedboard.Y < 7)
            {
                selectedboard.Y = selectedboard.Y + 1;
                SM["Click"].Play();
                setsel();
            }
        }
        public void setsel()
        {
            selectedscreen.X = selectedboard.X * (isZuneHD ? 34 : 30);
            selectedscreen.Y = selectedboard.Y * (isZuneHD ? 34 : 30);
        }

        public void MakeMove(int color, int row, int col)
        {
            // Set the disc on the square.
            //tx2dcol.spots[row, col].col = color;
            tx2dcol.setp(row, col, color);
            // Flip any flanked opponents.
            int dr, dc;
            int r, c;
            for (dr = -1; dr <= 1; dr++)
                for (dc = -1; dc <= 1; dc++)
                    // Are there any outflanked opponents?
                    if (!(dr == 0 && dc == 0) && IsOutflanking(color, row, col, dr, dc))
                    {
                        Log("run 1");
                        r = row + dr;
                        c = col + dc;
                        // Flip 'em.
                        int change = 0;
                        while (tx2dcol.spots[r, c].col == -color)
                        {
                            change++;
                            tx2dcol.setp(r, c, color);
                            r += dr;
                            c += dc;
                        }
                        Log("Changed " + change.ToString() + " pieces");
                        Log(String.Format("{0} layed a piece at {1}, {2}.", color == -1 ? "Black" : "White", row, color));
                        /*
                        int i, j;
                        for (i = 0; i < 8; i++)
                            for (j = 0; j < 8; j++)
                            {
                                // Mark the newly added disc.
                                if (i == row && j == col)
                                    tx2dcol.setp(i, j, turn);                    
                            }
                        // Update the counts.
                        //this.UpdateCounts();
                         * */
                    }

        }
        public void count()
        {
            whiteCount = 0;
            blackCount = 0;
            emptyCount = 0;
            int r, c;
            for (r = 0; r < 8; r++)
                for (c = 0; c < 8; c++)
                    if (tx2dcol.spots[r, c].col == 1)
                    {
                        whiteCount++;
                    }
                    else if (tx2dcol.spots[r, c].col == -1)
                    {
                        blackCount++;
                    }
                    else
                    {
                        emptyCount++;
                    }

        }
#endregion
        #region Multiplayer code
        public void Client()
        {
            this.multiplayerRole = MultiplayerRole.Client;
            this.Log("Client");

            this.Log("Looking for sessions...");
            try
            {
                NetworkSession.BeginFind(NetworkSessionType.SystemLink, 1, null, OnSessionsFound, (object)"Searching games");
            }
            catch (Microsoft.Xna.Framework.Net.NetworkException e)
            {
                this.Log("Wifi not on!");

            }
        }

        #region NET
        void OnSessionsFound(IAsyncResult result)
        {

            try
            {
                AvailableNetworkSessionCollection availableSessions = NetworkSession.EndFind(result);
                sessions = availableSessions;
                if (availableSessions.Count == 0)
                {
                    mode = Mode.start;
                }
                else
                {
                    int x = 5;
                    
                    foreach (AvailableNetworkSession s in availableSessions)
                    {
                        if (s.CurrentGamerCount <2)
                        {
                            if (s.SessionProperties.Count > 0)
                            {
                                if (s.SessionProperties[0] == 41523457)
                                {
                                    gameselect.Add(new Othello.Menu.MenuItem(s.HostGamertag + " " + s.QualityOfService.AverageRoundtripTime.Milliseconds.ToString(), new Vector2(5, x), x == 5 ? true : false, x == 5 ? Color.Red : Color.White));
                                    x = x + 30;
                                }
                            }
                        }
                    }
                    mode = Mode.gameselect;
                }
                

            }
            catch (Microsoft.Xna.Framework.Net.NetworkException e)
            {
                Log("ERROR: WIFI NOT ON");

            }
        }
        AvailableNetworkSessionCollection sessions;
        void Connect(int id)
        {
            //AvailableNetworkSessionCollection availableSessions = NetworkSession.EndFind(result);
            try
            {
                this.networkSession = NetworkSession.Join(sessions[id]);

                turn = -1;
                //tx2dcol.spots[3, 4] = new Piece(black, new Point(3, 4), -1);
                //tx2dcol.spots[3, 3] = new Piece(white, new Point(3, 3), 1);
                //tx2dcol.spots[4, 3] = new Piece(black, new Point(4, 3), -1);
                //tx2dcol.spots[4, 4] = new Piece(white, new Point(4, 4), 1);
                //sendMessage(encrypt(Gamer.SignedInGamers[0].Gamertag + " has joined the chat!"));

                //maxLength = maxLength - 2 - Gamer.SignedInGamers[0].Gamertag.Length;

                this.networkSession.GamerJoined += new EventHandler<GamerJoinedEventArgs>(networkSession_GamerJoined);
                this.networkSession.GamerLeft += new EventHandler<GamerLeftEventArgs>(networkSession_GamerLeft);
                this.networkSession.GameEnded += new EventHandler<GameEndedEventArgs>(networkSession_GameEnded);
                this.networkSession.GameStarted += new EventHandler<GameStartedEventArgs>(networkSession_GameStarted);
                this.networkSession.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(networkSession_SessionEnded);
            }
            catch (NetworkException exception)
            {
                this.Log(exception.Message);
            }
        }
        void sendMessage(Point Message)
        {
            if (networkSession == null)
            {
                Log("ERROR: SESSION NOT STARTED");
                //messages.Add("ERROR: SESSION NOT STARTED");
            }
            else
            {
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                {
                    packetWriter.Write(vec2frompoint(selectedboard));
                    gamer.SendData(packetWriter, SendDataOptions.ReliableInOrder);
                }

            }

        }

        void recieveMessage()
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                // Keep reading as long as incoming packets are available.
                while (gamer.IsDataAvailable)
                {
                    NetworkGamer sender;

                    // Read a single packet from the network.
                    gamer.ReceiveData(packetReader, out sender);

                    Vector2 message = packetReader.ReadVector2();

                    if (sender.Gamertag != gamer.Gamertag)
                    {
                        //tx2dcol.setp(int.Parse(message.X.ToString()), int.Parse(message.Y.ToString()), turn);
                        MakeMove(turn, int.Parse(message.X.ToString()), int.Parse(message.Y.ToString()));
                        //turn = -turn;
                        changeturn();
                    }

                    int last = 0;
                    /*
                    while (NormalFont.MeasureString(message).X > allowableWidth)
                    {
                        for (int x = 0; x < message.Length; x++)
                        {
                            if (message[x] == ' ' && NormalFont.MeasureString(message.Substring(0, x)).X < allowableWidth)
                                last = x;
                            else if (message[x] == ' ')
                            {
                                addString(message.Substring(0, last));
                                message = message.Substring(last + 1);
                                last = 0;
                                x = 325235;
                            }
                        }
                        if (last != 0)
                        {
                            //addString(message.Substring(0, last));
                            //message = message.Substring(last + 1);

                        }
                        last = 0;
                    }
                    */

                    /*if (message.Length > letWidth)
                    {
                        while (message.Length > letWidth)
                        {
                            int breakAt = letWidth;
                            for (int x = letWidth; x > 0; x--)
                            {
                                if (message[x] == ' ')
                                {
                                    breakAt = x+1;
                                    x = -5;
                                }
                            }
                            addString(message.Substring(0, breakAt));
                            message = message.Substring(breakAt);
                        }
                    }*/

                    //addString(decrypt(message));

                }
            }
        }
        void createSession()
        {
            this.multiplayerRole = MultiplayerRole.Server;
            Log("Server");
            //createSession();
            NetworkSessionProperties nsp = new NetworkSessionProperties();
            nsp[0] = 41523457;
            this.networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2, 0, nsp);
           

            this.networkSession.GamerJoined += new EventHandler<GamerJoinedEventArgs>(networkSession_GamerJoined);
            this.networkSession.GamerLeft += new EventHandler<GamerLeftEventArgs>(networkSession_GamerLeft);
            this.networkSession.GameEnded += new EventHandler<GameEndedEventArgs>(networkSession_GameEnded);
            this.networkSession.GameStarted += new EventHandler<GameStartedEventArgs>(networkSession_GameStarted);
            this.networkSession.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(networkSession_SessionEnded);

            Log("Session started");
            //messages.Add("New Chat started");
            //maxLength = maxLength - 2 - Gamer.SignedInGamers[0].Gamertag.Length;
        }


        

        void networkSession_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            Log("Session ended");
            if (this.multiplayerRole == MultiplayerRole.Client)
            {
                this.multiplayerRole = MultiplayerRole.None;
                this.networkSession.Dispose();
            }
        }
        void networkSession_GameStarted(object sender, GameStartedEventArgs e)
        {
            Log("Game started");
        }

        void networkSession_GameEnded(object sender, GameEndedEventArgs e)
        {
            SM["Wifi End"].Play();
            Log("Game ended");
        }

        void networkSession_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            SM["Turn"].Play();
            Log("Gamer left");
            if (this.multiplayerRole == MultiplayerRole.Client)
            {
                this.multiplayerRole = MultiplayerRole.None;
                this.networkSession.Dispose();
            }
            mode = Mode.single;
            if (turn == 1)
            {
                MakeComputerMove();
            }
        }

        void networkSession_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (e.Gamer != networkSession.LocalGamers[0])
            {
                mode = Mode.wireless;
            }
            networkSession.StartGame();
            SM["Wifi Start"].Play();
            Log("Gamer joined");
        }
        #endregion
        #endregion
        void Log(String message)
        {
            System.Diagnostics.Debug.WriteLine(message);
            
        }
        
        #region Computer

        /*public void MakeComputerMove()
        {
            Random r = new Random(0);
            Int32 x = 0;
            Int32 y = 0;
            
            Int64 runcount = 0;
            while (IsValidMove(1, x, y) && runcount != 0)
            {
                Int32 xnum = r.Next(10);
                Int32 ynum = r.Next(10);
                for (int ix = 0; ix < xnum; ix++)
                {
                    x = r.Next(8);
                }
                for (int iy = 0; iy < ynum; iy++)
                {
                    y = r.Next(8);
                }
                runcount++;
            }
            if (IsValidMove(1, x, y))
            {
                MakeMove(1, x, y);
            }
        }
         * */
       
        public void MakeComputerMove()
        {
            
            Random ra = new Random(0);
            
            Int32 x = 0;
            Int32 y = 0;
            Int32 r;
            Int32 c;
            List<Int32> arr = new List<int>();
            List<Point> arr2 = new List<Point>();
            
            for (int co = 0; co < 8; co++)
            {
                for (int ro = 0; ro < 8; ro++)
                {

                    if (IsValidMove(1, co, ro))
                    {
                        Int32 num = 1;
                        for (int dr = -1; dr <= 1; dr++)
                            for (int dc = -1; dc <= 1; dc++)
                                // Are there any outflanked opponents?
                                if (!(dr == 0 && dc == 0) && IsOutflanking(1, ro, co, dr, dc))
                                {
                                    //Log("run 1");
                                    r = ro + dr;
                                    c = co + dc;
                                    // Flip 'em.
                                    while (tx2dcol.spots[r, c].col == -1)
                                    {
                                        //Log("changeing");
                                        num++;
                                        r += dr;
                                        c += dc;
                                    }
                                }
                        arr.Add(num);
                        arr2.Add(new Point(co, ro));
                    }
                }
            }

            searchv = arr.Max<int>();
            
            Int32 id = arr.FindIndex(find);
            Point p = arr2[id];
            if (IsValidMove(1, p.X, p.Y))
            {
                MakeMove(1, p.X, p.Y);
            }
            SM["Change Turn"].Play();
            changeturn();
        }
        public int Calchardness(int i)
        {
            Decimal d = Decimal.Parse(i.ToString());
            Decimal doutput = Math.Round(d);
            Int32 output = Int32.Parse(doutput.ToString());
            return output;
        }
        int searchv;
        public bool find(Int32 inp)
        {
            if (inp == searchv)
            {
                return true;
            }
            return false;
        }
        #endregion
        #region Check Move Code
        public void hasValidMove()
        {
            if (!HasAnyValidMove(turn))
            {
                turn = -turn;
            }
        }
        public void changeturn()
        {
            turn = -turn;
            Log("Changed Turn");
            if (!HasAnyValidMove(1) && !HasAnyValidMove(-1))
            {
                if (blackCount > whiteCount)
                {

                    Win(1);

                }
                else
                {

                    Win(1);

                }
            }
            else if (!HasAnyValidMove(turn))
            {
                changeturn();
            }
            else if (HasAnyValidMove(turn))
            {
                if (mode == Mode.single)
                {
                    if (turn == 1)
                    {
                        //MakeComputerMove();
                        t.Start();
                    }
                    else
                    {
                        SM["Change Turn"].Play();
                    }
                }
                else if (mode == Mode.wireless)
                {
                    if (multiplayerRole == MultiplayerRole.Server)
                    {
                        if (turn == -1)
                        {
                            SM["Change Turn"].Play();
                        }
                    }
                    else
                    {
                        if (turn == 1)
                        {
                            SM["Change Turn"].Play();
                        }
                    }
                }
            }
        }
        #endregion
    }
    
}
