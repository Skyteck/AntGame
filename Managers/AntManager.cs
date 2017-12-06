using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntGame.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace AntGame.Managers
{
    class AntManager
    {
        List<Ant> AntList;
        ColonyManager _ColonyManager;
        FoodManager _FoodManager;

        public AntManager()
        {
            AntList = new List<Ant>();
        }

        public void SetColonyManager(ColonyManager cm, FoodManager fm)
        {
            _ColonyManager = cm;
            _FoodManager = fm;
        }

        public void CreateAnts(ContentManager content)
        {
            for(int i = 0; i < 10000; i++)
            {
                Ant a = new Ant();
                a.LoadContent(content);
                a.ChangeTeam(Colony.AntTeams.kTeamNone);
                a.Deactivate();
                AntList.Add(a);
            }
        }

        public void AddAnt(Colony.AntTeams team, Vector2 pos, int amt = 1)
        {
            for(int i = 0; i < amt; i++)
            {
                AntList.Find(x => x._CurrentState == Sprite.SpriteState.kStateInActive)
                              .ActivateTeam(team, pos);
            }
        }

        public void Update(GameTime gt)
        {
            foreach(Ant a in AntList.Where(x=>x._CurrentState == Sprite.SpriteState.kStateActive))
            {
                a.Update(gt);

                List<Ant> badAnts = FindAntsNear(a._Position, a.myTeam);

                foreach(Ant bA in badAnts)
                {
                    if(a._BoundingBox.Intersects(bA._BoundingBox))
                    {
                        a.Die();
                        bA.Die();
                        continue;
                    }
                }

                List<Colony> badColonies = _ColonyManager.FindColoniesNear(a._Position, a.myTeam, 32);

                foreach(Colony bC in badColonies)
                {
                    //if (bC.myTeam == Colony.AntTeams.kTeamNone) continue;

                    if(a._BoundingBox.Intersects(bC._BoundingBox))
                    {
                        bC.currentHP--;

                        if(bC.currentHP <= 0)
                        {
                            bC.ChangeTeam(a.myTeam);
                        }
                        a.Die();
                        break;
                    }
                }

                //foreach(FoodPellet fp in _FoodManager.GetFoodsNear(a._Position, 16))
                //{
                //    if(a._BoundingBox.Intersects(fp._BoundingBox))
                //    {
                //        fp.addAnt(a);
                //        a.SetPellet(fp);
                //    }
                //}
            }
        }

        public List<Ant> GetAntsOnTeam(Colony.AntTeams team)
        {
            return AntList.FindAll(x => x.myTeam == team && x._CurrentState == Sprite.SpriteState.kStateActive);
        }

        private List<Ant> FindAntsNear(Vector2 pos, Colony.AntTeams team ,int range = 15)
        {
            List<Ant> ants = new List<Ant>();
            ants.AddRange(AntList.FindAll(x => x.myTeam != team && x._CurrentState == Sprite.SpriteState.kStateActive));

            //got active ants not on the sent in team
            List<Ant> antsInRange = new List<Ant>();
            antsInRange.AddRange(ants.FindAll(x => x._Position.X >= (pos.X - range) &&
                                                   x._Position.X <= (pos.X + range) &&
                                                   x._Position.Y >= (pos.Y - range) &&
                                                   x._Position.Y <= (pos.Y + range)));
            return antsInRange;
        }

        public void Draw(SpriteBatch sb)
        {
            foreach(Ant a in AntList)
            {
                a.Draw(sb);
            }
        }
    }
}
