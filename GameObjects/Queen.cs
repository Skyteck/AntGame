using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AntGame.GameObjects
{
    class Queen : Sprite
    {
        public Colony.AntTeams myTeam = Colony.AntTeams.kTeamNone;

        Colony targetColony = null;

        protected override void UpdateActive(GameTime gt)
        {
            int speed = 50;
            if (targetColony != null)
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
                    targetColony.ChangeTeam(myTeam);
                }
            }


            base.UpdateActive(gt);
        }

        public void SetColony(Colony c)
        {
            targetColony = c;
        }
    }
}
