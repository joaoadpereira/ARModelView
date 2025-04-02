using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{ 
    /// <summary>
    /// Main App Manager to handle state machine
    /// </summary>
    public class AppStateManager : MonoBehaviour
    {
        #region fields
        private static AppStateManager instance;

        //current app state
        private AppState currentAppState;
        private Action<AppState> onStateChange;

        #endregion

        #region properties

        /// <summary>
        /// Property to access the Singleton instance of AppManager
        /// </summary>
        public static AppStateManager Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Action to listen for App state change
        /// </summary>
        public event Action<AppState> StateChanged
        { 
            add { onStateChange += value; } 
            remove { onStateChange -= value; }
        }

        /// <summary>
        /// Returns the current App State
        /// </summary>
        public AppState CurrentAppState
        {
            get { return currentAppState; }
        }

        #endregion

        #region private methods

        private void Awake()
        {
            // Set the current instance as the singleton or destroy case duplicated
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        #endregion

        #region public methods

        /// <summary>
        /// Method to provide and define new spp state
        /// </summary>
        /// <param name="appState"></param>
        public void SetAppState(AppState appState) 
        {
            currentAppState = appState;

            onStateChange.Invoke(appState);
        }

        #endregion
    }
}
