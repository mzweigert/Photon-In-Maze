
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace PhotonInMaze.GameCamera {

    internal enum GestureType {
        None,
        Pinch,
        Swipe
    }

    internal struct Gesture {
        [DefaultValue(GestureType.None)]
        internal GestureType Type { get; private set; }
        internal Vector2 Delta { get; private set; }
        internal float Magnitude { get; private set; }

        public Gesture(GestureType type, Vector2 delta, float magnitude) : this() {
            Type = type;
            Delta = delta;
            Magnitude = magnitude;
        }

    }

    internal struct Gestures : IEnumerable {
        private HashSet<Gesture> set;

        internal Gestures(Touch touchZero, Touch touchOne) {
            set = new HashSet<Gesture>();

            Vector2 deltaOne = touchOne.deltaPosition;
            Vector2 deltaZero = touchZero.deltaPosition;
            bool isAnyDeltaZero = IsNearZero(deltaZero) || IsNearZero(deltaOne);
            if(isAnyDeltaZero) {
                return;
            }

            bool isPinch =  ((Mathf.Abs(deltaZero.x) > 2.5f && Mathf.Abs(deltaOne.x) > 2.5f) && HasNotSameSign(deltaZero.x, deltaOne.x)) || 
                            ((Mathf.Abs(deltaZero.y) > 2.5f && Mathf.Abs(deltaOne.y) > 2.5f) && HasNotSameSign(deltaZero.y, deltaOne.y));
       
            if(isPinch) {
                Gesture gesture = CalculatePinchDelta(touchZero, touchOne);
                set.Add(gesture);
            }

            bool isSwipe = HasSameSign(deltaZero.x, deltaOne.x) || HasSameSign(deltaZero.y, deltaOne.y);
            if(isSwipe) {
                Gesture gesture = CalculateSwipeDelta(touchZero, touchOne);
                set.Add(gesture);
            }
        }

        private bool HasNotSameSign(float first, float second) {
            return Mathf.Sign(first) != Mathf.Sign(second);
        }

        private bool HasSameSign(float first, float second) {
            return Mathf.Sign(first) == Mathf.Sign(second);
        }

        private bool IsNearZero(Vector2 vector) {
            return Mathf.Round(vector.x) == 0 && Mathf.Round(vector.y) == 0;
        }

        private Gesture CalculatePinchDelta(Touch touchZero, Touch touchOne) {
  
            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            Vector2 prevTouchDelta = (touchZeroPrevPos - touchOnePrevPos);
            Vector2 touchDelta = (touchZero.position - touchOne.position);

            float magnitude = Mathf.Clamp(prevTouchDelta.magnitude - touchDelta.magnitude, -10f, 10f);
          
            // Find the difference in the distances between each frame.
            Vector2 delta = new Vector2(Mathf.Abs(prevTouchDelta.x) - Mathf.Abs(touchDelta.x),
                Mathf.Abs(prevTouchDelta.y) - Mathf.Abs(touchDelta.y));

            return new Gesture(GestureType.Pinch, delta, magnitude);
        }


        private Gesture CalculateSwipeDelta(Touch touchZero, Touch touchOne) {
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            Vector2 positionCenter = (touchZero.position + touchOne.position) / 2;
            Vector2 previousPositionCenter = (touchZeroPrevPos + touchOnePrevPos) / 2;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            Vector2 prevTouchDelta = (touchZeroPrevPos - touchOnePrevPos);
            Vector2 touchDelta = (touchZero.position - touchOne.position);

            float magnitude = prevTouchDelta.magnitude - touchDelta.magnitude;

            Vector2 delta = new Vector2(positionCenter.x - previousPositionCenter.x, positionCenter.y - previousPositionCenter.y);

            return new Gesture(GestureType.Swipe, delta, magnitude);
        }

        internal void Reset() {
            set = null;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return set.GetEnumerator();
        }
    }

}