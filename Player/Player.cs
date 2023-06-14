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
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;
using TheGame.Core;

namespace TheGame
{
    [Serializable]
    internal class Player : Creature
    {
        public Inventory Inventory;
        public Crafting Crafting;
        private DateTime lastAttackTime;
        private DateTime actualTime;
        private PlayerMovement playerMovement;
        private PlayerEffectHandler playerEffects;
        public event EventHandler OnAttackPressed;
        private List<Apple> apples = new List<Apple>();
        private List<NettleLeaf> nettles = new List<NettleLeaf>();
        private List<MintLeaf> mints = new List<MintLeaf>();
        public Shadow shadow;
        private bool canMove = true;
        private bool stunEnemies = false;
        public bool immortal = false;

        public event EventHandler onMove;
        public event EventHandler onRandomNoise;

        public Player(Vector3 Position, string modelFileName, string textureFileName) : base(Position, modelFileName, textureFileName)
        {
            SetScale(1.2f);
            this.setBSRadius(3);
            setBSposition(2);

            AssignParameters(300, 1, 20, 0.5f);
            playerEffects = new PlayerEffectHandler(this);
            Inventory = new Inventory();
            Crafting = new Crafting(Inventory, playerEffects);
            playerMovement = new PlayerMovement(this);
            shadow = new Shadow(this.GetPosition());
            // Uruchomienie timera
            //playerEffects.Start();
            
        }
        public void Start()
        {
            playerEffects.Start();
        }

        public void Stop()
        {
            playerEffects.Stop();
        }

        public void Update(World world, float deltaTime, Enemies enemies,GameTime gametime) //Logic player here
        {
            Update();
            playerMovement.UpdatePlayerMovement(world, deltaTime);
            Crafting.Update(gametime);

            foreach (Apple apple in apples.ToList())
            {
                apple.Update(deltaTime, enemies);
            }
            foreach (NettleLeaf nettle in nettles.ToList())
            {
                nettle.Update(deltaTime, enemies);
            }
            foreach (MintLeaf mint in mints.ToList())
            {
                mint.Update(deltaTime, enemies);
            }

            if (playerMovement.isMoving)
            {
                onMove?.Invoke(this, EventArgs.Empty);
            }

            if (stunEnemies)
            {
                StunEnemies(enemies);
                stunEnemies = false;
            }

            Random random = new Random();
            int rand = random.Next(0, 1000);
            if(rand == 0)
            {
                onRandomNoise?.Invoke(this, EventArgs.Empty);
            }
            shadow.UpdatingPlayer(this.GetPosition());

        }
        public override void Hit(int damage)
        {
            if (!immortal)
            {
                base.Hit(damage);
                if (this.Health <= 0)
                {
                    playerEffects.Stop();
                }  
            }
        }

        public override void LoadContent()
        {
            base.LoadContent();
            shadow.LoadContent();
            Crafting.LoadContent();
        }

        public override void DrawPlayer(Vector3 lightpos)
        {
            foreach (NettleLeaf nettle in nettles)
            {
                nettle.Draw(lightpos);
            }

            foreach (MintLeaf mint in mints.ToList())
            {
                mint.Draw(lightpos);
            }

            foreach (Apple apple in apples)
            {
                apple.Draw(lightpos);
            }
        }
        public void DrawEffectsShadow(Vector3 lightpos)
        {
            shadow.SetPositionY(-0.99f);
            shadow.Draw(lightpos);
        }
        public void DrawAnimation()
        {
            Crafting.DrawAnimation();
        }

        public void Attack()
        {
            actualTime = DateTime.Now;
            TimeSpan time = actualTime - lastAttackTime;
            if (time.TotalSeconds > this.getAttackSpeed())
            {
                OnAttackPressed?.Invoke(this, EventArgs.Empty);
                lastAttackTime = actualTime;
            }
        }

        public bool isCrafting() { return playerMovement.isCraftingTea; }
        public bool isThrowing() { return playerMovement.isThrowing; }

        public void ThrowApple()
        {
            if(Inventory.checkAppleLeafNumber())
            {
                Inventory.removeAppleLeaf();
                Apple apple = new Apple(this.GetPosition(), this.getLookingDirection());
                apple.LoadContent();
                apple.OnDestroy += RemoveApple;
                apples.Add(apple);
            }
        }

        public void ThrowNettle()
        {
            if (Inventory.checkNettleLeafNumber()) 
            {
                Inventory.removeNettleLeaf();
                NettleLeaf nettle = new NettleLeaf(GetPosition(), 1, 10);
                nettle.OnDestroy += RemoveNettle;
                nettle.LoadContent();
                nettle.SetScale(2);
                nettles.Add(nettle);

            }
        }

        public void ThrowMint()
        {
            if (Inventory.checkMintLeafNumber())
            {
                Inventory.removeMintLeaf();
                MintLeaf mint = new MintLeaf(GetPosition(), 0.5f, 10);
                mint.OnDestroy += RemoveMint;
                mint.LoadContent();
                mint.SetScale(2);
                mints.Add(mint);
            }
        }

        public void ThrowMelise()
        {
            if (Inventory.checkMeliseLeafNumber())
            {
                Inventory.removeMeliseLeaf();
                stunEnemies = true;
            }
        }

        private void StunEnemies(Enemies enemies)
        {
            foreach (Enemy enemy in enemies.EnemiesList) 
            {
                if (this.boundingSphere.Intersects(enemy.boundingBox))
                {
                    enemy.Stun(10);
                }
            }
        }

        public void AddIngredientA()
        {
            Crafting.addIngredient('A');
        }
        public void AddIngredientB()
        {
            Crafting.addIngredient('B');
        }
        public void AddIngredientX()
        {
            Crafting.addIngredient('X');
        }
        public void AddIngredientY()
        {
            Crafting.addIngredient('Y');
        }

        public void setOriginalSpeed()
        {
            this.ActualSpeed = MaxSpeed;
        }

        public void setStun(int time)
        {
            playerEffects.Stun(time);
        }

        public void setOriginalStrenght()
        {
            this.Strength = this.MaxStrength;
        }

        public void PlayerRegenarateHealth(int hp, int time)
        {
            playerEffects.RegenarateHP(hp, time);
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
        public bool getcanMove()
        {
            return this.canMove;
        }
        public void setcanMove(bool can)
        {
            this.canMove=can;
        }

        public void RemoveApple(object sender, EventArgs e)
        {
            apples.Remove((Apple)sender);
        }

        public void RemoveNettle(object sender, EventArgs e)
        {
            nettles.Remove((NettleLeaf)sender);
        }
        public void RemoveMint(object sender, EventArgs e)
        {
            mints.Remove((MintLeaf)sender);
        }

        private class MintLeaf : SceneObject
        {
            public BoundingSphere BSphere;
            private float slowness;
            private float maxTime, elapsedTime, interval;
            private DateTime lastIntervalTime;
            public event EventHandler OnDestroy;

            public MintLeaf(Vector3 position, float slow, float maxTime) : base(position, "Objects/test", "Textures/slow")
            {
                this.SetPositionY(-2f);
                this.BSphere = new BoundingSphere(this.GetPosition(), 6);

                this.slowness = slow;
                this.maxTime = maxTime;
                this.elapsedTime = 0;
                this.interval = 1;
                this.lastIntervalTime = DateTime.Now;
            }

            public void Update(float delta, Enemies enemies)
            {
                elapsedTime += delta;
                DateTime actualTime = DateTime.Now;

                if (elapsedTime < maxTime)
                {
                    TimeSpan span = actualTime - lastIntervalTime;
                    if (span.TotalSeconds >= interval)
                    {
                        lastIntervalTime = actualTime;

                        foreach (Enemy enemy in enemies.EnemiesList.ToList())
                        {
                            if (this.BSphere.Intersects(enemy.boundingBox))
                            {
                                enemy.Slow(this.slowness);
                            } 
                        }

                    }

                }
                else
                {
                    OnDestroy?.Invoke(this, EventArgs.Empty);
                }
            }

        }


        private class NettleLeaf : SceneObject
        {
            public BoundingSphere BSphere;
            private int damage;
            private float maxTime, elapsedTime, interval;
            private DateTime lastIntervalTime;
            public event EventHandler OnDestroy;

            public NettleLeaf(Vector3 position, int damage, float maxTime) : base(position, "Objects/test", "Textures/bul")
            {
                SetPositionY(-1.99f);
                this.BSphere = new BoundingSphere(GetPosition(), 6);

                this.damage = damage;
                this.maxTime = maxTime;
                this.elapsedTime = 0;
                this.interval = 1;
                this.lastIntervalTime = DateTime.Now;
            }

            public void Update(float delta, Enemies enemies)
            {
                elapsedTime += delta;
                DateTime actualTime = DateTime.Now;

                if (elapsedTime < maxTime)
                {
                    TimeSpan span = actualTime - lastIntervalTime;
                    if (span.TotalSeconds >= interval)
                    {
                        lastIntervalTime = actualTime;

                        foreach(Enemy enemy in enemies.EnemiesList.ToList())
                        {
                            if (this.BSphere.Intersects(enemy.boundingBox))
                            {
                                enemy.Hit(damage);
                            }
                        }

                    }

                } else
                {
                    OnDestroy?.Invoke(this, EventArgs.Empty);
                }
            }

          

        }

        public BoundingBox[] returnApplesBB()
        {
            BoundingBox[] boundingBoxes = new BoundingBox[apples.Count];
            for (int i = 0; i < apples.Count; i++)
            {
                boundingBoxes[i] = apples[i].returnBB();
            }
            return boundingBoxes;
        }

        private class Apple : SceneObject
        {
            private Vector3 velocity;
            private int dmg = 100;
            private float speed = 35f;
            private float time = 0, maxTime = 4;

            public event EventHandler OnDestroy;

            public Apple(Vector3 Position, Vector2 direction) : base(Position, "Objects/japco", "Textures/appleTexture")
            {
                Vector3 startPosition = Position;
                startPosition.Y += 3;
                this.SetPosition(startPosition);

                SetScale(1.4f);
                velocity = new Vector3(-direction.X * speed, 0, -direction.Y * speed);
            }

            public void Update(float deltaTime, Enemies enemies)
            {
                time += deltaTime;
                // Move the bullet
                this.Move(velocity * deltaTime);

                // Check if the bullet has collided with the player
                foreach (Enemy enemy in enemies.EnemiesList.ToList())
                {
                    if (this.boundingBox.Intersects(enemy.boundingBox))
                    {
                        enemy.Hit(dmg);
                        OnDestroy?.Invoke(this, EventArgs.Empty);
                    }
                }

                // Check if the bullet has hit the ground
                if (time >= maxTime)
                {
                    OnDestroy?.Invoke(this, EventArgs.Empty);
                }
            }

            public BoundingBox returnBB()
            {
                return this.boundingBox;
            }

        }


    }
}