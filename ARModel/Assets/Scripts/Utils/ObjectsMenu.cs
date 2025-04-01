using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Utils
{
    /// <summary>
    /// Handles object menu selection
    /// </summary>
    public class ObjectsMenu : MonoBehaviour
    {
        #region fields

        [Header("Objects buttons")]
        [SerializeField]
        private Button[] objectButtons;

        // action to listen when object menu is selected
        public Action<Object> ObjectSelected;

        #endregion

        #region private methods

        private void Awake()
        {
            // adds a callback to each button to communicate it was clicked
            for(int i = 0; i < objectButtons.Length; i++)
            {
                int index = i;
                objectButtons[i].onClick.AddListener(() => HandleButtonClick(index));
            }
        }

        /// <summary>
        /// Handles the click based on menu
        /// </summary>
        /// <param name="menuIndex"></param>
        private void HandleButtonClick(int menuIndex)
        {
            Object objectSelectedMenu;
            switch (menuIndex)
            {
                case 0:
                    objectSelectedMenu = Object.Drill;
                    break;
                case 1:
                    objectSelectedMenu = Object.Toolbox;
                    break;
                default:
                    objectSelectedMenu = Object.None;
                    break;

            }
        
            // communicate the obejct selected
            ObjectSelected.Invoke(objectSelectedMenu);
        }

        #endregion
    }
}