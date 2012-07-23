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
using System.Xml.Serialization;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using TapAndConquer.Resources;
using TapAndConquer.PersistentState;


namespace TapAndConquer
{
  enum TooltipState
  {
    Opening,
    Showing,
    Hidden,
    Closing
  }
  public class MainGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
  {
    SpriteBatch spriteBatch;
    RenderingState renderingState;
    GameState gameState;
    WinLoseState winLoseState = WinLoseState.None;
    bool drawEnable = true;

    public event Action Back;
    public MainGameComponent(Game game, bool forceNewGame, GameDifficulty difficulty)
      : base(game)
    {
      TouchPanel.EnabledGestures = GestureType.Tap | GestureType.FreeDrag | GestureType.DragComplete;

      PhoneApplicationService.Current.Closing += (s, e) => SaveGameState();
      PhoneApplicationService.Current.Deactivated += (s, e) => SaveGameState();
      if (forceNewGame)
          gameState = new GameState(difficulty);
      else
          gameState = CreateOrResumeState(difficulty);

    }

    ContentManager Content { get { return Game.Content; } }

    protected override void LoadContent()
    {
      renderingState = new RenderingState(Content);

      base.LoadContent();
    }


    public override void Initialize()
    {
      spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;

      base.Initialize();
    }

    IsolatedStorageFile IS = IsolatedStorageFile.GetUserStoreForApplication();
    string savePath = "./current.sav";

    private GameState CreateOrResumeState(GameDifficulty createDifficulty)
    {
      if (IS.FileExists(savePath))
      {
        try
        {
          GameState gameState = null;
          using (var saveFile = IS.OpenFile(savePath, System.IO.FileMode.Open))
          {
            var deserializer = new XmlSerializer(typeof(GameState));
            gameState = deserializer.Deserialize(saveFile) as GameState;
          }
          IS.DeleteFile(savePath);
          return gameState;
        }
        catch (Exception e)
        {
          if (IS.FileExists(savePath))
            IS.DeleteFile(savePath);
          return new GameState(createDifficulty);
        }
      }
      else
      {
        return new GameState(createDifficulty);
      }
    }

    private void SaveGameState()
    {
      if (winLoseState != WinLoseState.None)
        return;
      try
      {
        using (var saveFile = IS.CreateFile(savePath))
        {
          var serializer = new XmlSerializer(typeof(GameState));
          serializer.Serialize(saveFile, gameState);
        }
      }
      catch (Exception)
      {
      }
    }

    public override void Update(GameTime gameTime)
    {
      UpdateMainGame(gameTime);
      base.Update(gameTime);
    }

    private void UpdateMainGame(GameTime gameTime)
    {
      switch (winLoseState)
      {
        case WinLoseState.Win:
        case WinLoseState.Lose:
          while (TouchPanel.IsGestureAvailable)
          {
            var gs = TouchPanel.ReadGesture();
            if (gs.GestureType == GestureType.Tap)
            {
              gameState = new GameState(gameState.difficulty);
              winLoseState = WinLoseState.None;
              SaveGameState();
            }
          }
          try
          {
            if (IS.FileExists(savePath))
              IS.DeleteFile(savePath);
          }
          catch (Exception)
          {
          }
          if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
              if (Back != null)
              {
                  drawEnable = false;
                  Back();
              }
          break;
        case WinLoseState.None:
          gameState.UpdateTime(gameTime, renderingState);
          gameState.UpdateInput(renderingState, gameTime);
          //winLoseState = gameState.UpdateBlocks(gameTime, renderingState, random);
          if (winLoseState != WinLoseState.None)
          {
            HighScores.AddScore(gameState.difficulty, new HighScore()
            {
              timeStamp = DateTime.Now,
              score = gameState.score
            });
          }
          if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
          {
            this.Enabled = false;
            drawEnable = false;
            Guide.BeginShowMessageBox(Strings.ConfirmExit, Strings.ConfirmExitMessage,
              new[] { Strings.Yes, Strings.No }, 0, MessageBoxIcon.Alert, new AsyncCallback(result =>
              {
                var dialogResult = Guide.EndShowMessageBox(result);
                this.Enabled = true;
                if (dialogResult.HasValue && dialogResult.Value == 0)
                {
                    if (Back != null)
                    {
                        SaveGameState();
                        Back();
                    }
                }
                else
                    drawEnable = true;
              }), null);
          }
          break;
      }
    }

    public override void Draw(GameTime gameTime)
    {
        if (drawEnable)
        {
            spriteBatch.Begin();

            if (winLoseState == WinLoseState.Lose)
            {
                //  renderingState.DrawCentered(spriteBatch, Strings.GameLoseMessage,
                //    new Vector2(400, 240), Color.White);
            }
            else
            {
            renderingState.Draw(spriteBatch, gameState);
            }

            spriteBatch.End();
        }
      base.Draw(gameTime);
    }
  }
}
