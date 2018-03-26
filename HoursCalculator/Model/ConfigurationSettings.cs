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
        /// Gets or sets the timesheet filename contains.
        /// </summary>
        /// <value>
        /// The timesheet filename contains.
        /// </value>
        public string TimesheetFilenameContains { get; set; }

        /// <summary>
        /// Gets or sets the leave days taken.
        /// </summary>
        /// <value>
        /// The leave days taken.
        /// </value>
        public int LeaveDaysTaken { get; set; }

        /// <summary>
        /// Gets or sets the leave days allowed.
        /// </summary>
        /// <value>
        /// The leave days allowed.
        /// </value>
        public int LeaveDaysAllowed { get; set; }

        /// <summary>
        /// Gets or sets the sick leave days taken.
        /// </summary>
        /// <value>
        /// The sick leave days taken.
        /// </value>
        public int SickLeaveDaysTaken { get; set; }

        /// <summary>
        /// Gets or sets the sick leave days allowed.
        /// </summary>
        /// <value>
        /// The sick leave days allowed.
        /// </value>
        public int SickLeaveDaysAllowed { get; set; }

        /// <summary>
        /// Gets or sets the family leave days taken.
        /// </summary>
        /// <value>
        /// The family leave days taken.
        /// </value>
        public int FamilyLeaveDaysTaken { get; set; }

        /// <summary>
        /// Gets or sets the family leave days allowed.
        /// </summary>
        /// <value>
        /// The family leave days allowed.
        /// </value>
        public int FamilyLeaveDaysAllowed { get; set; }

        /// <summary>
        /// Gets or sets the public holidays.
        /// </summary>
        /// <value>
        /// The public holidays.
        /// </value>
        public IEnumerable<DateTime> PublicHolidays { get; set; }
    }
}
