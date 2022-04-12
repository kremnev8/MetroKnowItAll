using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Util;

namespace Gameplay.Questions.Model
{
    public class UIQuestionOrderStations : BaseUIQuestion
    {
        public UIDraggableButtonList buttons;
        public TMP_Text taskLabel;
        public TMP_Text feedbackLabel;
        public RectTransform bottomPane;

        public GameObject checkButton;
        
        private int hideFeedbackIn;
        private List<TMP_Text> mapLabels = new List<TMP_Text>();
        public Transform tmpLabelTransform;

        public override BaseQuestionGenerator GetController()
        {
            return new OrderStationsGenerator();
        }

        public override void HideElements()
        {
            buttons.gameObject.SetActive(false);
            checkButton.SetActive(false);
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
                mapLabels[i].color = Color.black;
            }
        }

        public void SetQuestion(List<MetroStation> stations)
        {
            buttons.gameObject.SetActive(true);
            checkButton.SetActive(true);
            buttons.SetData(stations.Select(station => station as INamedArrayElement).ToList());

            mapLabels.Capacity = stations.Count;
            List<MetroStation> orderedStations = stations.OrderBy(station => station.globalId).ToList();
            
            foreach (MetroStation station in orderedStations)
            {
                GameObject labelObject = renderer.getStationDisplay(station).label.gameObject;
                GameObject newLabelObject = Instantiate(labelObject, tmpLabelTransform, true);
                newLabelObject.SetActive(true);
                mapLabels.Add(newLabelObject.GetComponent<TMP_Text>());
            }
            
            OnOrderChanged();
            
            
            taskLabel.text = "Поместите станций в порядке на линий";
            float height = 130 + taskLabel.preferredHeight + stations.Count * 80;
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
                
                button.SetColor(correct ? PaletteHelper.theme.rightAnswer : PaletteHelper.theme.wrongAnswer);
            }
            
            feedbackLabel.text = allCorrect ? "Верно!" : "Неправильно!";
            hideFeedbackIn = 120;
        }
        
        private void Update()
        {
            if (hideFeedbackIn > 0)
            {
                hideFeedbackIn--;
                if (hideFeedbackIn == 0)
                {
                    feedbackLabel.text = "";
                }
            }
        }
    }
}