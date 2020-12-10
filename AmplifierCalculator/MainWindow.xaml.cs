using System;
using System.Windows;
using System.Windows.Controls;

namespace AmplifierCalculator {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        readonly SolutionFlowController solver = new SolutionFlowController();
        readonly TextBox ni_rGain = new TextBox();

        public MainWindow() => InitializeComponent();

        void InvBaseVoltageChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(uOutEff, Math.Sqrt(2), uOut, sender);
            solver.SolveMultiplication(uInEff, Math.Sqrt(2), uIn, sender);
            solver.SolveDivision(uOut, uIn, gain, sender);
            solver.SolveDivision(r2, r1, gain, gain, -1);
            solver.EndEditing(sender);
        }

        void InvEffVoltageChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(uOutEff, Math.Sqrt(2), uOut, sender);
            solver.SolveMultiplication(uInEff, Math.Sqrt(2), uIn, sender);
            solver.EndEditing(sender);
        }

        void InvBaseResistanceChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveDivision(r2, r1, gain, sender, -1);
            solver.SolveDivision(uOut, uIn, gain, gain);
            solver.EndEditing(sender);
        }

        void InvBaseGainChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveDivision(uOut, uIn, gain, sender);
            solver.SolveDivision(r2, r1, gain, sender, -1);
            solver.SolveGainForVoltage(gain, gainDb, sender, -1);
            solver.EndEditing(sender);
        }

        void InvBaseGainDbChanged(object sender, TextChangedEventArgs e) => solver.DecibelFieldForVoltage(gain, gainDb, -1);

        void InvBaseReset(object sender, RoutedEventArgs e) {
            uOut.Text = uOutEff.Text = uIn.Text = uInEff.Text = r2.Text = r1.Text = gain.Text = gainDb.Text = string.Empty;
            InvLowpassReset(sender, e);
        }

        void InvCapacitorChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(cmF, .001, cF, sender);
            solver.SolveMultiplication(cuF, .001, cmF, sender);
            solver.SolveMultiplication(cnF, .001, cuF, sender);
            if (solver.Parse(cF, out double _c) && solver.Parse(r2, out double _r2))
                solver.Apply(omega2, 1 / (_r2 * _c));
            RecountInvLowpass();
            solver.EndEditing(sender);
        }

        void InvOmegaChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            RecountInvLowpass();
            solver.EndEditing(sender);
        }

        void InvPeakInputVoltageChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(uInPEff, Math.Sqrt(2), uInP, sender);
            RecountInvLowpass();
            solver.EndEditing(sender);
        }

        void RecountInvLowpass() {
            if (solver.Parse(omega1, out double o1) && solver.Parse(omega2, out double o2)) {
                if (solver.Parse(r1, out double _r1) && solver.Parse(r2, out double _r2)) {
                    double _uOutP = _r2 / _r1 / Math.Sqrt(1 + (o1 * o1) / (o2 * o2));
                    if (solver.Parse(uInP, out double _uInP)) {
                        solver.Apply(uOutP, _uInP * _uOutP);
                        solver.SolveMultiplication(uOutPEff, Math.Sqrt(2), uOutP, uOutP);
                    }
                    solver.Apply(uAmpP, _uOutP);
                    solver.Apply(uAmpP_dB, 20 * Math.Log10(_uOutP));
                }
                solver.Apply(angle, -180 - Math.Atan(o1 / o2) * 180 / Math.PI);
            }
        }

        void InvLowpassReset(object sender, RoutedEventArgs e) => uOutP.Text = uOutPEff.Text = angle.Text = omega1.Text = omega2.Text =
            cF.Text = cmF.Text = cuF.Text = cnF.Text = uInP.Text = ni_uInPEff.Text = string.Empty;

        void NonInvBaseVoltageChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(ni_uOutEff, Math.Sqrt(2), ni_uOut, sender);
            solver.SolveMultiplication(ni_uInEff, Math.Sqrt(2), ni_uIn, sender);
            solver.SolveDivision(ni_uOut, ni_uIn, ni_gain, sender);
            solver.SolveAddition(ni_rGain, 1, ni_gain, ni_gain);
            solver.SolveDivision(ni_r2, ni_r1, ni_rGain, ni_rGain);
            solver.EndEditing(sender);
        }

        void NonInvEffVoltageChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(ni_uOutEff, Math.Sqrt(2), ni_uOut, sender);
            solver.SolveMultiplication(ni_uInEff, Math.Sqrt(2), ni_uIn, sender);
            solver.EndEditing(sender);
        }

        void NonInvBaseResistanceChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveDivision(ni_r2, ni_r1, ni_rGain, sender);
            solver.SolveAddition(ni_rGain, 1, ni_gain, ni_rGain);
            solver.SolveDivision(ni_uOut, ni_uIn, ni_gain, ni_gain);
            solver.EndEditing(sender);
        }

        void NonInvBaseGainChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveDivision(ni_uOut, ni_uIn, ni_gain, sender);
            solver.SolveAddition(ni_rGain, 1, ni_gain, sender);
            solver.SolveDivision(ni_r2, ni_r1, ni_rGain, ni_gain);
            solver.SolveGainForVoltage(ni_gain, ni_gainDb, sender);
            solver.EndEditing(sender);
        }

        void NonInvBaseGainDbChanged(object sender, TextChangedEventArgs e) => solver.DecibelFieldForVoltage(gain, gainDb);

        void NonInvBaseReset(object sender, RoutedEventArgs e) {
            ni_uOut.Text = ni_uOutEff.Text = ni_uIn.Text = ni_uInEff.Text = ni_r2.Text = ni_r1.Text =
            ni_rGain.Text = ni_gain.Text = ni_gainDb.Text = string.Empty;
            NonInvLowpassReset(sender, e);
        }

        void NonInvCapacitorChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(ni_cmF, .001, ni_cF, sender);
            solver.SolveMultiplication(ni_cuF, .001, ni_cmF, sender);
            solver.SolveMultiplication(ni_cnF, .001, ni_cuF, sender);
            if (solver.Parse(ni_cF, out double _c) && solver.Parse(ni_r2, out double _r2)) {
                solver.Apply(ni_omega3, 1 / (_r2 * _c));
                if (solver.Parse(ni_r1, out double _r1))
                    solver.Apply(ni_omega2, (_r1 + _r2) / (_r1 * _r2 * _c));
            }
            RecountNonInvLowpass();
            solver.EndEditing(sender);
        }

        void NonInvOmegaChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            RecountNonInvLowpass();
            solver.EndEditing(sender);
        }

        void NonInvPeakInputVoltageChanged(object sender, TextChangedEventArgs e) {
            solver.StartEditing(sender);
            solver.SolveMultiplication(ni_uInPEff, Math.Sqrt(2), ni_uInP, sender);
            RecountNonInvLowpass();
            solver.EndEditing(sender);
        }

        void RecountNonInvLowpass() {
            if (solver.Parse(ni_omega1, out double o1) && solver.Parse(ni_omega2, out double o2) && solver.Parse(ni_omega3, out double o3)) {
                if (solver.Parse(ni_r1, out double _r1) && solver.Parse(ni_r2, out double _r2)) {
                    double _uOutP = (_r1 + _r2) / _r1 * Math.Sqrt(1 + (o1 * o1) / (o2 * o2)) / Math.Sqrt(1 + (o1 * o1) / (o3 * o3));
                    if (solver.Parse(ni_uInP, out double _uInP)) {
                        solver.Apply(ni_uOutP, _uInP * _uOutP);
                        solver.SolveMultiplication(ni_uOutPEff, Math.Sqrt(2), ni_uOutP, ni_uOutP);
                    }
                    solver.Apply(ni_uAmpP, _uOutP);
                    solver.Apply(ni_uAmpP_dB, 20 * Math.Log10(_uOutP));
                }
                solver.Apply(ni_angle, (Math.Atan(o1 / o2) - Math.Atan(o1 / o3)) * 180 / Math.PI);
            }
        }

        void NonInvLowpassReset(object sender, RoutedEventArgs e) => ni_uOutP.Text = ni_uOutPEff.Text = ni_angle.Text =
            ni_omega1.Text = ni_omega2.Text = ni_omega3.Text = ni_cF.Text = ni_cmF.Text = ni_cuF.Text = ni_cnF.Text =
            ni_uInP.Text = ni_uInPEff.Text = string.Empty;
    }
}