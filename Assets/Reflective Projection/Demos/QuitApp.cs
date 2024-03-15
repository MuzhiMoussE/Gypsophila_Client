using UnityEngine;

namespace ReflectiveProjectionDemo
{
    public class QuitApp : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) {
                Quit();
            }
        }

        void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
