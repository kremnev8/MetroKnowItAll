using System.Collections;
using Gameplay.Core;
using ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;
using RectUtils = Util.RectUtils;

namespace Gameplay.Controls
{
    public class TouchCameraController : MonoBehaviour
    {
        public new Camera camera;
        public PlayerConfig config;
        public bool controlEnabled = true;

        public Rect worldBounds;

        // INPUT
        private PlayerInput input;

        private InputAction primaryPositionAction;
        private InputAction primaryContactAction;

        private InputAction firstDeltaAction;
        private InputAction firstPositionAction;

        private InputAction secondDeltaAction;
        private InputAction secondPositionAction;
        private InputAction secondContactAction;
        
#if UNITY_EDITOR
        private InputAction scrollAction;
#endif
        
        // STATE
        private Coroutine zoomCoroutine;

        private Vector2 initialPosition;
        private Vector3 currentDir;
        private Vector3 cameraVelocity;
        
        private bool isDragging;
        private int zoomCounter;
        private bool isZooming => zoomCounter > 0;
        
        // CAMERA LERP
        private Vector3 lerpTarget;
        private bool shouldLerp;
        private float lerpTimeElapsed;
        public float lerpTime = 5;
        public float endLerpThreshold = 1;

        public void LerpTo(Vector3 position) 
        {
            position.z = -10; 
            lerpTarget = position;
            shouldLerp = true;
            lerpTimeElapsed = 0;
        }
        
        
        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;

            primaryPositionAction = input.actions["primaryPosition"];
            primaryContactAction = input.actions["primaryContact"];

            firstDeltaAction = input.actions["delta0"];
            firstPositionAction = input.actions["position0"];

            secondDeltaAction = input.actions["delta1"];
            secondPositionAction = input.actions["position1"];
            secondContactAction = input.actions["contact1"];
#if UNITY_EDITOR
            scrollAction = input.actions["Scroll"];
#endif

            primaryContactAction.started += StartDrag;
            primaryContactAction.canceled += EndDrag;

            secondContactAction.started += ZoomStart;
            secondContactAction.canceled += ZoomEnd;
        }

        private void StartDrag(InputAction.CallbackContext obj)
        {
            initialPosition = primaryPositionAction.ReadValue<Vector2>();
            initialPosition = camera.ScreenToWorldPoint(initialPosition);

            isDragging = true;
        }

        private void EndDrag(InputAction.CallbackContext obj)
        {
            isDragging = false;
        }

        private void Update()
        {
#if UNITY_EDITOR
            float mouseScroll = scrollAction.ReadValue<Vector2>().y / config.scrollSensitivity;
            camera.orthographicSize += mouseScroll * Time.deltaTime;
#endif
            
            if (zoomCounter > 0)
                zoomCounter--;
            
            bool onUI = UIUtil.IsPointerOverAnyUI(primaryPositionAction.ReadValue<Vector2>());
            
            if (controlEnabled && isDragging && !isZooming && !onUI)
            {
                Vector2 touchPos = primaryPositionAction.ReadValue<Vector2>();
                if (!touchPos.IsInfinity())
                {
                    Vector2 targetDir = camera.ScreenToWorldPoint(touchPos);
                    targetDir -= initialPosition;
                    
                    currentDir = targetDir.ToVector3() * config.normalMaxSpeed;
                }
            }
            else
            {
                currentDir *= config.friction;
            }

            if (shouldLerp)
            {
                lerpTimeElapsed += Time.deltaTime;
                if (lerpTimeElapsed < lerpTime)
                {
                    float t = lerpTimeElapsed / lerpTime;
                    Vector3 pos = Vector3.Lerp(camera.transform.position, lerpTarget, t);

                    if ((pos - lerpTarget).magnitude < endLerpThreshold)
                    {
                        shouldLerp = false;
                    }
                    
                    pos -= currentDir * Time.deltaTime / 4;
                    camera.transform.position = worldBounds.Clamp3(pos);
                }
                else
                {
                    shouldLerp = false;
                }
            }
            else
            {
                Vector3 newPos = camera.transform.position - currentDir * Time.deltaTime;
                camera.transform.position = worldBounds.Clamp3(newPos);
            }
        }

        private void ZoomStart(InputAction.CallbackContext obj)
        {
            zoomCoroutine = StartCoroutine(ZoomCoroutine());
        }

        private void ZoomEnd(InputAction.CallbackContext obj)
        {
            StopCoroutine(zoomCoroutine);
            zoomCounter = 0;
        }

        private IEnumerator ZoomCoroutine()
        {
            float previousDistance = 0;
            float distance = 0;

            Vector2 firstPos = firstPositionAction.ReadValue<Vector2>();
            Vector2 secondPos = secondPositionAction.ReadValue<Vector2>();

            distance = Vector2.Distance(firstPos, secondPos);
            previousDistance = distance;


            while (true)
            {
                if (!controlEnabled)
                {
                    yield return null;
                    continue;
                }

                firstPos = firstPositionAction.ReadValue<Vector2>();
                secondPos = secondPositionAction.ReadValue<Vector2>();
                Vector2 firstDelta = firstDeltaAction.ReadValue<Vector2>();
                Vector2 secondDelta = secondDeltaAction.ReadValue<Vector2>();

                distance = Vector2.Distance(firstPos, secondPos);

                if (Vector2.Dot(firstDelta, secondDelta) < -0.8f)
                {
                    float change = distance - previousDistance;

                    if (Mathf.Abs(distance) > 0.1f)
                    {
                        float targetZoom = camera.orthographicSize - change;
                        float newZoom = Mathf.Lerp(camera.orthographicSize, targetZoom, Time.deltaTime * config.zoomSpeed);
                        newZoom = Mathf.Clamp(newZoom, config.minZoom, config.maxZoom);
                        camera.orthographicSize = newZoom;
                        zoomCounter = 10;

                        previousDistance = distance;
                        yield return null;
                        continue;
                    }
                }

                previousDistance = distance;
                yield return null;
            }
        }
    }
    
    [CustomEditor(typeof(TouchCameraController))]
    public class TouchCameraControllerEditor : Editor
    {
        private void OnSceneGUI()
        { 
            Debug.Log("Scene GUI");
            var rectExample = (TouchCameraController)target;

            var rect = RectUtils.ResizeRect(
                rectExample.worldBounds,
                Handles.CubeHandleCap,
                Color.green,
                Color.yellow,
                HandleUtility.GetHandleSize(Vector3.zero) * .1f,
                1); 

            rectExample.worldBounds = rect;
        }
    }
}