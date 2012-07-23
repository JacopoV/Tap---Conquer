using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.IO.IsolatedStorage;
using System.IO;

namespace TapAndConquer.StateManager
{
  using Game = Microsoft.Xna.Framework.Game;

  public class TransitionScreen : Microsoft.Xna.Framework.DrawableGameComponent
  {
    public event Action OnTransitionEnd;
    public event Action OnTransitionStart;

    SpriteBatch SpriteBatch;
    ContentManager Content { get { return Game.Content; } }

    Matrix screen_transform;
    Texture2D transitionRenderTarget;
    float transitionTimer;
    static readonly float TRANSITION_TIME = 1.0f;
    int transitionMode;

    public TransitionScreen(Game game)
      : base(game)
    {
      this.UpdateOrder = 10;
      this.DrawOrder = 10;

      try
      {
        screen_transform = Matrix.Identity;

        //SpriteBatch = new SpriteBatch(Game.GraphicsDevice);
        SpriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
        var rt = new RenderTarget2D(Game.GraphicsDevice, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
        Game.GraphicsDevice.SetRenderTarget(rt);

        (Game as MainGame).Draw();

        Game.GraphicsDevice.SetRenderTarget(null);

        transitionRenderTarget = rt;
        transitionTimer = TRANSITION_TIME;

        transitionMode = 5; // (new Random()).Next(7);
      }
      catch (Exception e)
      {
        this.Enabled = this.Visible = false;
        this.Dispose();
      }
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
      base.OnEnabledChanged(sender, args);
    }

    bool transition_started = false;
    public override void Update(GameTime gameTime)
    {
      if (transition_started == false)
      {
        if (OnTransitionStart != null)
          OnTransitionStart();
        transition_started = true;
      }

      float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
      transitionTimer -= dt;
      if (transitionTimer <= 0.0f)
      {
        this.Dispose();
        if (OnTransitionEnd != null)
          OnTransitionEnd();
      }

      base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      float mu = 1.0f - transitionTimer / TRANSITION_TIME;
      float alpha = 1 - mu;

      SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, screen_transform);

      switch (transitionMode)
      {
        case 1:
          DrawOpenCurtainsTransition(alpha);
          break;

        case 2:
        case 5:
          DrawSpinningSquaresTransition(mu, alpha);
          break;

        case 3:
        case 4:
          DrawChequeredAppearTransition(mu);
          break;

        case 6:
          DrawFallingLinesTransition(mu);
          break;
        case 7:
          DrawBlendingInAndOutTransition();
          break;

        default:
          DrawShrinkAndSpinTransition(mu, alpha);
          break;
      }

      SpriteBatch.End();

      base.Draw(gameTime);
    }

    void DrawBlendingInAndOutTransition()
    {
      var r = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

      if (transitionTimer >= TRANSITION_TIME * 0.5f)
      {
        SpriteBatch.Draw(transitionRenderTarget, r, null, Color.Black);
        SpriteBatch.Draw(transitionRenderTarget, r, Color.White *
            ((transitionTimer / 2.0f - 0.5f) * 2.0f));
      }
      else
        SpriteBatch.Draw(transitionRenderTarget, r, null, Color.Black * (transitionTimer * 2.0f));
    }

    void DrawOpenCurtainsTransition(float alpha)
    {
      int w = (int)(240 * alpha * alpha);

      SpriteBatch.Draw(transitionRenderTarget, new Rectangle(0, 0, w, 800), new Rectangle(0, 0, 240, 800), Color.White * alpha);
      SpriteBatch.Draw(transitionRenderTarget, new Rectangle(480 - w, 0, w, 800), new Rectangle(240, 0, 240, 800), Color.White * alpha);
    }

    /// <summary>
    /// Transition effect where the screen splits into pieces, each spinning off in a different direction.
    /// </summary>
    void DrawSpinningSquaresTransition(float mu, float alpha)
    {
      Random random = new Random(23);

      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 4; y++)
        {
          Rectangle rect = new Rectangle(800 * x / 8, 480 * y / 4, 800 / 8, 480 / 4);

          Vector2 origin = new Vector2(rect.Width, rect.Height) / 2;

          float rotation = (float)(random.NextDouble() - 0.5) * mu * mu * 2;
          float scale = 1 + (float)(random.NextDouble() - 0.5f) * mu * mu;

          Vector2 pos = new Vector2(rect.Center.X, rect.Center.Y);

          pos.X += (float)(random.NextDouble() - 0.5) * mu * mu * 400;
          pos.Y += (float)(random.NextDouble() - 0.5) * mu * mu * 400;

          SpriteBatch.Draw(transitionRenderTarget, pos, rect, Color.White * alpha, rotation, origin, scale, 0, 0);
        }
      }
    }

    void DrawChequeredAppearTransition(float mu)
    {
      Random random = new Random(23);

      for (int x = 0; x < 8; x++)
      {
        for (int y = 0; y < 16; y++)
        {
          Rectangle rect = new Rectangle(480 * x / 8, 800 * y / 16, 480 / 8, 800 / 16);

          if (random.NextDouble() > mu * mu)
            SpriteBatch.Draw(transitionRenderTarget, rect, rect, Color.White);
        }
      }
    }

    void DrawFallingLinesTransition(float mu)
    {
      Random random = new Random(23);

      const int segments = 60;

      for (int x = 0; x < segments; x++)
      {
        Rectangle rect = new Rectangle(480 * x / segments, 0, 480 / segments, 800);

        Vector2 pos = new Vector2(rect.X, 0);

        pos.Y += 800 * (float)Math.Pow(mu, random.NextDouble() * 10);

        SpriteBatch.Draw(transitionRenderTarget, pos, rect, Color.White);
      }
    }

    void DrawShrinkAndSpinTransition(float mu, float alpha)
    {
      Vector2 origin = new Vector2(240, 400);
      Vector2 translate = (new Vector2(32, 800 - 32) - origin) * mu * mu;

      float rotation = mu * mu * -4;
      float scale = alpha * alpha;

      Color tint = Color.White * (float)Math.Sqrt(alpha);

      SpriteBatch.Draw(transitionRenderTarget, origin + translate, null, tint, rotation, origin, scale, 0, 0);
    }
  }
}