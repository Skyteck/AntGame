using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AntGame.GameObjects
{
    class Colony : Sprite
    {
        public double SpawnTimer = 5.0f;
        public int currentHP = 10;
        Texture2D _GreenTex;
        Texture2D _BrownTex;
        Texture2D _NoneTex;

        public enum AntTeams
        {
            kTeamGreen,
            kTeamBrown,
            kTeamNone
        }

        public AntTeams myTeam { get; private set; } = AntTeams.kTeamNone;

        public override void LoadContent(string path, ContentManager content)
        {
            _GreenTex = content.Load<Texture2D>(@"Art/Fire");
            _BrownTex = content.Load<Texture2D>(@"Art/LavaTile");
            _NoneTex = content.Load<Texture2D>(@"Art/FishingHole");
        }

        protected override void UpdateActive(GameTime gameTime)
        {
            if(myTeam != AntTeams.kTeamNone)
            {
                SpawnTimer -= gameTime.ElapsedGameTime.TotalSeconds;
            }
            base.UpdateActive(gameTime);
        }
        
        public void ChangeTeam(AntTeams team)
        {
            myTeam = team;
            if(team == AntTeams.kTeamBrown)
            {
                this._Texture = _BrownTex;
            }
            else if (team == AntTeams.kTeamGreen)
            {
                this._Texture = _GreenTex;
            }
            else if (team == AntTeams.kTeamNone)
            {
                this._Texture = _NoneTex;
            }

            currentHP = 10;
            frameHeight = _Texture.Height;
            frameWidth = _Texture.Width;
        }

        public void ActivateTeam(AntTeams team, Vector2 pos)
        {
            ChangeTeam(team);
            Activate(pos);
        }
    }
}
