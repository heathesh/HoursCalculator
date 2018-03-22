using System;

namespace HoursCalculator.Model
{
    /// <summary>
    /// Time entry entity
    /// </summary>
    public class TimeEntry
    {
        /// <summary>
        /// Gets or sets the entry date.
        /// </summary>
        /// <value>
        /// The entry date.
        /// </value>
        public DateTime EntryDate { get; set; }

        /// <summary>
        /// Gets or sets the hours.
        /// </summary>
        /// <value>
        /// The hours.
        /// </value>
        public int Hours { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is holiday.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is holiday; otherwise, <c>false</c>.
        /// </value>
        public bool IsHoliday { get; set; }
    }
}
