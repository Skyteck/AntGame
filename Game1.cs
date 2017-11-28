using Comora;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace AntGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Camera camera;

        SpriteFont font;
        bool typingMode = false;

        //Managers


        //UI 
        Vector2 mouseClickPos = new Vector2(0, 0);
        bool drawSelectRect = false;
        Texture2D _SelectTex;
        bool BankMode = false;

        List<GameObjects.Ant> AntList;
        List<GameObjects.Ant> SelectedAnts;
        List<GameObjects.Colony> ColonyList;

        List<GameObjects.Colony> BadColonies;
        List<GameObjects.Ant> BadAnts;

        Rectangle SelectRect
        {
            get
            {
                Vector2 currentPos = InputHelper.MouseWorldPos;
                //one corner is always where click was
                //figure out which corner it is by checking where mouse currently is
                //if current mouse is left and up from click then top left of rect is current mouse pos. bottom right is clicked pos
                if(currentPos.X < mouseClickPos.X && currentPos.Y < mouseClickPos.Y)
                {
                    return new Rectangle((int)currentPos.X, (int)currentPos.Y, (int)(mouseClickPos.X - currentPos.X), (int)(mouseClickPos.Y - currentPos.Y));
                }
                //if current mouse is left and down from click top left is mouse click spot y with current mouse pos X
                else if (currentPos.X < mouseClickPos.X && currentPos.Y >= mouseClickPos.Y)
                {
                    return new Rectangle((int)currentPos.X, (int)mouseClickPos.Y, (int)(mouseClickPos.X - currentPos.X), (int)(currentPos.Y - mouseClickPos.Y));
                }
                //if current mouse is right and up then top left is clicked X and currentY                
                else if(currentPos.X > mouseClickPos.X && currentPos.Y < mouseClickPos.Y)
                {
                    return new Rectangle((int)mouseClickPos.X, (int)currentPos.Y, (int)(currentPos.X - mouseClickPos.X), (int)(mouseClickPos.Y - currentPos.Y));
                }
                else
                {

                    return new Rectangle((int)mouseClickPos.X, (int)mouseClickPos.Y, (int)(currentPos.X - mouseClickPos.X), (int)(currentPos.Y - mouseClickPos.Y));

                }
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            AntList = new List<GameObjects.Ant>();
            ColonyList = new List<GameObjects.Colony>();
            SelectedAnts = new List<GameObjects.Ant>();
            BadAnts = new List<GameObjects.Ant>();
            BadColonies = new List<GameObjects.Colony>();
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

            camera = new Camera(GraphicsDevice);


            InputHelper.Init(camera);
            base.Initialize();

        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            _SelectTex = Content.Load<Texture2D>(@"Art/WhiteTexture");
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
            if (this.IsActive)
            {
                // TODO: Add your update logic here
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                {
                    Exit();
                }
                List<Sprite> toKill = new List<Sprite>();

                InputHelper.Update();
                ProcessMouse(gameTime);
                ProcessKeyboard(gameTime);

                foreach (GameObjects.Ant ant in AntList)
                {
                    ant.Update(gameTime);

                    foreach(GameObjects.Ant b in BadAnts)
                    {
                        if(b._BoundingBox.Intersects(ant._BoundingBox))
                        {
                            b._CurrentState = Sprite.SpriteState.kStateInActive;
                            ant._CurrentState = Sprite.SpriteState.kStateInActive;
                        }
                    }

                    foreach(GameObjects.Colony c in BadColonies)
                    {
                        if(c._CurrentState == Sprite.SpriteState.kStateActive && c._BoundingBox.Intersects(ant._BoundingBox))
                        {
                            c.currentHP--;
                            if(c.currentHP <= 0)
                            {
                                c._CurrentState = Sprite.SpriteState.kStateInActive;
                            }
                            ant._CurrentState = Sprite.SpriteState.kStateInActive;
                        }
                    }
                }

                AntList.RemoveAll(x => x._CurrentState == Sprite.SpriteState.kStateInActive);
                BadAnts.RemoveAll(x => x._CurrentState == Sprite.SpriteState.kStateInActive);
                BadColonies.RemoveAll(x => x._CurrentState == Sprite.SpriteState.kStateInActive);

                foreach (GameObjects.Colony c in ColonyList)
                {
                    c.Update(gameTime);

                    if(c.SpawnTimer <= 0.0f)
                    {
                        GameObjects.Ant newAnt = new GameObjects.Ant();
                        newAnt._Position = c._Position;
                        newAnt.SetOrbitPoint(c._Position);
                        newAnt.LoadContent(@"Art/SlimeShot", Content);
                        AntList.Add(newAnt);
                        c.SpawnTimer = 5.0f;
                    }
                }

                foreach (GameObjects.Ant ant in BadAnts)
                {
                    ant.Update(gameTime);
                }

                foreach (GameObjects.Colony c in BadColonies)
                {
                    c.Update(gameTime);
                    if (c.SpawnTimer <= 0.0f)
                    {
                        GameObjects.Ant newAnt = new GameObjects.Ant();
                        newAnt._Position = c._Position;
                        newAnt.SetOrbitPoint(c._Position);
                        newAnt.LoadContent(@"Art/logItem", Content);
                        BadAnts.Add(newAnt);
                        c.SpawnTimer = 5.0f;
                    }
                }


                base.Update(gameTime);
                ProcessCamera(gameTime);

                //Show FPS
                if ((1 / gameTime.ElapsedGameTime.TotalSeconds) <= 59)
                {
                    Console.WriteLine("BAD FPS!!!!!!!!!!");
                }
            }
        }

        private void ProcessKeyboard(GameTime gameTime)
        {

            if (InputHelper.IsKeyPressed(Keys.Space))
            {
                GameObjects.Colony newColony = new GameObjects.Colony();
                newColony._Position = InputHelper.MouseWorldPos;
                newColony.LoadContent(@"Art/Fire", Content);
                ColonyList.Add(newColony);
            }

            if (InputHelper.IsKeyPressed(Keys.H))
            {
                GameObjects.Colony newColony = new GameObjects.Colony();
                newColony._Position = InputHelper.MouseWorldPos;
                newColony.LoadContent(@"Art/LavaTile", Content);
                BadColonies.Add(newColony);
            }
        }

        private void ProcessMouse(GameTime gameTime)
        {
            if (InputHelper.LeftButtonClicked)
            {
                SelectedAnts.Clear();
                mouseClickPos = InputHelper.MouseWorldPos;
            }
            if(InputHelper.LeftButtonHeld)
            {
                drawSelectRect = true;
            }
            if(InputHelper.LeftButtonReleased)
            {
                drawSelectRect = false;
                CheckSelectedAnts();
            }

            if (InputHelper.RightButtonClicked)
            {
                foreach (GameObjects.Ant ant in SelectedAnts)
                {
                    ant.SetOrbitPoint(InputHelper.MouseWorldPos);
                    ant.ChangeOrbit();
                }
                SelectedAnts.Clear();
            }

        }

        private void CheckSelectedAnts()
        {
            foreach(GameObjects.Ant a in AntList)
            {
                if(a._BoundingBox.Intersects(SelectRect))
                {
                    SelectedAnts.Add(a);
                }
            }
            Console.WriteLine(SelectedAnts.Count);
        }

        private void ProcessCamera(GameTime gameTime)
        {

            camera.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // TODO: Add your drawing code here
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(camera);



            foreach (GameObjects.Colony c in ColonyList)
            {
                c.Draw(spriteBatch);
            }

            foreach (GameObjects.Ant ant in AntList)
            {
                ant.Draw(spriteBatch);
            }

            foreach (GameObjects.Colony c in BadColonies)
            {
                c.Draw(spriteBatch);
            }

            foreach (GameObjects.Ant ant in BadAnts)
            {
                ant.Draw(spriteBatch);
            }


            if (drawSelectRect) DrawSelectRect(spriteBatch);

            base.Draw(gameTime);
            spriteBatch.End();

        }

        private void DrawSelectRect(SpriteBatch sb)
        {
            int border = 3;
            Rectangle rect = SelectRect;

            sb.Draw(_SelectTex, new Rectangle(rect.X, rect.Y, border, rect.Height + border), Color.White);
            sb.Draw(_SelectTex, new Rectangle(rect.X, rect.Y, rect.Width + border, border), Color.White);
            sb.Draw(_SelectTex, new Rectangle(rect.X + rect.Width, rect.Y, border, rect.Height + border), Color.White);
            sb.Draw(_SelectTex, new Rectangle(rect.X, rect.Y + rect.Height, rect.Width + border, border), Color.White);
        }
    }

}
