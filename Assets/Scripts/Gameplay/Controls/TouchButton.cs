using System;
using Gameplay.MetroDisplay;
using Gameplay.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using Util;

namespace Gameplay.Controls
{
    /// <summary>
    /// Defines a selectable UI element
    /// </summary>
    public interface ISelectable
    {
        public bool IsFocused(MetroRenderer metroRenderer);
        public void SetSelected(MetroRenderer metroRenderer, bool value);
    }
    
    /// <summary>
    /// UI control to select objects, like metro stations and lines
    /// </summary>
    public class TouchButton : MonoBehaviour
    {
        private PlayerInput input;
        private new MetroRenderer renderer;
        
        private InputAction primaryDeltaAction;
        private InputAction primaryPositionAction;
        private InputAction primaryContactAction;

        private ISelectable selectedStation;
        public new Camera camera;

        public bool isEnabled;
        public Func<ISelectable, bool> filter;
        public bool overrideFocus;

        private RaycastHit2D[] hits = new RaycastHit2D[5];

        public void Enable( Func<ISelectable, bool> filter, bool overrideFocus = false)
        {
            isEnabled = true;
            this.filter = filter;
            this.overrideFocus = overrideFocus;
        }

        public void Disable()
        {
            isEnabled = false;
            overrideFocus = false;
            selectedStation?.SetSelected(renderer, false);
            selectedStation = null;
        }

        public T GetSelected<T>() where T : class
        {
            if (selectedStation == null) throw new InvalidOperationException("User did not select anything!");
            
            return selectedStation as T;
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
        }
        

        private void CheckPress(InputAction.CallbackContext obj)
        {
            if (!isEnabled) return;
            
            Vector2 position = primaryPositionAction.ReadValue<Vector2>();
            
            bool onUI = UIUtil.IsPointerOverAnyUI(position);
            if (onUI) return;
            
            if (primaryDeltaAction.ReadValue<Vector2>().magnitude < 1f)
            {
                Ray ray = camera.ScreenPointToRay(position);

                int size = Physics2D.GetRayIntersectionNonAlloc(ray, hits, Mathf.Infinity);
                for (int i = 0; i < size; i++)
                {
                    RaycastHit2D hit = hits[i];
                    if (hit.collider == null) continue;
                    
                    ISelectable display = hit.collider.gameObject.GetComponent<ISelectable>();
                    if (display != null && filter(display) && (display.IsFocused(renderer) || overrideFocus))
                    {
                        selectedStation?.SetSelected(renderer, false);
                        display.SetSelected(renderer, true);
                        selectedStation = display;
                        break;
                    }
                }
            }
            
        }
    }
}