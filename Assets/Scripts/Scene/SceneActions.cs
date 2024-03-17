using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneActions : MonoBehaviour
{
    public Slider slider;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnEnable()
    {
        ChangeScene(1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeScene(int _sid)
    {
        slider.gameObject.SetActive(true);
        LoadNextLeaver();
    }
    public void LoadNextLeaver()
    {
        StartCoroutine(LoadLeaver());
    }
    IEnumerator LoadLeaver()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1); //获取当前场景并加一
                                                                                                              //operation.allowSceneActivation = false;
        while (!operation.isDone)   //当场景没有加载完毕
        {
            slider.value = operation.progress;  //进度条与场景加载进度对应
            yield return null;
        }
    }
}
