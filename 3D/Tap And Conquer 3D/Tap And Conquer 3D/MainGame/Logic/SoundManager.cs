using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using StateGameLayer;

namespace TapAndConquer3D
{
    public class SoundManager
    {
        //private GameState gameState;
        private RenderingState renderingState;
        public StateGame stateGame;

        public bool isSoundActive{get; set;}
        private SoundEffect selection;
        private SoundEffect explosion;
        private SoundEffect laser;

        private AudioListener cameraListener;

        private List<AudioEmitter> explosionEmitter;
        private List<SoundEffectInstance> explosionInstance;
        private List<Vector3> explosionPos;

        private List<AudioEmitter> laserEmitter;
        private List<SoundEffectInstance> laserInstance;
        private List<Vector3> laserPos;

        public SoundManager(ContentManager content)
        {
            //gameState = content.ServiceProvider.GetService(typeof(GameState)) as GameState;
            stateGame = content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;
            renderingState = content.ServiceProvider.GetService(typeof(RenderingState)) as RenderingState;
            selection = content.Load<SoundEffect>("Sounds/select");
            explosion = content.Load<SoundEffect>("Sounds/explosion2");
            laser = content.Load<SoundEffect>("Sounds/laser");

            cameraListener = new AudioListener();
            cameraListener.Position =
                new Vector3(
                    renderingState.spriteBatch.GraphicsDevice.Viewport.Width / 2,
                    renderingState.spriteBatch.GraphicsDevice.Viewport.Height / 2,
                    0);

            explosionEmitter = new List<AudioEmitter>();
            explosionInstance = new List<SoundEffectInstance>();
            explosionPos = new List<Vector3>();

            laserEmitter = new List<AudioEmitter>();
            laserInstance = new List<SoundEffectInstance>();
            laserPos = new List<Vector3>();

            
            isSoundActive = true;
        }

        public void updateSounds()
        {
            if (!isSoundActive)
                return;

            foreach (StateGameLayer.Factory i in stateGame.getFactories())
            {
                if (i.selectionSound == true)
                {
                    selection.Play();
                    i.selectionSound = false;
                }
            }

            updateEmitters();
            foreach (StateGameLayer.Explosion i in stateGame.explosions)
            {
                if (i.soundExplosion == true)
                {
                    AudioEmitter emitter = new AudioEmitter();
                    
                    explosionPos.Add(i.Position);

                    Vector3 near = renderingState.spriteBatch.GraphicsDevice.Viewport.Project
                    (i.Position, renderingState.Projection, renderingState.View, renderingState.World);

                    emitter.Position = near;
                    explosionEmitter.Add(emitter);

                    SoundEffectInstance instance = explosion.CreateInstance();
                    instance.Apply3D(cameraListener, emitter);
                    
                    explosionInstance.Add(instance);

                    instance.Play();
                }

                i.soundExplosion = false;
            }

            foreach (StateGameLayer.Laser i in stateGame.lasers)
            {
                if (i.soundLaser == true)
                {

                    AudioEmitter emitter = new AudioEmitter();

                    laserPos.Add(i.Position);

                    Vector3 near = renderingState.spriteBatch.GraphicsDevice.Viewport.Project
                    (i.Position, renderingState.Projection, renderingState.View, renderingState.World);

                    emitter.Position = near;
                    laserEmitter.Add(emitter);

                    SoundEffectInstance instance = laser.CreateInstance();
                    instance.Apply3D(cameraListener, emitter);

                    laserInstance.Add(instance);

                    instance.Play();
                }

                i.soundLaser = false;
 
            }
        }

        private void updateEmitters()
        {
            searchForEmittersToUpdate(explosionInstance, explosionPos, explosionEmitter);

            searchForEmittersToUpdate(laserInstance, laserPos, laserEmitter);

        }


        private void searchForEmittersToUpdate(List<SoundEffectInstance> instance, List<Vector3> position, List<AudioEmitter> emitter)
        {
            for (int i = 0; i < instance.Count; i++)
            {
                if (instance[i].State == SoundState.Playing)
                {

                    Vector3 near = renderingState.spriteBatch.GraphicsDevice.Viewport.Project
                        (position[i], renderingState.Projection, renderingState.View, renderingState.World);

                    emitter[i].Position = near;

                    float d = (int)Vector3.Distance(near, cameraListener.Position);

                    if (d > 600)
                        instance[i].Volume = 0;
                    else
                        if (d > 250)
                            instance[i].Volume = 0.3f;
                        else
                            if (d > 150)
                                instance[i].Volume = 0.6f;
                            else
                                if (d > 60)
                                    instance[i].Volume = 0.8f;
                                else
                                    if (d > 0)
                                        instance[i].Volume = 1.0f;
                }
                else
                {
                    instance.RemoveAt(i);
                    position.RemoveAt(i);
                    emitter.RemoveAt(i);
                    i--;
                }

            }
 
        }
    }
}
