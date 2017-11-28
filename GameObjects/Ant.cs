using AntGame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntGame.GameObjects
{
    class Ant : Sprite
    {
        Vector2 orbitPoint;
        Vector2 currentPoint;
        int orbitRadius = 48;
        double seed = 0;
        float orbitSpeed;
        bool orbitClockwise = true;

        public bool selected = false;


        enum MoveStatus
        {
            kOrbiting,
            kMoving,
            kGoingToOrbit
        }

        MoveStatus currentMove = MoveStatus.kMoving;

        public Ant()
        {
            //this._Position = Vector2.Zero;
            ChangeOrbit();
        }

        protected override void UpdateActive(GameTime gt)
        {
            if(currentMove == MoveStatus.kMoving)
            {
                //check if within orbitradius of orbitPoint
                //if in radius change to orbiting
                //else move to point
                if (Vector2.Distance(this._Position, orbitPoint) < 64)
                {
                    currentMove = MoveStatus.kGoingToOrbit;
                }
                else
                {
                    int speed = 50;
                    if (orbitPoint.X < this._Position.X)
                    {
                        this._Position.X -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        this._Position.X += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }

                    if (orbitPoint.Y < this._Position.Y)
                    {
                        this._Position.Y -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        this._Position.Y += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            else if(currentMove == MoveStatus.kGoingToOrbit)
            {
                CalculateOrbit(gt);
                //should be within range of the orbitpoint so we calculate where in orbit we ened to be then move towards it.
                //if close enough snap to position
                if(Vector2.Distance(this._Position, currentPoint) < 5)
                {
                    currentMove = MoveStatus.kOrbiting;
                }
                else
                {
                    int speed = 50;
                    if (currentPoint.X < this._Position.X)
                    {
                        this._Position.X -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        this._Position.X += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }

                    if (currentPoint.Y < this._Position.Y)
                    {
                        this._Position.Y -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        this._Position.Y += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                }
            }
            else
            {
                Orbit(gt);
            }

            base.UpdateActive(gt);
        }


        private void Orbit(GameTime gt)
        {
            CalculateOrbit(gt);
            _Position = currentPoint;
        }

        private void CalculateOrbit(GameTime gt)
        {
            if (orbitClockwise)
            {
                seed = seed - (orbitSpeed * gt.ElapsedGameTime.TotalSeconds);

            }
            else
            {
                seed = seed + (orbitSpeed * gt.ElapsedGameTime.TotalSeconds);
            }


            float offsetX = (float)(Math.Sin(seed) * orbitRadius);
            float offsetY = (float)(Math.Cos(seed) * orbitRadius);
            currentPoint.X = (orbitPoint.X + offsetX);
            currentPoint.Y = orbitPoint.Y + offsetY;
        }
        public void SetOrbitPoint(Vector2 pos)
        {
            orbitPoint = pos;

            currentMove = MoveStatus.kMoving;
        }

        public void ChangeOrbit()
        {
            Random newRan;
            if (this._Position == Vector2.Zero)
            {
                newRan = new Random();
            }
            else
            {
                newRan = new Random((int)this._Position.X);
            }
            int num = newRan.Next(3, 8);
            if (num % 2 == 0)
            {
                orbitClockwise = false;
            }
            else
            {
                orbitClockwise = true;
            }
            orbitSpeed = (float)(num / 10.0);

            orbitRadius = newRan.Next(16, 64);
        }
    }
}
