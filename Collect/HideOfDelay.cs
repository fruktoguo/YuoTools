using UnityEngine;

public class HideOfDelay : MonoBehaviour
{
    public float Delay;
    void Start()
    {
        this.YuoInvoke( () => gameObject.SetActive(false), Delay.Clamp());
    }
}
