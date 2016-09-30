using System;
using System.Collections.Generic;
using System.ComponentModel;

using Xamarin.Forms;

namespace AppLimpia
{
    /// <summary>
    /// The base <see cref="INotifyPropertyChanged"/> implementation for view-model classes.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// A value indicating whether the current view model is busy processing data.
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the current view model is busy processing data.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.SetProperty(ref this.isBusy, value, nameof(this.IsBusy));
            }
        }

        /// <summary>
        /// Gets or sets the navigation manager for the current view model.
        /// </summary>
        internal INavigation Navigation { get; set; }

        /// <summary>
        /// Notifies of a property changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that was changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            // Raise the property changed event
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the new value of the property.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="backingField">The backing field to hold the property value.</param>
        /// <param name="value">The new value of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <returns>
        ///   <c>true</c> - if the property value was changed;
        ///   <c>false</c> - otherwise.
        /// </returns>
        protected bool SetProperty<T>(ref T backingField, T value, string propertyName)
        {
            // If the value was not changed
            if (EqualityComparer<T>.Default.Equals(backingField, value))
            {
                return false;
            }

            // Set the new value
            backingField = value;

            // Raise the property changed event
            this.OnPropertyChanged(propertyName);
            return true;
        }
    }
}
