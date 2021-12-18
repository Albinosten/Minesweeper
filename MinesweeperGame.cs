using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using MinesweeperSolver;
using Minesweeper;

namespace Minesweeper
{
    public class MinesweeperGame : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont spriteFont;
        public TileHandler TileHandler{get;private set;}
        public MenuBarFactory MenuBarFactory {get;private set;}
        public MenuBar MenuBar {get;private set;}
        // public MinesweeperSolverFactory MinesweeperSolverFactory {get;private set;}
        public IMinesweeperSolver MinesweeperSolver {get;private set;}
        public GameLoader GameLoader{get;private set;}

        public GameContext GameContext {get; set;}
        private static float s_LockedTime => 0.4f;
        // private static float s_LockedTime => 0f;
        public bool DebugUpdateAllTiles{get;set;}
        public bool SimmulateOnly{get;set;}
        
        public MinesweeperGame(TileHandler tileHandler
            , MenuBarFactory menuBarFactory
            // , MinesweeperSolverFactory minesweeperSolverFactory
            , MinesweeperSolver.MinesweeperSolver minesweeperSolver
            , GameLoader gameLoader
            )
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            this.TileHandler = tileHandler;
            this.MenuBarFactory = menuBarFactory;
            // this.MinesweeperSolverFactory = minesweeperSolverFactory;
            this.MinesweeperSolver = minesweeperSolver;
            this.GameLoader = gameLoader;
        }
        protected override void Initialize()
        {
            this.MenuBar = this.MenuBarFactory.Create(this.GameContext, this._graphics);
            this.TileHandler.CreateTiles(this.GameContext, this._graphics, MenuBar.s_Height);
            // this.MinesweeperSolver = this.MinesweeperSolverFactory.Create(this.TileHandler, this.GameContext);

            if(!this.SimmulateOnly)
            {
                this._graphics.PreferredBackBufferHeight = this.GameContext.Height*Tile.s_Height+MenuBar.s_Height;
                this._graphics.PreferredBackBufferWidth = this.GameContext.Width*Tile.s_width;
                this._graphics.ApplyChanges();
            }
            else
            {
                this._graphics.PreferredBackBufferHeight = 40;
                this._graphics.PreferredBackBufferWidth = 200;
                this._graphics.ApplyChanges();
            }

            base.Initialize();
        }
        protected override void LoadContent()
        {
            base.LoadContent();

            this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
            this.spriteFont = this.Content.Load<SpriteFont>("Fonts/Font");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.R))
            {
                this.GameContext.LoadBombs = false;
                this.Initialize();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.P))
            {
                this.TileHandler.ToggleAll();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S)) //Save map
            {
                var bombs = this.TileHandler
                    .GetTiles()
                    .Where(x => x.IsBomb)
                    .Select(s => s.MyIndex)
                    .ToList();
                this.GameLoader.SaveResult(bombs);
            }
            if (Keyboard.GetState().IsKeyDown(Keys.L)) //Load map
            {
                this.GameContext.LoadBombs = true;
                this.Initialize();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.H))
            {
                this.TileHandler.UseHint();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                if(!this.mouseClickIsLocked)
                {

                    this.MinesweeperSolver.SolveNext(GameContext);
                    this.LockMouseClick();
                }

            }
            var mouseState = Mouse.GetState(this.Window);
            if(mouseState.LeftButton == ButtonState.Pressed)
            {
                if(!this.mouseClickIsLocked)
                {
                    var x = mouseState.Position.X;
                    var y = mouseState.Position.Y;
                    this.TileHandler.SelectTile(x,y,this.GameContext);

                    this.LockMouseClick();
                }
            }
            if(mouseState.RightButton == ButtonState.Pressed)
            {
                if(!this.mouseClickIsLocked)
                {
                    var x = mouseState.Position.X;
                    var y = mouseState.Position.Y;
                    
                    this.TileHandler.FlagAsBomb(x,y,this.GameContext);

                    this.LockMouseClick();
                }
            }
            if(this.SimmulateOnly)
            {
                var aaa = this.TileHandler.GetNumberOfBombLeft(this.GameContext);
                if(!(aaa > decimal.Zero))
                {
                    Console.WriteLine("End of sim, bombs wrongly exploded is: " + this.MenuBar.GetDeathScore(this.TileHandler.NumberOfDeaths(), this.GameContext.NumberOfBombs) + " %");
                    this.Exit();
                }
                else
                {
                    this.MinesweeperSolver.SolveNext(GameContext);
                }
            }

            if(this.DebugUpdateAllTiles && gameTime.TotalGameTime.Seconds > 3 && this.isFirstTime)
            {
                this.isFirstTime = false;   
                this.TileHandler.ToggleAll();
            }

            //this.CheckIfDead(); //få tillbaka ett gameState ifrån tileHandlern?
            this.UpdateMouseLock(gameTime);
            base.Update(gameTime);
        }

        private bool isFirstTime = true;

        protected override void Draw(GameTime gameTime)
        {
            if(this.SimmulateOnly)
            {
                return;
            }


            this.GraphicsDevice.Clear(Color.CornflowerBlue);

            this._spriteBatch.Begin();
            
            this.MenuBar.Draw(gameTime
                , this._spriteBatch
                , this.spriteFont
                , this.GameContext.NumberOfBombs
                , this.TileHandler.GetNumberOfBombLeft(this.GameContext)
                , this.TileHandler.NumberOfDeaths()
                );
            this.TileHandler.Draw(gameTime, this._spriteBatch, this.spriteFont);

            this._spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool mouseClickIsLocked;
        private float timeSinceLocked;        
        private void UpdateMouseLock(GameTime gameTime)
        {
            if(this.mouseClickIsLocked)
            {
                this.timeSinceLocked += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if(this.timeSinceLocked > s_LockedTime)
                {
                    this.timeSinceLocked = 0;
                    this.mouseClickIsLocked = false;
                }
            }
        }
        private void LockMouseClick()
        {
            this.mouseClickIsLocked = true;
        }
    }
}
