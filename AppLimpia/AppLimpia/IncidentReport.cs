using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AppLimpia
{
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
        private string status;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncidentReport"/> class.
        /// </summary>
        /// <param name="id">The identifier of the current incident report.</param>
        public IncidentReport(string id)
        {
            this.Id = id;
            this.dropPoint = string.Empty;
            this.type = string.Empty;
            this.status = string.Empty;
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
                if (!this.dropPoint.Equals(value))
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
                if (!this.type.Equals(value))
                {
                    this.type = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current incident report status.
        /// </summary>
        public string Status
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
