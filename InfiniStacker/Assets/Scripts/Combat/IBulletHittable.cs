using UnityEngine;

namespace InfiniStacker.Combat
{
    public interface IBulletHittable
    {
        bool OnBulletHit(int damage, Vector3 hitPoint);
    }
}
