using System;
using UnityEngine;

namespace Gameplay.Conrollers
{
    public abstract class BaseGameMode : MonoBehaviour
    {
        protected ISaveController model;

        public abstract ISaveController CreateModelHandler();

        public T GetModel<T>() where T : class
        {
            return model as T;
        }

        public virtual void Init(GameModeController mainController)
        {
            model = CreateModelHandler();
            model.SetFilename($"saves/{gameModeId}-save");
        }
        
        public void StartSession(Game gameState)
        {
            bool hasSaveData = model.Load();
            if (!hasSaveData)
            {
                SetupNewSession(gameState);
            }
            ContinueSession(gameState);
        }

        private void OnDestroy()
        {
            model.Save();
        }


        public abstract string gameModeId { get; }
        public abstract void SetupNewSession(Game gameState);
        public abstract void ContinueSession(Game gameState);
        public abstract void ConfirmPressed();
        public abstract void StartGamePressed();
        public abstract void ManualUpdate();
        public abstract string GetNextTip(int index);
    }
}