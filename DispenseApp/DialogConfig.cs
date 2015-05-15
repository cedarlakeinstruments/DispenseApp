using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DispenseApp
{
    public static class DialogConfig
    {
        /// <summary>
        /// Units used for display
        /// </summary>
        public string Units { get; set; }

        /// <summary>
        /// Name of the count groupbox
        /// </summary>
        public string CountGroupName { get; set; }

        /// <summary>
        /// Name of the count label
        /// </summary>
        public string CountLabelName { get; set; }

        /// <summary>
        /// Name of the output that is triggered when threshold reached
        /// </summary>
        public string OutputTriggerName { get; set; }

    }
}
