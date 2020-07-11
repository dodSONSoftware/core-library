using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dodSON.Core.Addon
{
    /// <summary>
    /// Defines the base class for all <see cref="IAddon"/>s that will be started in a separate <see cref="System.AppDomain"/>.
    /// </summary>
    /// <seealso cref="dodSON.Core.Addon.PluginFactory"/>
    [Serializable()]
    public abstract class PluginBase
        : MarshalByRefObject,
          IAddon
    {
        #region Ctor
        /// <summary>
        /// A protected constructor.
        /// </summary>
        protected PluginBase() : base() { }

        #endregion
        #region Private Fields
        private TimeSpan _OverallDuration = TimeSpan.Zero;
        #endregion
        #region IAddon Methods
        /// <summary>
        /// Returns whether this plugin is running. Returns <b>true</b> if the plugin is running; otherwise, <b>false</b>.
        /// </summary>
        public bool IsRunning { get; private set; } = false;
        /// <summary>
        /// The date this plugin was started.
        /// </summary>
        public DateTime DateLastStarted { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// The date this plugin was stopped.
        /// </summary>
        public DateTime DateLastStopped { get; private set; } = DateTime.MinValue;
        /// <summary>
        /// Returns the total duration the plugin has been running.
        /// </summary>
        /// <seealso cref="LastRunDuration"/>
        public TimeSpan OverallRunDuration
        {
            get
            {
                if (IsRunning)
                {
                    return _OverallDuration + LastRunDuration;
                }
                else
                {
                    return _OverallDuration;
                }
            }
        }
        /// <summary>
        /// Returns the duration the plugin has been running since it was last started.
        /// </summary>
        /// <seealso cref="OverallRunDuration"/>
        public TimeSpan LastRunDuration
        {
            get
            {
                if (DateLastStarted == DateTime.MinValue) { return TimeSpan.Zero; }
                if (DateLastStopped == DateTime.MinValue) { return (DateTime.Now - DateLastStarted); }
                return (DateLastStopped - DateLastStarted);
            }
        }
        /// <summary>
        /// Returns the number of times this plugin has been started.
        /// </summary>
        public int StartCount { get; private set; } = 0;
        /// <summary>
        /// Returns the number of times this plugin has been stopped.
        /// </summary>
        public int StopCount { get; private set; } = 0;
        /// <summary>
        /// Starts this plugin.
        /// </summary>
        /// <remarks>
        /// This will reset the <see cref="DateLastStopped"/> to <see cref="DateTime.MinValue"/>, the <see cref="DateLastStarted"/> to <see cref="DateTime.Now"/> and increment <see cref="StartCount"/>.
        /// </remarks>
        public void Start()
        {
            if (!IsRunning)
            {
                StartCount++;
                DateLastStarted = DateTime.Now;
                DateLastStopped = DateTime.MinValue;
                IsRunning = true;
                OnStart();
            }
        }
        /// <summary>
        /// Stops this plugin.
        /// </summary>
        /// <remarks>
        /// This will set the <see cref="DateLastStopped"/> to <see cref="DateTime.Now"/> and increment <see cref="StopCount"/>.
        /// </remarks>
        public void Stop()
        {
            if (IsRunning)
            {
                OnStop();
                StopCount++;
                DateLastStopped = DateTime.Now;
                _OverallDuration += LastRunDuration;
                IsRunning = false;
            }
        }
        #endregion
        #region Abstract Methods
        /// <summary>
        /// When overridden in a derived class, this methods should perform whatever steps are necessary to start this plugin. 
        /// </summary>
        protected abstract void OnStart();
        /// <summary>
        /// When overridden in a derived class, this methods should perform whatever steps are necessary to stop this plugin. 
        /// </summary>
        protected abstract void OnStop();
        #endregion
    }
}
