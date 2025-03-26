using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Utils;


namespace Managers
{ 
    /// <summary>
    /// Handles the initial instructions for the user
    /// </summary>
    public class InstructionsManager : MonoBehaviour
    {
        #region fields
        private static InstructionsManager instance;

        [Header("Initial App Panel to show")]
        [SerializeField]
        private GameObject initialAppPanel;

        [Header("instruction Panel to present instructions")]
        [SerializeField]
        private GameObject instructionsPanel;
        [SerializeField]
        TMP_Text instructionText;
        [SerializeField]
        private Button previousInstructionButton;
        [SerializeField]
        private Button forwardInstructionButton;
        [SerializeField]
        private Button closeButton;

        [Header("Help button")]
        [SerializeField]
        private Button helpButton;

        private InstructionData instructionData;
       
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

            // instantiate instructions
            instructionData = new InstructionData();

            // add listners to instructions panel
            SetInitialPanel();

            // set initial hidden state of instructions panel
            instructionsPanel.SetActive(false);

        }

        /// <summary>
        ///  Add listners to each instructions button control
        /// </summary>
        private void SetInitialPanel()
        {
            // Add listners to each instructions button control
            forwardInstructionButton.onClick.AddListener(SetForwardInstruction);
            previousInstructionButton.onClick.AddListener(SetPreviousInstruction);
            closeButton.onClick.AddListener(() => ShowUserInstructionsPanel(false));

            ResetInstructionsPanel();

            // Set help button to open panel
            helpButton.onClick.AddListener(() => ShowUserInstructionsPanel(true));

        }


        /// <summary>
        /// Sets the initial app panel and defines the logic for button click
        /// </summary>
        private void SetInitialAppPanel()
        {
            if (initialAppPanel != null)
            {
                initialAppPanel.SetActive(true);
                Button initalButton = initialAppPanel.GetComponentInChildren<Button>();

                if (initalButton != null)
                {
                    // When click the button, show panel instructions and hide the initialAppPanel
                    initalButton.onClick.AddListener(() => 
                    {
                        ShowUserInstructionsPanel(true);
                        
                        // hide the initialAppPanel
                        initialAppPanel.SetActive(false);
                    });
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
        /// Sets the user instructions panel
        /// </summary>
        /// <param name="state"></param>
        private void ShowUserInstructionsPanel(bool state)
        {
            // show the instructions Panel
            instructionsPanel.SetActive(state);

            ResetInstructionsPanel();

            // if instructions panel closes, show help menu
            if (!state)
            {
                helpButton.gameObject.SetActive(true);
            }
            else
            {
                helpButton.gameObject.SetActive(false);
            }

        }

        /// <summary>
        /// Changes the instruction text to the next instruction
        /// Case reaches the last instruction, it hides the forward button
        /// </summary>
        private void SetForwardInstruction()
        {
            string instruction = instructionData.GetforwardInstruction();
            instructionText.text = instruction;

            if (instructionData.IsLastInstruction)
            {
                forwardInstructionButton.gameObject.SetActive(false);
            }
            else
            {
                previousInstructionButton.gameObject.SetActive(true);
                forwardInstructionButton.gameObject.SetActive(true);
            }

        }

        /// <summary>
        /// Changes the instruction text to the previous instruction
        /// Case reaches the first instruction, it hides the previous button
        /// </summary>
        private void SetPreviousInstruction()
        {
            string instruction = instructionData.GetPreviousInstruction();
            instructionText.text = instruction;

            if (instructionData.IsFirstInstruction)
            {
                previousInstructionButton.gameObject.SetActive(false);
                forwardInstructionButton.gameObject.SetActive(true);
            }
            else
            {
                previousInstructionButton.gameObject.SetActive(true);
                forwardInstructionButton.gameObject.SetActive(true);
            }

        }

        /// <summary>
        /// Resets the instructions panel to the first instruction
        /// Hides previous button and shows forward button 
        /// </summary>
        private void ResetInstructionsPanel()
        {
            //reset instructions
            instructionData.ResetInstructions();

            //set instruction data
            instructionText.text = instructionData.GetCurrentInstruction();

            //reset buttons
            previousInstructionButton.gameObject.SetActive(false);
            forwardInstructionButton.gameObject.SetActive(true);
        }

        #endregion

        #region public methods

        /// <summary>
        /// Handle the instructions panel
        /// </summary>
        public void HandleInstructions()
        {
            SetInitialAppPanel();
        }

        #endregion
    }
}