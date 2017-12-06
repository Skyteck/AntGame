using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntGame.GameObjects;
using Microsoft.Xna.Framework.Graphics;

namespace AntGame.Managers
{
    class ColonyManager
    {
        List<Colony> ColonyList;
        AntManager _AntManager;

        public ColonyManager(AntManager am)
        {
            ColonyList = new List<Colony>();
            _AntManager = am;
        }

        public void CreateColonies(ContentManager content)
        {
            for(int i = 0; i < 50; i++)
            {
                Colony c = new Colony();
                c.LoadContent("blah", content);
                c.ChangeTeam(Colony.AntTeams.kTeamNone);
                c.Deactivate();
                ColonyList.Add(c);
            }
        }

        public void PlaceColony(Vector2 pos, Colony.AntTeams team)
        {
            ColonyList.Find(x => x._CurrentState == Sprite.SpriteState.kStateInActive)
                             .ActivateTeam(team, pos);
        }

        public void Update(GameTime gt)
        {
            foreach(Colony c in ColonyList)
            {
                c.Update(gt);

                if(c.myTeam != Colony.AntTeams.kTeamNone)
                {
                    if(c.SpawnTimer <= 0.0f)
                    {
                        c.SpawnTimer = 5.0f;
                        _AntManager.AddAnt(c.myTeam, c._Position);
                    }
                }
            }
        }

        public List<Colony> FindColoniesNear(Vector2 pos, Colony.AntTeams team, int range = 15)
        {
            List<Colony> colonies = new List<Colony>();
            colonies.AddRange(ColonyList.FindAll(x => x.myTeam != team && x._CurrentState == Sprite.SpriteState.kStateActive));

            //got active ants not on the sent in team
            List<Colony> ColonyInRange = new List<Colony>();
            ColonyInRange.AddRange(colonies.FindAll(x => x._Position.X >= (pos.X - range) &&
                                                   x._Position.X <= (pos.X + range) &&
                                                   x._Position.Y >= (pos.Y - range) &&
                                                   x._Position.Y <= (pos.Y + range)));
            return ColonyInRange;
        }

        public Colony FindClosestColony(Vector2 pos)
        {
            Colony closest = ColonyList.Find(x => x._CurrentState == Sprite.SpriteState.kStateActive && x.myTeam == Colony.AntTeams.kTeamGreen);
            float dist = Vector2.Distance(pos, closest._Position);
            foreach(Colony c in ColonyList.FindAll(x => x._CurrentState == Sprite.SpriteState.kStateActive && x.myTeam == Colony.AntTeams.kTeamGreen))
            {
                if(Vector2.Distance(pos, c._Position) < dist)
                {
                    dist = Vector2.Distance(pos, c._Position);
                    closest = c;
                }
            }

            return closest;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(Colony c in ColonyList)
            {
                c.Draw(sb);
            }
        }
    }
}
