using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SceneAction2 : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject door;
    public GameObject duck;

    private void OnEnable()
    {
        
        duck.GetComponent<Animator>().SetBool("Play", true);
        door.gameObject.SetActive(true);
        door.GetComponent<Animator>().enabled = true;
        //director.Stop();
        //director.enabled = false;
    }

}
