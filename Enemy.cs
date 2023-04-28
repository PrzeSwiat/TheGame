﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TheGame
{
    internal class Enemy : Creature
    {
        public event EventHandler OnAttack;

        private DateTime lastAttackTime, actualTime;
        private bool collides = false;


        public Enemy(Vector3 worldPosition, string modelFileName, string textureFileName) : base(worldPosition, modelFileName, textureFileName)
        {
            AssignParameters(100, 10, 2);
            Direction = new Vector2(0, 0);
            lastAttackTime = DateTime.Now;
            actualTime = lastAttackTime;
            SetScale(0.5f);
            
        }

        private void Attack()
        {
            actualTime = DateTime.Now;
            TimeSpan time = actualTime - lastAttackTime;
            if (time.TotalSeconds > 1)
            {
                OnAttack?.Invoke(this, EventArgs.Empty);
                lastAttackTime = actualTime;
            }
        }

        private void checkCollision(Player player)
        {
            if (this.boundingSphere.Intersects(player.boundingBox) == true) collides = true;
            else collides = false;
        }

        public virtual void Update(float deltaTime, Player player)
        {
            Update();
            checkCollision(player);
            RotateTowardsPlayer(player.GetPosition());
            if (collides)
            {
                Attack();
            }
            else
            {
                //FollowPlayer(player.GetPosition(), deltaTime, true);
                MoveForwards(deltaTime, true);
            }
        }

        //private Vector2 FlockBehaviour(Enemy enemy, double distance, double power)
        //{
        //    IEnumerable<Enemy> query = enemiesList.Where(x => x.GetDistance(enemy) < distance);
        //    List<Enemy> neighbors = query.ToList();
        //    double meanX = neighbors.Sum(x => x.GetPosition().X) / neighbors.Count();
        //    double meanZ = neighbors.Sum(x => x.GetPosition().Z) / neighbors.Count();
        //    double deltaCenterX = meanX - enemy.GetPosition().X;
        //    double deltaCenterZ = meanZ - enemy.GetPosition().Z;
        //    Vector2 output = new Vector2((float)deltaCenterX, (float)deltaCenterZ) * (float)power;
        //    //System.Diagnostics.Debug.WriteLine(output.ToString());
        //    return output;
        //}

        //private Vector2 AvoidanceBehaviour(Enemy enemy, double distance, double power)
        //{
        //    IEnumerable<Enemy> query = enemiesList.Where(x => x.GetDistance(enemy) < distance);
        //    List<Enemy> neighbors = query.ToList();
        //    (double sumClosenessX, double sumClosenessY) = (0, 0);
        //    foreach (var neighbor in neighbors)
        //    {
        //        double closeness = distance - enemy.GetDistance(neighbor);
        //        sumClosenessX += (enemy.GetPosition().X - neighbor.GetPosition().X) * closeness;
        //        sumClosenessY += (enemy.GetPosition().Z - neighbor.GetPosition().Z) * closeness;
        //    }
        //    Vector2 output = new Vector2((float)sumClosenessX, (float)sumClosenessY) * (float)power;
        //    //System.Diagnostics.Debug.WriteLine(output.ToString());
        //    return output;
        //}

        private void CalculateDirection(Vector3 playerPosition )
        {
            Vector2 newDirection = Vector2.Zero;
            Vector2 directionToPlayer = new Vector2(playerPosition.X - GetPosition().X, playerPosition.Z - GetPosition().Z);
            newDirection += directionToPlayer;
            newDirection.Normalize();
            Direction = newDirection;
        }

        private void RotateTowardsPlayer(Vector3 playerPosition)
        {
            CalculateDirection(playerPosition);
            Vector2 w1 = new Vector2(0, 1);    // wektor wyjsciowy od ktorego obliczam kat czyli ten do dolu
            Vector2 w2 = new Vector2(Direction.X, Direction.Y);

            float rotation = angle(w2, w1);

            // rotate bounding box
            rotateSphere(rotation - this.GetRotation().Y);

            // rotate model
            this.SetRotation(0, rotation, 0);
        }

        private void MoveBoundingBoxForwards(float speed)
        {
            Vector3 movement = new Vector3(Direction.X * speed, 0, Direction.Y * speed);
            boundingSphere.Center = boundingSphere.Center + movement;
            boundingBox.Min = boundingBox.Min + movement;
            boundingBox.Max = boundingBox.Max + movement;
        }

        public void MoveForwards(float deltaTime, bool shouldChase)
        {
            float currentSpeed = this.MaxSpeed;
            if (!shouldChase) { currentSpeed *= -1; }


            MoveModelForwards(currentSpeed * deltaTime);
            MoveBoundingBoxForwards(currentSpeed * deltaTime);
        }

        public virtual void FollowPlayer(Vector3 playerPosition, float deltaTime, bool shouldChase)
        {

        }
        public void rotateSphere(float Angle)
        {
            Matrix rotationMatrix = Matrix.CreateRotationY(Angle);
            boundingSphere.Center -= this.GetPosition();
            boundingSphere.Center = Vector3.Transform(boundingSphere.Center, rotationMatrix);
            boundingSphere.Center += this.GetPosition();
        }
        public static float angle(Vector2 a, Vector2 b)
        {
            float dot = dotProduct(a, b);
            float det = a.X * b.Y - a.Y * b.X;
            return (float)Math.Atan2(det, dot);
        }
        public static float dotProduct(Vector2 vector, Vector2 vector1)
        {
            float result = vector.X * vector1.X + vector.Y * vector1.Y;

            return result;
        }

    }
}
