﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

/* ISSUES
 * Getting collisions working
 * Solved by: switching to a hybrid collision technique
 * 
 * Getting stuck on a surface when sliding along it
 * Solved by: Replacing the ifs with else if because it was only checking to see if it was to the left
 * 
 * Player is able to dig right and left even if there is not a tile below
 * Solved by: Checking for a tile below
 * 
 * Player can sit directly in the middle of a block and not move down
 * Solved by: Increasing the intensity of the attraction to the tile when digging
 */
namespace GLOBALWARMINGDENIAL
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GlobalWarmingDenial : Game
    {
        public GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        InfiniteScroller background;
        InfiniteScroller leftWall;
        InfiniteScroller rightWall;

        Player player;
        public World world;
        MouseState mouse;
        KeyboardState keyboard;
        public Vector2 camera = new Vector2(0, 0);

        public GlobalWarmingDenial()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferredBackBufferHeight = 720;
            
            // Center the window on the screen
            this.Window.Position = new Point(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - 640, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - 360);
            graphics.ApplyChanges();
            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            background = new InfiniteScroller(this, 0, 2, 720);
            background.texture = Content.Load<Texture2D>("background");

            Texture2D wall = Content.Load<Texture2D>("wall");

            leftWall = new InfiniteScroller(this, -50, 3, 720);
            rightWall = new InfiniteScroller(this, GraphicsDevice.Viewport.Width - wall.Width + 50, 3, 720);
            leftWall.texture = wall;
            rightWall.texture = wall;

            world = new World(this);
            world.Load(Content);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            player = new Player(this);
            player.texture = Content.Load<Texture2D>("drill2");
            player.animations.Load("Drill_Idle", Content, 55, 60);
            player.animations.Load("Drill_Dig", Content, 61, 78);
            player.animations.Play("Drill_Idle");

            background.texture = Content.Load<Texture2D>("background");
        }

        protected override void UnloadContent()
        {

        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Update the current state of the mouse and keyboard
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Space)) player.position = new Vector2(100, 20);
            player.HandleInput(mouse, keyboard);

            // If mouse is clicked, dig out the specified tile

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                Tile tile = world.GetTile(mouse.Position.ToVector2() - camera);
                if (tile != null) tile.Dig();
            }

            player.Update();
            player.CollideWithWorld(world);
            world.Update();

            float centerOfScreen = GraphicsDevice.Viewport.Height / 5;
            camera.Y += (centerOfScreen - camera.Y - player.position.Y) / 1f;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            background.Draw(spriteBatch);
            world.Draw(spriteBatch);
            player.Draw(spriteBatch);
            leftWall.Draw(spriteBatch);
            rightWall.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
