using System;
using System.Collections.Generic;

namespace NumberSystemCalculator
{
    /// <summary>
    /// Supported arithmetic and bitwise operations.
    /// </summary>
    public enum Operation
    {
        None,
        Add,
        Subtract,
        Multiply,
        Divide,
        And,
        Or,
        Xor,
        ShiftLeft,
        ShiftRight
    }

    /// <summary>
    /// Encapsulates the calculator's mutable state: operand registers, the
    /// pending operation, and the recent calculation history. All arithmetic
    /// is performed on 64-bit signed integers (base-10), with conversion to
    /// the UI happening through <see cref="NumberConverter"/>.
    /// </summary>
    public class CalculatorEngine
    {
        private const int MaxHistory = 15;

        private readonly List<string> _history = new List<string>();
        private long _accumulator;
        private bool _hasPendingOperand;
        private Operation _pendingOperation = Operation.None;

        /// <summary>Read-only view of the calculation history (newest first).</summary>
        public IReadOnlyList<string> History => _history;

        /// <summary>
        /// Applies a binary operator. If no pending operand exists, the value
        /// becomes the accumulator and the operator is remembered for the next
        /// equals/operator press.
        /// </summary>
        public void PushOperation(long currentValue, Operation op)
        {
            if (_hasPendingOperation())
            {
                _accumulator = Compute(_accumulator, currentValue, _pendingOperation);
            }
            else
            {
                _accumulator = currentValue;
            }
            _pendingOperation = op;
            _hasPendingOperand = true;
        }

        /// <summary>
        /// Finalises a chain of operations and records it in history.
        /// </summary>
        /// <param name="currentValue">The right-hand operand.</param>
        /// <param name="displayBase">Base used when formatting the history line.</param>
        /// <returns>The numeric result.</returns>
        public long Equals(long currentValue, int displayBase)
        {
            if (!_hasPendingOperand || _pendingOperation == Operation.None)
                return currentValue;

            long left = _accumulator;
            long right = currentValue;
            long result = Compute(left, right, _pendingOperation);

            string entry = $"{NumberConverter.FromDecimal(left, displayBase)} " +
                           $"{OperationSymbol(_pendingOperation)} " +
                           $"{NumberConverter.FromDecimal(right, displayBase)} = " +
                           $"{NumberConverter.FromDecimal(result, displayBase)}  " +
                           $"({BaseName(displayBase)})";
            AddHistory(entry);

            _accumulator = result;
            _pendingOperation = Operation.None;
            _hasPendingOperand = false;
            return result;
        }

        /// <summary>
        /// Performs a unary NOT operation on the current value and records it.
        /// </summary>
        public long ApplyNot(long currentValue, int displayBase)
        {
            long result = ~currentValue;
            string entry = $"NOT {NumberConverter.FromDecimal(currentValue, displayBase)} = " +
                           $"{NumberConverter.FromDecimal(result, displayBase)}  ({BaseName(displayBase)})";
            AddHistory(entry);
            return result;
        }

        /// <summary>Clears state and history.</summary>
        public void ClearAll()
        {
            _accumulator = 0;
            _pendingOperation = Operation.None;
            _hasPendingOperand = false;
        }

        /// <summary>Clears history only.</summary>
        public void ClearHistory() => _history.Clear();

        /// <summary>Adds a free-form entry to history (used by NOT, conversions).</summary>
        public void AddHistory(string entry)
        {
            _history.Insert(0, entry);
            if (_history.Count > MaxHistory)
                _history.RemoveAt(_history.Count - 1);
        }

        private bool _hasPendingOperation() =>
            _hasPendingOperand && _pendingOperation != Operation.None;

        /// <summary>
        /// Executes a binary operation on two longs, with explicit handling of
        /// edge cases like division by zero and shift counts.
        /// </summary>
        private static long Compute(long left, long right, Operation op)
        {
            switch (op)
            {
                case Operation.Add:
                    return checked(left + right);
                case Operation.Subtract:
                    return checked(left - right);
                case Operation.Multiply:
                    return checked(left * right);
                case Operation.Divide:
                    if (right == 0)
                        throw new DivideByZeroException("Деление на ноль невозможно.");
                    return left / right;
                case Operation.And:
                    return left & right;
                case Operation.Or:
                    return left | right;
                case Operation.Xor:
                    return left ^ right;
                case Operation.ShiftLeft:
                    return left << (int)(right & 63);
                case Operation.ShiftRight:
                    return left >> (int)(right & 63);
                default:
                    return right;
            }
        }

        /// <summary>Pretty-prints an operation enum value as its symbol.</summary>
        public static string OperationSymbol(Operation op)
        {
            switch (op)
            {
                case Operation.Add: return "+";
                case Operation.Subtract: return "-";
                case Operation.Multiply: return "*";
                case Operation.Divide: return "/";
                case Operation.And: return "AND";
                case Operation.Or: return "OR";
                case Operation.Xor: return "XOR";
                case Operation.ShiftLeft: return "<<";
                case Operation.ShiftRight: return ">>";
                default: return "";
            }
        }

        private static string BaseName(int b)
        {
            switch (b)
            {
                case 2: return "BIN";
                case 8: return "OCT";
                case 10: return "DEC";
                case 16: return "HEX";
                default: return b.ToString();
            }
        }
    }
}
