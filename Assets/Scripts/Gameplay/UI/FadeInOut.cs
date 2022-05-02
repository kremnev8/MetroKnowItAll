using UnityEngine;

namespace Gameplay.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeInOut : MonoBehaviour
    {
        public float fadeTime;

        private float timeElapsed;
        private bool isFading;
        private bool fadeMode;
        
        private CanvasGroup group;

        private void Awake()
        {
            group = GetComponent<CanvasGroup>();
        }

        public void Fade(bool direction)
        {
            if (isFading) return;

            fadeMode = !direction;
            isFading = true;
            timeElapsed = 0;
            
            if (direction)
                gameObject.SetActive(true);
        }


        public void FadeIn()
        {
            if (isFading) return;

            fadeMode = false;
            isFading = true;
            timeElapsed = 0;
            gameObject.SetActive(true);
        }

        public void FadeOut()
        {
            if (isFading) return;
            
            fadeMode = true;
            isFading = true;
            timeElapsed = 0;
        }

        private void Update()
        {
            if (isFading)
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed < fadeTime)
                {
                    float alpha =  timeElapsed / fadeTime;
                    if (fadeMode) alpha = 1 - alpha;

                    group.alpha = alpha;
                }
                else
                {
                    group.alpha = fadeMode ? 0 : 1;
                    isFading = false;
                    gameObject.SetActive(!fadeMode);
                }
            }
        }
    }
}