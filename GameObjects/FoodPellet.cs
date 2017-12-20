using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AntGame.GameObjects
{
    class FoodPellet : Sprite
    {
        double weight = 10;
        double currentAnts = 0;
        List<Ant> antsAttached;
        public bool atColony = false;
        public bool FindColony = false;
        public Colony targetColony = null;

        public Colony.AntTeams myTeam = Colony.AntTeams.kTeamNone;

        public FoodPellet()
        {
            antsAttached = new List<Ant>();
        }

        protected override void UpdateActive(GameTime gt)
        {
            currentAnts = 0;
            List<Ant> attchedAnts = new List<Ant>();
            int GreenCount = 0;
            int BrownCount = 0;
            double speed = 50;

            foreach(Ant a in antsAttached)
            {
                if(a._BoundingBox.Intersects(this._BoundingBox))
                {
                    currentAnts++;
                    attchedAnts.Add(a);
                    if(a.myTeam == Colony.AntTeams.kTeamBrown)
                    {
                        BrownCount++;
                    }
                    else if(a.myTeam == Colony.AntTeams.kTeamGreen)
                    {
                        GreenCount++;
                    }
                }
            }
            antsAttached.Clear();
            antsAttached.AddRange(attchedAnts);

            bool flipped = false;
            if(GreenCount > BrownCount)
            {
                if (myTeam == Colony.AntTeams.kTeamBrown) flipped = true;
                myTeam = Colony.AntTeams.kTeamGreen;
            }
            else if(BrownCount > GreenCount)
            {
                if (myTeam == Colony.AntTeams.kTeamGreen) flipped = true;
                myTeam = Colony.AntTeams.kTeamBrown;
            }
            

            if(currentAnts >= 1)
            {
                speed = (currentAnts / weight) * 50;

                if(speed > 50)
                {
                    speed = 50;
                }
                if (!FindColony && targetColony == null)
                {
                    FindColony = true;
                }
                
                
                if(currentAnts >= (weight*2))
                {
                    speed *= 2;
                }
                if(targetColony != null)
                {
                    if (targetColony._Position.X < this._Position.X)
                    {
                        this._Position.X -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        this._Position.X += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }

                    if (targetColony._Position.Y < this._Position.Y)
                    {
                        this._Position.Y -= (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }
                    else
                    {
                        this._Position.Y += (float)(speed * gt.ElapsedGameTime.TotalSeconds);
                    }

                    if(this._BoundingBox.Intersects(targetColony._BoundingBox))
                    {
                        atColony = true;
                    }
                }

            }
            base.UpdateActive(gt);
        }

        public void addAnt(Ant a)
        {
            currentAnts++;
            antsAttached.Add(a);
        }

        public void setColony(Colony c)
        {
            if(c == null)
            {
                targetColony = null;
                FindColony = false;
                currentAnts = 0;
            }
            targetColony = c;
        }

        public override void Deactivate()
        {
            targetColony = null;
            FindColony = false;
            antsAttached.Clear();
            base.Deactivate();
        }
    }
}
