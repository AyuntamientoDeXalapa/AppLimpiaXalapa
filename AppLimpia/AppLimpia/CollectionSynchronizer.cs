using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AppLimpia
{
    /// <summary>
    /// The collection synchronizer class.
    /// </summary>
    /// <typeparam name="T">The generic type of the collection.</typeparam>
    public sealed class CollectionSynchronizer<T> : IDisposable
    {
        /// <summary>
        /// The source collection to synchronize.
        /// </summary>
        private readonly ObservableCollection<T> source;

        /// <summary>
        /// The destination collection to synchronize.
        /// </summary>
        private readonly IList<T> destination;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionSynchronizer{T}"/> class.
        /// </summary>
        /// <param name="source">The source collection to synchronize.</param>
        /// <param name="destination">The destination collection to synchronize.</param>
        public CollectionSynchronizer(ObservableCollection<T> source, IList<T> destination)
        {
            this.source = source;
            this.destination = destination;

            // Setup update handler
            this.source.CollectionChanged += this.OnCollectionChanged;
            this.Syncronize();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.source.CollectionChanged -= this.OnCollectionChanged;
        }

        /// <summary>
        /// Handles the CollectionChanged event of <see cref="source"/>.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="NotifyCollectionChangedEventArgs"/> with arguments of the event.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        // The element was added to the collection
                        var index = e.NewStartingIndex;
                        foreach (var element in e.NewItems)
                        {
                            this.destination.Insert(index++, (T)element);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Remove:
                    {
                        // The element was removed from the collection
                        var index = e.OldStartingIndex;
                        // ReSharper disable once UnusedVariable
                        foreach (var element in e.OldItems)
                        {
                            this.destination.RemoveAt(index++);
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Replace:
                    {
                        // The collection item was replaced
                        var index = e.NewStartingIndex;
                        foreach (var element in e.NewItems)
                        {
                            this.destination[index++] = (T)element;
                        }

                        break;
                    }

                case NotifyCollectionChangedAction.Move:
                    {
                        // The item was moved in the collection
                        var item = this.destination[e.OldStartingIndex];
                        this.destination.RemoveAt(e.OldStartingIndex);
                        this.destination.Insert(e.NewStartingIndex, item);
                        break;
                    }

                case NotifyCollectionChangedAction.Reset:
                    this.Syncronize();
                    break;
            }
        }

        /// <summary>
        /// Performs the collection synchronization.
        /// </summary>
        private void Syncronize()
        {
            this.destination.Clear();
            foreach (var item in this.source)
            {
                this.destination.Add(item);
            }
        }
    }
}
