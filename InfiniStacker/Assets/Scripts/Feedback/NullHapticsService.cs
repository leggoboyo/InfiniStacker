using UnityEngine;

namespace InfiniStacker.Feedback
{
    public sealed class NullHapticsService : MonoBehaviour, IHapticsService
    {
        public void LightImpact()
        {
        }

        public void MediumImpact()
        {
        }

        public void Success()
        {
        }
    }
}
