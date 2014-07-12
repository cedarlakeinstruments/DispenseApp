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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;
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
        enum calibration {liter= 450};
        double rate;
        DateTime lastCalc;
        double lastVolume;
        double dispenseVolume;

        public MainWindow()
        {
            InitializeComponent();
            myTimer= new DispatcherTimer();
            myTimer.Interval= new TimeSpan(0,0,0,0,500);
            myTimer.IsEnabled= true;
            myTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispenseVolume = -1;

            this.control = new Prt232Control("COM1", 19200);
            if (true == this.control.connect())
            {
                myTimer.Start();
                this.lastCalc = DateTime.Now;
                this.lastVolume = 0;
            }
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            double liters= this.control.readCount()/450.0;
            this.labelCount.Content = String.Format("{0:0.00}", liters);
            if ((DateTime.Now - this.lastCalc).Seconds >= 6)
            {
                this.lastCalc = DateTime.Now;
                rate = (liters - this.lastVolume) * 10;
                this.lastVolume = liters;
                if (rate >= 0)
                {
                    labelRate.Content = String.Format("{0:0.00}", rate);
                }
            }
            if (this.dispenseVolume > 0 && liters > this.dispenseVolume)
            {
                // Pump off
                this.control.setOutput(7, false);
                // Valve closed
                this.control.setOutput(5, false);
                this.labelPumpStat.Background = Brushes.Black;
            }
        }

        private void buttonToggle_Click(object sender, RoutedEventArgs e)
        {
            this.dispenseVolume = -1;
            this.control.setOutput(7, true);
            this.labelPumpStat.Background = Brushes.Red;
        }

        private void buttonOff_Click(object sender, RoutedEventArgs e)
        {
            this.dispenseVolume = -1;
            this.control.setOutput(7, false);
            this.labelPumpStat.Background = Brushes.Black;
        }

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

        private void buttonValve_Click(object sender, RoutedEventArgs e)
        {
            this.control.setOutput(5, true);
        }

        private void buttonValveOff_Click(object sender, RoutedEventArgs e)
        {
            this.control.setOutput(5, false);
        }
    }
}
