using UnityEngine;

namespace Util
{
    public class HideOnStart : MonoBehaviour
    {
        private void Update()
        {
            gameObject.SetActive(false);
            Destroy(this);
        }
    }
}