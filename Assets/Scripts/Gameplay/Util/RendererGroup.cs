using System;
using UnityEngine;

namespace Util
{
    [ExecuteInEditMode]
    public class RendererGroup : MonoBehaviour
    {
        public bool active;
        
        private Renderer[] renderers;
#if UNITY_EDITOR
        private bool lastState;
#endif

        private void OnEnable()
        {
            renderers = transform.GetComponentsInChildren<Renderer>();
        }

        public void SetState(bool value)
        {
            foreach (Renderer renderer1 in renderers)
            {
                renderer1.enabled = active;
            }
            
#if UNITY_EDITOR
            lastState = active;
#endif  
        }
#if UNITY_EDITOR
        public void Update()
        {
            if (lastState != active)
            {
                SetState(active);
            }
        }
#endif
    }
}