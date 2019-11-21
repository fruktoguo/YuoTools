using UnityEngine;
namespace YuoTools
{
    public class ShowOfDelay : MonoBehaviour
    {
        public float Delay = 3;
        bool Trigger = true;
        void OnEnable()
        {
            Trigger = !Trigger;
            if (Trigger)
            {
                return;
            }
            gameObject.Hide();
            this.YuoDelay(() => gameObject.Show(), Delay.Clamp());
        }
    }
}