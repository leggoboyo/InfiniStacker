using InfiniStacker.Gates;
using NUnit.Framework;

namespace InfiniStacker.Tests.EditMode
{
    public sealed class GateMathTests
    {
        [Test]
        public void Apply_AddOperation_IncreasesCount()
        {
            var operation = new GateOperation(GateOperationType.Add, 6);
            var result = GateMath.Apply(4, operation);
            Assert.AreEqual(10, result);
        }

        [Test]
        public void Apply_SubtractOperation_FloorsAtZero()
        {
            var operation = new GateOperation(GateOperationType.Subtract, 20);
            var result = GateMath.Apply(7, operation);
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Apply_MultiplyOperation_ScalesCount()
        {
            var operation = new GateOperation(GateOperationType.Multiply, 3);
            var result = GateMath.Apply(5, operation);
            Assert.AreEqual(15, result);
        }

        [Test]
        public void Apply_SequentialOperations_StayDeterministic()
        {
            var current = 4;
            current = GateMath.Apply(current, new GateOperation(GateOperationType.Add, 3));
            current = GateMath.Apply(current, new GateOperation(GateOperationType.Multiply, 2));
            current = GateMath.Apply(current, new GateOperation(GateOperationType.Subtract, 5));

            Assert.AreEqual(9, current);
        }
    }
}
