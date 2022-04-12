using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Util
{
    public static class UIUtil
    {
        public static int UILayer = LayerMask.NameToLayer("UI");

        public static bool IsPointerOverUIElement(GameObject target, Vector2 cursorPos)
        {
            return IsPointerOverUIElement(GetEventSystemRaycastResults(cursorPos), target);
        }

        public static bool IsPointerOverAnyUI(Vector2 cursorPos)
        {
            return IsPointerOverAnyUI(GetEventSystemRaycastResults(cursorPos));
        }


        //Returns 'true' if we touched or hovering on Unity UI element.
        private static bool IsPointerOverAnyUI(List<RaycastResult> eventSystemRaysastResults)
        {
            for (int index = 0; index < eventSystemRaysastResults.Count; index++)
            {
                RaycastResult curRaysastResult = eventSystemRaysastResults[index];
                if (curRaysastResult.gameObject.layer == UILayer)
                    return true;
            }

            return false;
        }

        private static bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults, GameObject target)
        {
            if (eventSystemRaysastResults.Count == 0) return false;
            
            RaycastResult curRaysastResult = eventSystemRaysastResults[0];
            
            return curRaysastResult.gameObject.layer == UILayer && curRaysastResult.gameObject == target;
        }


        //Gets all event system raycast results of current mouse or touch position.
        private static List<RaycastResult> GetEventSystemRaycastResults(Vector2 cursorPos)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current) {position = cursorPos};

            List<RaycastResult> raysastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, raysastResults);
            return raysastResults;
        }
    }
}