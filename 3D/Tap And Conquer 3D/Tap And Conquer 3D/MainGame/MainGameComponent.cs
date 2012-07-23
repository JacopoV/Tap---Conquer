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
using System.IO.IsolatedStorage;
using TapAndConquer3D.Resources;
using TapAndConquer3D.PersistentState;
using StateGameLayer;


namespace TapAndConquer3D
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
    SoundManager soundState;

      //State of the game
    StateGame stategame;

    WinLoseState winLoseState = WinLoseState.None;
    bool drawEnable = true;

    public event Action Back;
    public event Action Win;
    public event Action Lose;
    public MainGameComponent(Game game, bool forceNewGame, GameDifficulty difficulty)
      : base(game)
    {
      TouchPanel.EnabledGestures = 
          GestureType.Tap 
          | GestureType.DoubleTap 
          | GestureType.FreeDrag
          |GestureType.DragComplete
          |GestureType.Pinch 
          |GestureType.PinchComplete
          | GestureType.Hold;

      if (forceNewGame)
      {
          stategame = new StateGame(true);
          Game.Services.AddService((typeof(StateGame)), stategame);
      }
      else
      {
          stategame = CreateOrResumeState();
          Game.Services.AddService((typeof(StateGame)), stategame);
      }

      gameState = new GameState(difficulty, Content);
      Game.Services.AddService((typeof(GameState)), gameState);
      renderingState = new RenderingState(Content);
      Game.Services.AddService((typeof(RenderingState)), renderingState);
      soundState = new SoundManager(Content);
    }

    ContentManager Content { get { return Game.Content; } }

    protected override void LoadContent()
    {
      base.LoadContent();
    }


    public override void Initialize()
    {
      spriteBatch = Game.Services.GetService(typeof(SpriteBatch)) as SpriteBatch;
      base.Initialize();
    }

#if WINDOWS_PHONE
    IsolatedStorageFile IS = IsolatedStorageFile.GetUserStoreForApplication();
#endif

#if WINDOWS
    IsolatedStorageFile IS = IsolatedStorageFile.GetUserStoreForDomain();
#endif

    string savePath = "./current.sav";

    private StateGame CreateOrResumeState()
    {
        if (IS.FileExists(savePath))
        {
            try
            {
                StateGame stateGame = null;
                using (var saveFile = IS.OpenFile(savePath, System.IO.FileMode.Open))
                {
                    var deserializer = new XmlSerializer(typeof(StateGame));
                    stateGame = deserializer.Deserialize(saveFile) as StateGame;
                }
                IS.DeleteFile(savePath);
                return stateGame;
            }
            catch (Exception e)
            {
                return new StateGame(true);
            }
        }
        else
        {
            return new StateGame(true);
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
                var serializer = new XmlSerializer(typeof(StateGame));
                serializer.Serialize(saveFile, stategame);
            }
        }
        catch (Exception e)
        {
            System.Console.Write(e.Message);
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
              Game.Services.RemoveService(typeof(StateGame));
              Win();
              break;

        case WinLoseState.Lose:
          try
          {
            if (IS.FileExists(savePath))
              IS.DeleteFile(savePath);
          }
          catch (Exception)
          {
          }
          Game.Services.RemoveService(typeof(StateGame));
          Lose();
          break;


        case WinLoseState.None:
        
          gameState.UpdateTime(gameTime, renderingState);
#if WINDOWS_PHONE

              gameState.UpdateInput(renderingState, gameTime);


#endif
#if WINDOWS
        gameState.UpdateInputWindows(renderingState, gameTime);

        KeyboardState keyboard = Keyboard.GetState();

        if(keyboard.IsKeyDown(Keys.Escape))
        {
            if (Back != null)
                    {
                        SaveGameState();
                        Game.Services.RemoveService(typeof(StateGame));
                        Game.Services.RemoveService((typeof(GameState)));
                        Game.Services.RemoveService((typeof(RenderingState)));
                        Back();
                    }
        }
#endif
          winLoseState = gameState.UpdateWinLose();
          if (winLoseState != WinLoseState.None)
          {
            //HighScores.AddScore(gameState.difficulty, new HighScore()
            //{
            //  timeStamp = DateTime.Now,
            //  score = gameState.score
            //});
              Game.Services.RemoveService(typeof(GameState));
              Game.Services.RemoveService(typeof(RenderingState));
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
                        Game.Services.RemoveService(typeof(StateGame));
                        Game.Services.RemoveService((typeof(GameState)));
                        Game.Services.RemoveService((typeof(RenderingState)));
                        Back();
                    }
                }
                else{
                    drawEnable = true;
                }
              }
              ), null);
          }
          break;
          }

          soundState.updateSounds();
    }

    public override void Draw(GameTime gameTime)
    {
        if (drawEnable)
        {
            renderingState.Draw(gameState, GraphicsDevice);
        }
      base.Draw(gameTime);
    }
  }
}
