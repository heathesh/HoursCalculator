using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HoursCalculator.Model;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace HoursCalculator
{
    class Program
    {
        /// <summary>
        /// The configuration settings
        /// </summary>
        private static ConfigurationSettings _configurationSettings;

        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            LoadConfig();

            var hoursWorked = 0;
            var hoursRequiredToWork = 0;

            foreach (var timeEntry in GetTimeEntries())
            {
                hoursWorked += timeEntry.Hours;

                if (!timeEntry.IsHoliday)
                    hoursRequiredToWork += 8;
            }

            Console.WriteLine(
                $"Leave days taken: {_configurationSettings.LeaveDaysTaken} of {_configurationSettings.LeaveDaysAllowed}");
            Console.WriteLine(
                $"Sick leave days taken: {_configurationSettings.SickLeaveDaysTaken} of {_configurationSettings.SickLeaveDaysAllowed}");
            Console.WriteLine(
                $"Family leave days taken: {_configurationSettings.FamilyLeaveDaysTaken} of {_configurationSettings.FamilyLeaveDaysAllowed}");

            UpdateHoursRequiredWithLeave(ref hoursRequiredToWork);

            Console.WriteLine();
            Console.WriteLine($"Hours worked: {hoursWorked}");
            Console.WriteLine($"Hours required: {hoursRequiredToWork}");
            Console.WriteLine($"Hours short: {hoursRequiredToWork - hoursWorked}");
            Console.WriteLine();
            Console.Write("Hit enter to continue...");
            Console.ReadLine();
        }

        /// <summary>
        /// Updates the hours required with leave.
        /// </summary>
        /// <param name="hoursRequired">The hours required.</param>
        private static void UpdateHoursRequiredWithLeave(ref int hoursRequired)
        {
            if (_configurationSettings.LeaveDaysTaken > 0)
                UpdateHoursRequiredWithLeaveDetails(ref hoursRequired, _configurationSettings.LeaveDaysTaken,
                    _configurationSettings.LeaveDaysAllowed);

            if (_configurationSettings.SickLeaveDaysTaken > 0)
                UpdateHoursRequiredWithLeaveDetails(ref hoursRequired, _configurationSettings.SickLeaveDaysTaken,
                    _configurationSettings.SickLeaveDaysAllowed);

            if (_configurationSettings.FamilyLeaveDaysTaken > 0)
                UpdateHoursRequiredWithLeaveDetails(ref hoursRequired, _configurationSettings.FamilyLeaveDaysTaken,
                    _configurationSettings.FamilyLeaveDaysAllowed);
        }

        /// <summary>
        /// Updates the hours required with leave details.
        /// </summary>
        /// <param name="hoursRequired">The hours required.</param>
        /// <param name="leaveDaysTaken">The leave days taken.</param>
        /// <param name="leaveDaysAllowed">The leave days allowed.</param>
        private static void UpdateHoursRequiredWithLeaveDetails(ref int hoursRequired, int leaveDaysTaken,
            int leaveDaysAllowed)
        {
            //remove the leave days taken from the hours required
            hoursRequired = hoursRequired - (leaveDaysTaken * 8);

            //add any extra days taken as hours required to be worked back
            if (leaveDaysTaken > leaveDaysAllowed)
            {
                var extraLeaveDays = leaveDaysTaken - leaveDaysAllowed;

                hoursRequired = hoursRequired + (extraLeaveDays * 8);
            }
        }

        /// <summary>
        /// Gets the time entries.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<TimeEntry> GetTimeEntries()
        {
            var files = Directory.GetFiles(_configurationSettings.TimesheetFolder);
            var timeEntries = new List<TimeEntry>();

            foreach (var timesheetFile in files)
            {
                if (timesheetFile.Contains(_configurationSettings.TimesheetFilenameContains))
                {
                    var file = new FileInfo(timesheetFile);
                    using (var package = new ExcelPackage(file))
                    {
                        var worksheet = package.Workbook.Worksheets[1];

                        for (var row = 9; row <= 15; row++)
                        {
                            AddTimeEntryIfRequired(GetTimeEntry(worksheet, row, 1), timeEntries);
                            AddTimeEntryIfRequired(GetTimeEntry(worksheet, row, 3), timeEntries);
                            AddTimeEntryIfRequired(GetTimeEntry(worksheet, row, 5), timeEntries);
                            AddTimeEntryIfRequired(GetTimeEntry(worksheet, row, 7), timeEntries);
                            AddTimeEntryIfRequired(GetTimeEntry(worksheet, row, 9), timeEntries);
                        }
                    }
                }
            }

            return timeEntries;
        }

        /// <summary>
        /// Adds the time entry if required.
        /// </summary>
        /// <param name="timeEntry">The time entry.</param>
        /// <param name="timeEntries">The time entries.</param>
        private static void AddTimeEntryIfRequired(TimeEntry timeEntry, ICollection<TimeEntry> timeEntries)
        {
            if (timeEntry != null)
                if (timeEntry.Hours > 0 || !timeEntry.IsHoliday)
                    timeEntries.Add(timeEntry);
        }

        /// <summary>
        /// Gets the time entry.
        /// </summary>
        /// <param name="worksheet">The worksheet.</param>
        /// <param name="row">The row.</param>
        /// <param name="dateCell">The date cell.</param>
        /// <returns></returns>
        private static TimeEntry GetTimeEntry(ExcelWorksheet worksheet, int row, int dateCell)
        {
            var entryDate =
                DateTime.FromOADate(Convert.ToInt64(Convert.ToString(worksheet.Cells[row, dateCell].Value)));

            if (!string.IsNullOrWhiteSpace(Convert.ToString(worksheet.Cells[row, dateCell + 1].Value)))
            {
                var hours = Convert.ToInt32(worksheet.Cells[row, dateCell + 1].Value);

                var timeEntry = new TimeEntry
                {
                    EntryDate = entryDate,
                    Hours = hours,
                    IsHoliday = CheckIfDateIsHoliday(entryDate)
                };

                return timeEntry;
            }

            return null;
        }

        /// <summary>
        /// Checks if date is holiday.
        /// </summary>
        /// <param name="entryDate">The entry date.</param>
        /// <returns></returns>
        private static bool CheckIfDateIsHoliday(DateTime entryDate)
        {
            return _configurationSettings.PublicHolidays.Any(_ => _.Date == entryDate.Date) ||
                   entryDate.DayOfWeek == DayOfWeek.Saturday || entryDate.DayOfWeek == DayOfWeek.Sunday;
        }

        /// <summary>
        /// Loads the configuration.
        /// </summary>
        private static void LoadConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json");

            var configuration = builder.Build();

            _configurationSettings = new ConfigurationSettings
            {
                TimesheetFolder = configuration["TimesheetFolder"],
                TimesheetFilenameContains = configuration["TimesheetFilenameContains"],
                LeaveDaysTaken = Convert.ToInt32(configuration["LeaveDaysTaken"]),
                LeaveDaysAllowed = Convert.ToInt32(configuration["LeaveDaysAllowed"]),
                SickLeaveDaysTaken = Convert.ToInt32(configuration["SickLeaveDaysTaken"]),
                SickLeaveDaysAllowed = Convert.ToInt32(configuration["SickLeaveDaysAllowed"]),
                FamilyLeaveDaysTaken = Convert.ToInt32(configuration["FamilyLeaveDaysTaken"]),
                FamilyLeaveDaysAllowed = Convert.ToInt32(configuration["FamilyLeaveDaysAllowed"])
            };

            var publicHolidaysConfigurationSection = configuration.GetSection("PublicHolidays");
            var publicHolidays = new List<DateTime>();

            foreach (var publicHoliday in publicHolidaysConfigurationSection.AsEnumerable())
            {
                if (!string.IsNullOrWhiteSpace(publicHoliday.Value))
                {
                    var publicHolidayDate = GetConfiguredPublicHoliday(publicHoliday.Value);
                    publicHolidays.Add(publicHolidayDate);

                    if (publicHolidayDate.DayOfWeek == DayOfWeek.Sunday)
                        publicHolidays.Add(publicHolidayDate.AddDays(1));
                }
            }

            _configurationSettings.PublicHolidays = publicHolidays;
        }

        /// <summary>
        /// Gets the configured public holiday.
        /// </summary>
        /// <param name="publicHoliday">The public holiday.</param>
        /// <returns></returns>
        private static DateTime GetConfiguredPublicHoliday(string publicHoliday)
        {
            var dateParts = publicHoliday.Split('/');
            return new DateTime(Convert.ToInt32(dateParts[0]), Convert.ToInt32(dateParts[1]),
                Convert.ToInt32(dateParts[2]));
        }
    }
}
