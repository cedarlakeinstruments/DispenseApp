using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Prt232Io;

namespace DispenseApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //PRT232_IO controller;
        Prt232Control control;
        DispatcherTimer myTimer;
        double dispenseVolume;

        /// <summary>
        /// Definitions for pump & valve channels
        /// </summary>
        private enum Output {PumpChannel = 0, ValveChannel = 1};

        public MainWindow()
        {
            InitializeComponent();
            myTimer= new DispatcherTimer();
            myTimer.Interval= new TimeSpan(0,0,0,0,500);
            myTimer.IsEnabled= true;
            myTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispenseVolume = -1;
            labelUnits.Content = Properties.Settings.Default.flowUnits;
            this.control = new Prt232Control(Properties.Settings.Default.port, 19200);

            // Add com ports
            MenuItem mi = menuSetup.Items[0] as MenuItem;
            string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string name in theSerialPortNames)
            {
                mi.Items.Add(name);
            }

            if (true == this.control.connect())
            {
                myTimer.Start();
            }
        }

        /// <summary>
        /// Periodic timer to update display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            double liters= this.control.readCount() * Properties.Settings.Default.scale;
            this.labelCount.Content = String.Format("{0:0.00}", liters);
        }

        /// <summary>
        /// Turn pump on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonToggle_Click(object sender, RoutedEventArgs e)
        {
            this.control.setOutput((int)Output.PumpChannel, true);
            this.labelPumpStat.Background = Brushes.Red;
        }

        /// <summary>
        /// Turn pump off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOff_Click(object sender, RoutedEventArgs e)
        {
            this.control.setOutput((int)Output.PumpChannel, false);
            this.labelPumpStat.Background = Brushes.Black;
        }

        /// <summary>
        /// Reset the pulse count
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReset_Click(object sender, RoutedEventArgs e)
        {
            this.control.resetCount();
        }

        private void buttonDisp_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(textBoxDispense.Text, out this.dispenseVolume))
            {
                this.control.resetCount();
                // Valve open
                this.control.setOutput(5, true);
                // Pump on
                this.control.setOutput(7, true);
                this.labelPumpStat.Background = Brushes.Red;
            }
        }

        /// <summary>
        /// Turn valve on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonValve_Click(object sender, RoutedEventArgs e)
        {
            this.control.setOutput((int)Output.ValveChannel, true);
        }

        /// <summary>
        /// Turn valve off
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonValveOff_Click(object sender, RoutedEventArgs e)
        {
            this.control.setOutput((int)Output.ValveChannel, false);
        }
        
        /// <summary>
        /// Handler for the main menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem mi = e.Source as MenuItem;
            if (mi.Name == "menuItemPort")
            {
                foreach (MenuItem m in mi.Items)
                {
                    bool b = m.IsPressed;
                    if (b)
                        break;
                }
            }
        }
    }
}
