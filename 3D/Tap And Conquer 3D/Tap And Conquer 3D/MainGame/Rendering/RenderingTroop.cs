using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using StateGameLayer;

namespace TapAndConquer3D
{
    public class RenderingTroop : RenderingLaser
    {
        private SpriteBatch spriteBatch;
        private static Model red_tank;
        private static Model blue_tank;

        public BoundingSphere boundingSphereModelShipBlue;
        public BoundingSphere boundingSphereModelShipRed;

        private static Texture2D battle;
        private static SpriteFont font;

        public StateGame stateGame;

        public RenderingTroop(ContentManager Content)
            : base(Content)
        {
            spriteBatch = Content.ServiceProvider.GetService(typeof(SpriteBatch)) as SpriteBatch;
            red_tank = Content.Load<Model>("ModelTroops/gunship");
            blue_tank = Content.Load<Model>("ModelTroops/bomber");
            battle = Content.Load<Texture2D>("TroopTexture/war");
            font = Content.Load<SpriteFont>("Fonts/DefaultFont");

            //bounding sphere red
            boundingSphereModelShipRed = new BoundingSphere();

            foreach (ModelMesh mesh in red_tank.Meshes)
            {
                if (boundingSphereModelShipRed.Radius == 0)
                    boundingSphereModelShipRed = mesh.BoundingSphere;
                else
                    boundingSphereModelShipRed = BoundingSphere.CreateMerged(boundingSphereModelShipRed, mesh.BoundingSphere);
            }

            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;

            //bounding sphere blue
            boundingSphereModelShipBlue = new BoundingSphere();

            foreach (ModelMesh mesh in blue_tank.Meshes)
            {
                if (boundingSphereModelShipBlue.Radius == 0)
                    boundingSphereModelShipBlue = mesh.BoundingSphere;
                else
                    boundingSphereModelShipBlue = BoundingSphere.CreateMerged(boundingSphereModelShipBlue, mesh.BoundingSphere);
            }
        }

        public Model Red_Tank
        {
            set { red_tank = value; }
            get { return red_tank; }
        }

        public Model Blue_Tank
        {
            set { blue_tank = value; }
            get { return blue_tank; }
        }


        public void DrawTroop(GameState gameState, bool map)
        {
            if (map == false)
            {
                base.DrawLasers(gameState);
            }
            else
            {
                base.DrawFactory(gameState, map);
            }

            List<StateGameLayer.Troop> troops = gameState.getTroops();

            ModelMeshCollection modelMesh = null;

            for (int i = 0; i < troops.Count; i++)
            {
                StateGameLayer.Troop troop = troops[i];
                if (troop.Fighting)
                {
                    Vector2 troopOrigin = new Vector2(battle.Width * 0.5f, battle.Height * 0.5f);
                }

                switch (troop.ObjectType)
                {
                    case 1:
                        modelMesh = blue_tank.Meshes;
                        break;
                    case 2:
                        modelMesh = red_tank.Meshes;
                        break;
                }


                int spaceSeparationTroop = 100;

                for (int numElement = 0; numElement < troop.Elements / 3 + 1; numElement++)
                {
                    foreach (ModelMesh mesh in modelMesh)
                    {
                        // This is where the mesh orientation is set, as well 
                        // as our camera and projection.
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.EnableDefaultLighting();
                            effect.World = World;
                            effect.World *= Matrix.CreateRotationY((float)-troop.Rotation);

                            switch (numElement)
                            {
                                case 0:
                                    effect.World *= Matrix.CreateTranslation(/*-3200*/troop.CurrentPos.X/*COORD TROOP X*/, troop.CurrentPos.Y/*COORD TROOP Y*/, /*-4500*/troop.CurrentPos.Z/*COORD TROOP Z*/);
                                    break;
                                case 1:
                                    effect.World *= Matrix.CreateTranslation(/*-3200*/troop.CurrentPos.X - spaceSeparationTroop/*COORD TROOP X*/, troop.CurrentPos.Y/*COORD TROOP Y*/, /*-4500*/troop.CurrentPos.Z + spaceSeparationTroop/*COORD TROOP Z*/);
                                    break;
                                case 2:
                                    effect.World *= Matrix.CreateTranslation(/*-3200*/troop.CurrentPos.X + spaceSeparationTroop/*COORD TROOP X*/, troop.CurrentPos.Y/*COORD TROOP Y*/, /*-4500*/troop.CurrentPos.Z + spaceSeparationTroop/*COORD TROOP Z*/);
                                    break;
                                case 3:
                                    effect.World *= Matrix.CreateTranslation(/*-3200*/troop.CurrentPos.X + spaceSeparationTroop * 2/*COORD TROOP X*/, troop.CurrentPos.Y/*COORD TROOP Y*/, /*-4500*/troop.CurrentPos.Z + spaceSeparationTroop * 2/*COORD TROOP Z*/);
                                    break;
                                case 4:
                                    effect.World *= Matrix.CreateTranslation(/*-3200*/troop.CurrentPos.X - spaceSeparationTroop * 2/*COORD TROOP X*/, troop.CurrentPos.Y/*COORD TROOP Y*/, /*-4500*/troop.CurrentPos.Z + spaceSeparationTroop * 2/*COORD TROOP Z*/);
                                    break;
                                case 5:
                                    effect.World *= Matrix.CreateTranslation(/*-3200*/troop.CurrentPos.X/*COORD TROOP X*/, troop.CurrentPos.Y/*COORD TROOP Y*/, /*-4500*/troop.CurrentPos.Z + spaceSeparationTroop * 2/*COORD TROOP Z*/);
                                    break;
                                case 6:
                                    effect.World *= Matrix.CreateTranslation(/*-3200*/troop.CurrentPos.X/*COORD TROOP X*/, troop.CurrentPos.Y/*COORD TROOP Y*/, /*-4500*/troop.CurrentPos.Z + spaceSeparationTroop * 3/*COORD TROOP Z*/);
                                    break;
                            }
                            if (map)
                            {
                                effect.View = ViewMap;
                            }
                            else
                            {
                                effect.View = View;
                            }
                            effect.Projection = Projection;
                            // Draw the mesh, using the effects set above.
                            mesh.Draw();
                            effect.Projection = Projection;
                            mesh.Draw();
                        }
                    }
                }
            }
        }
    }
}
