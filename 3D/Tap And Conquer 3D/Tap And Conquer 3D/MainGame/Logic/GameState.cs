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
using StateGameLayer;


namespace TapAndConquer3D
{
    using Difficulty = GameDifficulty;
    using System.Threading;

#if WINDOWS_PHONE
    using Microsoft.Devices;
#endif

    public enum WinLoseState
    {
        Win,
        Lose,
        None
    }

    public class GameState : BonusPowerState
    {
        public Difficulty difficulty;
        public float turn = 5;

        public StateGame stateGame;

        //OWN VARIABLE

        public GameState(Difficulty difficulty, ContentManager Content)
            :base(Content)
        {
            this.difficulty = difficulty;
            switch (difficulty)
            {
                case 0:
                    turn = 8;
                    break;
                case (Difficulty)1:
                    turn = 6;
                    break;
                case (Difficulty)2:
                    turn = 4;
                    break;
            }
            stateGame = Content.ServiceProvider.GetService(typeof(StateGame)) as StateGame;
           
            loadNeutralFactories();
        }

        //UPDATE GAME
        public void UpdateTime(GameTime gameTime, RenderingState renderingState)
        {
            if (stateGame.state.enabled)
            {
                var deltaT = (float)gameTime.ElapsedGameTime.TotalSeconds;

                stateGame.state.totalGameTime += gameTime.ElapsedGameTime.TotalSeconds;

                stateGame.state.lastTurn += deltaT;

                if (stateGame.state.lastTurn >= turn)
                {
                    regen();
                    stateGame.state.lastTurn = 0;
                    bool done = false;
                    for (int i = 0; i < stateGame.state.player1_orders.Count && !done; ++i)
                    {
                        int create = 0;
                        if (stateGame.state.player1_orders[i].Origin.Count > 0 && stateGame.state.player1_orders[i].Origin[0].ObjectType == 1 && stateGame.state.player1_orders[i].Origin[0].getSendableTroops(false) > 0)
                        {
                            addTroop(stateGame.state.player1_orders[i].Origin[0].ObjectType, stateGame.state.player1_orders[i].Origin[0].CurrentPos, stateGame.state.player1_orders[i].Origin[0].getSendableTroops(true), stateGame.state.player1_orders[i].Target[0], stateGame.state.player1_orders[i].Origin[0].MaxHitPoints, stateGame.state.player1_orders[i].Origin[0].Damage, stateGame.state.player1_orders[i].Origin[0].Defense, stateGame.state.player1_orders[i].Origin[0].Shields);
                            create++;
                        }
                        if (stateGame.state.player1_orders[i].Origin.Count > 1 && stateGame.state.player1_orders[i].Origin[1].ObjectType == 1 && stateGame.state.player1_orders[i].Origin[1].getSendableTroops(false) > 0)
                        {
                            addTroop(stateGame.state.player1_orders[i].Origin[1].ObjectType, stateGame.state.player1_orders[i].Origin[1].CurrentPos, stateGame.state.player1_orders[i].Origin[1].getSendableTroops(true), stateGame.state.player1_orders[i].Target[0], stateGame.state.player1_orders[i].Origin[1].MaxHitPoints, stateGame.state.player1_orders[i].Origin[1].Damage, stateGame.state.player1_orders[i].Origin[1].Defense, stateGame.state.player1_orders[i].Origin[1].Shields);
                            if (create > 0)
                                setFellow();
                            create++;
                        }
                        stateGame.state.player1_orders.RemoveAt(0);
                        i--;
                        if (create > 0)
                            done = true;
                    }
                    done = false;

                    for (int i = 0; i < stateGame.state.player2_orders.Count && !done; ++i)
                    {
                        int create = 0;
                        if (stateGame.state.player2_orders[i].Origin.Count > 0 && stateGame.state.player2_orders[i].Origin[0].ObjectType == 2 && stateGame.state.player2_orders[i].Origin[0].getSendableTroops(false) > 0)
                        {
                            addTroop(stateGame.state.player2_orders[i].Origin[0].ObjectType, stateGame.state.player2_orders[i].Origin[0].CurrentPos, stateGame.state.player2_orders[i].Origin[0].getSendableTroops(true), stateGame.state.player2_orders[i].Target[0], stateGame.state.player2_orders[i].Origin[0].MaxHitPoints, stateGame.state.player2_orders[i].Origin[0].Damage, stateGame.state.player2_orders[i].Origin[0].Defense, stateGame.state.player2_orders[i].Origin[0].Shields);
                            create++;
                        }
                        if (stateGame.state.player2_orders[i].Origin.Count > 1 && stateGame.state.player2_orders[i].Origin[1].ObjectType == 2 && stateGame.state.player2_orders[i].Origin[1].getSendableTroops(false) > 0)
                        {
                            addTroop(stateGame.state.player2_orders[i].Origin[1].ObjectType, stateGame.state.player2_orders[i].Origin[1].CurrentPos, stateGame.state.player2_orders[i].Origin[1].getSendableTroops(true), stateGame.state.player2_orders[i].Target[0], stateGame.state.player2_orders[i].Origin[1].MaxHitPoints, stateGame.state.player2_orders[i].Origin[1].Damage, stateGame.state.player2_orders[i].Origin[1].Defense, stateGame.state.player2_orders[i].Origin[1].Shields);
                            if (create > 0)
                                setFellow();
                            create++;
                        }
                        stateGame.state.player2_orders.RemoveAt(0);
                        i--;
                        if (create > 0)
                            done = true;
                    }
                }

                base.update(deltaT, renderingState);
            }
        }

        //LASTTURN SET/GET
        public float LastTurn
        {
            set { stateGame.state.lastTurn = value; }

            get { return stateGame.state.lastTurn; }
        }

        //TOUCHBEGIN SET/GET
        public bool TouchBegin
        {
            set { stateGame.state.touchBegin = value; }

            get { return stateGame.state.touchBegin; }
        }

        // RETURN TRUE IF ORDER LIST IS FULL
        private bool orderFull(int player)
        {
            if (player == 1 && stateGame.state.player1_orders.Count > 2)
                return true;
            if (player == 2 && stateGame.state.player2_orders.Count > 2)
                return true;
            return false;
        }

        /*
         * Return number of order for the player "player"
         */
        public int numberOrders(int player)
        {
            if (player == 1)
                return stateGame.state.player1_orders.Count;
            if (player == 2)
                return stateGame.state.player2_orders.Count;
            return -1;
        }

        public void stop() 
        {
            stateGame.state.enabled = false;
        }

        public void start()
        {
            stateGame.state.enabled = true;
        }

        public override void regen()
        {
            base.regen();
        }

        public List<Vector3> arrowPosition = new List<Vector3>();
        public StateGameLayer.Factory firstFactory, secondFactory, thirdFactory;

        private void manageBonus(int bonus, RenderingState renderingState)
        {
            if (stateGame.state.enabled)
            {
                List<float> pwr = getPowers();
                if (pwr[bonus] == 1.0f)
                {
                    /*
                     * Bonus 0 = speed up di truppe e produzione pianeti
                     * Bonus 1 = blocca produzione truppe nemiche
                     * Bonus 2 = dimezza truppe nemiche nelle fabbriche
                     * Bnous 3 = distruzione di tutte le truppe in viaggio nemiche
                     */
                    activatePower(bonus);

                    switch (bonus)
                    {
                        case 0:
                            renderingState.isSpeedBonusActive = false;
                            break;
                        case 1:
                            renderingState.isFreezeBonusActive = false;
                            break;
                        case 2:
                            renderingState.isHalfFactoryBonusActive = false;
                            break;
                        case 3:
                            renderingState.isArmageddonBonusActive = false;
                            break;
                    }
                }
            }
        }

        public void UpdateInput(RenderingState renderingState, GameTime time)
        {
            while (TouchPanel.IsGestureAvailable)
            {
                var touchGesture = TouchPanel.ReadGesture();
                Ray pickRay;

                switch (touchGesture.GestureType)
                {
                    /*
                     * Single tap select a factory
                     */
                    case GestureType.Tap:
                        {
                            if (stateGame.state.enabled)
                            {
                                if (renderingState.speedBonusRect.Contains(
                                    new Point((int)touchGesture.Position.X, (int)touchGesture.Position.Y)))
                                {
                                    /*
                                     * Player clicked on the speed bonus.
                                     */
                                    manageBonus(0, renderingState);
                                    return;
                                }

                                if (renderingState.freezeBonusRect.Contains(
                                    new Point((int)touchGesture.Position.X, (int)touchGesture.Position.Y)))
                                {
                                    /*
                                     * Player clicked on the attack bonus
                                     */
                                    manageBonus(1, renderingState);
                                    return;
                                }

                                if (renderingState.halfFactoryBonusRect.Contains(
                                    new Point((int)touchGesture.Position.X, (int)touchGesture.Position.Y)))
                                {
                                    /*
                                     * Player clicked on the shield bonus
                                     */
                                    manageBonus(2, renderingState);
                                    return;
                                }

                                if (renderingState.armageddonBonusRect.Contains(
                                new Point((int)touchGesture.Position.X, (int)touchGesture.Position.Y)))
                                {
                                    manageBonus(3, renderingState);
                                    return;
                                }

                                StateGameLayer.Factory targeted = null;

                                pickRay = calculateRay(new Vector2(touchGesture.Position.X, touchGesture.Position.Y), renderingState);

                                targeted = calculateTargetedFactory(pickRay, renderingState);

                                if (targeted != null && targeted.ObjectType == 1)
                                {
                                    /*
                                    * ...player selected the factory "targeted".
                                    * If here, "targeted" is property of player. A player 
                                    * can only select his own factories (type == 1).
                                    * If firstFactory is null, this is the first factory
                                    * the player selected.
                                    */
                                    if (firstFactory == null)
                                    {
                                        firstFactory = targeted;
                                        firstFactory.Selected = true;
                                        firstFactory.selectionSound = true;
                                    }
                                    else
                                    {
                                        /*
                                         * ...else this is the second factory.
                                         * Player can only double select his own factories.
                                         * Cannot select: firstFactory and an enemy factory.
                                         * Cannot select: firstFactory and a neutral factory.
                                         */

                                        /*
                                         * Player maybe have already tapped a second factory and
                                         * selects another secondFactory. Reset previous secondFactory.
                                         */
                                        if (secondFactory != null)
                                            secondFactory.Selected = false;

                                        secondFactory = targeted;
                                        if (secondFactory.ObjectType == 1)
                                        {
                                            secondFactory.Selected = true;
                                            secondFactory.selectionSound = true;
                                        }
                                        else
                                        {
                                            secondFactory.Selected = false;
                                            secondFactory = null;
                                        }
                                    }

                                    return;
                                }

                                /*
                                * If here, player targeted an enemy or neutral factory.
                                * Or maybe player tapped on nothing (no collision). 
                                * Reset.
                                */
                                if (firstFactory != null)
                                {
                                    firstFactory.Selected = false;
                                    firstFactory = null;
                                }

                                if (secondFactory != null)
                                {
                                    secondFactory.Selected = false;
                                    secondFactory = null;
                                }

                                if (thirdFactory != null)
                                {
                                    thirdFactory.Selected = false;
                                    thirdFactory = null;
                                }
                            }
                        }
                        if (renderingState.pauseButton.Contains(
                        new Point((int)touchGesture.Position.X, (int)touchGesture.Position.Y)))
                        {
                            /*
                            * Player clicked on pause button
                            */
                            if (stateGame.state.enabled)
                                stop();
                            else
                                start();


                            return;
                        }
                        break;

                    /*
                     * Double tap to do an action
                     */
                    case GestureType.Hold:
                        {
                            if (stateGame.state.enabled)
                            {
                                /*
                                * Player wants to do an action.
                                * If a firstFactory was not selected, then cannot do anything
                                */
                                if (firstFactory == null)
                                    return;

                                pickRay = calculateRay(new Vector2(touchGesture.Position.X, touchGesture.Position.Y), renderingState);

                                StateGameLayer.Factory targeted = calculateTargetedFactory(pickRay, renderingState);

                                if (targeted == null)
                                    return;

                                /*Here targeted is not null*/
                                if (secondFactory == null)
                                {
                                    /*
                                    * If here, player has only selected a first factory.
                                    * Selecting the second factory with a double tap 
                                    * means he wants to attack an enemy factory (type == 2) or defend
                                    * his own factory (type == 1).
                                    */

                                    secondFactory = targeted;
                                    //secondFactory.Selected = true;

                                    /*
                                     * Player cannot move tropps from and to the same factory
                                     */
                                    if (secondFactory.Equals(firstFactory))
                                        return;

                                    List<StateGameLayer.Factory> order = new List<StateGameLayer.Factory>();
                                    order.Add(firstFactory);
                                    order.Add(secondFactory);
                                    stateGame.state.addOrder(order, 1);

                                    arrowPosition.Clear();
                                    arrowPosition.Add(firstFactory.CurrentPos);
                                    arrowPosition.Add(secondFactory.CurrentPos);

                                    stateGame.state.touchBegin = true;
                                }
                                else
                                {
                                    /*Double attack*/

                                    /*
                                     * ...else the player has already selected a second factory and
                                     * wants to do a double attack or move his troops from
                                     * two factories to another one of his.
                                     */

                                    thirdFactory = targeted;
                                    //thirdFactory.Selected = true;

                                    /*Player cannot move troops to one of the two selected factories*/
                                    if (thirdFactory.Equals(firstFactory) || thirdFactory.Equals(secondFactory))
                                        return;

                                    List<StateGameLayer.Factory> order = new List<StateGameLayer.Factory>();
                                    order.Add(firstFactory);
                                    order.Add(secondFactory);
                                    order.Add(thirdFactory);

                                    stateGame.state.addOrder(order, 1);

                                    arrowPosition.Clear();
                                    arrowPosition.Add(firstFactory.CurrentPos);
                                    arrowPosition.Add(secondFactory.CurrentPos);
                                    arrowPosition.Add(thirdFactory.CurrentPos);
                                }

                                /*
                                * If here, player launched a single, or double attack, or 
                                * moved his troops or double tapped on nothing.
                                * Reset.
                                */
                                if (firstFactory != null)
                                {
                                    firstFactory.Selected = false;
                                    firstFactory = null;
                                }

                                if (secondFactory != null)
                                {
                                    secondFactory.Selected = false;
                                    secondFactory = null;
                                }

                                if (thirdFactory != null)
                                {
                                    thirdFactory.Selected = false;
                                    thirdFactory = null;
                                }

                                stateGame.state.touchBegin = true;
                            }
                        }
                        break;

                    case GestureType.Pinch:
                        {
                            float distance;

                            Vector2 firstFinger = new Vector2(touchGesture.Delta.X, touchGesture.Delta2.Y);
                            Vector2 secondFinger = new Vector2(touchGesture.Delta2.X, touchGesture.Delta2.Y);

                            distance = Vector2.Distance(firstFinger, secondFinger);

                            if (stateGame.state.previousDistance == null)
                                stateGame.state.previousDistance = distance;

                            if (distance < stateGame.state.previousDistance)
                            {
                                /*Aumenta il fieldOfVIew*/
                                renderingState.zoomOut();
                            }
                            else
                                renderingState.zoomIn();
                        }
                        break;

                    case GestureType.FreeDrag:
                        {
                            if (stateGame.state.enabled)
                            {
                                float speedOfDragging = 7.8f;
                                if (stateGame.state.dragging.Count < 2)
                                {
                                    stateGame.state.dragging.Add(new Vector2(touchGesture.Position.X, touchGesture.Position.Y));
                                }
                                else
                                {
                                    /*If here, 2 drag position*/
                                    float x = stateGame.state.dragging[0].X - stateGame.state.dragging[1].X;

                                    /*Calc the Z translation (Z for the world, Y for the touchscreen*/
                                    float z = stateGame.state.dragging[0].Y - stateGame.state.dragging[1].Y;

                                    /*See you.*/
                                    renderingState.TranslateX(x * speedOfDragging);
                                    renderingState.TranslateZ(z * speedOfDragging);

                                    /*Remove the ancient position*/
                                    stateGame.state.dragging.RemoveAt(0);
                                }
                            }
                        }
                        break;

                    case GestureType.DragComplete:
                        /*Reset info about dragging*/
                        stateGame.state.dragging.Clear();
                        break;
                }

            }
        }

        private StateGameLayer.Factory calculateTargetedFactory(Ray pickRay, RenderingState renderingState)
        {
            StateGameLayer.Factory targeted = null;
            float? nearestObject = -100000000000000;
            foreach (StateGameLayer.Factory i in stateGame.factory.factories)
            {
                Vector3 position = i.CurrentPos;
                BoundingSphere boundingSphere = renderingState.BoundingSpherePlanet;

                float? distance = 0;
                bool collision = false;
                /*NB: da ottimizzare con il for && !found*/
                boundingSphere.Radius *= i.scaleFactor;
                boundingSphere.Center = position;
                distance = pickRay.Intersects(boundingSphere);
                if (pickRay.Intersects(boundingSphere) != null)
                {
                    collision = true;

                }
                /*
                 * If collision is true, ray intersected at least one of the bounding spheres.
                 * Factory "i" collided.                     
                 * Check if this factory is the nearest to player's point of view.
                 */
                if (collision && i.CurrentPos.Z > nearestObject)
                {
                    nearestObject = i.CurrentPos.Z;
                    targeted = i;
                }
            }

            return targeted;

        }

        private Ray calculateRay(Vector2 position, RenderingState renderingState)
        {
            /* Calculate from the tap position X and Y the ray
            * that intersects the target factory's bounding sphere
            */
            Vector3 nearPoint = new Vector3(position.X, position.Y, 0);
            Vector3 farPoint = new Vector3(position.X, position.Y, 1);

            nearPoint = renderingState.spriteBatch.GraphicsDevice.Viewport.Unproject
                (nearPoint, renderingState.Projection, renderingState.View, renderingState.World);

            farPoint = renderingState.spriteBatch.GraphicsDevice.Viewport.Unproject
                (farPoint, renderingState.Projection, renderingState.View, renderingState.World);

            Vector3 direction = Vector3.Normalize(farPoint - nearPoint);

            return new Ray(nearPoint, direction);
        }


        private ButtonState buttonState;
        private ButtonState lastButtonState = ButtonState.Released;
        private double myTimer = 0.0f;
        private double firstClick = 0.0f, secondClick = 0.0f;
        private const float maxTimeForDoubleClick = 200;
        private bool playerClicked = false;
        private float movingSpeed = 50.0f;

        private void doubleClick(RenderingState renderingState, GameTime time)
        {
            float x = Mouse.GetState().X;
            float y = Mouse.GetState().Y;

            Ray pickRay;

            /*
            * Player wants to do an action.
            * If a firstFactory was not selected, then cannot do anything
            */
            if (firstFactory == null)
                return;

            pickRay = calculateRay(new Vector2(x, y), renderingState);

            StateGameLayer.Factory targeted = calculateTargetedFactory(pickRay, renderingState);

            if (targeted == null)
                return;

            /*Here targeted is not null*/
            if (secondFactory == null)
            {
                /*
                * If here, player has only selected a first factory.
                * Selecting the second factory with a double tap 
                * means he wants to attack an enemy factory (type == 2) or defend
                * his own factory (type == 1).
                */

                secondFactory = targeted;
                //secondFactory.Selected = true;

                /*
                 * Player cannot move tropps from and to the same factory
                 */
                if (secondFactory.Equals(firstFactory))
                    return;



                List<StateGameLayer.Factory> order = new List<StateGameLayer.Factory>();
                order.Add(firstFactory);
                order.Add(secondFactory);
                stateGame.state.addOrder(order, 1);

                arrowPosition.Clear();
                arrowPosition.Add(firstFactory.CurrentPos);
                arrowPosition.Add(secondFactory.CurrentPos);


                //firstFactory.Selected = false;
                //firstFactory = null;
                stateGame.state.touchBegin = true;
            }
            else
            {
                /*Double attack*/

                /*
                 * ...else the player has already selected a second factory and
                 * wants to do a double attack or move his troops from
                 * two factories to another one of his.
                 */

                thirdFactory = targeted;
                //thirdFactory.Selected = true;

                /*Player cannot move troops to one of the two selected factories*/
                if (thirdFactory.Equals(firstFactory) || thirdFactory.Equals(secondFactory))
                    return;

                List<StateGameLayer.Factory> order = new List<StateGameLayer.Factory>();
                order.Add(firstFactory);
                order.Add(secondFactory);
                order.Add(thirdFactory);

                stateGame.state.addOrder(order, 1);

                arrowPosition.Clear();
                arrowPosition.Add(firstFactory.CurrentPos);
                arrowPosition.Add(secondFactory.CurrentPos);
                arrowPosition.Add(thirdFactory.CurrentPos);

                //firstFactory.Selected = false;
                //firstFactory = null;

                //secondFactory.Selected = false;
                //secondFactory = null;
            }

            /*
            * If here, player launched a single, or double attack, or 
            * moved his troops or double tapped on nothing.
            * Reset.
            */
            if (firstFactory != null)
            {
                firstFactory.Selected = false;
                firstFactory = null;
            }

            if (secondFactory != null)
            {
                secondFactory.Selected = false;
                secondFactory = null;
            }

            if (thirdFactory != null)
            {
                thirdFactory.Selected = false;
                thirdFactory = null;
            }

            stateGame.state.touchBegin = true;


        }

        private void singleClick(RenderingState renderingState, GameTime time)
        {
            float x = Mouse.GetState().X;
            float y = Mouse.GetState().Y;

            Ray pickRay;

            if (renderingState.speedBonusRect.Contains(
                new Point((int)x, (int)y)))
            {

                /*
                 * Player clicked on the speed bonus.
                 */
                manageBonus(0, renderingState);
                return;
            }

            if (renderingState.freezeBonusRect.Contains(
                new Point((int)x, (int)y)))
            {
                /*
                 * Player clicked on the attack bonus
                 */
                manageBonus(1, renderingState);
                return;

            }

            if (renderingState.halfFactoryBonusRect.Contains(
                new Point((int)x, (int)y)))
            {
                /*
                 * Player clicked on the shield bonus
                 */
                manageBonus(2, renderingState);
                return;

            }


            if (renderingState.armageddonBonusRect.Contains(
            new Point((int)x, (int)y)))
            {
                manageBonus(3, renderingState);
                return;
            }

            StateGameLayer.Factory targeted = null;

            pickRay = calculateRay(new Vector2(x, y), renderingState);

            targeted = calculateTargetedFactory(pickRay, renderingState);

            if (targeted != null && targeted.ObjectType == 1)
            {
                /*
                * ...player selected the factory "targeted".
                * If here, "targeted" is property of player. A player 
                * can only select his own factories (type == 1).
                * If firstFactory is null, this is the first factory
                * the player selected.
                */
                if (firstFactory == null)
                {
                    firstFactory = targeted;
                    firstFactory.Selected = true;
                    firstFactory.selectionSound = true;
                }
                else
                {
                    /*
                     * ...else this is the second factory.
                     * Player can only double select his own factories.
                     * Cannot select: firstFactory and an enemy factory.
                     * Cannot select: firstFactory and a neutral factory.
                     */

                    /*
                     * Player maybe have already tapped a second factory and
                     * selects another secondFactory. Reset previous secondFactory.
                     */
                    if (secondFactory != null)
                        secondFactory.Selected = false;

                    secondFactory = targeted;
                    if (secondFactory.ObjectType == 1)
                    {
                        secondFactory.Selected = true;
                        secondFactory.selectionSound = true;
                    }
                    else
                    {
                        secondFactory.Selected = false;
                        secondFactory = null;
                    }
                }

                return;
            }

            /*
            * If here, player targeted an enemy or neutral factory.
            * Or maybe player tapped on nothing (no collision). 
            * Reset.
            */
            if (firstFactory != null)
            {
                firstFactory.Selected = false;
                firstFactory = null;
            }

            if (secondFactory != null)
            {
                secondFactory.Selected = false;
                secondFactory = null;
            }

            if (thirdFactory != null)
            {
                thirdFactory.Selected = false;
                thirdFactory = null;
            }
        }

        public void UpdateInputWindows(RenderingState renderingState, GameTime time)
        {
            MouseState touchGesture = Mouse.GetState();

            if (touchGesture.LeftButton == ButtonState.Pressed
                && lastButtonState == ButtonState.Released)
            {
                playerClicked = true;
                lastButtonState = ButtonState.Pressed;

                if (firstClick == 0)
                    firstClick = time.ElapsedGameTime.TotalMilliseconds;
                else
                    secondClick = time.ElapsedGameTime.TotalMilliseconds;
            }

            if (playerClicked)
                myTimer += time.ElapsedGameTime.Milliseconds;

            if (myTimer > maxTimeForDoubleClick)
            {
                if (secondClick != 0 && secondClick - firstClick < maxTimeForDoubleClick)
                {
                    /*doubleclick*/
                    doubleClick(renderingState, time);
                }
                else
                {
                    /*singleclick*/
                    singleClick(renderingState, time);
                }

                playerClicked = false;
                firstClick = 0;
                secondClick = 0;
                myTimer = 0;
            }

            if (Mouse.GetState().LeftButton == ButtonState.Released)
            {
                lastButtonState = ButtonState.Released;
            }

            /*Move around*/
            if (Mouse.GetState().X > renderingState.spriteBatch.GraphicsDevice.Viewport.Width-50)
            {
                renderingState.TranslateX(movingSpeed);
            }

            if (Mouse.GetState().X < 50)
            {
                renderingState.TranslateX(-movingSpeed);
            }

            if (Mouse.GetState().Y > renderingState.spriteBatch.GraphicsDevice.Viewport.Height-50)
            {
                renderingState.TranslateZ(movingSpeed);
            }

            if (Mouse.GetState().Y < 50)
            {
                renderingState.TranslateZ(-movingSpeed);
            }

            /* Zoom */
            int prova = Mouse.GetState().ScrollWheelValue;
            if (Mouse.GetState().ScrollWheelValue > 100)
                renderingState.zoomIn();
            else
            {
                if(Mouse.GetState().ScrollWheelValue < 0)
                renderingState.zoomOut();
            }


        }

        public WinLoseState UpdateWinLose()
        {

            int type = -1;
            for (int i = 0; i < stateGame.factory.factories.Count; i++)
            {
                if (stateGame.factory.factories[i].ObjectType != 0 && type == -1)
                    type = stateGame.factory.factories[i].ObjectType;
                if (stateGame.factory.factories[i].ObjectType != type && stateGame.factory.factories[i].ObjectType != 0)
                {
                    return WinLoseState.None;
                }
            }

            if (type == 1)
                return WinLoseState.Win;
            else
                return WinLoseState.Lose;
        }
    }
}

