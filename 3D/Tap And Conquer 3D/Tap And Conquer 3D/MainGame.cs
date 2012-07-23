using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using TapAndConquer3D.StateManager;

namespace TapAndConquer3D
{
  public class MainGame : Microsoft.Xna.Framework.Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    public MainGame()
    {
        this.IsMouseVisible = true;
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
#if WINDOWS_PHONE
      graphics.IsFullScreen = true;
#endif
#if WINDOWS
      graphics.PreferredBackBufferHeight = 480;
      graphics.PreferredBackBufferWidth = 800;
#endif
      TargetElapsedTime = TimeSpan.FromTicks(333333);
    }

    protected override void Initialize()
    {
      spriteBatch = new SpriteBatch(GraphicsDevice);

      Services.AddService(typeof(SpriteBatch), spriteBatch);

      //this.ShowLogoScreen(spriteBatch);
      this.ShowMainMenu();

      base.Initialize();
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(Color.Black);

      base.Draw(gameTime);
    }

    public void Draw()
    {
      Draw(new GameTime());
    }
  }
}
