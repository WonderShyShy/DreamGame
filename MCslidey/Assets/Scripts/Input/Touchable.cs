/********************************************************************************************
    BlowFire Framework
    Module: Core/Input
    Author: HU QIWEI
********************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using BFF;

namespace BFF
{
    /// <summary>
    /// touchable component, can be attached to GameObject you want players to touch
    /// </summary>
    public class Touchable : MonoBehaviour
    {
        /// <summary>
        /// user-defined integer ID
        /// </summary>
        public int intID;

        /// <summary>
        /// user defined string ID
        /// </summary>
        public string stringID;

        /// <summary>
        /// user defined object ID
        /// </summary>
        [HideInInspector]
        public object objectID;


        /// <summary>
        /// touch down event
        /// </summary>
        [HideInInspector]
        public Action<Touchable> OnTouchDown;

        /// <summary>
        /// touch move event
        /// </summary>
        [HideInInspector]
        public Action<Touchable> OnTouchMove;

        /// <summary>
        /// touch up event
        /// </summary>
        [HideInInspector]
        public Action<Touchable> OnTouchUp;

        /// <summary>
        /// tap event
        /// </summary>
        [HideInInspector]
        public Action<Touchable> OnTouchTap;

        /// <summary>
        /// swipe event
        /// </summary>
        [HideInInspector]
        public Action<Touchable> OnTouchSwipe;

        internal static List<Touchable> touchableList = new List<Touchable>();

        static Camera _globalCamera = null;
        static bool _globalCameraSet = false;
        public static void SetGlobalCamera(Camera cam)
        {
            _globalCamera = cam;
            _globalCameraSet = true;
        }

        Camera _canvasCamera = null;
        bool _canvasCameraSet = false;
        public Camera canvasCamera
        {
            get { return _canvasCamera; }
            set
            {
                _canvasCamera = value;
                _canvasCameraSet = true;
            }
        }

        void CheckCamera()
        {
            if (!_canvasCameraSet)
            {
                if (_globalCameraSet)
                {
                    _canvasCamera = _globalCamera;
                }
                else
                {
                    Canvas canvas = GetComponentInParent<Canvas>();
                    if (canvas != null &&
                        (canvas.renderMode == RenderMode.ScreenSpaceCamera || canvas.renderMode == RenderMode.WorldSpace))
                        _canvasCamera = canvas.worldCamera;
                    else
                        _canvasCamera = null;
                }
                _canvasCameraSet = true;
            }
        }

        void OnEnable()
        {
            touchableList.Add(this);
        }

        void OnDisable()
        {
            touchableList.Remove(this);
        }

        void EventFingerDown(TouchFinger finger)
        {
            OnTouchDown?.Invoke(this);
        }

        void EventFingerMove(TouchFinger finger)
        {
            OnTouchMove?.Invoke(this);
        }

        void EventFingerUp(TouchFinger finger)
        {
            OnTouchUp?.Invoke(this);
        }

        void EventFingerTap(TouchFinger finger)
        {
            OnTouchTap?.Invoke(this);
        }

        void EventFingerSwipe(TouchFinger finger)
        {
            OnTouchSwipe?.Invoke(this);
        }

        /// <summary>
        /// convert screen position to local position
        /// </summary>
        /// <param name="screenPos">screen coordinate</param>
        /// <param name="localPos">local corrdinate</param>
        /// <returns>true if succeed</returns>
        public bool ToLocal(Vector2 screenPos, out Vector2 localPos)
        {
            RectTransform rt = transform as RectTransform;
            if (rt == null)
            {
                localPos = Vector2.negativeInfinity;
                return false;
            }
            CheckCamera();
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, screenPos, _canvasCamera, out localPos);
        }

        /// <summary>
        /// get Touchable component on GameObject. create one if not found
        /// </summary>
        /// <param name="gObject">GameObject</param>
        /// <returns></returns>
        public static Touchable GetTouchable(GameObject gObject)
        {
            Touchable ret;
            if ((ret = gObject.GetComponent<Touchable>()) == null)
                ret = gObject.AddComponent<Touchable>();
            return ret;
        }
    }
}
