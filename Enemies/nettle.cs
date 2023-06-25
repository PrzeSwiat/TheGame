﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TheGame
{
    internal class Nettle : Enemy
    {
        private float movementCooldown = 2f;
        private float windUpCooldown = 0.5f;
        float armortime = 10; // not used

        private DateTime lastAttackTime, actualTime;
        public event EventHandler OnAttack;
        private int attackCounter = 0; // not used
        private float movementTimer = 0f;
        private float windUpTimer = 0f;
        private bool isMoving = false;
        private bool isWindingUp = false;
        Vector3 playerPosition;
        Vector3 lastPlayerPosition;
        Vector3 directionTowardsTarget;
        int lastHealth; // not used
        private float deltaTime;
        public Nettle(Vector3 worldPosition, string modelFileName, string textureFileName) : base(worldPosition, modelFileName, textureFileName)
        {
            AssignParameters(6, 12, 25, 1f);
            this.setBSRadius(2.5f);
            this.shadow.SetScale(0.95f);
            this.visionRange = 25f;
            this.leaf = new Leafs.NettleLeaf(worldPosition, "Objects/nettle_pickup", "Textures/nettle_pickup");
            lastHealth = this.Health;
        }


        public override void Update(float deltaTime, Player player)
        {
            this.deltaTime = deltaTime;
            base.Update();
            playerPosition = player.GetPosition();
            HandleStunnedStatus(deltaTime);
            if (isStunned)
            {
                return;
            }
            HandleSlowedStatus(deltaTime);

            movementTimer += deltaTime;
            if (CheckDistanceAndMovementCooldown(player) && !isMoving && !isWindingUp)
            {
                lastPlayerPosition = playerPosition;
                isWindingUp = true;
                directionTowardsTarget = lastPlayerPosition - this.GetPosition();
                directionTowardsTarget.Normalize();
                this.Direction = new Vector2(directionTowardsTarget.X, directionTowardsTarget.Z);
                RotateTowardsCurrentDirection();
            }
            if (isWindingUp)
            {
                WindUp();
            }
            if (isMoving)
            {
                Movement(player);
            }
            this.shadow.UpdatingEnemy(this.GetPosition(), new Vector3(1.8f, 0, -1.9f));
        }

        private void WindUp()
        {
            windUpTimer += deltaTime;
            if (windUpTimer < windUpCooldown)
                return;
         
            windUpTimer = 0f;
            isWindingUp = false;
            isMoving = true;
        }

        private void Movement(Player player)
        {
            CheckCollision(player);
            if (Collides)
            {
                StopMovement();
                Debug.WriteLine("COLLISION");
                OnAttack?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                CheckArrivedAtTargetPosition(lastPlayerPosition);
                MoveForwards(deltaTime, true);
            }
        }

        private bool CheckArrivedAtTargetPosition(Vector3 targetPosition)
        {
            bool isCloseEnough = Vector3.Distance(lastPlayerPosition, this.GetPosition()) < 1f;

            if (!isCloseEnough)
                return false;

            StopMovement();
            return true;
        }

        private void StopMovement()
        {
            isMoving = false;
            movementTimer = 0f;
        }

        private bool CheckDistanceAndMovementCooldown(Player player)
        {
            float distanceToTarget = Vector3.Distance(player.GetPosition(), this.GetPosition());
            if (distanceToTarget < visionRange && movementTimer > movementCooldown)
                return true;
            else
                return false;
        }


        public override void LoadContent()
        {
            base.LoadContent();
            float vlll = 2f;
            BoundingBox helper;
            helper.Min = new Vector3(-vlll, 0, -vlll);
            helper.Max = new Vector3(vlll, 4, vlll);

            SetBoundingBox(new BoundingBox(helper.Min + this.GetPosition(), helper.Max + this.GetPosition()));
        }

        protected override void Attack(Player player)
        {
            actualTime = DateTime.Now;
            TimeSpan time = actualTime - lastAttackTime;
            if (time.TotalSeconds > 1)
            {
                OnAttack?.Invoke(this, EventArgs.Empty);
                lastAttackTime = actualTime;
                player.Hit(this.Strength);
            }
        }
    }
}
