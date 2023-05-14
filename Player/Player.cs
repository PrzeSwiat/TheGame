﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

namespace TheGame
{
    [Serializable]
    internal class Player : Creature
    {
        public Inventory Inventory;
        private Crafting craft = new Crafting();
        public bool canMove = true;
        //private float StunTimer = 0;
        private DateTime lastAttackTime, actualTime;
        private float atackSpeed = 0.5f;

        public string test = "";
        private PlayerMovement playerMovement;
        private PlayerEffectHandler playerEffects;
        public event EventHandler OnAttackPressed;

        private Boolean isCraftingTea;
        private Boolean padButtonAClicked;
        private Boolean padButtonBClicked;
        private Boolean padButtonXClicked;
        private Boolean padButtonYClicked;

        MouseState lastMouseState, currentMouseState;

        public Player(Vector3 Position, string modelFileName, string textureFileName) : base(Position, modelFileName, textureFileName)
        {
            SetScale(1f);
             Inventory = new Inventory();
            AssignParameters(200, 20, 20);
            playerMovement = new PlayerMovement(this);
            isCraftingTea = false;
            padButtonAClicked = false;
            padButtonYClicked = false;
            padButtonXClicked = false;
            
            //Matrix bs = Matrix.Transpose();
            //this.boundingSphere.Transform()
            //this.boundingSphere.Radius = 
            // Dołączenie metody, która będzie wykonywana przy każdym ticku timera
            playerEffects = new PlayerEffectHandler(this);
            // Uruchomienie timera
            playerEffects.Start();
        }

        public void Update(World world, float deltaTime) //Logic player here
        {
            Update();

            playerMovement.UpdatePlayerMovement(world, deltaTime);

            GamePadClick();
        }

        public void setStun(int time)
        {
            playerEffects.Stun(time);
        }

        public void AddHealth(int amount)
        {
            if (this.Health + amount > this.MaxHealth)
            {
                this.Health = this.MaxHealth;
            }
            else { this.Health += amount; }
        }

        public void SubstractHealth(int amount)
        {
            if (this.Health - amount < 0)
            {
                this.Health = 0;
            }
            else { this.Health -= amount; }
        }



        #region Getters
        //GET'ERS

        //.................
        #endregion

        #region Setters
        //SET'ERS

        #endregion
        //.................


        public void PlayerRegenarateHealth(int hp, int time)
        {
            playerEffects.RegenarateHP(hp, time);
        }


      


        public void GamePadClick()
        {
            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);
            if (capabilities.IsConnected)
            {
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
                // Button A
                if (capabilities.HasAButton)
                {
                    if (gamePadState.IsButtonDown(Buttons.A))
                    {
                        if (isCraftingTea && !padButtonAClicked) // W momencie jak lewy triger jest wcisniety
                        {

                            if(this.Inventory.checknettleLeafNumber())
                            {
                                if (craft.howLong() < 3)
                                {
                                    this.Inventory.removeNettleLeaf();
                                    craft.addA();
                                    Debug.Write(craft.recepture);
                                }
                                if(craft.howLong() >= 3)
                                {
                                    craft.makeTea(this.Inventory);
                                }
                            }
                            padButtonAClicked = true;

                        } else // normalny atak gracza
                        {

                            actualTime = DateTime.Now;
                            TimeSpan time = actualTime - lastAttackTime;
                            if (time.TotalSeconds > atackSpeed)
                            {
                                OnAttackPressed?.Invoke(this, EventArgs.Empty);
                                lastAttackTime = actualTime;
                            }
                        }
                        
                    } else if (gamePadState.IsButtonUp(Buttons.A)) {
                        padButtonAClicked = false;
                    }
                }

                // Button B
                if (capabilities.HasBButton)
                {
                    if (gamePadState.IsButtonDown(Buttons.B))
                    {
                        if (isCraftingTea && !padButtonBClicked)
                        {
                            // tutaj dodac skladnik do tworzenia herbaty pod B

                            if (this.Inventory.checkmintLeafNumber())
                            {
                                if (craft.howLong() < 3)
                                {
                                    this.Inventory.removeMintLeaf();
                                    craft.addB();
                                    Debug.Write(craft.recepture);
                                }
                                if (craft.howLong() >= 3)
                                {
                                    craft.makeTea(this.Inventory);
                                }
                            }
                            padButtonBClicked = true;
                            

                        }
                        else
                        {
                            // tutaj ewentualny dash/przewrot
                        }

                    }
                    else if (gamePadState.IsButtonUp(Buttons.B))
                    {
                        padButtonBClicked = false;
                    }
                }
                if (capabilities.HasBButton)
                {
                    if (gamePadState.IsButtonDown(Buttons.Y))
                    {
                        if (isCraftingTea && !padButtonYClicked)
                        {
                            if (this.Inventory.checkappleLeafNumber())
                            {
                                if (craft.howLong() < 3)
                                {
                                    this.Inventory.removeAppleLeaf();
                                    craft.addY();
                                    Debug.Write(craft.recepture);
                                }
                                if (craft.howLong() >= 3)
                                {
                                    craft.makeTea(this.Inventory);
                                }
                            }
                            padButtonYClicked = true;


                        }
                        else
                        {
                            // tutaj ewentualny dash/przewrot
                        }

                    }
                    else if (gamePadState.IsButtonUp(Buttons.Y))
                    {
                        padButtonBClicked = false;
                    }
                }
                if (capabilities.HasBButton)
                {
                    if (gamePadState.IsButtonDown(Buttons.X))
                    {
                        if (isCraftingTea && !padButtonXClicked)
                        {
                            if (this.Inventory.checkMelissaLeafNumber())
                            {
                                if (craft.howLong() < 3)
                                {
                                    this.Inventory.removeMelissaLeaf();
                                    craft.addX();
                                    Debug.Write(craft.recepture);
                                }
                                if (craft.howLong() >= 3)
                                {
                                    craft.makeTea(this.Inventory);
                                }
                            }
                            padButtonXClicked = true;


                        }
                        else
                        {
                            // tutaj ewentualny dash/przewrot
                        }

                    }
                    else if (gamePadState.IsButtonUp(Buttons.X))
                    {
                        padButtonXClicked = false;
                    }
                }


                if (gamePadState.IsButtonDown(Buttons.LeftTrigger)) 
                { 
                    isCraftingTea = true;
                } else if (gamePadState.IsButtonUp(Buttons.LeftTrigger))
                {
                    craft.makeTea(this.Inventory);
                    isCraftingTea = false;
                }




            } else
            {
                lastMouseState = currentMouseState;

                // Get the mouse state relevant for this frame
                currentMouseState = Mouse.GetState();

                // Recognize a single click of the left mouse button
                if (lastMouseState.LeftButton == ButtonState.Released && currentMouseState.LeftButton == ButtonState.Pressed)
                {
                    actualTime = DateTime.Now;
                    TimeSpan time = actualTime - lastAttackTime;
                    if (time.TotalSeconds > atackSpeed)
                    {
                        OnAttackPressed?.Invoke(this, EventArgs.Empty);
                        lastAttackTime = actualTime;
                    }
                }

            }
        }

    }
}