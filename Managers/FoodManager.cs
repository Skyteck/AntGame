﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AntGame.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace AntGame.Managers
{
    class FoodManager
    {

        List<FoodPellet> PelletList;
        ColonyManager _ColonyManager;
        public int foodCount = 3;
        public int enemyFood = 3;
        double SpawnTimer = 15.0;

        public FoodManager(ColonyManager cm)
        {
            _ColonyManager = cm;
            PelletList = new List<FoodPellet>();
        }


        public void LoadContent(ContentManager c)
        {
            Texture2D foodtex = c.Load<Texture2D>(@"Art/StrawberryItem");

            for (int i = 0; i < 50; i++)
            {
                FoodPellet np = new FoodPellet();
                np.LoadContent(foodtex);
                np.Deactivate();
                PelletList.Add(np);
            }
        }

        public void Update(GameTime gt)
        {
            foreach(FoodPellet p in PelletList.FindAll(x=>x._CurrentState == Sprite.SpriteState.kStateActive))
            {
                p.Update(gt);

                if(p.FindColony)
                {
                    p.setColony(_ColonyManager.FindClosestColony(p._Position, p.myTeam));
                }



                foreach(Colony c in _ColonyManager.GetColoniesOnTeam( p.myTeam))
                {
                    if(p._BoundingBox.Intersects(c._BoundingBox))
                    {
                        if(p.myTeam == Colony.AntTeams.kTeamGreen)
                        {
                            foodCount++;
                        }
                        else if(p.myTeam == Colony.AntTeams.kTeamBrown)
                        {
                            enemyFood++;
                        }
                        p.Deactivate();
                        break;
                    }
                }
            }

            SpawnTimer -= gt.ElapsedGameTime.TotalSeconds;

            if(SpawnTimer <= 0.0)
            {
                //Create pellet
                CreatePellet();
                SpawnTimer += 15.0;
            }
        }


        public void CreatePellet()
        {
            Vector2 newPos = Vector2.Zero;
            newPos.X = HelperFunctions.GetRandomNum(-150, 630);
            newPos.Y = HelperFunctions.GetRandomNum(-70, 390);
            CreatePellet(newPos);
        }

        public void CreatePellet(Vector2 pos)
        {
            PelletList.Find(x => x._CurrentState == Sprite.SpriteState.kStateInActive)
                             .Activate(pos); 
        }

        public List<FoodPellet> GetFoodsNear(Vector2 pos, int range = 16)
        {
            List<FoodPellet> foods = new List<FoodPellet>();
            foods.AddRange(PelletList.FindAll(x=> x._CurrentState == Sprite.SpriteState.kStateActive));

            //got active ants not on the sent in team
            List<FoodPellet> ColonyInRange = new List<FoodPellet>();
            ColonyInRange.AddRange(foods.FindAll(x => x._Position.X >= (pos.X - range) &&
                                                   x._Position.X <= (pos.X + range) &&
                                                   x._Position.Y >= (pos.Y - range) &&
                                                   x._Position.Y <= (pos.Y + range)));
            return ColonyInRange;
        }

        public List<FoodPellet> GetAllFoods()
        {
            return PelletList.FindAll(x => x._CurrentState == Sprite.SpriteState.kStateActive);
        }

        public void Draw(SpriteBatch sb)
        {
            foreach (FoodPellet p in PelletList.FindAll(x=>x._Draw == true))
            {
                p.Draw(sb);
            }
        }
    }
}
