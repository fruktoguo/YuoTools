using UnityEngine;

namespace YuoTools
{
    public class HideOnStart : MonoBehaviour
    {
        private void Start()
        {
            gameObject.SetActive(false);
        }
    }
}