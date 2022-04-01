using System;
using System.Collections;
using Model;
using Platformer.Core;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Gameplay
{
    public class TouchCameraController : MonoBehaviour
    {
        public new Camera camera;
        public PlayerConfig config;
        public bool controlEnabled = true;

        private PlayerInput input;
        
        private InputAction primaryPositionAction;
        private InputAction primaryContactAction;
        
        private InputAction firstDeltaAction;
        private InputAction firstPositionAction;
        
        private InputAction secondDeltaAction;
        private InputAction secondPositionAction;
        private InputAction secondContactAction;

        private Coroutine zoomCoroutine;

        private Vector2 initialPosition;

        private Vector3 currentDir;
        private Vector3 cameraVelocity;
        
        private bool isDragging;
        private bool isZooming;
         

        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;

            primaryPositionAction = input.actions["primaryPosition"];
            primaryContactAction = input.actions["primaryContact"];
            
            firstDeltaAction =  input.actions["delta0"];
            firstPositionAction = input.actions["position0"];
            
            secondDeltaAction =  input.actions["delta1"];
            secondPositionAction = input.actions["position1"];
            secondContactAction = input.actions["contact1"];

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
             Vector2 targetDir = Vector2.zero;;
             if (controlEnabled && isDragging && !isZooming)
             {
                 targetDir = camera.ScreenToWorldPoint(primaryPositionAction.ReadValue<Vector2>());
                 
                 targetDir -= initialPosition;
             }
 
             currentDir = Vector3.SmoothDamp(currentDir, targetDir.ToVector3() *config.normalMaxSpeed, ref cameraVelocity,  config.moveSmoothTime);
 
             camera.transform.position -= currentDir * Time.deltaTime;
        }

        private void ZoomStart(InputAction.CallbackContext obj)
        {
            zoomCoroutine = StartCoroutine(ZoomCoroutine());
            isZooming = true;
        }
        
        private void ZoomEnd(InputAction.CallbackContext obj)
        {
            StopCoroutine(zoomCoroutine);
            isZooming = false;
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
                    }
                }

                previousDistance = distance;
                yield return null;
            }

        }
    }
}