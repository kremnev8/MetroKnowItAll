using System;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class StationDisplay : MonoBehaviour
    {
        public MetroStation station;
        private MetroLine line;
        
        public SpriteRenderer spriteRenderer;
        public TMP_Text label;
        
        private static readonly int colorProp = Shader.PropertyToID("_Color");
        private static MaterialPropertyBlock block;
        private static readonly Vector2 offset = new Vector2(1, 1);

        public bool shouldLabelDisplay = false;
        public int timeToHideLabel;
        
        
        public void SetStation(MetroStation station, MetroLine line)
        {
            this.station = station;
            this.line = line;

            Refresh();
        }

        public void Refresh()
        { 
            SetColor(line.lineColor);

            if (station.m_override)
            {
                SetText(station.currnetName, station.namePosition, station.nameAlignment);
            }
            else
            {
                SetText(station.currnetName, line.namePosition, line.nameAlignment);
            }

            SetTextVisible(!station.hideName);
        }

        public void SetColor(Color color)
        {
            block ??= new MaterialPropertyBlock();
            
            spriteRenderer.GetPropertyBlock(block);
            block.SetColor(colorProp, color);
            spriteRenderer.SetPropertyBlock(block);
        }

        public void SetText(string name, NamePosition position, TextAlignmentOptions alignment)
        {
            label.text = name;
            
            Vector2 newPosition = position.GetOffset() * label.rectTransform.sizeDelta + position.GetDirection() * offset;
            label.rectTransform.anchoredPosition = newPosition;
            
            label.alignment = alignment;
        }

        public void SetTextVisible(bool value)
        {
            label.gameObject.SetActive(value);
            shouldLabelDisplay = value;
            timeToHideLabel = 1;
        }

        public void DisplayLabel(Color color)
        {
            if (shouldLabelDisplay)
            {
                label.color = color;
                label.gameObject.SetActive(true);

                timeToHideLabel = 120;
            }
        }

        private void Update()
        {
            if (shouldLabelDisplay && timeToHideLabel > 0)
            {
                timeToHideLabel--;
                if (timeToHideLabel == 0)
                {
                    label.gameObject.SetActive(false);
                }
                
            }
        }
    }
}