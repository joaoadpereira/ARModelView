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

        [SerializeField]
        private ARPlaneManager aRPlaneManager;

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
        private bool supportsClassification = false;

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
            // listen to ARPlanes have been added, updated or removed
            aRPlaneManager.planesChanged += PlanesChanged;

            // Define if device can classify ARPlanes automatically
            //supportsClassification = aRPlaneManager.descriptor.supportsClassification;
        }

        /// <summary>
        /// Handles the logic when AR planes have changed (been added, updated or removed)
        /// </summary>
        /// <param name="arfs"></param>
        private void PlanesChanged(ARPlanesChangedEventArgs args)
        {
            // handle added new AR planes
            List<ARPlane> allPlanes = new List<ARPlane> ();

            allPlanes.AddRange(args.added);
            allPlanes.AddRange (args.updated);

            foreach (ARPlane plane in allPlanes)
            {
                PlaneClassification planeClassifiction; 

                // case 
                if (supportsClassification)
                {
                    planeClassifiction = plane.classification;
                }
                else
                {
                    planeClassifiction = ClassifyThePlan(plane);
                }

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
            // If the plane classification is vertical, assume as a wall plane
            if (planeAlignment == PlaneAlignment.Vertical)
            {
                return PlaneClassification.Wall;
            }

            Vector3 planeNormal = aRPlane.normal;

            if (Mathf.Abs(planeNormal.y) > Mathf.Abs(planeNormal.x) && Mathf.Abs(planeNormal.y) > Mathf.Abs(planeNormal.z))
            {
                return PlaneClassification.Floor;
            }

            return PlaneClassification.None;
        }

        /// <summary>
        /// Updates the given material in the given plane
        /// </summary>
        /// <param name="plane"></param>
        /// <param name="material"></param>
        private void UpdateARPlaneMaterial(ARPlane plane, Material material)
        {
            MeshRenderer meshRenderer = plane.GetComponent<MeshRenderer>();

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