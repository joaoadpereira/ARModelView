using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Managers
{ 
    /// <summary>
    /// Handles the initial instructions for the user
    /// </summary>
    public class InstructionsManager : MonoBehaviour
    {
        #region fields
        private static InstructionsManager instance;

        [SerializeField]
        private GameObject initialAppPanel;

        #endregion

        #region properties

        /// <summary>
        /// Property to access the Singleton instance of InstructionsManager
        /// </summary>
        public static InstructionsManager Instance 
        { 
            get 
            {
                return instance; 
            } 
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

        private void SetInitialAppPanel()
        {
            if (initialAppPanel != null)
            {
                initialAppPanel.SetActive(true);
                Button initalButton = initialAppPanel.GetComponentInChildren<Button>();

                if (initalButton != null)
                {
                    initalButton.onClick.AddListener(HideInitialPanel);
                }
                else
                {
                    Debug.LogError("Button in initialAppPanel not found.");
                }
            }
            else 
            {
                Debug.LogError("It was not possible to find the InitialAppPanel.");
            }
        }

        /// <summary>
        /// Hides the initialAppPanel
        /// </summary>
        private void HideInitialPanel()
        {
            initialAppPanel.SetActive(false);
        }

        #endregion

        #region public methods

        public void HandleInstructions()
        {
            SetInitialAppPanel();
        }

        #endregion
    }
}