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
using TapAndConquer3D.Menu;


namespace TapAndConquer3D.StateManager
{
    public static class StateManager
    {
        public static void ShowLogoScreen(this Game game, SpriteBatch spriteBatch)
        {
            var isTrial = Guide.IsTrialMode;

            game.CleanupComponents();
            var logo_screen = new LogoScreen(game, () => { game.ShowMainMenu(); 
                if (isTrial) game.Components.Add(new TrialComponent(game, spriteBatch, 120, "triangleshooter")); }, 
                "Textures/vst_logo");
            game.Components.Add(logo_screen);
        }

        public static void ShowMainMenu(this Game game)
        {
            game.CleanupComponents();
            var menu = new MainMenu(game);
            menu.Exit += game.Exit;
            menu.NewGame += game.ShowNewGameMenu;
            menu.ResumeGame += game.ResumeGame;
            menu.HighScores += game.ShowHighScoresMenu;
            game.Components.Add(menu);
        }

        public static void ShowNewGameMenu(this Game game)
        {
            game.CleanupComponents();
            var menu = new NewGameMenu(game);
            menu.Back += game.ShowMainMenu;
            menu.Easy += () => game.StartGame(GameDifficulty.Easy);
            menu.Medium += () => game.StartGame(GameDifficulty.Normal);
            menu.Hard += () => game.StartGame(GameDifficulty.Hard);
            game.Components.Add(menu);
        }

        public static void ShowHighScoresMenu(this Game game)
        {
            game.CleanupComponents();
            var menu = new HighScoresMenu(game);
            menu.Back += game.ShowMainMenu;
            game.Components.Add(menu);
        }

        public static void StartGame(this Game game, GameDifficulty difficulty)
        {
            game.CleanupComponents();
            var mainGame = new MainGameComponent(game, true, difficulty);
            mainGame.Back += game.ShowMainMenu;
            mainGame.Win += () => game.ShowEndMenu(true);
            mainGame.Lose += () => game.ShowEndMenu(false);
            game.Components.Add(mainGame);
        }

        public static void ShowEndMenu(this Game game, bool isWinner)
        {
            game.CleanupComponents();
            var endMenu = new EndGameMenu(game, isWinner);
            endMenu.restart += game.ShowNewGameMenu;
            endMenu.menu += game.ShowMainMenu;
            game.Components.Add(endMenu);
        }

        public static void ResumeGame(this Game game)
        {
            game.CleanupComponents();
            var mainGame = new MainGameComponent(game, false, GameDifficulty.Easy);
            mainGame.Back += game.ShowMainMenu;
            mainGame.Win += () => game.ShowEndMenu(true);
            mainGame.Lose += () => game.ShowEndMenu(false);
            game.Components.Add(mainGame);
        }

        public static void CleanupComponents(this Game game)
        {
            game.StartNewTransitionScreen();

            for (int i = 0; i < game.Components.Count; i++)
            {
                if (game.Components[i] is TransitionScreen || game.Components[i] is TrialComponent) continue;
                ((GameComponent)game.Components[i]).Dispose();
                i--;
            }
        }



        static bool first_transition = true;
        public static void StartNewTransitionScreen(this Game game)
        {
            TransitionScreen transition = null;
            if (first_transition != true)
                transition = new TransitionScreen(game);

            if (first_transition != true)
            {
                game.Components.Add(transition);

                transition.OnTransitionStart += () =>
                {
                    for (int i = 0; i < game.Components.Count; i++)
                    {
                        GameComponent gc = (GameComponent)game.Components[i];
                        if (gc != transition)
                            gc.Enabled = false;
                    }
                };

                transition.OnTransitionEnd += () =>
                {
                    for (int i = 0; i < game.Components.Count; i++)
                    {
                        GameComponent gc = (GameComponent)game.Components[i];
                        if (gc != transition)
                            gc.Enabled = true;
                        else
                        {
                            game.Components.RemoveAt(i);
                            i--;
                        }
                    }
                };
            }

            first_transition = false;
        }

    }
}
