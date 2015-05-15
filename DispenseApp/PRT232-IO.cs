using System;
using System.Text;
using System.Threading;
using System.IO;
using System.IO.Ports;

namespace Prt232Io
{
    /// <summary>
    /// Provides an interface to PRT232 controller board
    /// All methods may throw an IOException on failure
    /// </summary>
    class Prt232Control
    {
        bool connected;
        SerialPort port;
        String name;
        int rate;
        IOException lastEx;
        byte outputState;

        private Prt232Control()
        {
        }

        public Prt232Control(String n, int r)
        {
            this.name = n;
            this.rate = r;
            this.connected= false;
            this.outputState = 0;
        }


        /// <summary>
        /// Connect to named COM port
        /// </summary>
        /// <returns>true if connect successful</returns>
        public bool connect()
        {
            try
            {
                if (this.port != null)
                {
                    this.port.Dispose();
                }

                this.port = new SerialPort(this.name, this.rate);
                this.port.Handshake = Handshake.None;
                this.port.DataBits = 8;
                this.port.StopBits = StopBits.One;
                this.port.Parity = Parity.None;
                this.port.Encoding = new ASCIIEncoding();
                this.port.Open();
                this.connected = true;
                String dispose= this.port.ReadExisting();
            }
            catch (IOException e)
            {
                this.lastEx = e;
            }
            return this.connected;
        }

        /// <summary>
        /// Reads the current count
        /// </summary>
        /// <returns>Actual count or -1 to indicate error</returns>
        public int readCount()
        {
            int count = -1;
            if (this.connected == true)
            {
                try
                {
                    this.port.Write("c\r");
                    Thread.Sleep(50);
                    String returned = this.port.ReadExisting();
                    if (false == Int32.TryParse(returned, out count))
                    {
                        count = -1;
                    }
                }
                catch (IOException e)
                {
                    this.lastEx = e;
                }
            }
           return count;
        }

        /// <summary>
        /// Reads the switch inputs.
        /// true = switch closed
        /// false = switch open
        /// </summary>
        /// <returns>array of 3 boolean values</returns>
        public bool[] readSwitch()
        {
            bool[] state = new bool[3];
            return state;
        }

        /// <summary>
        /// Resets the count to zero
        /// </summary>
        /// <returns>true if successful</returns>
        public bool resetCount()
        {
            bool success = false;
            if (this.connected == true)
            {
                try
                {
                    this.port.Write("z\r");
                    success = true;
                }
                catch (IOException e)
                {
                    this.lastEx = e;
                }
            }
            return success;
        }

        /// <summary>
        /// Sets an output channel 0-7 to a boolean on/off state
        /// </summary>
        /// <param name="ch">Output channel 0-7 to be se</param>
        /// <param name="state">true=ON, false=OFF</param>
        /// <returns>true if successful</returns>
        public bool setOutput(int ch, bool state)
        {
            bool success = false;
            if (this.connected == true)
            {
                try
                {
                    int val = 0;
                    if (state == true)
                    {
                        this.outputState |= (byte)(1 << ch);
                    }
                    else
                    {
                        this.outputState &= (byte)~(1 << ch);
                    }
                    val = this.outputState;
                    this.port.Write(String.Format("o{0}\r", val));
                    success = true;
                }
                catch (IOException e)
                {
                    this.lastEx = e;
                }
            }
            return success;
        }        
    }
}
