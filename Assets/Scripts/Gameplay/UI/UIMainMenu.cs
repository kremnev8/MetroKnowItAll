using UnityEngine;

namespace Gameplay.UI
{
    /// <summary>
    /// Main menu UI controller
    /// </summary>
    public class UIMainMenu : MonoBehaviour
    {
        public GameObject gameModeScreen;
        public GameObject crossfade;

        private int selectedGameMode;

        public void SelectGameMode(int gameMode)
        {
            selectedGameMode = gameMode;
            gameModeScreen.SetActive(false);
            StartLoad();
        }
        

        private void StartLoad()
        {
            if (SceneTransitionManager.instance != null)
            {
                if (crossfade != null)
                    crossfade.SetActive(true);

                SceneTransitionManager.instance.StartGame(selectedGameMode);
            }
        }
    }
}