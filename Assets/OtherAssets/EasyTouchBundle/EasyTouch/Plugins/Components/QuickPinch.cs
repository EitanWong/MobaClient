﻿/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/

	  The.Hedgehog.Team@gmail.com

**********************************************/

using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
    [AddComponentMenu("EasyTouch/Quick Pinch")]
    public class QuickPinch : QuickBase
    {
        #region Events

        [System.Serializable] public class OnPinchAction : UnityEvent<Gesture> { }

        [SerializeField]
        public OnPinchAction onPinchAction;

        #endregion Events

        #region enumeration

        public enum ActionTiggering { InProgress, End };

        public enum ActionPinchDirection { All, PinchIn, PinchOut };

        #endregion enumeration

        #region Members

        public bool isGestureOnMe = false;
        public ActionTiggering actionTriggering;
        public ActionPinchDirection pinchDirection;
        private float axisActionValue = 0;
        public bool enableSimpleAction = false;

        #endregion Members

        #region MonoBehaviour callback

        public QuickPinch()
        {
            quickActionName = "QuickPinch" + System.Guid.NewGuid().ToString().Substring(0, 7);
        }

        public override void OnEnable()
        {
            EasyTouch.On_Pinch += On_Pinch;
            EasyTouch.On_PinchIn += On_PinchIn;
            EasyTouch.On_PinchOut += On_PinchOut;
            EasyTouch.On_PinchEnd += On_PichEnd;
        }

        public override void OnDisable()
        {
            UnsubscribeEvent();
        }

        private void OnDestroy()
        {
            UnsubscribeEvent();
        }

        private void UnsubscribeEvent()
        {
            EasyTouch.On_Pinch -= On_Pinch;
            EasyTouch.On_PinchIn -= On_PinchIn;
            EasyTouch.On_PinchOut -= On_PinchOut;
            EasyTouch.On_PinchEnd -= On_PichEnd;
        }

        #endregion MonoBehaviour callback

        #region EasyTouch event

        private void On_Pinch(Gesture gesture)
        {
            if (actionTriggering == ActionTiggering.InProgress && pinchDirection == ActionPinchDirection.All)
            {
                DoAction(gesture);
            }
        }

        private void On_PinchIn(Gesture gesture)
        {
            if (actionTriggering == ActionTiggering.InProgress & pinchDirection == ActionPinchDirection.PinchIn)
            {
                DoAction(gesture);
            }
        }

        private void On_PinchOut(Gesture gesture)
        {
            if (actionTriggering == ActionTiggering.InProgress & pinchDirection == ActionPinchDirection.PinchOut)
            {
                DoAction(gesture);
            }
        }

        private void On_PichEnd(Gesture gesture)
        {
            if (actionTriggering == ActionTiggering.End)
            {
                DoAction(gesture);
            }
        }

        #endregion EasyTouch event

        #region Private method

        private void DoAction(Gesture gesture)
        {
            axisActionValue = gesture.deltaPinch * sensibility * Time.deltaTime;

            if (isGestureOnMe)
            {
                if (realType == GameObjectType.UI)
                {
                    if (gesture.isOverGui)
                    {
                        if ((gesture.pickedUIElement == gameObject || gesture.pickedUIElement.transform.IsChildOf(transform)))
                        {
                            onPinchAction.Invoke(gesture);
                            if (enableSimpleAction)
                            {
                                DoDirectAction(axisActionValue);
                            }
                        }
                    }
                }
                else
                {
                    if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI)
                    {
                        if (gesture.GetCurrentPickedObject(true) == gameObject)
                        {
                            onPinchAction.Invoke(gesture);
                            if (enableSimpleAction)
                            {
                                DoDirectAction(axisActionValue);
                            }
                        }
                    }
                }
            }
            else
            {
                if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI)
                {
                    onPinchAction.Invoke(gesture);
                    if (enableSimpleAction)
                    {
                        DoDirectAction(axisActionValue);
                    }
                }
            }
        }

        #endregion Private method
    }
}