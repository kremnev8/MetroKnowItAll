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
        private new MetroRenderer renderer;
        
        private InputAction primaryDeltaAction;
        private InputAction primaryPositionAction;
        private InputAction primaryContactAction;

        public StationDisplay selectedStation { get; private set; }

        public new Camera camera;
        
        public RectTransform confirm;

        public void HideSelector()
        {
            selectedStation = null;
            confirm.gameObject.SetActive(false);
        }
        
        private void Start()
        {
            GameModel model = Simulation.GetModel<GameModel>();
            input = model.input;
            renderer = model.renderer;

            primaryPositionAction = input.actions["primaryPosition"];
            primaryContactAction = input.actions["primaryContact"];
            primaryDeltaAction =  input.actions["primaryDelta"];
            
            primaryContactAction.started += CheckPress;
            TouchCameraController.cameraHasMoved += HideSelector;
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
                    if (display != null && renderer.IsFocused(display))
                    {
                        Vector3 needPos = hit.transform.position;

                        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, needPos);
                        
                        Vector2 anchoredPosition = confirm.parent.InverseTransformPoint(screenPoint);
                        
                        confirm.anchoredPosition = anchoredPosition;
                        confirm.gameObject.SetActive(true);

                        selectedStation = display;

                    }
                }   
            }
            
        }
    }
}