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
using TapAndConquer.StateManager;

namespace TapAndConquer
{
  public class MainGame : Microsoft.Xna.Framework.Game
  {
    GraphicsDeviceManager graphics;
    SpriteBatch spriteBatch;

    public MainGame()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
      graphics.IsFullScreen = true;

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
