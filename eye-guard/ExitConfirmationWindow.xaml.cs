using System;
using System.Windows;

namespace eye_guard
{
    public partial class ExitConfirmationWindow : Window
    {
        public enum ExitOption
        {
            ExitCompletely,
            TemporaryExit10Minutes,
            TemporaryExit15Minutes,
            TemporaryExit20Minutes,
            TemporaryExit1Hour
        }
        
        public event EventHandler<ExitOption> ExitConfirmed;
        
        public ExitConfirmationWindow()
        {
            InitializeComponent();
        }
        
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            ExitOption selectedOption = GetSelectedExitOption();
            ExitConfirmed?.Invoke(this, selectedOption);
            Close();
        }
        
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        
        private ExitOption GetSelectedExitOption()
        {
            int selectedIndex = ExitOptionComboBox.SelectedIndex;
            switch (selectedIndex)
            {
                case 0:
                    return ExitOption.ExitCompletely;
                case 1:
                    return ExitOption.TemporaryExit10Minutes;
                case 2:
                    return ExitOption.TemporaryExit15Minutes;
                case 3:
                    return ExitOption.TemporaryExit20Minutes;
                case 4:
                    return ExitOption.TemporaryExit1Hour;
                default:
                    return ExitOption.ExitCompletely;
            }
        }
    }
}