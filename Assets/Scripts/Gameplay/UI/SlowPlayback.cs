using UnityEngine;
using UnityEngine.Playables;

namespace Gameplay.UI
{
    public class SlowPlayback : MonoBehaviour
    {
        public float speed;
        public float currentTime;
        public float maxTime;
        
        private PlayableDirector director;

        private void Awake()
        {
            director = GetComponent<PlayableDirector>();
        }

        private void Update()
        {
            currentTime += speed * Time.deltaTime;
            if (currentTime > maxTime) currentTime = 0;
            
            director.time = currentTime;
            director.Evaluate();
        }
    }
}