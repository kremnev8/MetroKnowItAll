using System.Collections.Generic;
using System.Linq;
using Gameplay.MetroDisplay.Model;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using Util;

namespace Gameplay.MetroDisplay
{
    public class HighlightDisplay : MonoBehaviour
    {
        public RenderTexture renderTexture;
        public Metro metro;
        public Region region;

        public Transform quadTrans;
        
        private Texture2D tempTexture;
        
        public Texture2D smallBlur;
        public Material smallBlueMat;

        public Vector2 scale;

        [InspectorButton]
        public void Refresh()
        {
            tempTexture = new Texture2D(256, 256, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
            Color[] pixels = Enumerable.Repeat(Color.clear, Screen.width * Screen.height).ToArray();
            tempTexture.SetPixels(pixels);
            tempTexture.Apply();
            Graphics.Blit(tempTexture, renderTexture);

            Vector2 quadScale = quadTrans.localScale / 2;

            RenderTexture.active = renderTexture;
            GL.PushMatrix ();                               
            GL.LoadPixelMatrix (0, 256, 256, 0);   

            foreach (MetroLine line in metro.lines)
            {
                foreach (MetroStation station in line.stations)
                {
                    if (region.Contains(station) || region.regionType == RegionType.GLOBAL)
                    {
                        var drawPos = new Vector2(quadScale.x + station.position.x, quadScale.y - station.position.y);
                        drawPos = drawPos * (128 / quadScale.x);
                        Rect rect = new Rect(drawPos.x - scale.x / 2, drawPos.y - scale.x / 2, scale.x,
                            scale.x);
                        Graphics.DrawTexture(rect, smallBlur, smallBlueMat);
                    }
                }
            }

            GL.PopMatrix ();
            RenderTexture.active = null;
        }

        public void Clear()
        {
            tempTexture = new Texture2D(256, 256, GraphicsFormat.R8G8B8A8_UNorm, TextureCreationFlags.None);
            Color[] pixels = Enumerable.Repeat(Color.white, Screen.width * Screen.height).ToArray();
            tempTexture.SetPixels(pixels);
            tempTexture.Apply();
            Graphics.Blit(tempTexture, renderTexture);
        }
    }
}