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

        string Units { get; set; }

        /// <summary>
        /// Definitions for pump & valve channels
        /// </summary>
        private enum Output {PumpChannel = 0, ValveChannel = 1};

        public MainWindow()
        {
            Units = Properties.Settings.Default.flowUnits;
            InitializeComponent();
            myTimer= new DispatcherTimer();
            myTimer.Interval= new TimeSpan(0,0,0,0,500);
            myTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispenseVolume = -1;

            // Add com ports
            MenuItem mi = menuSetup.Items[0] as MenuItem;
            string[] theSerialPortNames = System.IO.Ports.SerialPort.GetPortNames();
            foreach (string name in theSerialPortNames)
            {
                //mi.Items.Add(name);
                MenuItem newMi = new MenuItem();
                newMi.Header = name;
                newMi.Name = name;
                mi.Items.Add(newMi);
            }

            // Shutdown if no ports found
            if (theSerialPortNames.Length == 0)
            {
                MessageBox.Show("This device needs a serial connection", "No serial ports found");
                //Application.Current.Shutdown(); 
            }

            // Disable controls
            buttonReset.IsEnabled = false;
            buttonDispLimit.IsEnabled = false;
            buttonPumpOff.IsEnabled = false;
            buttonPumpOn.IsEnabled = false;
            buttonValveOff.IsEnabled = false;
            buttonValveOn.IsEnabled = false;
 
            // Attempt reconnection to previous port
            if (Properties.Settings.Default.port != "")
            {
                if (connect(Properties.Settings.Default.port))
                {                    
                    // Enable buttons
                    buttonReset.IsEnabled = true;
                    buttonDispLimit.IsEnabled = true;
                    buttonPumpOff.IsEnabled = true;
                    buttonPumpOn.IsEnabled = true;
                    buttonValveOff.IsEnabled = true;
                    buttonValveOn.IsEnabled = true;
                }
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
                this.control.setOutput((int)Output.ValveChannel, true);
                // Pump on
                this.control.setOutput((int)Output.PumpChannel, true);
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
            if (connect(mi.Name))
            {
                // Save port name if successful
                Properties.Settings.Default.port = mi.Name;
                Properties.Settings.Default.Save();
            }
        }

        /// <summary>
        /// Attempts to open COM port. Saves port if successful
        /// </summary>
        /// <param name="port">COMn</param>
        /// <returns>true if connection to hardware successful</returns>
        private bool connect(string port)
        {
            string status = "Dispense Controller: no connection";
            bool connection = false;

            this.control = new Prt232Control(port, 19200);
            if (true == this.control.connect())
            {
                // Port is open, let's look for a response
                if (-1 != this.control.readCount())
                {
                    Properties.Settings.Default.port = port;
                    myTimer.Start();
                    status = string.Format("Dispense Controller: connected on {0}", port);

                    // Enable buttons
                    buttonReset.IsEnabled = true;
                    buttonDispLimit.IsEnabled = true;
                    buttonPumpOff.IsEnabled = true;
                    buttonPumpOn.IsEnabled = true;
                    buttonValveOff.IsEnabled = true;
                    buttonValveOn.IsEnabled = true;
                    connection = true;
                }
            }
            this.Title = status;
            return connection;
        }
    }
}
