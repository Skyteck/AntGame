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
        List<Ant> ActiveAnts;
        ColonyManager _ColonyManager;
        FoodManager _FoodManager;

        public AntManager()
        {
            AntList = new List<Ant>();
            ActiveAnts = new List<Ant>();
        }

        public void SetColonyManager(ColonyManager cm, FoodManager fm)
        {
            _ColonyManager = cm;
            _FoodManager = fm;
        }

        public void CreateAnts(ContentManager content)
        {
            for(int i = 0; i < 2000; i++)
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
            ActiveAnts.Clear();
            ActiveAnts.AddRange(AntList.FindAll(x => x._CurrentState == Sprite.SpriteState.kStateActive));
            List<Ant> goodAnts = GetAntsOnTeam(Colony.AntTeams.kTeamGreen);
            List<Ant> badAnts = GetAntsOnTeam(Colony.AntTeams.kTeamBrown);
            foreach(Ant a in ActiveAnts)
            {
                a.Update(gt);
                List<Ant> otherTeam = new List<Ant>();
                if(a.myTeam == Colony.AntTeams.kTeamGreen)
                {
                    otherTeam.AddRange(badAnts);
                }
                else
                {
                    otherTeam.AddRange(goodAnts);
                }

                foreach(Ant bA in otherTeam)
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
                    if (bC.myTeam == a.myTeam) continue;

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
            return ActiveAnts.FindAll(x => x.myTeam == team);
        }

        public List<Ant> GetAllAnts()
        {
            return ActiveAnts;
        }
        private List<Ant> FindAntsNear(Vector2 pos, Colony.AntTeams team ,int range = 15)
        {
            List<Ant> ants = new List<Ant>();
            ants.AddRange(ActiveAnts.FindAll(x => x.myTeam != team ));

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
            foreach(Ant a in ActiveAnts)
            {
                a.Draw(sb);
            }
        }
    }
}
