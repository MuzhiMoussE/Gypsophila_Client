using UnityEngine;

namespace ReflectiveProjectionDemo
{
    public class testBtn : MonoBehaviour
    {
        ReflectiveProjection rp;

        void Awake()
        {
            rp = GetComponent<ReflectiveProjection>();
        }

        // Update is called once per frame
        void Update()
        {
            // start the ricochet when pressing space
            if(Input.GetKeyDown(KeyCode.Space)){
                rp.ObjectRicochet = true;
            }

            // hide the ricochet object when done
            if (rp.ricochetFinished) 
            {
                rp.ricochetObject.GetComponent<MeshRenderer>().enabled = false;
            }else{
                rp.ricochetObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
