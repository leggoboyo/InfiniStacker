using System;
using UnityEngine;

namespace InfiniStacker.Player
{
    public static class SquadFormation
    {
        public static Vector3[] GetSlots(int squadCount, float spacing)
        {
            if (squadCount <= 0)
            {
                return Array.Empty<Vector3>();
            }

            var clampedSpacing = Mathf.Max(0.15f, spacing);
            var slots = new Vector3[squadCount];
            var index = 0;
            var row = 0;

            while (index < squadCount)
            {
                var rowCount = Mathf.Min(3, squadCount - index);
                var rowStartX = -((rowCount - 1) * 0.5f * clampedSpacing);

                for (var i = 0; i < rowCount; i++)
                {
                    slots[index++] = new Vector3(rowStartX + (i * clampedSpacing), 0f, -(row * clampedSpacing * 1.15f));
                }

                row++;
            }

            return slots;
        }
    }
}
