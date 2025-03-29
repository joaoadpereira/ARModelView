using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


namespace Managers 
{
    /// <summary>
    /// Gets and handles the information about ARPlanes generated
    /// </summary>
    public class PlaneGeneratedManager : MonoBehaviour
    {
        #region fields
        private static PlaneGeneratedManager instance;

        [Header("AR Managers")]
        [SerializeField]
        private ARPlaneManager aRPlaneManager;
        [SerializeField]
        private AROcclusionManager aROcclusionManager;

        [Header("Materials to distinguish between different plane classification")]
        [SerializeField]
        private Material wallMaterial;
        [SerializeField]
        private Material tableMaterial;
        [SerializeField]
        private Material floorMaterial;
        [SerializeField]
        private Material defaultSurfaceMaterial;

        // track if device can classify planes
        private bool deviceSupportsClassification = false;

        // track if device supports occlusion
        private bool deviceSupportsOcclusion = false;

        // track of floor hight
        private float floorHight = float.MaxValue;
        private float thresholdHight = 0.2f;

        // track all planes 
        private List<ARPlane> allARPlanes = new List<ARPlane>();

        #endregion

        #region properties

        /// <summary>
        /// Property to acess the Singletion instance of PlaneGeneratedManager
        /// </summary>
        public static PlaneGeneratedManager Instance
        {
            get {  return instance; }
        }

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

        #endregion

        #region private methods

        // Start is called before the first frame update
        private void Start()
        {
            // only compile and make verification when running on android/ios device
#if !UNITY_EDITOR && PLATFORM_ANDROID || PLATFORM_IOS

            if (aRPlaneManager != null)
            {
                // listen to ARPlanes have been added, updated or removed
                aRPlaneManager.planesChanged += PlanesChanged;

                // Define if device can classify ARPlanes automatically
                deviceSupportsClassification = aRPlaneManager.descriptor.supportsClassification;
            } 
            else
            {
                Debug.LogError("ARPlaneManager is not assigned.");
            }
            
            if (aROcclusionManager != null)
            {
                // Define id device supports occlusion
                deviceSupportsOcclusion = aROcclusionManager.descriptor.environmentDepthImageSupported == Supported.Supported;
                Debug.Log("deviceSupportsOcclusion: " + deviceSupportsOcclusion.ToString());
            }
            else
            {
                Debug.LogError("AROclusionManager is not assigned.");
            }
#endif
        }


        /// <summary>
        /// Handles the logic when AR planes have changed (been added, updated or removed)
        /// </summary>
        /// <param name="arfs"></param>
        private void PlanesChanged(ARPlanesChangedEventArgs args)
        {
            // handle added new AR planes
            allARPlanes = new List<ARPlane> ();

            // track all ARplane
            allARPlanes.AddRange(args.added);
            allARPlanes.AddRange(args.updated);

            // loop every ARPlane
            foreach (ARPlane plane in allARPlanes)
            {
                PlaneClassification planeClassifiction; 

                // case device supports automatic detection. If not, try to classify
                if (deviceSupportsClassification)
                {
                    planeClassifiction = plane.classification;
                }
                else
                {
                    // try to classify the plan
                    planeClassifiction = ClassifyThePlan(plane);
                }

                // change material on ARPlane based on classification
                switch (planeClassifiction)
                {
                    case PlaneClassification.Wall:
                        UpdateARPlaneMaterial(plane, wallMaterial);
                        break;
                    case PlaneClassification.Floor:
                        UpdateARPlaneMaterial(plane, floorMaterial);
                        break;
                    case PlaneClassification.Table:
                        UpdateARPlaneMaterial(plane, tableMaterial);
                        break;
                    default:
                        UpdateARPlaneMaterial(plane, defaultSurfaceMaterial);
                        break;
                }
                
            }
        }

        /// <summary>
        /// Method to try to classify an ARPlane based on its normal and height
        /// </summary>
        /// <param name="aRPlane"></param>
        /// <returns></returns>
        private PlaneClassification ClassifyThePlan(ARPlane aRPlane)
        {
            PlaneAlignment planeAlignment = aRPlane.alignment;

            switch (planeAlignment)
            {
                case PlaneAlignment.Vertical:
                    return PlaneClassification.Wall;
                case PlaneAlignment.HorizontalUp:
                case PlaneAlignment.HorizontalDown:
                    // get plane position and hight as y component
                    Vector3 planePosition = aRPlane.transform.position;

                    // redefine floorHight if a lower plane is found 
                    if (planePosition.y < floorHight)
                    {
                        floorHight = planePosition.y;
                        return PlaneClassification.Floor;
                    }

                    // if the plane is inside floor hight threshold, assume that is also a floor 
                    if (Mathf.Abs(planePosition.y - floorHight) < thresholdHight)
                    {
                        return PlaneClassification.Floor;
                    }

                    // assume all horizontal planes above floor level as table
                    return PlaneClassification.Table;
                default:
                    return PlaneClassification.None;
            }
        }

        /// <summary>
        /// Updates the given material in the given plane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="material"></param>
        private void UpdateARPlaneMaterial(ARPlane plane, Material material)
        {
            MeshRenderer meshRenderer = plane.GetComponent<MeshRenderer>();

            // change material array in MeshRender with an array with the given material 
            if (meshRenderer != null)
            {
                Material[] mats = new Material[1];
                mats[0] = material;

                meshRenderer.materials = mats;
            }
            else
            {
                Debug.LogError("The ARPlane does not have a Renderer Component");
            }
        }

        #endregion
    }
}