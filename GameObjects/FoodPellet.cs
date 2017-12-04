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
        int weight = 5;
        int currentAnts = 0;
        public bool atColony = false;
        public bool FindColony = false;
        public Colony targetColony = null;

        protected override void UpdateActive(GameTime gt)
        {
            if(currentAnts >= weight)
            {
                if (!FindColony) FindColony = true;

                int speed = 25;
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
        }

        public void setColony(Colony c)
        {
            targetColony = c;
        }
    }
}
