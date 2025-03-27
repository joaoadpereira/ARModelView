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
    public class AppManager : MonoBehaviour
    {
        #region fields
        private static AppManager instance;

        //current app state
        private AppState currentAppState;
        private Action<AppState> onStateChange;

        #endregion

        #region properties

        /// <summary>
        /// Property to access the Singleton instance of AppManager
        /// </summary>
        public static AppManager Instance
        {
            get { return instance; }
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

            // listen for app state changes
            onStateChange += OnStateChange;
        }

        private void Start()
        {
            // Set initial App State
            SetAppState(AppState.Initializing);
        }

        /// <summary>
        /// Handles the state change
        /// </summary>
        /// <param name="appState"></param>
        private void OnStateChange(AppState appState)
        {
            switch (appState)
            {
                case AppState.Initializing:
                    Debug.Log("Initializing");
                    InstructionsManager.Instance.HandleWelcomeApp();
                    break;
                case AppState.ShowingInstructions:
                    Debug.Log("ShowingInstructions");
                    break;
                case AppState.Idle:
                    Debug.Log("Idle");
                    break;
                case AppState.PlacingObject:
                    Debug.Log("PlacingObject");
                    break;
                case AppState.ObjectSelected:
                    Debug.Log("ObjectSelected");
                    break;
                case AppState.ManipulatingObject:
                    Debug.Log("ManipulatingObject");
                    break;
                default:
                    break;
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
