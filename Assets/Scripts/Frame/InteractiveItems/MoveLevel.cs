using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveLevel : MonoBehaviour
{
    public GameObject Level1;
    public GameObject Level2;
    public GameObject Level3;
    private bool play1, play2, play3;

    public void MoveLevel1()
    {
        play1 = !play1;
        if (play1) Level1.GetComponent<Animator>().SetBool("Play", true);
        else Level1.GetComponent<Animator>().SetBool("Play", false);
    }

    public void MoveLevel2()
    {
        play2 = !play2;
        if (play2) Level2.GetComponent<Animator>().SetBool("Play", true);
        else Level2.GetComponent<Animator>().SetBool("Play", false);
    }

    public void MoveLevel3()
    {
        play3 = !play3;
        if (play3) Level3.GetComponent<Animator>().SetBool("Play", true);
        else Level3.GetComponent<Animator>().SetBool("Play", false);
    }
}
