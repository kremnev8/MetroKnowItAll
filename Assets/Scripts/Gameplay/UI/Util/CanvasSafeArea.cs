using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.UI
{
    /// <summary>
    /// Helper UI class that ensures that all UI elements are within device safe area
    /// </summary>
    [RequireComponent(typeof(Canvas))]
    public class CanvasSafeArea : MonoBehaviour
    {
        private static List<CanvasSafeArea> helpers = new List<CanvasSafeArea>();

        public static UnityEvent OnResolutionOrOrientationChanged = new UnityEvent();

        private static bool screenChangeVarsInitialized = false;
        private static ScreenOrientation lastOrientation = ScreenOrientation.Landscape;
        private static Vector2 lastResolution = Vector2.zero;
        private static Rect lastSafeArea = Rect.zero;

        private Canvas canvas;
        private RectTransform safeAreaTransform;

        void Awake()
        {
            if (!helpers.Contains(this))
                helpers.Add(this);

            canvas = GetComponent<Canvas>();

            safeAreaTransform = transform.Find("SafeArea") as RectTransform;

            if (!screenChangeVarsInitialized)
            {
                lastOrientation = Screen.orientation;
                lastResolution.x = Screen.width;
                lastResolution.y = Screen.height;
                lastSafeArea = Screen.safeArea;

                screenChangeVarsInitialized = true;
            }

            ApplySafeArea();
        }

        void Update()
        {
            if (helpers[0] != this)
                return;

            if (Application.isMobilePlatform && Screen.orientation != lastOrientation)
                OrientationChanged();

            if (Screen.safeArea != lastSafeArea)
                SafeAreaChanged();

            if (Screen.width != lastResolution.x || Screen.height != lastResolution.y)
                ResolutionChanged();
        }

        void ApplySafeArea()
        {
            if (safeAreaTransform == null)
                return;

            var safeArea = Screen.safeArea;

            var anchorMin = safeArea.position;
            var anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= canvas.pixelRect.width;
            anchorMin.y /= canvas.pixelRect.height;
            anchorMax.x /= canvas.pixelRect.width;
            anchorMax.y /= canvas.pixelRect.height;

            safeAreaTransform.anchorMin = anchorMin;
            safeAreaTransform.anchorMax = anchorMax;
        }

        void OnDestroy()
        {
            if (helpers != null && helpers.Contains(this))
                helpers.Remove(this);
        }

        private static void OrientationChanged()
        {
            lastOrientation = Screen.orientation;
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;

            OnResolutionOrOrientationChanged.Invoke();
        }

        private static void ResolutionChanged()
        {
            lastResolution.x = Screen.width;
            lastResolution.y = Screen.height;

            OnResolutionOrOrientationChanged.Invoke();
        }

        private static void SafeAreaChanged()
        {
            lastSafeArea = Screen.safeArea;

            foreach (CanvasSafeArea area in helpers)
            {
                area.ApplySafeArea();
            }
        }
    }
}