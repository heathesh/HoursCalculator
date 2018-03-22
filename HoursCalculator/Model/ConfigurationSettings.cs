using System;
using System.Collections.Generic;

namespace HoursCalculator.Model
{
    /// <summary>
    /// Configuration settings
    /// </summary>
    public class ConfigurationSettings
    {
        /// <summary>
        /// Gets or sets the timesheet folder.
        /// </summary>
        /// <value>
        /// The timesheet folder.
        /// </value>
        public string TimesheetFolder { get; set; }

        /// <summary>
        /// Gets or sets the timesheet filename prefix.
        /// </summary>
        /// <value>
        /// The timesheet filename prefix.
        /// </value>
        public string TimesheetFilenamePrefix { get; set; }

        /// <summary>
        /// Gets or sets the public holidays.
        /// </summary>
        /// <value>
        /// The public holidays.
        /// </value>
        public IEnumerable<DateTime> PublicHolidays { get; set; }
    }
}
