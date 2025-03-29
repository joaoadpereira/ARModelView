using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{ 
    /// <summary>
    /// Handles the logic about each menu button
    /// </summary>
    public class ButtonMenu : MonoBehaviour
    {
        #region fields

        // hold reference to button components for manipulation
        private Image backgroundRenderer;
        private Button thisButton;

        // hold and define unselected and selected colors
        private Color unselectedColor;
        private Color selectedColor = Color.green;

        // control if button is currently selected
        private bool buttonIsSelected = false;
        
        #endregion

        #region private methods

        private void Awake()
        {
            // define components
            thisButton = GetComponent<Button>();
            if (thisButton == null)
            {
                Debug.LogError("Button component in menu button was not found.");
            }

            backgroundRenderer = GetComponentInChildren<Image>();

            if(backgroundRenderer == null)
            {
                Debug.LogError("Canvas Renderer Background in Menu button is missing.");
            }

            // define unselected color based on init color
            unselectedColor = backgroundRenderer.color;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Sets the button with logic based.
        /// Handles icon color case selected or not.
        /// </summary>
        /// <param name="onSelect"></param>
        public void SetButton(Action onSelect)
        {
            // set onSelect to listen to button click and handles icon color selection
            thisButton.onClick.AddListener(() => {
                onSelect.Invoke();

                buttonIsSelected = !buttonIsSelected;

                Color backgroundColor = buttonIsSelected ? selectedColor : unselectedColor;
                backgroundRenderer.color = backgroundColor;
            });
        }

        #endregion
    }
}