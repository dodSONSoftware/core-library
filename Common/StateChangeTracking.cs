using System;

namespace dodSON.Core.Common
{
    /// <summary>
    /// Provides control over commonly tracked states.
    /// </summary>
    /// <example>
    /// The following code example will demonstrate the execution of the StateChangeTracking type. 
    /// <para>
    /// <br/>
    /// Create a console application and add the following code:
    /// </para>
    /// <code>
    /// static void Main(string[] args)
    /// {
    ///     // create state and a state view
    ///     var state = new dodSON.Core.Common.StateChangeTracking();
    ///     // connect to the Property Changed event
    ///     state.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
    ///                              {
    ///                                  Console.WriteLine($"   ^{e.PropertyName} Changed");
    ///                              };
    ///     var stateView = (dodSON.Core.Common.IStateChangeView)state;
    ///     // display initial state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.MarkAll();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.ClearNew();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.ClearDirty();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.ClearDeleted();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.MarkNew();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.MarkDirty();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.MarkDeleted();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     // mutate state 
    ///     state.ClearAll();
    ///     // display mutated state
    ///     Console.WriteLine(string.Format("IsNew= {0,-5}; IsDirty= {1,-5}; IsDeleted= {2,-5}", stateView.IsNew, stateView.IsDirty, stateView.IsDeleted));
    ///     Console.WriteLine();
    ///     Console.WriteLine("press anykey...");
    ///     Console.ReadKey(true);
    /// 
    ///     // This code produces output similar to the following:
    ///     // Actual, come to think of it, it better produce the EXTACT same results as below!
    /// 
    ///     // IsNew= False; IsDirty= False; IsDeleted= False
    ///     //    ^IsDirty Changed
    ///     //    ^IsNew Changed
    ///     //    ^IsDeleted Changed
    ///     // IsNew= True ; IsDirty= True ; IsDeleted= True
    ///     //    ^IsNew Changed
    ///     // IsNew= False; IsDirty= True ; IsDeleted= True
    ///     //    ^IsDirty Changed
    ///     // IsNew= False; IsDirty= False; IsDeleted= True
    ///     //    ^IsDeleted Changed
    ///     // IsNew= False; IsDirty= False; IsDeleted= False
    ///     //    ^IsNew Changed
    ///     // IsNew= True ; IsDirty= False; IsDeleted= False
    ///     //    ^IsDirty Changed
    ///     // IsNew= True ; IsDirty= True ; IsDeleted= False
    ///     //    ^IsDeleted Changed
    ///     // IsNew= True ; IsDirty= True ; IsDeleted= True
    ///     //    ^IsDirty Changed
    ///     //    ^IsNew Changed
    ///     //    ^IsDeleted Changed
    ///     // IsNew= False; IsDirty= False; IsDeleted= False
    ///     // 
    ///     // press anykey...
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public class StateChangeTracking
        : IStateChangeTracking
    {
        #region System.ComponentModel.INotifyPropertyChanged Methods
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Will raise the property changed events with the provided property name.
        /// </summary>
        /// <param name="propertyName">The name of the property which has changed.</param>
        protected void RaisePropertyChangedEvent([System.Runtime.CompilerServices.CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
        #endregion
        #region Ctor
        /// <summary>
        /// Initializes a new instance of the StateChangeTracking. All values will be false.
        /// </summary>
        public StateChangeTracking() { }
        /// <summary>
        /// Initializes a new instance of the StateChangeTracking using the specified value.
        /// </summary>
        /// <param name="dirtyState">The initial dirty state.</param>
        /// <param name="newState">The initial new state.</param>
        /// <param name="deletedState">The initial deleted state.</param>
        public StateChangeTracking(bool dirtyState,
                                   bool newState,
                                   bool deletedState)
            : this()
        {
            _IsDirty = dirtyState;
            _IsNew = newState;
            _IsDeleted = deletedState;
        }
        /// <summary>
        /// Initializes a new instance of the StateChangeTracking using the specified <see cref="IStateChangeView"/>. Copy constructor.
        /// </summary>
        /// <param name="state">The <see cref="IStateChangeView"/> to copy.</param>
        public StateChangeTracking(IStateChangeView state) : this(state.IsDirty, state.IsNew, state.IsDeleted) { }
        #endregion
        #region Private Fields
        private bool _IsDirty = false;
        private bool _IsNew = false;
        private bool _IsDeleted = false;
        #endregion
        #region IStateChangeTracking Methods
        /// <summary>
        /// Represents the state of change.
        /// </summary>
        public bool IsDirty
        {
            get { return _IsDirty; }
            private set
            {
                _IsDirty = value;
                RaisePropertyChangedEvent(nameof(IsDirty));
            }
        }
        /// <summary>
        /// Represents the state of age.
        /// </summary>
        public bool IsNew
        {
            get { return _IsNew; }
            private set
            {
                _IsNew = value;
                RaisePropertyChangedEvent(nameof(IsNew));
            }
        }
        /// <summary>
        /// Represents the state of existence.
        /// </summary>
        public bool IsDeleted
        {
            get { return _IsDeleted; }
            private set
            {
                _IsDeleted = value;
                RaisePropertyChangedEvent(nameof(IsDeleted));
            }
        }
        /// <summary>
        /// Sets IsDirty to true.
        /// </summary>
        public void MarkDirty() => IsDirty = true;
        /// <summary>
        /// Sets IsNew to true.
        /// </summary>
        public void MarkNew() => IsNew = true;
        /// <summary>
        /// Sets IsDeleted to true.
        /// </summary>
        public void MarkDeleted() => IsDeleted = true;
        /// <summary>
        /// Sets all states to true.
        /// </summary>
        public void MarkAll()
        {
            IsDirty = true;
            IsNew = true;
            IsDeleted = true;
        }
        /// <summary>
        /// Sets IsDirty to false.
        /// </summary>
        public void ClearDirty() => IsDirty = false;
        /// <summary>
        /// Sets IsNew to false.
        /// </summary>
        public void ClearNew() => IsNew = false;
        /// <summary>
        /// Sets IsDeleted to false.
        /// </summary>
        public void ClearDeleted() => IsDeleted = false;
        /// <summary>
        /// Sets all states to false.
        /// </summary>
        public void ClearAll()
        {
            IsDirty = false;
            IsNew = false;
            IsDeleted = false;
        }
        #endregion
        #region Overrides
        /// <summary>
        /// Returns a string representation of the StateChangeTracking class.
        /// </summary>
        /// <returns>A string representation of the StateChangeTracking class.</returns>
        public override string ToString() => string.Format($"IsDirty={IsDirty}; IsNew={IsNew}; IsDeleted={IsDeleted}");
        #endregion
    }
}
