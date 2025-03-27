using Enums;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

namespace Managers 
{ 
    /// <summary>
    /// Handles the placing, moving, rotation and scaling of AR objects with AR planes
    /// </summary>
    public class ARInteractionsManager : MonoBehaviour
    {
        #region fields
        private static ARInteractionsManager instance;

        // condition to enable objet placing
        private bool canPlaceObject = false;

        // keep track of all objects added into the scene
        List<GameObject> objectsInScene = new List<GameObject>();

        [Header("AR Raycast Manager in scene")]
        [SerializeField]
        private ARRaycastManager aRRaycastManager;

        [Header("Object to Add in AR")]
        [SerializeField]
        private GameObject objectToAR;

        #endregion

        #region properties

        /// <summary>
        /// Property to access the Singletion instance of ARInteractionsManager
        /// </summary>
        public static ARInteractionsManager Instance
        {
            get { return instance; }
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

        private void OnEnable()
        {
            // enable enhanced touch support
            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            // disable enhanced touch support
            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Disable();
        }

        private void Start()
        {
            AppManager.Instance.StateChanged += EnableObjectPlacing;
        }

        private void Update()
        {
            // only enable object placing case canPlaceObject
            if (canPlaceObject)
            {
                CheckForTouchToAddObject();
            }
        }

        /// <summary>
        /// Controls if user can Place objects or not
        /// </summary>
        /// <param name="status"></param>
        private void SetObjectPlacing(bool status)
        {
            canPlaceObject = status;
        }

        /// <summary>
        /// Gets a screen touch and raycasts until it hits in an ARPlane
        /// Based on the hit point position, it instantiates an object
        /// </summary>
        private void CheckForTouchToAddObject()
        {
            var activeTouches = Touch.activeTouches;
            if (activeTouches.Count == 0)
            {
                return;
            }

            Debug.Log("Screen touch was detected");

            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (aRRaycastManager.Raycast(activeTouches[0].screenPosition, hits))
            {
                Vector3 hitPosePosition = hits[0].pose.position;

                GameObject newObjectAdded = Instantiate(objectToAR);

                newObjectAdded.transform.position = hitPosePosition;    

                objectsInScene.Add(newObjectAdded);
            }

        }

        #endregion

        #region public methods

        /// <summary>
        /// Enables the user interaction to add objects into planes
        /// </summary>
        public void EnableObjectPlacing(AppState appState)
        {
            SetObjectPlacing(appState == AppState.Idle);
        }

        #endregion


    }
}