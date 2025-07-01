/********************************************************************************************
    BlowFire Framework
    Module: Core/Input
    Author: HU QIWEI
    TouchManager is based on Unity LeanTouch code
********************************************************************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Threading.Tasks;

namespace BFF
{
    /// <summary>
    /// global touch manager
    /// </summary>
    public static class TouchManager
    {
        /// <summary>
        /// all active fingers list. Fingers.Count means how many fingers are currently down
        /// </summary>
        public static List<TouchFinger> Fingers = new List<TouchFinger>(10);

        /// <summary>
        /// Inactive fingers list
        /// </summary>
        public static List<TouchFinger> InactiveFingers = new List<TouchFinger>(10);
        private static List<RaycastResult> tempRaycastResults = new List<RaycastResult>(10);

        /// <summary>
        /// touch down event
        /// </summary>
        public static Action<TouchFinger> OnTouchDown;

        /// <summary>
        /// touch up event
        /// </summary>
        public static Action<TouchFinger> OnTouchUp;

        /// <summary>
        /// touch move event
        /// </summary>
        public static Action<TouchFinger> OnTouchMove;

        /// <summary>
        /// touch tap event: touch down, no move, then touch up
        /// </summary>
        public static Action<TouchFinger> OnTouchTap;

        /// <summary>
        /// touch swipe event: touch down then move
        /// </summary>
        public static Action<TouchFinger> OnTouchSwipe;

        /// <summary>
        /// Idle event: no touch events after idleTimeout seconds
        /// </summary>
        public static Action OnIdle;

        static int lockCount = 0;

        /// <summary>
        /// touch down and hold less than TapThreshold (seconds) means a tap
        /// </summary>
        public static float TapThreshold = 0.5f;

        /// <summary>
        /// touch down and move more than SwipeThreshold (pixels) means a swipe
        /// this actual value is affected by ScalingFactor
        /// </summary>
        public static float SwipeThreshold = 50.0f;

        /// <summary>
        /// Reference screen DPI. This will affect ScalingFactor
        /// </summary>
        public static int ReferenceDpi = 200;

        private static float idleTimeout = 5;
        /// <summary>
        /// Trigger OnIdle event no touch events after idleTimeout seconds
        /// </summary>
        public static float IdleTimeout {
            get { return idleTimeout; }
            set { idleTimeout = value; if (idleTimeout < 1) idleTimeout = 1; }
        }

        /// <summary>
        /// layerMask for object touch test
        /// </summary>
        public static LayerMask GUILayers;

        private static int maxMouseButton = 7;
        private static EventSystem tempEventSystem;
        private static PointerEventData tempPointerEventData;
        private static float lastInputTime = 0;

        static TouchManager()
        {
            // start Task
//            var _ = InputManagerLoop();
        }

        static bool updateIdleTimer;
        public static void InputManagerLoop()
        {
//            while (true)
//            {
                if (lockCount <= 0)
                {
                    updateIdleTimer = true;
                    BeginFingers();
                    ProcessFingers();
                    EndFingers();
                    UpdateEvents();

                    if (Fingers.Count != 0 && updateIdleTimer)
                        lastInputTime = Time.time;
                    else
                    {
                        if (Time.time - lastInputTime > IdleTimeout)
                        {
                            lastInputTime = Time.time;
                            if (OnIdle != null)
                                OnIdle();
                        }
                    }
                }

//                await Task.Delay(100);
//            }
        }

        /// <summary>
        /// Called in event handlers, don't update idle timer if the touch target is not to be concerned
        /// </summary>
        public static void DontUpdateIdleTimer()
        {
            updateIdleTimer = false;
        }

        // Update all Fingers and InactiveFingers so they're ready for the new frame
        static private void BeginFingers()
        {
            // Age inactive fingers
            for (var i = InactiveFingers.Count - 1; i >= 0; i--)
            {
                InactiveFingers[i].duration += Time.unscaledDeltaTime;
            }

            // Reset finger data
            for (var i = Fingers.Count - 1; i >= 0; i--)
            {
                var finger = Fingers[i];

                // Was this set to up last time? If so, it's now inactive
                if (finger.Up == true)
                {
                    // Make finger inactive
                    Fingers.RemoveAt(i); InactiveFingers.Add(finger);

                    // Reset age so we can time how long it's been inactive
                    finger.duration = 0;
                }
                else
                {
                    finger.lastDown = finger.down;
                    finger.lastScreenPosition = finger.screenPosition;

                    finger.down = false;
                }
            }
        }

        // Update all Fingers based on the new finger data
        static private void EndFingers()
        {
            for (var i = Fingers.Count - 1; i >= 0; i--)
            {
                var finger = Fingers[i];

                // Up?
                if (finger.Up == true)
                {
                    // Tap?
                    if (finger.duration <= TapThreshold)
                    {
                        if (finger.startDelta.magnitude * ScalingFactor < SwipeThreshold)
                        {
                            finger.tap = true;
                            finger.tapCount += 1;
                        }
                        else
                        {
                            finger.tapCount = 0;
                            finger.swipe = true;
                        }
                    }
                    else
                        finger.tapCount = 0;
                }
                else if (finger.Down == false)
                    finger.duration += Time.unscaledDeltaTime;
            }
        }

        // Read new hardware finger data
        static private void ProcessFingers()
        {
            // Update real fingers
            if (Input.touchCount > 0)
            {
                for (var i = 0; i < Input.touchCount; i++)
                {
                    var touch = Input.GetTouch(i);
                    UpdateFinger(touch.fingerId, touch.position);
                }
            }
            // If there are no real touches, simulate some from the mouse?
            else if (AnyMouseButtonSet == true)
            {
                var screen = new Rect(0, 0, Screen.width, Screen.height);
                var mousePosition = (Vector2)Input.mousePosition;

                // Is the mouse within the screen?
                if (screen.Contains(mousePosition) == true)
                    UpdateFinger(0, mousePosition);
            }
        }

        static private void UpdateEvents()
        {
            if (lockCount > 0)
                return;

            var fingerCount = Fingers.Count;

            if (fingerCount > 0)
            {
                for (var i = 0; i < fingerCount; i++)
                {
                    var finger = Fingers[i];

                    if (finger.Down == true) {
                        finger.touchable = null;
                        if (Touchable.touchableList.Count > 0) {
                            List<RaycastResult> ret = finger.HitUI();
                            if (ret != null && ret.Count > 0) {
                                finger.touchable = ret[0].gameObject.GetComponent<Touchable>();
                            }
                        }

                        OnTouchDown?.Invoke(finger);
                        // finger.touchable?.SendMessage("EventFingerDown", finger);
                    }

                    if (finger.down == true && finger.lastScreenPosition != finger.screenPosition) {
                        OnTouchMove?.Invoke(finger);
                        // finger.touchable?.SendMessage("EventFingerMove", finger);
                    }

                    if (finger.Up == true) {
                        OnTouchUp?.Invoke(finger);
                        // finger.touchable?.SendMessage("EventFingerUp", finger);
                    }

                    if (finger.tap == true) {
                        OnTouchTap?.Invoke(finger);
                        // finger.touchable?.SendMessage("EventFingerTap", finger);
                    }

                    if (finger.swipe == true) {
                        OnTouchSwipe?.Invoke(finger);
                        // finger.touchable?.SendMessage("EventFingerSwipe", finger);
                    }
                }
            }
        }

        // Add a finger based on index, or return the existing one
        private static void UpdateFinger(int index, Vector2 screenPosition)
        {
            var finger = FindFinger(index);

            // No finger found?
            if (finger == null)
            {
                var inactiveIndex = FindInactiveFingerIndex(index);

                // Use inactive finger?
                if (inactiveIndex >= 0)
                {
                    finger = InactiveFingers[inactiveIndex]; InactiveFingers.RemoveAt(inactiveIndex);

                    // Inactive for too long?
                    if (finger.duration > TapThreshold)
                    {
                        finger.tapCount = 0;
                    }

                    // Reset values
                    finger.duration = 0.0f;
                    finger.down = false;
                    finger.lastDown = false;
                    finger.tap = false;
                    finger.swipe = false;
                }
                // Create new finger?
                else
                {
                    finger = new TouchFinger();

                    finger.index = index;
                }

                finger.startScreenPosition = screenPosition;
                finger.lastScreenPosition = screenPosition;
                finger.screenPosition = screenPosition;
                finger.startFromUI = PointOverUI(screenPosition); ;

                Fingers.Add(finger);
            }

            finger.down = true;
            finger.screenPosition = screenPosition;
        }

        /// <summary>
        /// check if screen point is over given UI object
        /// </summary>
        /// <param name="screenPosition">screen coordinate</param>
        /// <param name="gObject">UI object</param>
        /// <param name="first">only check if gObject is the first hit</param>
        /// <returns>true if point is on UI control</returns>
        public static bool PointOverUIControl(Vector2 screenPosition, GameObject gObject, bool first = true)
        {
            var list = HitUI(screenPosition);
            if (list.Count > 0)
            {
                if (first)
                    return list[0].gameObject == gObject;
                else
                {
                    for (int i = 0; i < list.Count; i++)
                        if (list[i].gameObject == gObject)
                            return true;
                }
            }
            return false;
        }

        /// <summary>
        /// check if screen point is over any UI control
        /// </summary>
        /// <param name="screenPosition">screen coordinate</param>
        /// <returns>true if point is over any UI object</returns>
        public static bool PointOverUI(Vector2 screenPosition)
        {
            return HitUI(screenPosition).Count > 0;
        }

        /// <summary>
        /// Get a list of all UI object under given point, using default GUILayers
        /// </summary>
        /// <param name="screenPosition">screen coordinate</param>
        /// <returns>List of hit UI objects</returns>
        public static List<RaycastResult> HitUI(Vector2 screenPosition)
        {
            return HitUI(screenPosition, GUILayers);
        }

        /// <summary>
        /// list all UI controls under given screen point
        /// </summary>
        /// <param name="screenPosition">screen coordinate</param>
        /// <param name="layerMask">layer mask to check</param>
        /// <returns>List of hit UI objects</returns>
        public static List<RaycastResult> HitUI(Vector2 screenPosition, LayerMask layerMask)
        {
            tempRaycastResults.Clear();

            var currentEventSystem = EventSystem.current;
            if (layerMask.value == 0)
                layerMask.value = 0x7fffffff;

            if (currentEventSystem != null)
            {
                // Create point event data for this event system?
                if (currentEventSystem != tempEventSystem)
                {
                    tempEventSystem = currentEventSystem;

                    if (tempPointerEventData == null)
                        tempPointerEventData = new PointerEventData(tempEventSystem);
                    else
                        tempPointerEventData.Reset();
                }

                // Raycast event system at the specified point
                tempPointerEventData.position = screenPosition;

                currentEventSystem.RaycastAll(tempPointerEventData, tempRaycastResults);

                // Loop through all results and remove any that don't match the layer mask
                if (tempRaycastResults.Count > 0)
                {
                    for (var i = tempRaycastResults.Count - 1; i >= 0; i--)
                    {
                        var raycastResult = tempRaycastResults[i];
                        var raycastLayer = 1 << raycastResult.gameObject.layer;

                        if ((raycastLayer & layerMask) == 0)
                            tempRaycastResults.RemoveAt(i);
                    }
                }
            }

            return tempRaycastResults;
        }

        // Find the finger with the specified index, or return null
        private static TouchFinger FindFinger(int index)
        {
            for (var i = Fingers.Count - 1; i >= 0; i--)
            {
                var finger = Fingers[i];

                if (finger.index == index)
                    return finger;
            }

            return null;
        }

        // Find the index of the inactive finger with the specified index, or return -1
        private static int FindInactiveFingerIndex(int index)
        {
            for (var i = InactiveFingers.Count - 1; i >= 0; i--)
            {
                if (InactiveFingers[i].index == index)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// screen scale factor
        /// </summary>
        public static float ScalingFactor
        {
            get
            {
                var dpi = Screen.dpi;

                if (dpi <= 0)
                    return 1.0f;

                return Mathf.Sqrt(ReferenceDpi) / Mathf.Sqrt(dpi);
            }
        }

        /// <summary>
        /// Returns true if any mouse button is pressed
        /// </summary>
        public static bool AnyMouseButtonSet
        {
            get
            {
                for (var i = 0; i < maxMouseButton; i++)
                {
                    if (Input.GetMouseButton(i) == true)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Enable touch manager (use internal counter)
        /// </summary>
        public static void Enable()
        {
            lockCount--;
            if (lockCount <= 0)
                lastInputTime = Time.time;
        }

        /// <summary>
        /// disable touch manager (use internal counter)
        /// </summary>
        public static void Disable()
        {
            lockCount++;
        }

        /// <summary>
        /// reset idle timer, restart idle time counting
        /// </summary>
        public static void ResetIdleTimer()
        {
            lastInputTime = Time.time;
        }
    }
}
