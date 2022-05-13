using System;
using UnityEngine;

namespace Util
{
    [ExecuteInEditMode]
    public class SortingLayer : MonoBehaviour
    {
        public string sortingLayer;
        public int sortingOrder;

        private Renderer renderer;
        
        private void OnEnable()
        {
            renderer = GetComponent<Renderer>();
            renderer.sortingLayerName = sortingLayer;
            renderer.sortingOrder = sortingOrder;
        }
    }
}