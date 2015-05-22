using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DispenseApp
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();
            // Load values from settings storage
            UnitsConv.Text = Properties.Settings.Default.scale.ToString();
            UnitsName.Text = Properties.Settings.Default.UnitsDesc;
            PumpDesc.Text = Properties.Settings.Default.PumpDesc;
            ValveDesc.Text = Properties.Settings.Default.ValveDesc;
        }

        /// <summary>
        /// Save changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double scale = 0;
            if (double.TryParse(UnitsConv.Text, out scale))
            {
                Properties.Settings.Default.scale = scale;
            }
            Properties.Settings.Default.UnitsDesc = UnitsName.Text;
            Properties.Settings.Default.PumpDesc = PumpDesc.Text;
            Properties.Settings.Default.ValveDesc = ValveDesc.Text;
            Properties.Settings.Default.Save();
            Close();
        }
    }
}
