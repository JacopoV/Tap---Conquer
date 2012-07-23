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
    public class RenderingFactory : RenderingBackground
    {
        private static Model planet;

        private static BoundingSphere boundingSpherePlanet;
        private SpriteBatch spriteBatch;
        public StateGame stateGame;

        private static List<Texture2D> textureList = new List<Texture2D>();

        public Model Planet
        {
            set { }
            get { return planet; }
        }

        public BoundingSphere BoundingSpherePlanet
        {
            set {}
            get { return boundingSpherePlanet; }
        }

        private GameState gs;
        private List<Factory> factories;

        public RenderingFactory(ContentManager Content)
            : base(Content)
        {
            planet = Content.Load<Model>("ModelTroops/Planet");

            gs = Content.ServiceProvider.GetService(typeof(GameState)) as GameState;
            spriteBatch = Content.ServiceProvider.GetService(typeof(SpriteBatch)) as SpriteBatch;

            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;

            factories = stateGame.getFactories();

            for (int i = 0; i < factories.Count; i++)
            {
                if (factories[i].ObjectType == 1)
                {
                    TranslateX((float)factories[i].CurrentPos.X);
                    TranslateZ((float)factories[i].CurrentPos.Z);
                    break;
                }
            }
                /*
                 * setting up the new texture when planet is clicked
                 */
            textureList.Add(Content.Load<Texture2D>("ModelTroops/marsTexture"));
            textureList.Add(Content.Load<Texture2D>("ModelTroops/earthTexture"));
            textureList.Add(Content.Load<Texture2D>("ModelTroops/venusTexture"));
            textureList.Add(Content.Load<Texture2D>("ModelTroops/planetTexture3"));
            textureList.Add(Content.Load<Texture2D>("ModelTroops/planetTexture4"));
            textureList.Add(Content.Load<Texture2D>("ModelTroops/planetTexture1"));
            textureList.Add(Content.Load<Texture2D>("ModelTroops/planetTexture2"));
            
            
             

            boundingSpherePlanet = new BoundingSphere();

            foreach (ModelMesh mesh in planet.Meshes)
            {
                if (boundingSpherePlanet.Radius == 0)
                    boundingSpherePlanet = mesh.BoundingSphere;
                else
                    boundingSpherePlanet = BoundingSphere.CreateMerged(boundingSpherePlanet, mesh.BoundingSphere);
            }

        } 

        public void DrawFactory(GameState gameState, bool map)
        {
            base.DrawBackground();

            for (int i = 0; i < factories.Count; i++)
            {

                StateGameLayer.Factory factory = factories[i];
                foreach (ModelMesh mesh in planet.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        if (factories[i].Selected == true)
                        {
                            effect.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
                        }
                        else
                        {
                            effect.EnableDefaultLighting();
                        }

                        switch (factories[i].ObjectType)
                        {
                            case 1:
                                effect.AmbientLightColor = new Vector3(0.0f, 0.0f, 1.0f);
                                effect.SpecularColor = new Vector3(0.0f, 0.0f, 1.0f);
                                break;
                            case 2:
                                effect.AmbientLightColor = new Vector3(1.0f, 0.0f, 0.0f);
                                effect.SpecularColor = new Vector3(1.0f, 0.0f, 0.0f);
                                break;
                            default:
                                effect.AmbientLightColor = new Vector3(0.0f, 0.0f, 0.0f);
                                effect.SpecularColor = new Vector3(0.0f, 0.0f, 0.0f);
                                break;
                        }


                        effect.Texture = textureList.ElementAt(factories[i].textureType);
                        effect.World = Matrix.CreateRotationX(MathHelper.ToRadians(-70));
                        effect.World *= Matrix.CreateRotationY(MathHelper.ToRadians(factories[i].axisRotation()));
                        effect.World *=
                            Matrix.CreateScale(factories[i].scaleFactor)
                            * Matrix.CreateTranslation(factory.CurrentPos.X, factory.CurrentPos.Y, factory.CurrentPos.Z);
                        if (map)
                        {
                            effect.View = ViewMap;
                        }
                        else
                        {
                            effect.View = View;
                        }
                        effect.Projection = Projection;
                    }

                    mesh.Draw();
                }
            }
        }
    }
}
