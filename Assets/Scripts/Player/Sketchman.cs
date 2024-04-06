using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sketchman : MonoBehaviour
{
    [SerializeField] private GameObject spriteObject;
    // Start is called before the first frame update
    void Start()
    {
        SketchUtility.Instance.SketchInit();
    }

    // Update is called once per frame
    void Update()
    {
        SketchSystem.Instance.CopyActionToSkecthMan(gameObject);
        SketchUtility.Instance.StartReleasing();
    }
}
