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

        private Image backgroundRenderer;
        private Button thisButton;

        private Color unselectedColor;
        private Color selectedColor = Color.green;

        private bool buttonIsSelected = false;
        
        #endregion

        #region private methods

        private void Awake()
        {
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

            unselectedColor = backgroundRenderer.color;
        }

        #endregion

        #region public methods

        public void SetButton(Action onSelect)
        {
            // set onSelect to listen to button click
            thisButton.onClick.AddListener(() => {
                onSelect.Invoke();

                buttonIsSelected = !buttonIsSelected;

                Debug.Log("Button is " + buttonIsSelected.ToString());

                Color backgroundColor = buttonIsSelected ? selectedColor : unselectedColor;
                backgroundRenderer.color = backgroundColor;
            });
        }

        #endregion
    }
}