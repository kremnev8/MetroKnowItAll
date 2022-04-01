using Gameplay.Questions;
using Model;
using Platformer.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Gameplay
{
    public class TouchButton : MonoBehaviour
    {
        private PlayerInput input;
        
        private InputAction primaryDeltaAction;
        private InputAction primaryPositionAction;
        private InputAction primaryContactAction;

        private StationDisplay lastStation;

        public new Camera camera;
        public Canvas canvas;
        
        public RectTransform confirm;
        public UIMapQuestion question;

        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;

            primaryPositionAction = input.actions["primaryPosition"];
            primaryContactAction = input.actions["primaryContact"];
            primaryDeltaAction =  input.actions["primaryDelta"];
            
            primaryContactAction.started += CheckPress;
        }

        public void Confirm()
        {
            if (question.CheckAnswer(lastStation.station))
            {
                lastStation.DisplayLabel(Color.green);
            }
            else
            {
                lastStation.DisplayLabel(Color.red);
            }
            
            confirm.gameObject.SetActive(false);
        }
        
        public static Vector2 WorldPositionToScreenSpaceCameraPosition(Camera worldCamera, Canvas canvas, Vector3 position)
        {
            Vector2 viewport = worldCamera.WorldToViewportPoint(position);
            Ray canvasRay = worldCamera.ViewportPointToRay(viewport);
            return canvasRay.GetPoint(canvas.planeDistance);
        }

        private void CheckPress(InputAction.CallbackContext obj)
        {
            if (primaryDeltaAction.ReadValue<Vector2>().magnitude < 1f)
            {
                Vector2 position = primaryPositionAction.ReadValue<Vector2>();
                Ray ray = camera.ScreenPointToRay(position);

                RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);

                if (hit.collider != null)
                {
                    StationDisplay display = hit.collider.gameObject.GetComponent<StationDisplay>();
                    if (display != null)
                    {
                        Vector3 needPos = hit.transform.position;

                        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, needPos);
                        
                        Vector2 anchoredPosition = confirm.parent.InverseTransformPoint(screenPoint);
                        
                        //Vector3 screenPos = camera.WorldToScreenPoint(needPos);
                       // RectTransformUtility.ScreenPointToWorldPointInRectangle((RectTransform)confirm.parent, screenPos, null, out Vector3 point);
                       
                        confirm.anchoredPosition = anchoredPosition;
                        confirm.gameObject.SetActive(true);

                        lastStation = display;

                    }
                }   
            }
            
        }
    }
}