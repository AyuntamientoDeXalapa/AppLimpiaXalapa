using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace AppLimpia
{
    /// <summary>
    /// The incident report status.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by Parse method")]
    public enum IncidentReportStatus
    {
        /// <summary>
        /// The incident report is received by the server.
        /// </summary>
        Received = 1,

        /// <summary>
        /// The incident report is in process of resolution.
        /// </summary>
        InProcess = 2,

        /// <summary>
        /// The incident report is resolved.
        /// </summary>
        Completed = 3
    }

    /// <summary>
    /// The incident report.
    /// </summary>
    public class IncidentReport : INotifyPropertyChanged
    {
        /// <summary>
        /// The date and time of the current incident report.
        /// </summary>
        private DateTime date;

        /// <summary>
        /// The name of the drop point for the current incident report.
        /// </summary>
        private string dropPoint;

        /// <summary>
        /// The type of the current incident report.
        /// </summary>
        private string type;

        /// <summary>
        /// The current incident report status.
        /// </summary>
        private IncidentReportStatus status;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentReport"/> class.
        /// </summary>
        /// <param name="id">The identifier of the current incident report.</param>
        public IncidentReport(string id)
        {
            this.Id = id;
            this.status = IncidentReportStatus.Received;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the identifier of the current incident report.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets or sets the date and time of the current incident report.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return this.date;
            }

            set
            {
                if (!this.date.Equals(value))
                {
                    this.date = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets name of the drop point for the current incident report.
        /// </summary>
        public string DropPoint
        {
            get
            {
                return this.dropPoint;
            }

            set
            {
                if (!string.Equals(this.dropPoint, value))
                {
                    this.dropPoint = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the current incident report.
        /// </summary>
        public string Type
        {
            get
            {
                return this.type;
            }

            set
            {
                if (!string.Equals(this.type, value))
                {
                    this.type = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current incident report status.
        /// </summary>
        public IncidentReportStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                if (!this.status.Equals(value))
                {
                    this.status = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Notifies of a property changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
