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
        private bool showingARPlanesMenu = false;
        private bool physicsActivatedMenu = false;
        private bool showingInstructionsMenu = false;
        private bool showingObjectsMenu = false;

        private GameObject currentObjectToInstantiate;


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

        [Header("Menu")]
        [SerializeField]
        private GameObject mainMenu;
        [SerializeField]
        private GameObject objectMenu;
        [SerializeField]
        private ButtonMenu seeHideARNotesButton;
        [SerializeField]
        private ButtonMenu seeHideARPlanesButton;
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

        [Header("Object menu Buttons")]
        [SerializeField]
        private ButtonMenu deleteObjectButton;


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
        /// Returns if ShowingARPlanesMenu is activated
        /// </summary>
        public bool ShowingARPlanesMenu
        {
            get { return showingARPlanesMenu; }
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
            objectMenu.SetActive(false);

            // add listners to instructions panel
            SetInitialPanel();

            // sets logic when menu buttons are clicked
            seeHideARNotesButton.SetButton(() => OnShowHideArNotesButton());
            seeHideARPlanesButton.SetButton(() => OnShowHideARPlanes());
            otherObjectsMenuButton.SetButton(() => OnShowOtherObjectsMenu());
            enablePhysicsButton.SetButton(() => OnAddRemovePhysicsInObjects());
            deleteAllButton.SetButton(() => OnDeleteAllObjects(), false);
            seeInstructionsButton.SetButton(() => ShowUserInstructionsPanel());

            // start app with menu options activated
            seeHideARNotesButton.ClickButton(() => OnShowHideArNotesButton());
            seeHideARPlanesButton.ClickButton(() => OnShowHideARPlanes());

            // listen to object menu selected
            menuObjects.GetComponent<ObjectsMenu>().ObjectSelected += OnObjectMenuSelected;

            // listen to app state change
            AppManager.Instance.StateChanged += OnChangeState;

            // listen to delete obejct
            deleteObjectButton.SetButton(() => OnDeleteObject(), false);

        }

        /// <summary>
        /// Defines the object to instantiate
        /// </summary>
        /// <param name="objectSelected"></param>
        private void OnObjectMenuSelected(Object objectSelected)
        {
            // receive object selected 
            switch (objectSelected)
            {
                case Object.Drill:
                    currentObjectToInstantiate = objectsToInstantiate[0];
                    break;
                case Object.Toolbox:
                    currentObjectToInstantiate = objectsToInstantiate[1];
                    break;
                case Object.None:
                    break;
            }

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
        private void OnShowHideARPlanes()
        {
            showingARPlanesMenu = !showingARPlanesMenu;
            PlaneGeneratedManager.Instance.ShowHideARPlanes(showingARPlanesMenu);

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

        /// <summary>
        /// Handles the menu based on app state
        /// </summary>
        /// <param name="appState"></param>
        private void OnChangeState(AppState appState)
        {
            switch (appState)
            {
                case AppState.ObjectSelected:
                    objectMenu.SetActive(true);
                    mainMenu.SetActive(false);
                    break;
                case AppState.Idle:
                default:
                    objectMenu.SetActive(false);
                    mainMenu.SetActive(true); 
                    break;
                    

            }
        }

        /// <summary>
        /// Command to delete object
        /// </summary>
        private void OnDeleteObject()
        {
            //Communicate instruction
            ARInteractionsManager.Instance.DeleteObject();
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