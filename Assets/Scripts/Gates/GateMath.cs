using UnityEngine;

namespace InfiniStacker.Gates
{
    public static class GateMath
    {
        public static int Apply(int current, GateOperation operation)
        {
            var safeCurrent = Mathf.Max(0, current);
            var safeValue = Mathf.Max(0, operation.Value);

            return operation.Type switch
            {
                GateOperationType.Add => safeCurrent + safeValue,
                GateOperationType.Subtract => Mathf.Max(0, safeCurrent - safeValue),
                GateOperationType.Multiply => safeCurrent * Mathf.Max(1, safeValue),
                _ => safeCurrent
            };
        }
    }
}
