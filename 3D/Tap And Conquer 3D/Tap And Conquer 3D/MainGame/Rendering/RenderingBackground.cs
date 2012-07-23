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

namespace TapAndConquer3D
{
    public class RenderingBackground
    {
        private SpriteBatch spriteBatch;

        private Vector3 camera;
        private Vector3 cameraMap;
        private float fieldOfView;
        private Matrix world;
        private Matrix view;
        private Matrix projection;
        private Matrix viewMap;
        private float aspectRatio;
        public SpriteFont spriteFont;

        private float translateX;
        private float translateY;
        private float translateZ;

        public void TranslateX(float x)
        {
            if (x > 0)
            {
                if (camera.X <= 5000)
                {
                    camera.X += x;
                    View = Matrix.CreateLookAt(camera, new Vector3(translateX += x, translateY, translateZ), Vector3.Forward);
                }
            }
            else
            {
                if (camera.X >= -5000)
                {
                    camera.X += x;
                    View = Matrix.CreateLookAt(camera, new Vector3(translateX += x, translateY, translateZ), Vector3.Forward);
                }
            }
        }

        public void TranslateY(float x)
        {
            View = Matrix.CreateLookAt(camera, new Vector3(translateX, translateY, translateZ), Vector3.Forward);
        }

        public void TranslateZ(float x)
        {
            if (x > 0)
            {
                if (camera.Z <= 3800)
                {
                    camera.Z += x;
                    View = Matrix.CreateLookAt(camera, new Vector3(translateX, translateY, translateZ += x), Vector3.Forward);
                }
            }
            else
            {
                if (camera.Z >= -3800)
                {
                    camera.Z += x;
                    View = Matrix.CreateLookAt(camera, new Vector3(translateX, translateY, translateZ += x), Vector3.Forward);
                }
            }
        }


        private Texture2D background;

        public void zoomOut()
        {
            if (camera.Y > 7000)
                return;

            camera.Y += 100;
            View = Matrix.CreateLookAt(camera, new Vector3(translateX, translateY, translateZ), Vector3.Forward);
        }

        public void zoomIn()
        {
            if (camera.Y < 1000)
                return;

            camera.Y -= 100;
            View = Matrix.CreateLookAt(camera, new Vector3(translateX, translateY, translateZ), Vector3.Forward);
        }

        public Vector3 Camera
        {
            set { camera = value; }
            get { return camera; }
        }

         public Matrix World
        {
            set { world = value; }
            get { return world; }
        }

        public Matrix View
        {
            set { view = value; }
            get { return view; }
        }
        
        public Matrix ViewMap
        {
            set { viewMap = value; }
            get { return viewMap; }
        }

        public Matrix Projection
        {
            set { projection = value; }
            get { return projection; }
        }

        public float AspectRatio
        {
            set { aspectRatio = value; }
            get { return aspectRatio; }
        }        

        public RenderingBackground(ContentManager Content)
        {
            spriteBatch = Content.ServiceProvider.GetService(typeof(SpriteBatch)) as SpriteBatch;
            this.aspectRatio = 4.0f / 3.0f;// spriteBatch.GraphicsDevice.Viewport.AspectRatio;

            background = Content.Load<Texture2D>("backgroundStars");
            spriteFont = Content.Load<SpriteFont>("Fonts/DefaultFont");
            translateX = 0;
            translateY = 0;
            translateZ = 0;

            camera = new Vector3(translateX, 7000.0f, translateZ);
            cameraMap = new Vector3(0, 10000.0f, 0);
            fieldOfView = 45;

            world = Matrix.Identity;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fieldOfView), aspectRatio, 1.0f, 10000.0f);
            view = Matrix.CreateLookAt(camera, new Vector3(translateX, translateY, translateZ), Vector3.Forward);
            viewMap = Matrix.CreateLookAt(cameraMap, Vector3.Zero, Vector3.Forward);

        }

        public void DrawBackground()
        {
            spriteBatch.Begin(SpriteSortMode.Texture, spriteBatch.GraphicsDevice.BlendState, SamplerState.LinearClamp, spriteBatch.GraphicsDevice.DepthStencilState, spriteBatch.GraphicsDevice.RasterizerState);

            spriteBatch.Draw(background, new Rectangle(0, 0, spriteBatch.GraphicsDevice.Viewport.Width, spriteBatch.GraphicsDevice.Viewport.Height), new Rectangle(0, 0, background.Width, background.Height), Color.White, 0, Vector2.Zero, SpriteEffects.None, 1.0f);

            spriteBatch.End();
        }
    }
}
