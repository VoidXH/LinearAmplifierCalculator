using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Controls;

namespace AmplifierCalculator {
    /// <summary>
    /// Helps solving equations designed with <see cref="TextBox"/>es.
    /// </summary>
    public class SolutionFlowController {
        bool editing = false;
        readonly HashSet<object> edited = new HashSet<object>();
        object firstSender;

        /// <summary>
        /// Has to be called before editing any control.
        /// </summary>
        public void StartEditing(object sender) {
            if (!editing) {
                editing = true;
                firstSender = sender;
            }
            if (!edited.Contains(sender))
                edited.Add(sender);
        }

        /// <summary>
        /// Has to be called after editing any control.
        /// </summary>
        public void EndEditing(object sender) {
            if (firstSender == sender) {
                editing = false;
                edited.Clear();
            }
        }

        /// <summary>
        /// Get a number from a <see cref="TextBox"/>, if it's valid.
        /// </summary>
        public bool Parse(TextBox from, out double value) {
            if (from != null)
                return double.TryParse(from.Text.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out value);
            else {
                value = double.NaN;
                return false;
            }
        }

        /// <summary>
        /// Get a number from a <see cref="TextBox"/>, if it's valid and not 0.
        /// </summary>
        public bool ParseDivisor(TextBox from, out double value) {
            bool result = Parse(from, out value);
            return result && value != 0;
        }

        /// <summary>
        /// Set the value of a <see cref="TextBox"/> and disable further editing of it in the current cycle.
        /// </summary>
        public void Apply(TextBox to, double value) {
            if (to != null && !edited.Contains(to)) {
                to.Text = value.ToString("0.000000000").TrimEnd('0').TrimEnd(',', '.');
                edited.Add(to);
            }
        }

        /// <summary>
        /// Solve 3 fields which form an addition.
        /// </summary>
        public bool SolveAddition(TextBox a, TextBox b, TextBox result, object sender) {
            if (sender == result && Parse(result, out double res)) {
                if (Parse(a, out double _a)) {
                    Apply(b, res - _a);
                    return true;
                }
                if (Parse(b, out double _b)) {
                    Apply(a, res - _b);
                    return true;
                }
            } else {
                if (Parse(result, out double _res)) {
                    if (sender == a && Parse(a, out double _a)) {
                        Apply(b, _res - _a);
                        return true;
                    }
                    if (sender == b && Parse(b, out double _b)) {
                        Apply(a, _res - _b);
                        return true;
                    }
                } else if (Parse(a, out double _a) && Parse(b, out double _b)) {
                    Apply(result, _a + _b);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Solve 2 fields and a constant which form an addition.
        /// </summary>
        public bool SolveAddition(TextBox a, double b, TextBox result, object sender) {
            if (sender == result && Parse(result, out double res)) {
                Apply(a, res - b);
                return true;
            } else if (sender == a && Parse(a, out double _a)) {
                Apply(result, _a + b);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Solve 3 fields which form a subtraction.
        /// </summary>
        public bool SolveSubtraction(TextBox a, TextBox b, TextBox result, object sender) => SolveAddition(b, result, a, sender);

        bool SolveDivisionForNumerator(TextBox numerator, TextBox denumerator, TextBox result, double resultMultiplier) {
            if (Parse(numerator, out double num)) {
                if (ParseDivisor(denumerator, out double den)) {
                    Apply(result, num / (den * resultMultiplier));
                    return true;
                } else if (ParseDivisor(result, out double res)) {
                    Apply(denumerator, num / (res * resultMultiplier));
                    return true;
                }
            }
            return false;
        }

        bool SolveDivisionForNumerator(TextBox numerator, TextBox denumerator, double result, double resultMultiplier) {
            if (Parse(numerator, out double num)) {
                Apply(denumerator, num / (result * resultMultiplier));
                return true;
            }
            return false;
        }

        bool SolveDivisionForDenumerator(TextBox numerator, TextBox denumerator, TextBox result, double resultMultiplier) {
            if (Parse(denumerator, out double den)) {
                if (Parse(numerator, out double num) && den != 0) {
                    Apply(result, num / (den * resultMultiplier));
                    return true;
                } else if (Parse(result, out double res)) {
                    Apply(numerator, (res * resultMultiplier) * den);
                    return true;
                }
            }
            return false;
        }

        bool SolveDivisionForDenumerator(TextBox numerator, TextBox denumerator, double result, double resultMultiplier) {
            if (Parse(denumerator, out double den)) {
                Apply(numerator, (result * resultMultiplier) * den);
                return true;
            }
            return false;
        }

        bool SolveDivisionForResult(TextBox numerator, TextBox denumerator, TextBox result, double resultMultiplier) {
            if (Parse(result, out double res)) {
                if (Parse(numerator, out double num) && res != 0) {
                    Apply(denumerator, num / (res * resultMultiplier));
                    return true;
                } else if (ParseDivisor(denumerator, out double den)) {
                    Apply(numerator, res / resultMultiplier * den);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Solve 3 fields which form a multiplication.
        /// </summary>
        public bool SolveMultiplication(TextBox a, TextBox b, TextBox result, object sender) => SolveDivision(result, a, b, sender);

        /// <summary>
        /// Solve 2 fields and a constant which form a multiplication.
        /// </summary>
        public bool SolveMultiplication(TextBox a, double b, TextBox result, object sender) => SolveDivision(result, a, b, sender);

        /// <summary>
        /// Solve 3 fields which form a division.
        /// </summary>
        public bool SolveDivision(TextBox numerator, TextBox denumerator, TextBox result, object sender, double resultMultiplier = 1) {
            if (sender == numerator)
                return SolveDivisionForNumerator(numerator, denumerator, result, resultMultiplier);
            if (sender == denumerator)
                return SolveDivisionForDenumerator(numerator, denumerator, result, resultMultiplier);
            if (sender == result)
                return SolveDivisionForResult(numerator, denumerator, result, resultMultiplier);
            return false;
        }

        /// <summary>
        /// Solve 2 fields and a constant which form a division.
        /// </summary>
        public bool SolveDivision(TextBox numerator, TextBox denumerator, double result, object sender, double resultMultiplier = 1) {
            if (sender == numerator)
                return SolveDivisionForNumerator(numerator, denumerator, result, resultMultiplier);
            if (sender == denumerator)
                return SolveDivisionForDenumerator(numerator, denumerator, result, resultMultiplier);
            return false;
        }

        /// <summary>
        /// Solves 2 fields which form a gain - dB relation for a voltage gain.
        /// </summary>
        public bool SolveGainForVoltage(TextBox gain, TextBox gainDb, object sender, double gainMultiplier = 1) {
            if (sender == gain && Parse(gain, out double _gain)) {
                Apply(gainDb, 20 * Math.Log10(_gain * gainMultiplier));
                return true;
            }
            if (sender == gainDb && Parse(gainDb, out double db)) {
                Apply(gain, Math.Pow(10, db * .05) / gainMultiplier);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Completely handles a gain and dB gain field pair.
        /// </summary>
        public void DecibelFieldForVoltage(TextBox gain, object gainDb, double gainMultiplier = 1) {
            StartEditing(gainDb);
            SolveGainForVoltage(gain, (TextBox)gainDb, gainDb, gainMultiplier);
            EndEditing(gainDb);
        }
    }
}