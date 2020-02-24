using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chip8_Emulator
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Chip8 chip8;
        Keys[] mappedKeys;
        Texture2D pixel;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            chip8 = new Chip8();
            chip8.LoadProgram(@"invaders.c8");
            LoadKeyMapping();

            pixel = new Texture2D(GraphicsDevice,1,1);
            pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            for (int i = 0; i < 10; i++)
            {
                HandleInput();
                chip8.cycle();
            }
            base.Update(gameTime);
        }

        private void LoadKeyMapping()
        {
            mappedKeys = new Keys[16];
            mappedKeys[0x1] = Keys.D1;
            mappedKeys[0x2] = Keys.D2;
            mappedKeys[0x3] = Keys.D3;
            mappedKeys[0xC] = Keys.D4;
            mappedKeys[0x4] = Keys.Q;
            mappedKeys[0x5] = Keys.W;
            mappedKeys[0x6] = Keys.E;
            mappedKeys[0xD] = Keys.R;
            mappedKeys[0x7] = Keys.A;
            mappedKeys[0x8] = Keys.S;
            mappedKeys[0x9] = Keys.D;
            mappedKeys[0xE] = Keys.F;
            mappedKeys[0xA] = Keys.Z;
            mappedKeys[0x0] = Keys.X;
            mappedKeys[0xB] = Keys.C;
            mappedKeys[0xF] = Keys.V;
        }

        private void HandleInput()
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < mappedKeys.Length; i++)
                if (Keyboard.GetState().IsKeyDown(mappedKeys[i])) {
                    chip8.SetKeyState(mappedKeys[i], (char)1);
                } else if (Keyboard.GetState().IsKeyUp(mappedKeys[i])) {
                    chip8.SetKeyState(mappedKeys[i], (char)0);
                }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (!chip8.drawFrame)
            {
                return;
            }

            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Black);
            bool[,] screen = chip8.screen;
            spriteBatch.Begin();
            for (int x = 0; x < screen.GetLength(0); x++)
            {
                for (int y = 0; y < screen.GetLength(1); y++)
                {
                    if (screen[x, y])
                    {
                        spriteBatch.Draw(pixel, new Vector2(x, y), null, Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(pixel, new Vector2(x, y), null, Color.Black);
                    }
                }
            }
            spriteBatch.End();
            chip8.drawFrame = false;

            base.Draw(gameTime);

        }
    }
}
