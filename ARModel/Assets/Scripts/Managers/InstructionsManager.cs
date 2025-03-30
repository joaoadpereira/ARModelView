using Enums;
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

        // InstructionsData object
        private InstructionData instructionData;

        // control for menu buttons
        private bool showingARNotesMenu = false;
        private bool showingAllObjectsMenu = false;
        private bool physicsActivatedMenu = false;
        private bool showingInstructionsMenu = false;
        private bool showingObjectsMenu = false;

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
        private Button closeInstructionsButton;

        [Header("Menu buttons")]
        [SerializeField]
        private ButtonMenu seeHideARNotesButton;
        [SerializeField]
        private ButtonMenu seeHideObjectsButton;
        [SerializeField]
        private ButtonMenu otherObjectsMenuButton;
        [SerializeField]
        private ButtonMenu enablePhysicsButton;
        [SerializeField]
        private ButtonMenu deleteAllButton;
        [SerializeField]
        private ButtonMenu seeInstructionsButton;

        [Header("Selected Prefab to add")]
        [SerializeField]
        private GameObject menuObjects;
        [SerializeField]
        private GameObject[] objectsToInstantiate;

        private GameObject currentObjectToInstantiate;

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

        /// <summary>
        /// Returns if ARNotes menu is activated
        /// </summary>
        public bool ShowingARNotes
        {
            get { return showingARNotesMenu; }
        }

        /// <summary>
        /// Returns if showingAllOBjectsMenu is activated
        /// </summary>
        public bool ShowingAllObjectsMenu
        {
            get { return showingAllObjectsMenu; }
        }

        /// <summary>
        /// Returns if physics menu is activated
        /// </summary>
        public bool PhysicsActivated
        {
            get { return physicsActivatedMenu; }
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

            // set initial hidden state of instructions panel
            instructionsPanel.SetActive(false);

        }

        private void Start()
        {
            //define object to instantiate and communicate
            currentObjectToInstantiate = objectsToInstantiate[0];
            ARInteractionsManager.Instance.SetObjectToInstantiate(currentObjectToInstantiate);

            // hide initially the menu object
            menuObjects.SetActive(false);

            // add listners to instructions panel
            SetInitialPanel();

            // sets logic when menu buttons are clicked
            seeHideARNotesButton.SetButton(() => OnShowHideArNotesButton());
            seeHideObjectsButton.SetButton(() => OnShowHideObjects());
            otherObjectsMenuButton.SetButton(() => OnShowOtherObjectsMenu());
            enablePhysicsButton.SetButton(() => OnAddRemovePhysicsInObjects());
            deleteAllButton.SetButton(() => OnDeleteAllObjects(), false);
            seeInstructionsButton.SetButton(() => ShowUserInstructionsPanel());

            // start app with menu options activated
            seeHideARNotesButton.ClickButton(() => OnShowHideArNotesButton());
            seeHideObjectsButton.ClickButton(() => OnShowHideObjects());

            // listen to object menu selected
            menuObjects.GetComponent<ObjectsMenu>().ObjectSelected += OnObjectMenuSelected;
        }

        private void OnObjectMenuSelected(Object objectSelected)
        {
            // receive object selected 
            switch (objectSelected)
            {
                case Object.Avocado:
                    currentObjectToInstantiate = objectsToInstantiate[0];
                    break;
                case Object.Engine:
                    currentObjectToInstantiate = objectsToInstantiate[1];
                    break;
                case Object.BrainStem:
                    currentObjectToInstantiate = objectsToInstantiate[2];
                    break;
                case Object.None:
                    break;
            }

            Debug.Log("object to instantiate: " + currentObjectToInstantiate.name);

            // hide object menu by "clicking" in objectMenu button
            otherObjectsMenuButton.ClickButton(() => OnShowOtherObjectsMenu());

            // communicate the object to instantiate
            ARInteractionsManager.Instance.SetObjectToInstantiate(currentObjectToInstantiate);

        }

        /// <summary>
        /// Shows or hides the ARNotes
        /// </summary>
        private void OnShowHideArNotesButton()
        {
            showingARNotesMenu = !showingARNotesMenu;
            ARInteractionsManager.Instance.ShowHideARNotes(showingARNotesMenu);
        }

        /// <summary>
        /// Shows or hides all Objects
        /// </summary>
        private void OnShowHideObjects()
        {
            showingAllObjectsMenu = !showingAllObjectsMenu;
            ARInteractionsManager.Instance.ShowHideAllObjects(showingAllObjectsMenu);

        }

        private void OnShowOtherObjectsMenu()
        {
            showingObjectsMenu = !showingObjectsMenu;

            // show or hide object menu
            menuObjects.SetActive(showingObjectsMenu);

        }

        /// <summary>
        /// Adds or removes physics into the objects
        /// </summary>
        private void OnAddRemovePhysicsInObjects()
        {
            physicsActivatedMenu = !physicsActivatedMenu;
            ARInteractionsManager.Instance.AddRemovePhysicsInObjects(physicsActivatedMenu);
        }

        /// <summary>
        /// Deletes all objects in scene
        /// </summary>
        private void OnDeleteAllObjects()
        {
            ARInteractionsManager.Instance.DeleteAllObjects();
        }

        /// <summary>
        ///  Add listners to each instructions button control
        /// </summary>
        private void SetInitialPanel()
        {
            // Add listners to each instructions button control
            forwardInstructionButton.onClick.AddListener(SetForwardInstruction);
            previousInstructionButton.onClick.AddListener(SetPreviousInstruction);

            // add instructions button click action as listner the close instructions click
            closeInstructionsButton.onClick.AddListener(() => seeInstructionsButton.ClickButton(() => ShowUserInstructionsPanel()));

            ResetInstructionsPanel();
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
                        //ShowUserInstructionsPanel(true);
                        seeInstructionsButton.ClickButton(() => ShowUserInstructionsPanel());

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
        private void ShowUserInstructionsPanel()
        {
            showingInstructionsMenu = !showingInstructionsMenu;

            // show the instructions Panel
            instructionsPanel.SetActive(showingInstructionsMenu);

            ResetInstructionsPanel();

            if (!showingInstructionsMenu)
            {
                // change state to idle 
                AppManager.Instance.SetAppState(AppState.Idle);
            }
            else
            {
                // change state to showing instructions
                AppManager.Instance.SetAppState(AppState.ShowingInstructions);
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
        public void HandleWelcomeApp()
        {
            SetInitialAppPanel();
        }

        #endregion
    }
}