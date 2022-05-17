using UnityEngine;

namespace Gameplay.Conrollers
{
    public abstract class BaseGameMode : MonoBehaviour
    {
        public abstract string gameModeId { get; }

        public abstract void Init(GameModeController mainController);
        public abstract void StartNewSession(Game gameState);
        public abstract void ConfirmPressed();
        public abstract void StartGamePressed();
        public abstract void ManualUpdate();
        public abstract string GetNextTip(int index);
    }
}