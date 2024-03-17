using System.Collections;
using System.Collections.Generic;
using ReflectiveProjectionSpace;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class ReflectiveProjection : MonoBehaviour
{
    [Tooltip("The amount of reflections possible.")]
    public int reflections = 5;                    
    
    [Tooltip("Max distance the projection travels to find a reflective layer.")]
    public float maxDistance = 50f;            
    
    [Tooltip("The width of the line renderer (projection).")]
    public float projectionWidth = 0.03f;     
    
    [Tooltip("The layers that will reflect the projection.")]
    public LayerMask reflectiveLayers;              
    
    [Tooltip("The projection will stop when it hits a game object with this layer.")]
    public LayerMask stopReflectionLayers;
    
    public GameObject ricochetObject;               // the game object that will be used for ricochet
    public float ricochetSpeed = 60f;               // the speed of the object during ricochet
    public bool ricochetFinished = true;            // return a bool whether the ricochet has finished or not

    public Material projectionMaterial;             // the material used by the projection

    public int reflectionPoints = 0;
    
    int vertsLength = 0;
    int origReflections;                    // save the original number of reflections
    
    LineRenderer lr;                        // line renderer component            
    bool firstTime = true;                  // flag the first time the recursive algorithm is triggered to save the starting position
    
    struct Tuple {                          // make our struct
        public Vector3 vector3;                     // vector3 is the vertice (XYZ)
        public bool flag;                           // flag is a bool which either states if this vertice is reflective or not
    }

    List<Tuple> reflectPoints;              // make a list which contains several tuples struct
    List<GameObject> _hitObjects;           
    List<GameObject> _hitObjectsOfStopLayer;

    bool _ricochet = false;                         // property modifier for object ricochet



    void Awake()
    {
        //cache components
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;

        if(ricochetObject != null)
        {
            if (!ricochetObject.GetComponent<RP_Lerper>()) {
                ricochetObject.AddComponent<RP_Lerper>();
                ricochetObject.GetComponent<RP_Lerper>().mainScript = this;
            }
        }

        //instantiate the new lists
        reflectPoints = new List<Tuple>();
        _hitObjects = new List<GameObject>();
        _hitObjectsOfStopLayer = new List<GameObject>();

        //number of vertices inside the line renderer == reflections
        vertsLength = reflections;
        
        //save the starting reflections count
        origReflections = reflections;

        //set the size of line renderer to total of reflections
        lr.positionCount = reflections;

        //load material if set
        //or load default
        if (!projectionMaterial) {
            lr.material = Resources.Load("material/proj_mat", typeof(Material)) as Material;
        }
        else{
            lr.material = projectionMaterial;
        }
        
        //trigger the algorithm
        EmitProjection(this.transform.position + this.transform.forward * 0.75f, this.transform.forward, reflections);
    }


    void OnDisable()
    {
        GetHitObjects.Clear();
        GetHitStopObjects.Clear();
    }


    // use update due to the heavy use of raycast physics
    void Update()
    {
        lr.startWidth = projectionWidth;
        lr.endWidth = projectionWidth;
        
        RePaint();
    }


    //this method repaints (resets and recalculates) the rays and lines over the fixed updates
    //to make it real time when changing targets 
    void RePaint()
    {
        //re-flag first time as true
        firstTime = true;

        //clear the lists
        reflectPoints.Clear();
        _hitObjects.Clear();
        _hitObjectsOfStopLayer.Clear();
        
        // re-set the size of vertices of the line renderer
        lr.positionCount = vertsLength + 1;
        
        //run the algorithm
        EmitProjection(this.transform.position + this.transform.forward * 0.75f, this.transform.forward, reflections);
    }


    //this is the recursive algorithm which sends raycasts to predict the reflections
    void EmitProjection(Vector3 position, Vector3 direction, int reflectionsRemaining)
    {
        //when reflections hit 0, run the method which renders the saved vertices into lines
        if (reflectionsRemaining == 0) {
            RenderProjections();
            return;
        }

        //save the starting position
        Vector3 StartingPosition = position;

        //if this is the very first run
        //save the starting position into vector field
        //and flag as true in tuple
        //add tuple to list of tuples data structure
        if (firstTime) {
            Tuple tuple;
            tuple.vector3 = StartingPosition;
            tuple.flag = true;

            reflectPoints.Add(tuple);
            firstTime = false;
        }


        //make a raycast and calculate reflective angle and save the 
        //if the layer hit is the same as the set reflective layer variable
        //the tuple flag is set to true as that means it's a reflective layer
        //if not, the flag is set to false 
        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        int layersToUse = reflectiveLayers | stopReflectionLayers;

        if (Physics.Raycast(ray, out hit, maxDistance, layersToUse)) {
            // if game object has reflective layer
            if (reflectiveLayers == (reflectiveLayers | (1 << hit.transform.gameObject.layer))) {
                Tuple tuple;
            
                direction = Vector3.Reflect(direction, hit.normal);
                position = hit.point;
                
                tuple.vector3 = position;
                tuple.flag = true;
                
                reflectPoints.Add(tuple);
                _hitObjects.Add(hit.transform.gameObject);
                if(hit.transform.gameObject.tag == Global.ItemTag.PLAYER)
                {
                    Debug.Log("HIT PLAYER!!!");
                    //击中player
                    hit.transform.gameObject.GetComponent<Player>().PlayerDead();
                }
            }

            
            // if game object has a stop reflection layer
            if (stopReflectionLayers == (stopReflectionLayers | (1 << hit.transform.gameObject.layer))) {
                Tuple tuple;
                tuple.vector3 = hit.point;
                tuple.flag = false;

                reflectPoints.Add(tuple);
                _hitObjectsOfStopLayer.Add(hit.transform.gameObject);
            }
        }
        else 
        {
            //if nothing is hit, increase position with direction times max distance
            position += direction * maxDistance;

            Tuple tuple;
            tuple.vector3 = position;
            tuple.flag = false;

            reflectPoints.Add(tuple);
        }

        //call the method again, reflections remaining should be -1 (or else that's an infinite loop)
        EmitProjection(position, direction, reflectionsRemaining - 1);
    }


    // this method renders the saved vertices (inside the list of tuples) into lines
    void RenderProjections() 
    {
        int i = 0;
        bool prev = true;

        //loop the tuples and set positions inside the line renderer
        foreach (Tuple value in reflectPoints) {
            if (prev) {    
                if (i + 1 > vertsLength + 1) {
                    lr.positionCount += 1;
                }
                
                if (lr.positionCount >= i+1) lr.SetPosition(i, value.vector3);
                i+=1;
            }
            else {
                break;
            }
            
            prev = value.flag;
        }

        vertsLength = i-1;
        reflectionPoints = vertsLength;
    }

    // return a list of all the hit game objects
    public List<GameObject> GetHitObjects 
    {
        get { return _hitObjects; }
    }


    // return a list of all the hit game objects with stop layer
    public List<GameObject> GetHitStopObjects 
    {
        get { return _hitObjectsOfStopLayer; }
    }


    // start the object ricochet 
    public bool ObjectRicochet 
    {
        get { return _ricochet; }

        set {

            if (_ricochet && value == true) return;
            _ricochet = value;

            if (_ricochet && ricochetObject != null) {

                if (ricochetObject.GetComponent<RP_Lerper>() == null) {
                    ricochetObject.AddComponent<RP_Lerper>();
                }

                RP_Lerper objLerper = ricochetObject.GetComponent<RP_Lerper>();

                objLerper.EndPos.Clear();
                objLerper.EndPos.Capacity = lr.positionCount;
                objLerper.EmitterDefaultTrans = transform;
                objLerper.lerpTime = ricochetSpeed;
                objLerper.mainScript = GetComponent<ReflectiveProjection>();
                
                Vector3[] getPositions = new Vector3[lr.positionCount];
                lr.GetPositions(getPositions);

                int max = lr.positionCount;
                
                for (int i=0; i<max; i++) {
                    objLerper.EndPos.Add(getPositions[i]);
                }

                objLerper.StartMove = true;
            }
        }
    }
}
