/***********************************************
				EasyTouch V
	Copyright © 2014-2015 The Hedgehog Team
    http://www.thehedgehogteam.com/Forum/

	  The.Hedgehog.Team@gmail.com

**********************************************/

using UnityEngine;
using UnityEngine.Events;

namespace HedgehogTeam.EasyTouch
{
    [AddComponentMenu("EasyTouch/Quick Drag")]
    public class QuickDrag : QuickBase
    {
        #region Events

        [System.Serializable] public class OnDragStart : UnityEvent<Gesture> { }

        [System.Serializable] public class OnDrag : UnityEvent<Gesture> { }

        [System.Serializable] public class OnDragEnd : UnityEvent<Gesture> { }

        [SerializeField]
        public OnDragStart onDragStart;

        [SerializeField]
        public OnDrag onDrag;

        [SerializeField]
        public OnDragEnd onDragEnd;

        #endregion Events

        #region Members

        public bool isStopOncollisionEnter = false;

        private Vector3 deltaPosition;
        private bool isOnDrag = false;
        private Gesture lastGesture;

        #endregion Members

        #region Monobehaviour CallBack

        public QuickDrag()
        {
            quickActionName = "QuickDrag" + System.Guid.NewGuid().ToString().Substring(0, 7);
            axesAction = AffectedAxesAction.XY;
        }

        public override void OnEnable()
        {
            base.OnEnable();
            EasyTouch.On_TouchStart += On_TouchStart;
            EasyTouch.On_TouchDown += On_TouchDown;
            EasyTouch.On_TouchUp += On_TouchUp;
            EasyTouch.On_Drag += On_Drag;
            EasyTouch.On_DragStart += On_DragStart;
            EasyTouch.On_DragEnd += On_DragEnd;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            UnsubscribeEvent();
        }

        private void OnDestroy()
        {
            UnsubscribeEvent();
        }

        private void UnsubscribeEvent()
        {
            EasyTouch.On_TouchStart -= On_TouchStart;
            EasyTouch.On_TouchDown -= On_TouchDown;
            EasyTouch.On_TouchUp -= On_TouchUp;
            EasyTouch.On_Drag -= On_Drag;
            EasyTouch.On_DragStart -= On_DragStart;
            EasyTouch.On_DragEnd -= On_DragEnd;
        }

        private void OnCollisionEnter()
        {
            if (isStopOncollisionEnter && isOnDrag)
            {
                StopDrag();
            }
        }

        #endregion Monobehaviour CallBack

        #region EasyTouch Event

        private void On_TouchStart(Gesture gesture)
        {
            if (realType == GameObjectType.UI)
            {
                if (gesture.isOverGui)
                {
                    if ((gesture.pickedUIElement == gameObject || gesture.pickedUIElement.transform.IsChildOf(transform)) && fingerIndex == -1)
                    {
                        fingerIndex = gesture.fingerIndex;
                        transform.SetAsLastSibling();
                        onDragStart.Invoke(gesture);

                        isOnDrag = true;
                    }
                }
            }
        }

        private void On_TouchDown(Gesture gesture)
        {
            if (isOnDrag && fingerIndex == gesture.fingerIndex && realType == GameObjectType.UI)
            {
                if (gesture.isOverGui)
                {
                    if ((gesture.pickedUIElement == gameObject || gesture.pickedUIElement.transform.IsChildOf(transform)))
                    {
                        transform.position += (Vector3)gesture.deltaPosition;

                        if (gesture.deltaPosition != Vector2.zero)
                        {
                            onDrag.Invoke(gesture);
                        }
                        lastGesture = gesture;
                    }
                }
            }
        }

        private void On_TouchUp(Gesture gesture)
        {
            if (fingerIndex == gesture.fingerIndex && realType == GameObjectType.UI)
            {
                lastGesture = gesture;
                StopDrag();
            }
        }

        // At the drag beginning
        private void On_DragStart(Gesture gesture)
        {
            if (realType != GameObjectType.UI)
            {
                if ((!enablePickOverUI && gesture.pickedUIElement == null) || enablePickOverUI)
                {
                    if (gesture.pickedObject == gameObject && !isOnDrag)
                    {
                        isOnDrag = true;

                        fingerIndex = gesture.fingerIndex;

                        // the world coordinate from touch
                        Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position);
                        deltaPosition = position - transform.position;

                        //
                        if (resetPhysic)
                        {
                            if (cachedRigidBody)
                            {
                                cachedRigidBody.isKinematic = true;
                            }

                            if (cachedRigidBody2D)
                            {
                                cachedRigidBody2D.isKinematic = true;
                            }
                        }

                        onDragStart.Invoke(gesture);
                    }
                }
            }
        }

        // During the drag
        private void On_Drag(Gesture gesture)
        {
            if (fingerIndex == gesture.fingerIndex)
            {
                if (realType == GameObjectType.Obj_2D || realType == GameObjectType.Obj_3D)
                {
                    // Verification that the action on the object
                    if (gesture.pickedObject == gameObject && fingerIndex == gesture.fingerIndex)
                    {
                        // the world coordinate from touch
                        Vector3 position = gesture.GetTouchToWorldPoint(gesture.pickedObject.transform.position) - deltaPosition;
                        transform.position = GetPositionAxes(position);

                        if (gesture.deltaPosition != Vector2.zero)
                        {
                            onDrag.Invoke(gesture);
                        }
                        lastGesture = gesture;
                    }
                }
            }
        }

        // End of drag
        private void On_DragEnd(Gesture gesture)
        {
            if (fingerIndex == gesture.fingerIndex)
            {
                lastGesture = gesture;
                StopDrag();
            }
        }

        #endregion EasyTouch Event

        #region Private Method

        private Vector3 GetPositionAxes(Vector3 position)
        {
            Vector3 axes = position;

            switch (axesAction)
            {
                case AffectedAxesAction.X:
                    axes = new Vector3(position.x, transform.position.y, transform.position.z);
                    break;

                case AffectedAxesAction.Y:
                    axes = new Vector3(transform.position.x, position.y, transform.position.z);
                    break;

                case AffectedAxesAction.Z:
                    axes = new Vector3(transform.position.x, transform.position.y, position.z);
                    break;

                case AffectedAxesAction.XY:
                    axes = new Vector3(position.x, position.y, transform.position.z);
                    break;

                case AffectedAxesAction.XZ:
                    axes = new Vector3(position.x, transform.position.y, position.z);
                    break;

                case AffectedAxesAction.YZ:
                    axes = new Vector3(transform.position.x, position.y, position.z);
                    break;
            }

            return axes;
        }

        #endregion Private Method

        #region Public Method

        public void StopDrag()
        {
            fingerIndex = -1;

            if (resetPhysic)
            {
                if (cachedRigidBody)
                {
                    cachedRigidBody.isKinematic = isKinematic;
                }

                if (cachedRigidBody2D)
                {
                    cachedRigidBody2D.isKinematic = isKinematic2D;
                }
            }
            isOnDrag = false;

            onDragEnd.Invoke(lastGesture);
        }

        #endregion Public Method
    }
}