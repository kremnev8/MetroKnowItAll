using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Questions.Model
{
    public class FindStationGenerator : BaseQuestionGenerator
    {
        public int currentLineId;
        public RegionType currentRegionType;
        public MetroStation currentQuestion;


        public UIQuestionFindStation uiController;

        private List<int> blacklistedIds = new List<int>();

        public override void Init(MetroRenderer _renderer, BaseUIQuestion root)
        {
            base.Init(_renderer, root);

            uiController = root as UIQuestionFindStation;
        }

        public override void SetRegion(Region region)
        {
            blacklistedIds.Clear();
            currentLineId = region.lineId;
            currentRegionType = region.regionType;
        }

        public override void GenerateNew()
        {
            int count = 0;
            do
            {
                currentQuestion = metro.PickRandomStation(currentLineId);
                count++;
                if (count > 100) break;
            } while (blacklistedIds.Contains(currentQuestion.globalId));
            
            uiController.SetQuestion(currentQuestion);
            renderer.HideAllLabels();
            blacklistedIds.Add(currentQuestion.globalId);
            
            if (currentLineId != -1 && currentRegionType == RegionType.GLOBAL)
            {
                renderer.FocusLine(currentLineId);
            }
        }

        public override bool ValidateAnswer()
        {
            MetroStation selectedStation = uiController.CurrentSelection();

            bool result = currentQuestion.globalId == selectedStation.globalId;
            uiController.DisplayResult(result);

            return result;
        }
    }
}