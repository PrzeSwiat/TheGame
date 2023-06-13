﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using Color = Microsoft.Xna.Framework.Color;

namespace TheGame
{
    public class Game1 : Game
    {
        //DON'T TOUCH IT MORTALS
        int WindowWidth = 1900;
        int WindowHeight = 1000;

        Camera camera;
        EffectHandler effectHandler;
        EffectHandler effectPlayerHandler;
        Serializator serializator;
        Serializator PlayerNames;
        AudioMenager audioMenager;
        //.................


        private BasicEffect basicEffect;
        World world;
        HUD hud;
        InteractionEventHandler interactionEventHandler;
        Player player;
        Player animacyjnaPacynka;
        Enemies enemies;
        LeafList Leafs;
        SoundActorPlayer soundActorPlayer;
        AnimationMenager animationMenager;
       

        public Game1()
        {
            Globals._graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            Globals._graphics.PreferredBackBufferWidth = WindowWidth;
            Globals._graphics.PreferredBackBufferHeight = WindowHeight;
            Globals._graphics.ApplyChanges();
            Globals.Pause = false;
            Globals.Start = true;
            Globals.Tutorial = false;

        }


        protected override void Initialize()
        {
            //DON'T TOUCH IT MORTALS
            camera = new Camera();
            Leafs = new LeafList();
            enemies = new Enemies();
            Globals.projectionMatrix = Matrix.CreateOrthographicOffCenter(-(WindowWidth / 65), (WindowWidth / 65), -(WindowHeight / 65f), (WindowHeight / 65f), -10f, 100f);      // orthographic view 
                                                                                                                                                                              //projectionMatrix = Matrix.CreateOrthographic(20, 20, 1f, 1000f);                      // second type orthographic view

            // PERSPECTIVE point of view
            //projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f),GraphicsDevice.DisplayMode.AspectRatio, 1f, 1000f); // render range (from 1 near playing to 1000 far playing)
            // ................................................................................................................

            // tells the world of our orientantion
            // (the same as:
            // Vector3(0,1,0) - up and down is along y axis)
            Globals.worldMatrix = Matrix.CreateWorld(camera.CamTarget, Vector3.Forward, Vector3.Up);
            //.................

            

            Globals.effectHandler = new EffectHandler(Content.Load<Effect>("ShaderOne"));
            Globals.effectHandler1 = new EffectHandler(Content.Load<Effect>("MainShader"));
            Globals.Score = 0;
            hud = new HUD(WindowWidth, WindowHeight);
            world = new World();
            player = new Player(new Vector3(50,0,60), "Objects/mis", "Textures/MisTexture");
            animacyjnaPacynka = new Player(new Vector3(0, 15, 30), "Objects/mis", "Textures/tekstura");
            serializator = new Serializator("zapis.txt");
            PlayerNames = new Serializator("PlayerNames.txt");
            interactionEventHandler = new InteractionEventHandler(player, enemies.EnemiesList);
            audioMenager = new AudioMenager(Content);
            soundActorPlayer = new SoundActorPlayer(Content, player, enemies.EnemiesList);
            animationMenager = new AnimationMenager(Content, player, enemies.EnemiesList);
            Globals.viewport = GraphicsDevice.Viewport;

            base.Initialize();

        }

        protected override void LoadContent()
        {
            Globals.content = this.Content;

            world.LoadContent();
            Globals.spriteBatch = new SpriteBatch(GraphicsDevice);
            hud.LoadContent();
            basicEffect = new BasicEffect(GraphicsDevice);
            basicEffect.Projection = Globals.projectionMatrix;
            player.LoadContent();
            enemies.LoadModels();
            Leafs.LoadModels();
            animacyjnaPacynka.LoadContent();
            audioMenager.LoadContent();
            soundActorPlayer.LoadContent();
            animationMenager.LoadContent();
            player.OnDestroy += DestroyControl;

        }

        protected override void Update(GameTime gameTime)
        {
            if (Globals.Start)
            {
                hud.name = "";
                if(Globals.Tutorial)
                {
                    TurorialCheck();
                }
                else
                {
                    MainMenuCheck();
                }

                audioMenager.MainPlay();
            }
            else
            {
                PauseCheck();
                audioMenager.MainPlay();
                if (!Globals.Pause)
                {
                    if (Globals.Death)
                    {
                        if (Globals.LeaderBoard)
                        {
                            LeaderBoardCheck();
                        }
                        else
                        {
                            DeathMenuCheck();
                        }
                    }
                    else
                    {
                        var delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
                        Globals.time += delta;
                        camera.CamPosition = player.GetPosition() + camera.CamPositionState;
                        camera.nextpos = player.GetPosition();
                        Globals.viewMatrix = Matrix.CreateLookAt(camera.CamPosition, player.GetPosition(), Vector3.Up);
                        basicEffect.View = Matrix.CreateLookAt(camera.CamPosition, camera.camTracker, Vector3.Up);
                        player.Update(world, delta, enemies,gameTime);
                        enemies.AddEnemies(world.returnEnemiesList(player.GetPosition().X, player.GetPosition().Z));  // czemu w update ???
                        enemies.Move(delta, player);    // i po co 3 funkcje a nie 1
                        enemies.RefreshOnDestroy();
                        Leafs.RefreshInventory(this.player);
                        Leafs.UpdateScene(enemies.EnemiesList,gameTime);
                        camera.Update1(player.GetPosition());


                        interactionEventHandler.Update(enemies.EnemiesList);
                        Globals.viewport = GraphicsDevice.Viewport;
                         
                        animationMenager.Update(gameTime);
                        SaveControl();
                        base.Update(gameTime);
                    }
                }
                else

                    Globals.time = 0;
            }

        }

        protected override void Draw(GameTime gameTime)
        {
            Globals.prevState = GamePad.GetState(PlayerIndex.One);
            Globals.prevDeathState = GamePad.GetState(PlayerIndex.One);
            Globals.prevKeyBoardState = Keyboard.GetState();
            Globals.prevKeyBoardDeathState = Keyboard.GetState();
            if (Globals.Start)
            {
                hud.DrawMainMenu();
                if(Globals.Tutorial)
                {
                    hud.DrawTutorialMenu();
                }
            }
            else
            {
                if (!Globals.Pause)
                {
                    if(Globals.Death)
                    {
                        if(Globals.LeaderBoard)
                        {
                            hud.DrawLeaderBoard();
                        }
                        else
                        {
                            hud.DrawDeathMenu();
                        }
                    }
                    else
                    {
                        GraphicsDevice.Clear(Color.Black);
                        base.Draw(gameTime);
                        GraphicsDevice.DepthStencilState = DepthStencilState.Default;

                        world.Draw(player.GetPosition());
                        player.DrawPlayer(player.GetPosition());

                        enemies.Draw(player.GetPosition());

                        Leafs.Draw(player.GetPosition());
                        animationMenager.DrawAnimations();
                        player.DrawEffectsShadow(player.GetPosition());
                        
                        hud.Update(player.Inventory.returnLeafs(), player.isCrafting(), player.isThrowing(), player.Crafting.returnRecepture());
                        hud.DrawFrontground(player.Health, enemies.EnemiesList);  //hud jako OSTATNI koniecznie
                        Leafs.DrawHud();//Koniecznie ostatnie nawet za Hudem
                        player.DrawAnimation();
                    }

                }
                else
                {
                    hud.DrawPause();
                }
                //DrawBoundingBoxes();
            }


            //}
            // else
            // {
            // hud.DrawMainMenu(_spriteBatch);
            // }
            //DrawBoundingBoxes();
        }

        #region DrawingBB
        public void DrawBoundingBoxes()
        {
            basicEffect.CurrentTechnique.Passes[0].Apply();

            foreach (SceneObject obj in world.GetWorldList())
            {
                //obj.DrawBB();
            }
            foreach (Enemy enemy in enemies.EnemiesList)
            {
                //enemy.DrawBB();
                if (enemy.GetType() == typeof(AppleTree))
                {
                    AppleTree apple1 = (AppleTree)enemy;
                    foreach (Apple apple in apple1.bullet)
                    {
                        //apple.DrawBB();
                    }
                }
                //DrawBS(enemy.boundingSphere.Center, enemy.boundingSphere.Radius);
            }

            player.DrawBB();
            DrawBS(player.boundingSphere.Center, player.boundingSphere.Radius);
        }



        public void DrawBS(Vector3 center, float radius)
        {
            var X = new[]
            {
                new VertexPositionColor(center+new Vector3(radius,0,0), Color.White),
                new VertexPositionColor(center+new Vector3(-radius,0,0), Color.White),

            };

            var Y = new[]
            {
                new VertexPositionColor(center+new Vector3(0,radius,0), Color.White),
                new VertexPositionColor(center+new Vector3(0,-radius,0), Color.White),

            };

            var Z = new[]
            {
                new VertexPositionColor(center+new Vector3(0,0,radius), Color.White),
                new VertexPositionColor(center+new Vector3(0,0,-radius), Color.White),

            };

            var XTOY = new[]
            {

                new VertexPositionColor(center+new Vector3(0,radius,0), Color.White),
                new VertexPositionColor(center+new Vector3(-radius,0,0), Color.White),
                new VertexPositionColor(center+new Vector3(0,-radius,0), Color.White),
                new VertexPositionColor(center+new Vector3(radius,0,0), Color.White),
                new VertexPositionColor(center+new Vector3(0,radius,0), Color.White),
            };

            var ZTOY = new[]
            {

                new VertexPositionColor(center+new Vector3(0,radius,0), Color.White),
                new VertexPositionColor(center+new Vector3(0,0,-radius), Color.White),
                new VertexPositionColor(center+new Vector3(0,-radius,0), Color.White),
                new VertexPositionColor(center+new Vector3(0,0,radius), Color.White),
                new VertexPositionColor(center+new Vector3(0,radius,0), Color.White),
            };


            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, X, 0, 1);
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Y, 0, 1);
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, Z, 0, 1);
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, XTOY, 0, 4);
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, ZTOY, 0, 4);
        }
        #endregion

        #region Controls

        #region PAUSE_CHECK
        void PauseCheck()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState state = Keyboard.GetState();
            if ((gamePadState.Buttons.Start == ButtonState.Pressed && Globals.prevState.Buttons.Start == ButtonState.Released) || (state.IsKeyDown(Keys.Escape) && Globals.prevKeyBoardState.IsKeyUp(Keys.Escape)))
            {
                Globals.Pause = true;
                player.Stop();
            }
            Globals.prevState = gamePadState;
            Globals.prevKeyBoardState = state;

            if (Globals.Pause)
            {
                if ((gamePadState.ThumbSticks.Left.Y >= 0.5f && !(Globals.prevPauseState.ThumbSticks.Left.Y >= 0.5f)) || (state.IsKeyDown(Keys.W) && !Globals.prevKeyBoardPauseState.IsKeyDown(Keys.W)))
                {
                    hud.MenuOption -= 1;
                }
                if ((gamePadState.ThumbSticks.Left.Y <= -0.5f && !(Globals.prevPauseState.ThumbSticks.Left.Y <= -0.5f)) || (state.IsKeyDown(Keys.S) && !Globals.prevKeyBoardPauseState.IsKeyDown(Keys.S)))
                {
                    hud.MenuOption += 1;
                }
                if (hud.MenuOption < 1)
                {
                    hud.MenuOption = 1;
                }
                if (hud.MenuOption > 3)
                {
                    hud.MenuOption = 3;
                }

                Globals.prevPauseState = gamePadState;
                Globals.prevKeyBoardPauseState = state;

                if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 1) //reasume
                {
                    Globals.Pause = false;
                    player.Start();
                }
                if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 2) //mainmenu
                {
                    Globals.Death = false;
                    Globals.Tutorial = false;
                    LoadContent();
                    Initialize();
                    Globals.Start = true;
                }
                if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 3) //exit 
                {
                    Exit();
                }
                
            }
           
        }
        #endregion

        void LeaderBoardCheck()
        {
            if (Globals.LeaderBoard)
            {
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
                KeyboardState state = Keyboard.GetState();

                Globals.prevLeaderState = gamePadState;
                Globals.prevKeyBoardLeaderState = state;


                if ((gamePadState.Buttons.A == ButtonState.Pressed && Globals.prevDeathState.Buttons.A == ButtonState.Released || (state.IsKeyDown(Keys.Enter) && Globals.prevKeyBoardDeathState.IsKeyUp(Keys.Enter)))) //ACCEPT
                {
                    Window.TextInput -= hud.TextInputHandler;
                    PlayerNames.SavePlayerName(hud.name);
                    Globals.LeaderBoard = false;
                    Globals.Death = false;
                    Globals.Tutorial = false;
                    LoadContent();
                    Initialize();
                    Globals.Pause = false;
                    Globals.Start = true;
                }
            }
        }


        #region DEATH_CHECK
        void DeathMenuCheck()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState state = Keyboard.GetState();
            if ((gamePadState.ThumbSticks.Left.Y >= 0.5f && !(Globals.prevDeathState.ThumbSticks.Left.Y >= 0.5f)) || (state.IsKeyDown(Keys.W) && !Globals.prevKeyBoardDeathState.IsKeyDown(Keys.W)))
            {
                hud.MenuOption -= 1;
            }
            if ((gamePadState.ThumbSticks.Left.Y <= -0.5f && !(Globals.prevDeathState.ThumbSticks.Left.Y <= -0.5f)) || (state.IsKeyDown(Keys.S) && !Globals.prevKeyBoardDeathState.IsKeyDown(Keys.S)))
            {
                hud.MenuOption += 1;
            }
            if (hud.MenuOption < 1)
            {
                hud.MenuOption = 1;
            }
            if (hud.MenuOption > 4)
            {
                hud.MenuOption = 4;
            }

            Globals.prevDeathState = gamePadState;
            Globals.prevKeyBoardDeathState = state;


            if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 1) //leader board
            {
                Window.TextInput += hud.TextInputHandler;
                Globals.LeaderBoard = true;

            }
            if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 2) //try again 
            {
                Globals.Death = false;
                Globals.Tutorial = false;

                LoadContent();
                Initialize();
                player.Start();

            }
            if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 3) // main menu
            {
                Globals.Death = false;
                Globals.Tutorial = false;
                LoadContent();
                Initialize();
                Globals.Pause = false;
                Globals.Start = true;
            }
            if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 4) // exit
            {
                Exit();
            }
        }
        #endregion

        #region MENU_CHECK
        void MainMenuCheck()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState state = Keyboard.GetState();
            if(Globals.Start)
            {
                if ((gamePadState.ThumbSticks.Left.Y >= 0.5f && !(Globals.prevState.ThumbSticks.Left.Y >= 0.5f)) || (state.IsKeyDown(Keys.W) && !Globals.prevKeyBoardState.IsKeyDown(Keys.W)))
                {
                    hud.MenuOption -= 1;
                }
                if ((gamePadState.ThumbSticks.Left.Y <= -0.5f && !(Globals.prevState.ThumbSticks.Left.Y <= -0.5f)) || (state.IsKeyDown(Keys.S) && !Globals.prevKeyBoardState.IsKeyDown(Keys.S)))
                {
                    hud.MenuOption += 1;
                }
                if (hud.MenuOption < 1)
                {
                    hud.MenuOption = 1;
                }
                if (hud.MenuOption > 3)
                {
                    hud.MenuOption = 3;
                }
                Globals.prevState = gamePadState;
                Globals.prevKeyBoardState = state;
                if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 1)
                {
                    Globals.Tutorial = true;
                }
                if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 3)
                {
                    Exit();
                }
                if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 2)
                {
                    // notimplemented
                }

                
            }

        }
        #endregion

        #region TUTORIAL_CHECK
        void TurorialCheck()
        {
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState state = Keyboard.GetState();

            if (Globals.Tutorial)
            {
                if ((gamePadState.ThumbSticks.Left.Y >= 0.5f && !(Globals.prevTutorialState.ThumbSticks.Left.Y >= 0.5f)) || (state.IsKeyDown(Keys.W) && !Globals.prevKeyBoardTutorialState.IsKeyDown(Keys.W)))
                {
                    hud.MenuOption -= 1;
                }
                if ((gamePadState.ThumbSticks.Left.Y <= -0.5f && !(Globals.prevTutorialState.ThumbSticks.Left.Y <= -0.5f)) || (state.IsKeyDown(Keys.S) && !Globals.prevKeyBoardTutorialState.IsKeyDown(Keys.S)))
                {
                    hud.MenuOption += 1;
                }
                if (hud.MenuOption < 1)
                {
                    hud.MenuOption = 1;
                }
                if (hud.MenuOption > 2)
                {
                    hud.MenuOption = 2;
                }
                Globals.prevTutorialState = gamePadState;
                Globals.prevKeyBoardTutorialState = state;

                if ((gamePadState.Buttons.A == ButtonState.Pressed && Globals.prevState.Buttons.A == ButtonState.Released|| (state.IsKeyDown(Keys.Enter)) && Globals.prevKeyBoardDeathState.IsKeyUp(Keys.Enter)) && hud.MenuOption == 1)
                {
                    player.Start();
                    Globals.Start = false;
                }
                if ((gamePadState.Buttons.A == ButtonState.Pressed || state.IsKeyDown(Keys.Enter)) && hud.MenuOption == 2)
                {
                  
                }
            }

        }
        #endregion 

        void SaveControl()
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.P))
            {
                serializator.SavePlayer(player);
            }
            if (state.IsKeyDown(Keys.O))
            {
                Player copied;
                copied = serializator.LoadPlayer();
                if (copied != null)
                {
                    player = copied;
                    player.LoadContent();
                }

            }
        }

        void DestroyControl(object obj, EventArgs e)
        {
            if (obj is Player)
            {
                //Exit();
                Globals.Death = true;
                // GAME OVER HERE;
            }

        }
        #endregion

    }

}