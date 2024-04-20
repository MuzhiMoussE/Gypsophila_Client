using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneActions : MonoBehaviour
{
    AsyncOperation asyncLoad;
    // Start is called before the first frame update
    private void Awake()
    {
        ArchiveSystem.LoadScene(1);
        //StartCoroutine(LoadScene(1));
        //ArchiveSystem.SceneIndex = 1;
    }
    // Update is called once per frame
    private void OnEnable()
    {
        //asyncLoad.allowSceneActivation = true;
    }
    IEnumerator LoadScene(int index)
    {
        ArchiveSystem.SceneIndex = index;
        //获取加载对象
        SceneManager.LoadSceneAsync(index);
        //设置加载完成后不跳转
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
