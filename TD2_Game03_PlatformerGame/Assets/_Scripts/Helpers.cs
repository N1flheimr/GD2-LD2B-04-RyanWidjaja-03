using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NifuDev
{
    public static class Helpers
    {
        private static Camera _camera;

        public static Camera Camera
        {
            get
            {
                if (_camera == null) _camera = Camera.main;
                return _camera;
            }
        }

        private static readonly Dictionary<float, WaitForSeconds> WaitDictionary = new Dictionary<float, WaitForSeconds>();

        public static WaitForSeconds GetWaitForSeconds(float time)
        {
            if (WaitDictionary.TryGetValue(time, out WaitForSeconds wait))
            {
                return wait;
            }
            WaitDictionary[time] = new WaitForSeconds(time);
            return WaitDictionary[time];
        }

        private static readonly Dictionary<float, WaitForSecondsRealtime> WaitForRealTimeDictionary = new Dictionary<float, WaitForSecondsRealtime>();

        public static WaitForSecondsRealtime GetWaitForSecondsRealTime(float time)
        {
            if (WaitForRealTimeDictionary.TryGetValue(time, out WaitForSecondsRealtime wait))
            {
                return wait;
            }
            WaitForRealTimeDictionary[time] = new WaitForSecondsRealtime(time);
            return WaitForRealTimeDictionary[time];
        }

        private static PointerEventData _eventDataCurrentPosition;
        private static List<RaycastResult> _results;

        public static bool IsOverUI()
        {
            _eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
            _results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(_eventDataCurrentPosition, _results);
            return _results.Count > 0;
        }
    }
}