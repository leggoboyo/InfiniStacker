using InfiniStacker.Player;
using NUnit.Framework;
using UnityEngine;

namespace InfiniStacker.Tests.EditMode
{
    public sealed class SquadFormationTests
    {
        [Test]
        public void GetSlots_ReturnsRequestedCount()
        {
            var slots = SquadFormation.GetSlots(7, 0.8f);
            Assert.AreEqual(7, slots.Length);
        }

        [Test]
        public void GetSlots_ForSingleSoldier_ReturnsCenteredOriginSlot()
        {
            var slots = SquadFormation.GetSlots(1, 0.8f);
            Assert.AreEqual(1, slots.Length);
            Assert.That(slots[0], Is.EqualTo(Vector3.zero));
        }

        [Test]
        public void GetSlots_ForFiveSoldiers_UsesTwoRowsWithDeterministicSpacing()
        {
            var slots = SquadFormation.GetSlots(5, 1f);

            Assert.That(slots[0].x, Is.EqualTo(-1f).Within(0.0001f));
            Assert.That(slots[1].x, Is.EqualTo(0f).Within(0.0001f));
            Assert.That(slots[2].x, Is.EqualTo(1f).Within(0.0001f));

            Assert.That(slots[3].x, Is.EqualTo(-0.5f).Within(0.0001f));
            Assert.That(slots[4].x, Is.EqualTo(0.5f).Within(0.0001f));

            Assert.That(slots[3].z, Is.LessThan(0f));
            Assert.That(slots[4].z, Is.LessThan(0f));
        }
    }
}
