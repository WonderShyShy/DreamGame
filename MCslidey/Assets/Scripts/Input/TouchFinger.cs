/********************************************************************************************
    BlowFire Framework
    Module: Core/Input
    Author: HU QIWEI
    TouchFinger is based on Unity LeanTouch code
********************************************************************************************/
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

namespace BFF
{
    /// <summary>
    /// data object to manage a single finger 
    /// </summary>
    public class TouchFinger
	{
        internal int index;
        /// <summary>
        /// index in TouchManager.Finger
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// screen coordinate when touch down
        /// </summary>
        public Vector2 startScreenPosition;

        /// <summary>
        /// current touch screen coordinate
        /// </summary>
        public Vector2 screenPosition;

        /// <summary>
        /// touch screen coordinate on last frame
        /// </summary>
        public Vector2 lastScreenPosition;

        /// <summary>
        /// touch duration (seconds)
        /// </summary>
        public float duration;

        /// <summary>
        /// true if finger is down
        /// </summary>
        public bool down;

        /// <summary>
        /// true if finger is down on last frame
        /// </summary>
        public bool lastDown;

        /// <summary>
        /// true if finger is tapping
        /// </summary>
        public bool tap;

        /// <summary>
        /// finger tap count
        /// </summary>
        public int tapCount;

        /// <summary>
        /// true if finger is swiping
        /// </summary>
        public bool swipe;

        /// <summary>
        /// true if touch down starts from an UI object
        /// </summary>
        public bool startFromUI;

        /// <summary>
        /// touchable object if touch is down on an Touchable object. null if not
        /// </summary>
        public Touchable touchable;

        /// <summary>
        /// true if finger is up
        /// </summary>
        public bool Up
        {            
            get { return down == false && lastDown == true; }
        }

        /// <summary>
        /// true if finger is down
        /// </summary>
        public bool Down
        {
            get { return down == true && lastDown == false; }
        }

        /// <summary>
        /// offset from down position to current position
        /// </summary>
        public Vector2 startDelta
        {
            get
            {
                return screenPosition - startScreenPosition;
            }
        }

        /// <summary>
        /// offset from last screen position to current position
        /// </summary>
        public Vector2 screenDelta
        {
            get
            {
                return screenPosition - lastScreenPosition;
            }
        }

        /// <summary>
        /// scaled touch position (screenPosition*ScalingFactor)
        /// </summary>
        public Vector2 scaledPosition
        {
            get
            {
                return screenPosition * TouchManager.ScalingFactor;
            }
        }

        /// <summary>
        /// scaled touch delta (screenDelta*ScalingFactor)
        /// </summary>
        public Vector2 scaledDelta
        {
            get
            {
                return screenDelta * TouchManager.ScalingFactor;
            }
        }

        /// <summary>
        /// get the ray of the finger's current position
        /// </summary>
        /// <param name="camera">optional camera</param>
        /// <returns>Ray</returns>
        public Ray GetRay(Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;
            if (camera != null)
                return camera.ScreenPointToRay(screenPosition);

            return default(Ray);
        }

        /// <summary>
        /// get the ray of the finger's start position
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public Ray GetStartRay(Camera camera = null)
        {
            if (camera == null)
                camera = Camera.main;
            if (camera != null)
                return camera.ScreenPointToRay(startScreenPosition);

            return default(Ray);
        }

        /// <summary>
        /// returns true if finger is over given UI object
        /// </summary>
        /// <param name="gObject">UI object</param>
        /// <returns></returns>
        public bool IsOverUIControl(GameObject gObject)
        {
            return TouchManager.PointOverUIControl(screenPosition, gObject);
        }

        /// <summary>
        /// returns true if finger is over any UI object
        /// </summary>
        /// <returns></returns>
        public bool IsOverUI()
        {
            return TouchManager.PointOverUI(screenPosition);
        }

        /// <summary>
        /// get a list of UI object under current screen position
        /// </summary>
        /// <returns></returns>
        public List<RaycastResult> HitUI()
        {
            return TouchManager.HitUI(screenPosition);
        }

        /// <summary>
        /// local coordinate of touch position
        /// </summary>
        public Vector2 touchPosition
        {
            get {
                if (touchable == null)
                    return Vector2.zero;
                Vector2 pos;
                touchable.ToLocal(screenPosition, out pos);
                return pos;
            }
        }

        /// <summary>
        /// local coordinate of touch position on last frame
        /// </summary>
        public Vector2 lastTouchPosition
        {
            get {
                if (touchable == null)
                    return Vector2.zero;
                Vector2 pos;
                touchable.ToLocal(lastScreenPosition, out pos);
                return pos;
            }
        }

        /// <summary>
        /// local coordinate of touch start position
        /// </summary>
        public Vector2 startTouchPosition
        {
            get {
                if (touchable == null)
                    return Vector2.zero;
                Vector2 pos;
                touchable.ToLocal(startScreenPosition, out pos);
                return pos;
            }
        }

        /// <summary>
        /// local offset of screen delta (last postion to current position)
        /// </summary>
        public Vector2 touchDelta
        {
            get {
                if (touchable == null)
                    return Vector2.zero;
                Vector2 pos0;
                touchable.ToLocal(lastScreenPosition, out pos0);
                Vector2 pos1;
                touchable.ToLocal(screenPosition, out pos1);
                return pos1 - pos0;
            }
        }

        /// <summary>
        /// local offset of start delta (start position to current position)
        /// </summary>
        public Vector2 startTouchDelta
        {
            get
            {
                if (touchable == null)
                    return Vector2.zero;
                Vector2 pos0;
                touchable.ToLocal(startScreenPosition, out pos0);
                Vector2 pos1;
                touchable.ToLocal(screenPosition, out pos1);
                return pos1 - pos0;
            }
        }


        /// <summary>
        /// local drag offset of screen delta (last postion to current position)
        /// </summary>
        public Vector2 dragDelta
        {
            get
            {
                if (touchable == null)
                    return Vector2.zero;
                return touchDelta * touchable.transform.localScale.x;
            }
        }

        /// <summary>
        /// local drag offset of start delta (start position to current position)
        /// </summary>
        public Vector2 startDragDelta
        {
            get
            {
                if (touchable == null)
                    return Vector2.zero;
                return startTouchDelta * touchable.transform.localScale.x;
            }
        }
    }
}
