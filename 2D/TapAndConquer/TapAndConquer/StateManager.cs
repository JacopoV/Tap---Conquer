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

namespace TapAndConquer
{
    public static class StateManager
    {
        public static void ShowMainMenu(this Game game)
        {
            game.CleanupComponents();
            var menu = new MainMenu(game);
            menu.loadContent();
            menu.playGame += game.StartGame;
            menu.quit += game.Exit;
            game.Components.Add(menu);
        }

        public static void StartGame(this Game game)
        {
            game.CleanupComponents();
            var mainGame = new Manager(game);
            mainGame.win += () => game.EndGame(true);
            mainGame.loose += () => game.EndGame(false);
            game.Components.Add(mainGame);
        }

        public static void EndGame(this Game game, bool win)
        {
            game.CleanupComponents();
            var endMenu = new EndGameMenu(game, win);
            endMenu.restart += game.StartGame;
            endMenu.menu += game.ShowMainMenu;
            game.Components.Add(endMenu);
        }

        public static void CleanupComponents(this Game game)
        {
            game.StartNewTransitionScreen();

            for (int i = 0; i < game.Components.Count; i++)
            {
                if (game.Components[i] is TransitionScreen) continue;
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
