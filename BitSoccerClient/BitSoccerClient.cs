using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Client.Properties;
using Common;
using GameEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace BitSoccerClient
{
    public class BitSoccerClient : Game
    {
        private GraphicsDeviceManager _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private MouseState _previousMouseState;
        private KeyboardState _keyboardState;

        private int _maxFps = 60;
        private double _aspect;
        private ITeam _team1;
        private ITeam _team2;
        private BallInfo _ballInfo;
        public GameState gameState;

        private const string _title = "CloudBall";
        private int _matchLength = Constants.GameEngineMatchLength;
        private IGameEngine _gameEngine;

        private SpriteFont _spriteFont;
        private Texture2D _textureNumbers;
        private Texture2D _textureField;
        private Texture2D _textureCircle;
        private Texture2D _textureProgressDot;
        private Texture2D _textureBall;
        private Texture2D _texturePauseButton;
        private Texture2D _textureRunButton;
        private Texture2D _textureWhiteDot;
        private Texture2D _texturePlayer;

        private bool _successfullyLoaded;

        private bool c;
        private bool _shouldExecute;
        private bool ae;
        private bool af;
        private bool ab;

        private GameStateList _gameStates;
        private ScoreInfo _scoreInfo;
        private DevPromtDialog _devPromptDialog;

        private int _ballMaxPickUpDistance;
        private int v;
        private int w;
        private int _playerMaxTackleDistance;
        private double _scale;

        private Point _mousePosition;
        private Point _clientBounds;
        private MouseState _currentMousestate;
        private KeyboardState _currentKeyboardState;

        private Microsoft.Xna.Framework.Rectangle _rectRedButton;
        private Microsoft.Xna.Framework.Rectangle _rectBlueButton;
        private Microsoft.Xna.Framework.Rectangle _rectShowDevPromtButton;
        private Microsoft.Xna.Framework.Rectangle _rectRestartButton;
        private Microsoft.Xna.Framework.Rectangle _rectSaveReplayButton;
        private Microsoft.Xna.Framework.Rectangle _rectLoadReplayButton;
        private Microsoft.Xna.Framework.Rectangle _rectField = new Microsoft.Xna.Framework.Rectangle(128, 127, 724, 401);
        private Microsoft.Xna.Framework.Rectangle u;
        private Microsoft.Xna.Framework.Rectangle _rectStartPauseButton;
        private Microsoft.Xna.Framework.Rectangle _rectSliderButton;
        private Microsoft.Xna.Framework.Rectangle _rectStepBackButton;
        private Microsoft.Xna.Framework.Rectangle _rectStepForwardButton;


        private List<TeamInfo> Teams;

        public event EventHandler Initialized;

        #region Properties 

        public bool CheckForTimeouts { get; set; }

        public int TraceLength { get; set; }

        public bool Security { get; set; }

        public int MaxFps
        {
            get { return this._maxFps; }
            set
            {
                this._maxFps = value;
                this.TargetElapsedTime = new TimeSpan((long)(10000000 / this.MaxFps));
            }
        }

        #endregion

        #region Constructors 

        public BitSoccerClient()
        {
            BitSoccerClient.UpdateConfig();
            this.TraceLength = 0;
            this.Security = false;
            this._graphicsDeviceManager = new GraphicsDeviceManager((Game)this);
            this._graphicsDeviceManager.IsFullScreen = false;
            //this.Window.ClientSizeChanged += new EventHandler<EventArgs>(ClientSizeChanged);
            this.Exiting += new EventHandler<EventArgs>(Client_Exiting);
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan((long)(10000000 / this.MaxFps));
            this.Content.RootDirectory = "Content";
        }

        public BitSoccerClient(ITeam team1, ITeam team2)
            : this()
        {
            this._team1 = team1;
            this._team2 = team2;
        }

        #endregion

        public void ExecuteGame(bool shouldExecute)
        {
            this._shouldExecute = shouldExecute;
        }

        private static void UpdateConfig()
        {
            if (!config.Default.FirstTimeRunning)
                return;
            config.Default.team1Path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "\\Teams\\TeamOne.dll";
            config.Default.team2Path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName + "\\Teams\\TeamTwo.dll";
            config.Default.FirstTimeRunning = false;
            config.Default.Save();
        }

        protected override void Initialize()
        {
            Mouse.WindowHandle = this.Window.Handle;
            this._shouldExecute = false;
            this.ae = false;
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);

            this._previousMouseState = Mouse.GetState();
            this.Window.Title = "CloudBall";
            this.IsMouseVisible = true;
            this._scale = 0.5;

            //this.IsFixedTimeStep = false;
            //_graphicsDeviceManager.SynchronizeWithVerticalRetrace = false;

            this._graphicsDeviceManager.PreferredBackBufferWidth = 852;
            this._graphicsDeviceManager.PreferredBackBufferHeight = 634;
            this._graphicsDeviceManager.ApplyChanges();

            this._spriteFont = this.Content.Load<SpriteFont>("SergoeUIMono");
            this._textureNumbers = this.Content.Load<Texture2D>("Numbers");
            this._textureField = this.Content.Load<Texture2D>("field");
            this._textureCircle = this.Content.Load<Texture2D>("circle");
            this._textureProgressDot = this.Content.Load<Texture2D>("progressDot");
            this._textureBall = this.Content.Load<Texture2D>("ball");
            this._texturePauseButton = this.Content.Load<Texture2D>("PauseButton");
            this._textureRunButton = this.Content.Load<Texture2D>("runButton");
            this._textureWhiteDot = new Texture2D(this.GraphicsDevice, 1, 1);
            this._textureWhiteDot.SetData<Color>(new Color[1] { Color.White });
            this._texturePlayer = Content.Load<Texture2D>("player");

            this._aspect = (double)this._textureField.Bounds.Width / (double)this._textureField.Bounds.Height;
            base.Initialize();

            this.ClientSizeChanged1(new object(), new EventArgs());

            this._devPromptDialog = new DevPromtDialog();
            if (config.Default.showPrompt)
                this._devPromptDialog.ShowPromt();
            else
                this._devPromptDialog.HidePromt();

            this.Window.AllowUserResizing = true;
            this.RestartGame();
        }

        protected override void LoadContent()
        {
            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            this._textureNumbers.Dispose();
            this._spriteBatch.Dispose();
        }

        protected void RestartGame()
        {
            if (this._gameEngine != null)
            {
                this._gameEngine.Dispose();
                this._gameEngine = null;
            }
            this._shouldExecute = false;
            this.w = 0;
            this.v = 0;
            try
            {
                this._gameEngine = this._team1 == null || this._team2 == null ?
                    (!this.Security ? (IGameEngine)new GameEngine.GameEngine(config.Default.team1Path, config.Default.team2Path) :
                    (IGameEngine)new LocalSecureGameEngine(config.Default.team1Path, config.Default.team2Path)) :
                    (IGameEngine)new GameEngine.GameEngine(this._team1, this._team2);

                this._gameEngine.setTimeout(this.CheckForTimeouts);
                this.GetGameStates();
                this._devPromptDialog.AppendText(this._gameStates.Team1 + " vs " + this._gameStates.Team2 + ". Ready to start game.");
                this._successfullyLoaded = true;
            }
            catch (Exception ex)
            {
                this._devPromptDialog.AppendText("Could not load teams. Game paused.");
                this._successfullyLoaded = false;
            }
        }

        private void GetGameStates()
        {
            this.gameState = this._gameEngine.GetCurrent();
            this.Teams = this.gameState.Teams();
            this._ballInfo = this.gameState.BallInfo;
            this._gameStates = new GameStateList(this._gameEngine.Team1Name(), this._gameEngine.Team2Name());
        }

        protected override void Update(GameTime gameTime)
        {
            if (this._successfullyLoaded)
            {
                if (this._shouldExecute && this.w < this._matchLength && this.w == this.v)
                    this.NextGameState();
                else if (this._shouldExecute && this.w < this._matchLength)
                {
                    this.gameState = this._gameStates[this.w];
                    ++this.w;
                }
                else
                    this.gameState = this._gameStates == null || this._gameStates.Count == 0 ? new GameState() : (this.w < this._gameStates.Count ? this._gameStates[this.w] : this._gameStates[this._gameStates.Count - 1]);
            }
            else
                this.gameState = new GameState();


            base.Update(gameTime);

            if (this.gameState != null)
            {
                this.Teams = this.gameState.Teams();
                this._ballInfo = this.gameState.BallInfo;
                this._scoreInfo = this.gameState.GetScoreInfo();
            }
            this._currentMousestate = Mouse.GetState();
            this._mousePosition = new Point(this._currentMousestate.X, this._currentMousestate.Y);

            if (this._currentMousestate.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed
                && this.IsActive && this._previousMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
            {
                if ((this.u.Contains(this._mousePosition) || this.af) && this._successfullyLoaded)
                {
                    this.ae = true;
                    int num = Math.Min(Math.Max(0, this._matchLength * (this._mousePosition.X - this._rectSliderButton.X) / this._rectSliderButton.Width), this._matchLength);
                    if (num < this.v)
                    {
                        this.w = num;
                    }
                    else
                    {
                        this.w = this.v;
                        while (this.v < this._matchLength && this.w < num)
                            this.NextGameState();
                    }
                    this.af = true;
                }
                if (this._rectSliderButton.Contains(this._mousePosition) && this._successfullyLoaded)
                {
                    this.ae = true;
                    int num = Math.Min(Math.Max(0, this._matchLength * (this._mousePosition.X - this._rectSliderButton.X) / this._rectSliderButton.Width), this._matchLength);
                    if (num < this.v)
                    {
                        this.w = num;
                    }
                    else
                    {
                        this.w = this.v;
                        while (this.v < this._matchLength && this.w < num)
                            this.NextGameState();
                    }
                }
                if (this._rectStepBackButton.Contains(this._mousePosition) && this._successfullyLoaded)
                {
                    this._shouldExecute = false;
                    if (this.w > 0)
                        --this.w;
                }
                if (this._rectStepForwardButton.Contains(this._mousePosition) && this._successfullyLoaded)
                {
                    this._shouldExecute = false;
                    if (this.w < this.v)
                        ++this.w;
                    else if (this.w == this.v && this.w < this._matchLength)
                        this.NextGameState();
                }
                if (this._rectStartPauseButton.Contains(this._mousePosition) && !this.ab && this._successfullyLoaded)
                    this._shouldExecute = !this._shouldExecute;
                if (!this.ae)
                {
                    if (this._rectSaveReplayButton.Contains(this._mousePosition))
                        this.SaveReplay();
                    if (this._rectLoadReplayButton.Contains(this._mousePosition))
                        this.LoadReplay();
                    if (this._rectRedButton.Contains(this._mousePosition))
                        this.LoadTeams(1);
                    if (this._rectBlueButton.Contains(this._mousePosition))
                        this.LoadTeams(2);
                    if (this._rectRestartButton.Contains(this._mousePosition))
                        this.RestartGame();
                    if (this._rectShowDevPromtButton.Contains(this._mousePosition) && !this.ab)
                    {
                        config.Default.showPrompt = !config.Default.showPrompt;
                        config.Default.Save();
                        if (this._devPromptDialog.IsVisible())
                            this._devPromptDialog.HidePromt();
                        else
                            this._devPromptDialog.ShowPromt();
                    }
                }
                this.ab = true;
            }
            else
            {
                this.ab = false;
                this.af = false;
            }
            this._keyboardState = Keyboard.GetState();
            if (this.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left) && this._successfullyLoaded)
            {
                this._shouldExecute = false;
                if (this.w > 0)
                    --this.w;
            }
            if (this.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right) && this._successfullyLoaded)
            {
                this._shouldExecute = false;
                if (this.w < this.v)
                    ++this.w;
                else if (this.w == this.v && this.w < this._matchLength)
                    this.NextGameState();
            }
            if (this.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.End) && this._successfullyLoaded)
            {
                this._shouldExecute = false;
                this.w = this.v;
                while (this.v < this._matchLength)
                    this.NextGameState();
            }
            if (this.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Home) && this._successfullyLoaded)
            {
                this._shouldExecute = false;
                this.w = 0;
            }
            if ((this.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Pause) || this.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space)) && this._successfullyLoaded)
                this._shouldExecute = !this._shouldExecute;
            this._currentKeyboardState = this._keyboardState;
            if (this._currentMousestate.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Released)
                this.ae = false;
            if (this.ae)
                return;
            this._previousMouseState = this._currentMousestate;
        }

        private void NextGameState()
        {
            this.gameState = this._gameEngine.GetNext();
            this._gameStates.Add(this.gameState);
            ++this.v;
            ++this.w;
            if (!(this.gameState.GetDevMessage() != "") || this._devPromptDialog.IsDisposed)
                return;
            this._devPromptDialog.AppendText(this.gameState.GetDevMessage());
        }

        #region Draw Methods 

        protected override void Draw(GameTime gameTime)
        {
            if (this.GraphicsDevice.Viewport.Height == this.GraphicsDevice.DisplayMode.Height
                || this.GraphicsDevice.Viewport.Width == this.GraphicsDevice.DisplayMode.Width)
            {
                this._graphicsDeviceManager.PreferredBackBufferHeight = this._clientBounds.Y;
                this._graphicsDeviceManager.PreferredBackBufferWidth = this._clientBounds.X;
            }
            this.GraphicsDevice.Clear(Color.LightGray);
            this._spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend,
        SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            this._spriteBatch.Draw(this._textureField, new Rectangle(0, 0, this._graphicsDeviceManager.PreferredBackBufferWidth, this._graphicsDeviceManager.PreferredBackBufferHeight), Color.White);
            this.DrawTrace();
            this.DrawPlayers();
            this.DrawBall();
            this.DrawText();
            this._spriteBatch.End();
            base.Draw(gameTime);
        }

        private void DrawTrace()
        {
            for (int index = Math.Max(0, this.w - this.TraceLength); index < this.w - 1; ++index)
            {
                Color color = Color.Red;
                Vector vector;
                Vector2 position;
                foreach (TeamInfo team in this._gameStates[index].Teams())
                {
                    foreach (PlayerInfo player in team.GetPlayers())
                    {
                        vector = player.GetPosition();
                        float num1 = (float)(((double)this._rectField.X + (double)vector.X * (double)this._rectField.Width / (double)Field.Borders.Width) * this._scale);
                        float num2 = (float)(((double)this._rectField.Y + (double)vector.Y * (double)this._rectField.Height / (double)Field.Borders.Height) * this._scale);
                        int num3 = (int)((double)(this.TraceLength - this.w + index) / (double)this.TraceLength * 3.0 * this._scale);
                        position = new Vector2(num1 - (float)(num3 / 2), num2 - (float)(num3 / 2));
                        this._spriteBatch.Draw(this._textureWhiteDot, position, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, num3, num3)), color);
                    }
                    color = Color.Blue;
                }
                vector = this._gameStates[index].BallInfo.Position;
                float num4 = (float)(((double)this._rectField.X + (double)vector.X * (double)this._rectField.Width / (double)Field.Borders.Width) * this._scale);
                float num5 = (float)(((double)this._rectField.Y + (double)vector.Y * (double)this._rectField.Height / (double)Field.Borders.Height) * this._scale);
                int num6 = (int)((double)(this.TraceLength - this.w + index) / (double)this.TraceLength * 3.0 * this._scale);
                position = new Vector2(num4 - (float)(num6 / 2), num5 - (float)(num6 / 2));
                this._spriteBatch.Draw(this._textureWhiteDot, position, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, num6, num6)), Color.White);
            }
        }

        private void DrawBall()
        {
            int x = (int)(((double)this._rectField.X + (double)this._ballInfo.Position.X * (double)this._rectField.Width / (double)Field.Borders.Width) * this._scale - (double)(this._ballMaxPickUpDistance / 2));
            int y = (int)(((double)this._rectField.Y + (double)this._ballInfo.Position.Y * (double)this._rectField.Height / (double)Field.Borders.Height) * this._scale - (double)(this._ballMaxPickUpDistance / 2));
            Vector2 vector2 = new Vector2((float)x, (float)y);
            this._spriteBatch.Draw(this._textureBall, new Rectangle(x, y, this._ballMaxPickUpDistance, this._ballMaxPickUpDistance), new Rectangle(0, 0, 677, 672), Color.White);
        }

        int frame = 0;
        int frameCnt = 0;

        private void DrawPlayers()
        {
            var color = new Color(255, 27, 175);
            for (var team = 0; team < 2; ++team)
            {
                var teamInfos = Teams[team];
                var num = 1;
                for (var i = 0; i < teamInfos.GetPlayers().Count; ++i)
                {
                    var player = teamInfos.GetPlayers()[i];
                    var x =
                        (int)
                            ((_rectField.X + player.GetPosition().X * (double)_rectField.Width / Field.Borders.Width) *
                             _scale - _playerMaxTackleDistance / 2);
                    var y =
                        (int)
                            ((_rectField.Y + player.GetPosition().Y * (double)_rectField.Height / Field.Borders.Height) *
                             _scale - _playerMaxTackleDistance / 2);
                    DrawFallTime(
                        new Rectangle(x - 2, y - 2, _playerMaxTackleDistance + 4, _playerMaxTackleDistance + 4),
                        (1.0 - player.GetFallenTimer() / (double)Constants.PlayerFallenTime) * 2.0 * Math.PI, 2.0,
                        Color.White);

                    Rectangle frameRect = Rectangle.Empty;
                    if (frame == 0 || frame == 2)
                        frameRect = new Rectangle(2, 362, 13, 16);
                    if (frame == 1)
                        frameRect = new Rectangle(2, 380, 13, 16);
                    if (frame == 3)
                        frameRect = new Rectangle(2, 398, 13, 16);

                    //_spriteBatch.Draw(_texturePlayer, new Rectangle(x - 5, y - 16, (int)(26 * _scale), (int)(32 * _scale)),
                    //    frameRect, Color.White);

                    this._spriteBatch.Draw(this._textureCircle, new Rectangle(x, y, this._playerMaxTackleDistance, this._playerMaxTackleDistance), color);
                    _spriteBatch.Draw(_textureNumbers,
                        new Rectangle(x + 1, y + 1, _playerMaxTackleDistance - 2, _playerMaxTackleDistance - 2),
                        new Rectangle(100 * num, 0, 100, 100), new Color(155, 155, 155, 155));
                    ++num;
                }
                color = Color.Blue;
            }

            frameCnt++;
            if (frameCnt > 4)
            {
                frameCnt = 0;
                frame++;
                if (frame > 3)
                    frame = 0;
            }
        }

        private void DrawFallTime(Microsoft.Xna.Framework.Rectangle rect, double A_1, double A_2, Color A_3)
        {
            double num1 = (double)(rect.Width / 2);
            double num2 = 0.5 / num1;
            for (double num3 = num1 - A_2; num3 <= num1; ++num3)
            {
                double num4 = -1.0 * Math.PI / 2.0;
                while (num4 < 3.0 * Math.PI / 2.0 - A_1)
                {
                    this._spriteBatch.Draw(this._textureWhiteDot, new Microsoft.Xna.Framework.Rectangle((int)Math.Round((double)rect.Center.X + Math.Cos(num4) * num3), (int)Math.Round((double)rect.Center.Y + Math.Sin(num4) * num3), 1, 1), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 1, 1)), Color.White);
                    num4 += num2;
                }
            }
        }

        private void DrawText()
        {
            string text1 = "Not Loaded";
            if (this._gameEngine != null)
            {
                text1 = this.ClipString(this._gameStates.Team1, (float)(this._rectRedButton.Width - 15));
            }
            this._spriteBatch.DrawString(this._spriteFont, text1, new Vector2((float)(int)(((double)(this._textureField.Width / 2 - 80) - (double)this._spriteFont.MeasureString(text1).X) * this._scale), (float)(int)(555.0 * this._scale)), Color.White, 0.0f, Vector2.Zero, (float)this._scale, SpriteEffects.None, 0.0f);
            string text2 = "Not Loaded";
            if (this._gameEngine != null)
            {
                text2 = this.ClipString(this._gameStates.Team2, (float)(this._rectBlueButton.Width - 15));
            }
            this._spriteBatch.DrawString(this._spriteFont, text2, new Vector2((float)(int)((double)(this._textureField.Width / 2 + 80) * this._scale), (float)(int)(555.0 * this._scale)), Color.White, 0.0f, Vector2.Zero, (float)this._scale, SpriteEffects.None, 0.0f);
            string str1 = Convert.ToString(this._scoreInfo.Team1);
            int length1 = str1.Length;
            for (int startIndex = 0; startIndex < length1; ++startIndex)
                this._spriteBatch.Draw(this._textureNumbers, new Microsoft.Xna.Framework.Rectangle((int)((double)(this._textureField.Width / 2 - 85 - 30 * (length1 - startIndex)) * this._scale), (int)(590.0 * this._scale), (int)(40.0 * this._scale), (int)(40.0 * this._scale)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(100 * Convert.ToInt32(str1.Substring(startIndex, 1)), 0, 100, 100)), Color.White);
            // ISSUE: reference to a compiler-generated method
            string str2 = Convert.ToString(this._scoreInfo.Team2);
            int length2 = str2.Length;
            for (int startIndex = 0; startIndex < length2; ++startIndex)
                this._spriteBatch.Draw(this._textureNumbers, new Microsoft.Xna.Framework.Rectangle((int)((double)(this._textureField.Width / 2 + 75 + 30 * startIndex) * this._scale), (int)(590.0 * this._scale), (int)(40.0 * this._scale), (int)(40.0 * this._scale)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(100 * Convert.ToInt32(str2.Substring(startIndex, 1)), 0, 100, 100)), Color.White);
            double num = (double)this.w / (double)this._matchLength;
            int width = (int)(16.0 * this._scale);
            int height = (int)(16.0 * this._scale);
            this.u = new Microsoft.Xna.Framework.Rectangle((int)((double)(this._rectSliderButton.X + 1) + (double)(this._rectSliderButton.Width - 1 - width) * num), this._rectSliderButton.Y + 1, width, height);
            this._spriteBatch.Draw(this._textureProgressDot, this.u, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 18, 16)), Color.White);
            if (!this._shouldExecute)
                this._spriteBatch.Draw(this._textureRunButton, this._rectStartPauseButton, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32)), Color.White);
            else
                this._spriteBatch.Draw(this._texturePauseButton, this._rectStartPauseButton, new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, 32, 32)), Color.White);
        }

        #endregion

        #region Replay Methods

        private void SaveReplay()
        {
            this._shouldExecute = false;
            string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CloudBall");
            if (!Directory.Exists(str + "\\Replays"))
                Directory.CreateDirectory(str + "\\Replays");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = str + "\\Replays";
            saveFileDialog.Title = "Save replay";
            saveFileDialog.Filter = "Replay Files|*.cbr";
            saveFileDialog.FileName = Path.GetFileNameWithoutExtension(config.Default.team1Path) + "-" + Path.GetFileNameWithoutExtension(config.Default.team2Path) + "-" + DateTime.Now.ToShortDateString().Replace('-', '.') + "-" + DateTime.Now.ToShortTimeString().Replace(':', '.');
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                ReplayHelper.SaveReplay(saveFileDialog.FileName, this._gameStates);
            }
            catch (SystemException ex)
            {
                int num = (int)MessageBox.Show("Failed to save replay");
            }
        }

        private void LoadReplay()
        {
            this._shouldExecute = false;
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string str = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CloudBall");
            if (!Directory.Exists(str + "\\Replays"))
                Directory.CreateDirectory(str + "\\Replays");
            openFileDialog.InitialDirectory = str + "\\Replays";
            openFileDialog.Title = "Load a replay";
            openFileDialog.Filter = "Replay Files|*.cbr";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            try
            {
                this._gameStates.Clear();
                this._gameStates = ReplayHelper.LoadReplay(openFileDialog.FileName);
                this.v = this._gameStates.Count;
                this._matchLength = this._gameStates.Count;
                this.w = 0;
                this._devPromptDialog.AppendText("Replay successfully loaded");
            }
            catch (SystemException ex)
            {
                int num = (int)MessageBox.Show("Failed to load replay");
            }
        }

        #endregion

        private void LoadTeams(int teamNumber)
        {
            if (teamNumber == 1)
                this._team1 = (ITeam)null;
            else
                this._team2 = (ITeam)null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Select a team";
            try
            {
                if (teamNumber == 1)
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(config.Default.team1Path);
                else
                    openFileDialog.InitialDirectory = Path.GetDirectoryName(config.Default.team2Path);
            }
            catch
            {
                openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            }
            if (this.c)
            {
                openFileDialog.Filter = "Team|*.dll|Java|*.jar|Executable|*.exe|Python|*.py";
                try
                {
                    string str = teamNumber != 1 ? Path.GetExtension(config.Default.team2Path) : Path.GetExtension(config.Default.team1Path);
                    if (str.Equals(".dll"))
                        openFileDialog.FilterIndex = 1;
                    else if (str.Equals(".jar"))
                        openFileDialog.FilterIndex = 2;
                    else if (str.Equals(".exe"))
                        openFileDialog.FilterIndex = 3;
                    else if (str.Equals(".py"))
                        openFileDialog.FilterIndex = 4;
                }
                catch
                {
                    openFileDialog.FilterIndex = 1;
                }
            }
            else
                openFileDialog.Filter = "Team|*.dll";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GameEngine.GameEngine.teamOK(openFileDialog.FileName);
                    if (teamNumber == 1)
                        config.Default.team1Path = openFileDialog.FileName;
                    else
                        config.Default.team2Path = openFileDialog.FileName;
                    config.Default.Save();
                }
                catch
                {
                    int num = (int)MessageBox.Show("Team is not valid.");
                }
            }
            this.RestartGame();
        }

        private void ClientSizeChanged1 (object sender, EventArgs e)
        {
            this.Window.ClientSizeChanged -= new EventHandler<EventArgs>(this.ClientSizeChanged1);
            if (this.Window.ClientBounds.Width != this._clientBounds.X)
            {
                this._graphicsDeviceManager.PreferredBackBufferWidth = this.Window.ClientBounds.Width;
                this._graphicsDeviceManager.PreferredBackBufferHeight = (int)((double)this.Window.ClientBounds.Width / this._aspect);
            }
            else if (this.Window.ClientBounds.Height != this._clientBounds.Y)
            {
                this._graphicsDeviceManager.PreferredBackBufferWidth = (int)((double)this.Window.ClientBounds.Height * this._aspect);
                this._graphicsDeviceManager.PreferredBackBufferHeight = this.Window.ClientBounds.Height;
            }
            this._graphicsDeviceManager.ApplyChanges();
            this._clientBounds = new Point(this.Window.ClientBounds.Width, this.Window.ClientBounds.Height);

            this.Window.ClientSizeChanged += new EventHandler<EventArgs>(this.ClientSizeChanged1);

            this._scale = (double)this.Window.ClientBounds.Height / (double)this._textureField.Bounds.Height;
            this._playerMaxTackleDistance = (int)((double)Constants.PlayerMaxTackleDistance * this._scale * ((double)this._rectField.Width / (double)Field.Borders.Width));
            this._ballMaxPickUpDistance = Math.Max(1, (int)((double)Constants.BallMaxPickUpDistance * 2.0 * this._scale * ((double)this._rectField.Width / (double)Field.Borders.Width)) - this._playerMaxTackleDistance);
            this._rectSliderButton = new Microsoft.Xna.Framework.Rectangle((int)(248.0 * this._scale), (int)(653.0 * this._scale), (int)(480.0 * this._scale), (int)(17.0 * this._scale));
            this._rectStartPauseButton = new Microsoft.Xna.Framework.Rectangle((int)(472.0 * this._scale), (int)(581.0 * this._scale), (int)(40.0 * this._scale), (int)(40.0 * this._scale));
            this._rectStepBackButton = new Microsoft.Xna.Framework.Rectangle((int)(427.0 * this._scale), (int)(581.0 * this._scale), (int)(37.0 * this._scale), (int)(37.0 * this._scale));
            this._rectStepForwardButton = new Microsoft.Xna.Framework.Rectangle((int)(516.0 * this._scale), (int)(581.0 * this._scale), (int)(37.0 * this._scale), (int)(37.0 * this._scale));
            this._rectRedButton = new Microsoft.Xna.Framework.Rectangle((int)(246.0 * this._scale), (int)(555.0 * this._scale), (int)(172.0 * this._scale), (int)(82.0 * this._scale));
            this._rectBlueButton = new Microsoft.Xna.Framework.Rectangle((int)(559.0 * this._scale), (int)(555.0 * this._scale), (int)(172.0 * this._scale), (int)(82.0 * this._scale));
            this._rectRestartButton = new Microsoft.Xna.Framework.Rectangle((int)(943.0 * this._scale), (int)(619.0 * this._scale), (int)(33.0 * this._scale), (int)(32.0 * this._scale));
            this._rectShowDevPromtButton = new Microsoft.Xna.Framework.Rectangle((int)(943.0 * this._scale), (int)(660.0 * this._scale), (int)(33.0 * this._scale), (int)(32.0 * this._scale));
            this._rectSaveReplayButton = new Microsoft.Xna.Framework.Rectangle((int)(943.0 * this._scale), (int)(581.0 * this._scale), (int)(32.0 * this._scale), (int)(32.0 * this._scale));
            this._rectLoadReplayButton = new Microsoft.Xna.Framework.Rectangle((int)(943.0 * this._scale), (int)(545.0 * this._scale), (int)(32.0 * this._scale), (int)(32.0 * this._scale));
        }

        private void Client_Exiting(object sender, EventArgs e)
        {
            try
            {
                this._gameEngine.Dispose();
            }
            catch
            {
            }
        }

        private string ClipString(string text, float width)
        {
            for (double num = (double)this._spriteFont.MeasureString(text).X * this._scale; num > (double)width; num = (double)this._spriteFont.MeasureString(text).X * this._scale)
                text = text.Substring(0, text.Length - 1);
            return text;
        }

        private bool IsKeyDown(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (this._keyboardState.IsKeyDown(key))
                return this._currentKeyboardState.IsKeyUp(key);
            else
                return false;
        }

    }
}
