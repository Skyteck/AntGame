using AntGame;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

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


        Texture2D _GreenTex;
        Texture2D _BrownTex;
        Texture2D _NoneTex;

        public bool selected = false;

        FoodPellet targetPellet = null;

        public Colony.AntTeams myTeam { get; private set; } = Colony.AntTeams.kTeamNone;

        internal void LoadContent(ContentManager content)
        {
            _GreenTex = content.Load<Texture2D>(@"Art/SlimeShot");
            _BrownTex = content.Load<Texture2D>(@"Art/logItem");
            _NoneTex = content.Load<Texture2D>(@"Art/axe");
        }

        enum MoveStatus
        {
            kOrbiting,
            kMoving,
            kGoingToOrbit,
            kToPellet,
            kAtPellet,
            kToColony
        }

        MoveStatus currentMove = MoveStatus.kMoving;

        public Ant()
        {
            //this._Position = Vector2.Zero;
            ChangeOrbit();
        }

        public void ActivateTeam(Colony.AntTeams team, Vector2 pos)
        {
            ChangeTeam(team);
            this._Position = pos;
            this.SetOrbitPoint(pos);
            base.Activate();
        }

        protected override void UpdateActive(GameTime gt)
        {
            if(currentMove == MoveStatus.kMoving)
            {
                //check if within orbitradius of orbitPoint
                //if in radius change to orbiting
                //else move to point
                if (Vector2.Distance(this._Position, orbitPoint) < 16)
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
            else if(currentMove == MoveStatus.kOrbiting)
            {
                Orbit(gt);
            }
            else if(currentMove == MoveStatus.kToPellet)
            {
                if(targetPellet != null)
                {
                    if(Vector2.Distance(this._Position, targetPellet._Position) < 5)
                    {
                        currentMove = MoveStatus.kAtPellet;
                        targetPellet.addAnt(this);
                    }
                    else
                    {
                        int speed = 50;
                        if (targetPellet._Position.X < this._Position.X)
                        {
                            this._Position.X -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                        }
                        else
                        {
                            this._Position.X += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                        }

                        if (targetPellet._Position.Y < this._Position.Y)
                        {
                            this._Position.Y -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                        }
                        else
                        {
                            this._Position.Y += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                        }
                    }
                }
            }
            else if(currentMove == MoveStatus.kAtPellet)
            {
                if(targetPellet != null)
                {
                    float offsetX = (float)(Math.Sin(seed) * orbitRadius);
                    float offsetY = (float)(Math.Cos(seed) * orbitRadius);
                    currentPoint.X = (targetPellet._Position.X + offsetX);
                    currentPoint.Y = targetPellet._Position.Y + offsetY;
                }

                int speed = 50;
                if (targetPellet._Position.X < this._Position.X)
                {
                    this._Position.X -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    this._Position.X += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                }

                if (targetPellet._Position.Y < this._Position.Y)
                {
                    this._Position.Y -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                }
                else
                {
                    this._Position.Y += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                }
                if (targetPellet._CurrentState == SpriteState.kStateInActive)
                {
                    SetOrbitPoint(this._Position);
                    CalculateOrbit(gt);
                    currentMove = MoveStatus.kMoving;
                }
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

        public void SetPellet(FoodPellet p)
        {
            targetPellet = p;
            currentMove = MoveStatus.kToPellet;
        }

        public void ChangeTeam(Colony.AntTeams team)
        {
            myTeam = team;
            if (team == Colony.AntTeams.kTeamBrown)
            {
                this._Texture = _BrownTex;
            }
            else if (team == Colony.AntTeams.kTeamGreen)
            {
                this._Texture = _GreenTex;
            }
            else if (team == Colony.AntTeams.kTeamNone)
            {
                this._Texture = _NoneTex;
            }


            frameHeight = _Texture.Height;
            frameWidth = _Texture.Width;
        }

        public void Die()
        {
            this.Deactivate();
            ChangeTeam(Colony.AntTeams.kTeamNone);
        }
    }
}
