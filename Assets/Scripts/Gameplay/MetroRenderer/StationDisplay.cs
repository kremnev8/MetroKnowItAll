using System;
using TMPro;
using UnityEngine;

namespace Gameplay
{
    public class StationDisplay : MonoBehaviour, ISelectable
    {
        public MetroStation station;
        private MetroLine line;
        
        public SpriteRenderer spriteRenderer;
        public TMP_Text label;
        
        private static readonly int colorProp = Shader.PropertyToID("_Color");
        private static readonly int isFocused = Shader.PropertyToID("_IsFocused");
        private static MaterialPropertyBlock block;
        private static readonly Vector2 offset = new Vector2(1, 1);

        public bool shouldLabelDisplay = false;
        public int timeToHideLabel;
        
        public void SetFocused(bool value)
        {
            spriteRenderer.GetPropertyBlock(block);
            block.SetFloat(isFocused, value ? 1 : 0);
            spriteRenderer.SetPropertyBlock(block);
        }
        
        public void SetLabelVisible(bool isVisible, Color color)
        {
            if (shouldLabelDisplay && timeToHideLabel <= 0)
            {
                label.color = color;
                label.gameObject.SetActive(isVisible);

                timeToHideLabel = -1;
            }
        }
        
        public void ShowLabelFor(Color color, int time)
        {
            if (shouldLabelDisplay && timeToHideLabel <= 0)
            {
                label.color = color;
                label.fontSize = 2;
                label.gameObject.SetActive(true);

                timeToHideLabel = time;
            }
        }

        
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
                SetText(station.currentName, station.namePosition, station.nameAlignment);
            }
            else
            {
                SetText(station.currentName, line.namePosition, line.nameAlignment);
            }

            SetInitialVisible(!station.hideName);
        }

        private void SetColor(Color color)
        {
            block ??= new MaterialPropertyBlock();
            
            spriteRenderer.GetPropertyBlock(block);
            block.SetColor(colorProp, color);
            spriteRenderer.SetPropertyBlock(block);
        }

        private void SetText(string name, NamePosition position, TextAlignmentOptions alignment)
        {
            label.text = name;
            
            Vector2 newPosition = position.GetOffset() * label.rectTransform.sizeDelta + position.GetDirection() * offset;
            label.rectTransform.anchoredPosition = newPosition;
            
            label.alignment = alignment;
        }

        private void SetInitialVisible(bool value)
        {
            label.gameObject.SetActive(value);
            shouldLabelDisplay = value;
        }
        
        private void Update()
        {
            if (shouldLabelDisplay && timeToHideLabel > 0)
            {
                timeToHideLabel--;
                if (timeToHideLabel == 0)
                {
                    label.gameObject.SetActive(false);
                    label.fontSize = 1.8f;
                }
                
            }
        }

        public bool IsFocused(MetroRenderer metroRenderer)
        {
            if (metroRenderer.focusedLineId != -1)
            {
                return station.lineId == metroRenderer.focusedLineId;
            }

            return metroRenderer.focusArea.IsInside(station.position);
        }

        public void SetSelected(MetroRenderer metroRenderer, bool value)
        {
            GameObject selector = metroRenderer.stationSelect;
            selector.SetActive(value);
            if (value)
            {
                selector.transform.parent = transform;
                selector.transform.localPosition = Vector3.zero;
            }
        }
    }
}