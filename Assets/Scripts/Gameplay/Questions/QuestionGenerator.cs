﻿using System.Collections.Generic;
using Gameplay.MetroDisplay;
using Gameplay.MetroDisplay.Model;

namespace Gameplay.Questions
{
    public abstract class QuestionGenerator<T> : BaseQuestionGenerator where T : BaseUIQuestion
    {
        public Region currentRegion;
        protected List<int> blacklistedIds = new List<int>();
        
        public T uiController;
        
        public override void Init(MetroRenderer _renderer, BaseUIQuestion root)
        {
            base.Init(_renderer, root);

            uiController = root as T;
        }

        public override void SetRegion(Region region)
        {
            blacklistedIds.Clear();
            currentRegion = region;
        }
    }
}