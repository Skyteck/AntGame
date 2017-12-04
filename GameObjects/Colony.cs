using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace AntGame.GameObjects
{
    class Colony : Sprite
    {
        public double SpawnTimer = 5.0f;
        public int currentHP = 10;
        
        public enum AntTeams
        {
            kTeamGreen,
            kTeamBrown,
            kTeamNone
        }

        public AntTeams myTeam = AntTeams.kTeamNone;

        protected override void UpdateActive(GameTime gameTime)
        {
            SpawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            base.UpdateActive(gameTime);
        }
        
    }
}
