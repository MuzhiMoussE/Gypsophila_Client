using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuFunctions : MonoBehaviour
{
    public PlayableDirector director;
    public GameObject Menu;
    public Animator playerOriginAnim;
    public GameObject Settings;
    // Start is called before the first frame update
    void Start()
    {
        playerOriginAnim.speed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartNewGame()
    {
        Menu.GetComponent<Animator>().SetBool("Play", true);
        StartCoroutine(StartMenuAnim());
    }
    IEnumerator StartMenuAnim()
    {
        yield return new WaitForSeconds(1);
        director.enabled = true;
        director.Play();
    }
    public void ContinueGame()
    {
        
    }
    public void LoadLevelArchive()
    {
        ArchiveSystem.LoadLocalArchive();
        Menu.GetComponent<Animator>().SetBool("Play", true);
        StartCoroutine(StartMenuAnim());

    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ToSettings()
    {
        Settings.SetActive(true);
    }
}
