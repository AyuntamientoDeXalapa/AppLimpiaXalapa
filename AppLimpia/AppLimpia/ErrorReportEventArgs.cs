using System;

namespace AppLimpia
{
    /// <summary>
    /// Represents the class for the exception reporting.
    /// </summary>
    public sealed class ErrorReportEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorReportEventArgs"/> class for the provided exception.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/> to be reported. </param>
        public ErrorReportEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> associated with the current report.
        /// </summary>
        public Exception Exception { get; }
    }
}
