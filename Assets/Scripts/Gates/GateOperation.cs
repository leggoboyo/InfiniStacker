using System;

namespace InfiniStacker.Gates
{
    [Serializable]
    public struct GateOperation
    {
        public GateOperationType Type;
        public int Value;

        public GateOperation(GateOperationType type, int value)
        {
            Type = type;
            Value = value;
        }

        public string ToDisplayText()
        {
            return Type switch
            {
                GateOperationType.Add => $"+{Value}",
                GateOperationType.Subtract => $"-{Value}",
                GateOperationType.Multiply => $"x{Value}",
                _ => Value.ToString()
            };
        }

        public bool IsPositive()
        {
            return Type != GateOperationType.Subtract;
        }
    }
}
