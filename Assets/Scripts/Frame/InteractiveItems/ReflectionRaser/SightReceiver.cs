using Frame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReflectiveProjectionDemo
{
    public class SightReceiver : MonoBehaviour
    {
        public SightSwitch _switch;
        // get the script
        public ReflectiveProjection script;
        public Material OverMaterial;
        private Material OriginalMaterial;
        void Start()
        {
            OriginalMaterial = gameObject.GetComponent<MeshRenderer>().material;
        }
        void Update()
        {
            // GetHitObjects is a property within the core script
            // which returns a list of all the hit objects
            if (script.GetHitObjects.Contains(gameObject))
            {
                gameObject.GetComponent<MeshRenderer>().material = OverMaterial;
                if(_switch.state == ItemState.Off)
                {
                    _switch.state = ItemState.On;
                    EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
                }

            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material = OriginalMaterial;
                if(_switch.state == ItemState.On)
                {
                    _switch.state = ItemState.Off;
                    EventCenter.Broadcast(GameEvent.ItemStateChangeEvent);
                }

            }
        }
    }
}
