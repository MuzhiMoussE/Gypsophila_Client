using UnityEngine;

namespace ReflectiveProjectionDemo
{
    public class WallDetectRay : MonoBehaviour
    {
        // get the script
        public ReflectiveProjection script;
        
        // get the red material
        public Material RedMaterial;
        
        // save the original material
        private Material OriginalMaterial;


        void Start()
        {
            OriginalMaterial = gameObject.GetComponent<MeshRenderer>().material;
        }


        void Update()
        {
            // GetHitObjects is a property within the core script
            // which returns a list of all the hit objects
            if (script.GetHitObjects.Contains(gameObject)) {
                gameObject.GetComponent<MeshRenderer>().material = RedMaterial;
            }
            else {
                gameObject.GetComponent<MeshRenderer>().material = OriginalMaterial;
            }
            
        }
    }
}

