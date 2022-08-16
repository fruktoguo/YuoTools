using UnityEngine;

namespace YuoTools
{
    public class ShowOfDelay : MonoBehaviour
    {
        public float Delay = 3;
        private bool Trigger = true;

        private async void OnEnable()
        {
            Trigger = !Trigger;
            if (Trigger)
            {
                return;
            }
            gameObject.Hide();
            await YuoWait.WaitTimeAsync(Delay);
            gameObject.Show();
        }
    }
}