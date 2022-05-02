using System.Collections.Generic;
using System.Linq;
using Gameplay.Controls;
using Gameplay.MetroDisplay.Model;
using TMPro;
using UnityEngine;
using Util;

namespace Gameplay.Questions.Generators
{
    /// <summary>
    /// UI for <see cref="OrderStationsGenerator"/>
    /// </summary>
    public class UIQuestionOrderStations : BaseUIQuestion
    {
        public UIDraggableButtonList buttons;

        private List<TMP_Text> mapLabels = new List<TMP_Text>();
        public Transform tmpLabelTransform;

        public override BaseQuestionGenerator GetController()
        {
            return new OrderStationsGenerator();
        }

        public override void HideElements()
        {
            base.HideElements();
            buttons.gameObject.SetActive(false);
            foreach (TMP_Text label in mapLabels)
            {
                Destroy(label.gameObject);
            }
            mapLabels.Clear();
        }

        public void OnOrderChanged()
        {
            for (int i = 0; i < buttons.buttons.Count; i++)
            {
                MetroStation station = buttons.buttons[i].element as MetroStation;
                mapLabels[i].text = station.currentName;
                mapLabels[i].color = GameController.theme.textColor;
            }
        }

        public void SetQuestion(List<MetroStation> stations)
        {
            buttons.gameObject.SetActive(true);
            buttons.SetData(stations.Select(station => station as INamedArrayElement).ToList());

            mapLabels.Capacity = stations.Count;
            List<MetroStation> orderedStations = stations.OrderBy(station => station.globalId).ToList();
            
            foreach (MetroStation station in orderedStations)
            {
                GameObject labelObject = renderer.GetStationDisplay(station).label.gameObject;
                GameObject newLabelObject = Instantiate(labelObject, tmpLabelTransform, true);
                newLabelObject.SetActive(true);
                mapLabels.Add(newLabelObject.GetComponent<TMP_Text>());
            }
            
            OnOrderChanged();
            
            
            questionLabel.text = "Поместите станций в порядке на линий";
            float height = 130 + questionLabel.preferredHeight + stations.Count * 80;
            bottomPane.sizeDelta = new Vector2(bottomPane.sizeDelta.x, height);
        }
        
        public List<MetroStation> CurrentSelection()
        {
            return buttons.buttons.Select(button => button.element as MetroStation).ToList();
        }

        public void DisplayResult(List<bool> result, bool allCorrect)
        {
            for (int i = 0; i < buttons.buttons.Count; i++)
            {
                UIDragableButton button = buttons.buttons[i];
                bool correct = result[i];
                
                button.SetColor(correct ? GameController.theme.rightAnswer : GameController.theme.wrongAnswer);
            }
        }
    }
}