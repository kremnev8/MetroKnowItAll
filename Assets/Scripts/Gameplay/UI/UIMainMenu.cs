using UnityEngine;

namespace Gameplay.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public GameObject gameModeScreen;
        public GameObject questionTypeScreen;
        public GameObject crossfade;

        private int selectedGameMode;
        private int selectedQuestionType;

        public void SelectGameModeAndLoad(int gameMode)
        {
            selectedGameMode = gameMode;
            gameModeScreen.SetActive(false);
            questionTypeScreen.SetActive(false);
            StartLoad();
        }

        public void SelectGameMode(int gameMode)
        {
            selectedGameMode = gameMode;
            gameModeScreen.SetActive(false);
            questionTypeScreen.SetActive(true);
        }

        public void SelectQuestionMode(int questionMode)
        {
            selectedQuestionType = questionMode;
            questionTypeScreen.SetActive(false);
            StartLoad();
        }

        private void StartLoad()
        {
            if (SceneTransitionManager.instance != null)
            {
                if (crossfade != null)
                    crossfade.SetActive(true);

                SceneTransitionManager.instance.StartGame(selectedGameMode, selectedQuestionType);
            }
        }
    }
}