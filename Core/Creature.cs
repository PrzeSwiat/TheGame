﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Diagnostics;
using TheGame.Core;
using TheGame.Leafs;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace TheGame
{
    [Serializable]
    internal class Creature : SceneObject
    {
        public int maxHealth, health;
        private int maxStrenght, strenght;
        private float maxSpeed, actualSpeed;
        private float attackSpeed, actualAttackSpeed;
        public Leaf leaf;
        public Vector2 direction;
        public BoundingSphere boundingSphere;
        private float sphereRadius = 0;
        private float distanceFromCenter = 0;
        public bool canDestroy = true;
        public event EventHandler OnDestroy;
        public int score = 0;


        public Creature(Vector3 worldPosition, string modelFileName, string textureFileName) : base(worldPosition, modelFileName, textureFileName)
        {
            leaf = new Leaf(worldPosition, "Objects/mis4", "Textures/StarSparrow_Orange");
            boundingSphere = BoundingSphere.CreateFromBoundingBox(this.boundingBox);
        }

        public void AssignParameters(int health, int strenght, float speed, float attackSpeed)
        {
            this.maxHealth = health;
            this.health = this.maxHealth;
            this.maxStrenght = strenght;
            this.strenght = this.maxStrenght;
            this.maxSpeed = speed;
            this.actualSpeed = this.maxSpeed;
            this.attackSpeed = attackSpeed;
            this.actualAttackSpeed = this.attackSpeed;
        }

        public virtual void Hit(int damage)
        {
            health -= damage;

            if (health <= 0)
            {
                leaf.AddToWorld();
                leaf.SetPosition(this.GetPosition());
                OnDestroy?.Invoke(this, EventArgs.Empty);
                Globals.Score += score * Globals.ScoreMultiplier;
            }
        }

        public void MoveModelForwards(float speed)
        {
            this.SetPosition(this.GetPosition() + new Vector3(direction.X, 0f, direction.Y) * speed);
        }

        public double GetDistance(SceneObject entity)
        {
            double distance = (double)Vector3.Distance(this.GetPosition(), entity.GetPosition());
            return distance;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            boundingSphere.Radius = this.sphereRadius;
            if (distanceFromCenter != 0)
            {
                boundingSphere.Center += this.GetPosition() + new Vector3(0, 3, distanceFromCenter);
            }
            else
            {
                boundingSphere.Center += this.GetPosition() + new Vector3(0, 3, 3);

            }

        }

        // -------------- G E T T E R S --------------------

        public void MoveBoundingSphere(Vector3 vec)
        {
            this.boundingSphere.Center -= vec;
        }

        public float ActualAttackSpeed
        {
            get { return this.actualAttackSpeed; }
            set { this.actualAttackSpeed = value; }
        }

        public float AttackSpeed
        {
            get { return this.attackSpeed; }
            set { this.attackSpeed = value; }
        }

        public int MaxHealth
        {
            get { return this.maxHealth; }
            set { this.maxHealth = value; }
        }

        public int Health
        {
            get { return this.health; }
            set { this.health = value; }
        }

        public int Strength
        {
            get { return this.strenght; }
            set { this.strenght = value; }
        }

        public int MaxStrength
        {
            get { return this.maxStrenght; }
            set { this.maxStrenght = value; }
        }

        public float ActualSpeed
        {
            get { return this.actualSpeed; }
            set { this.actualSpeed = value; }
        }

        public float MaxSpeed
        {
            get { return this.maxSpeed; }
            set { this.maxSpeed = value; }
        }

        public Vector2 Direction
        {
            get { return this.direction; }
            set { this.direction = value; }
        }

        public void setDirectionX(float x)
        {
            this.direction.X = x;
        }

        public void setDirectionY(float y)
        {
            this.direction.Y = y;
        }
        public void setBSRadius(float radius)
        {
            this.sphereRadius = radius;
            this.distanceFromCenter = radius;
        }

        public void setBSposition(float distance)
        {
            this.distanceFromCenter = distance;
        }

        public void NormalizeDirection()
        {
            this.direction.Normalize();
        }
        public virtual Leaf GetLeaf()
        {
            return this.leaf;
        }
        public float getAttackSpeed()
        {
            return this.attackSpeed;
        }

        public Vector2 getLookingDirection()
        {
            float dx = (float)Math.Cos(this.rotation.Y);
            float dy = (float)Math.Sin(this.rotation.Y);
            return new Vector2(-dy, -dx);
        }

        public float getRotationY()
        {
            return this.rotation.Y;
        }
    

    }
}
