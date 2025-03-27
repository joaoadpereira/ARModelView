using Enums;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Managers 
{ 
    /// <summary>
    /// Handles the placing, moving, rotation and scaling of AR objects with AR planes
    /// </summary>
    public class ARInteractionsManager : MonoBehaviour
    {
        #region fields
        private static ARInteractionsManager instance;

        // condition to enable AR interaction 
        private bool canARInteract = false;

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
            // only enable interaction with AR objects if canARInteract
            if (canARInteract)
            {
                // gets the touches in screen
                var activeTouches = Touch.activeTouches;
                if (activeTouches.Count == 0) return;

                // only check for one time touch in screen
                if (activeTouches[0].phase == TouchPhase.Began)
                {
                    // check if touched an object
                    bool clickedInObject = CheckForObjectTouch(activeTouches[0]);

                    // if didnt touch any object, then check if it hits an ARPlane
                    if (!clickedInObject)
                    {
                        CheckForARPlaneTouch(activeTouches[0]);
                    }
                }
            }
        }

        /// <summary>
        /// Controls if user can Place objects or not
        /// </summary>
        /// <param name="status"></param>
        private void SetObjectPlacing(bool status)
        {
            canARInteract = status;
        }

        /// <summary>
        /// Checks is a touch in screen and raycasts any hit with an object
        /// </summary>
        /// <param name="touch"></param>
        /// <returns></returns>
        private bool CheckForObjectTouch(Touch touch)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.screenPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject hittedObject = hit.collider.gameObject;
                if (hittedObject.transform.parent.CompareTag("ObjectToManipulate"))
                {
                    hittedObject.SetActive(false);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets a screen touch and raycasts until it hits in an GameObject
        /// Based on the hit point position, Returns the hit object
        /// </summary>
        private void CheckForARPlaneTouch(Touch touch)
        {
            List<ARRaycastHit> hits = new List<ARRaycastHit>();

            if (aRRaycastManager.Raycast(touch.screenPosition, hits))
            {
                GameObject hittedGameObject = hits[0].trackable.gameObject;

                // it can hit a plane but it can also hit an instantiated object
                if (hittedGameObject.GetComponent<ARPlane>() != null)
                {
                    Vector3 hitPosePosition = hits[0].pose.position;

                    GameObject newObjectAdded = Instantiate(objectToAR);

                    newObjectAdded.transform.position = hitPosePosition;

                    objectsInScene.Add(newObjectAdded);
                }
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