using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsFunctions : MonoBehaviour
{
    public Slider FXVolume;
    public Slider BGMVolume;
    public AudioSource bgmVolume;
    public void ChangeFXVolume()
    {
        AudioManager.Instance.fxVolume = FXVolume.value;
    }
    public void ChangeBGMVolume()
    {
        bgmVolume.volume = BGMVolume.value;
    }
    public void CloseSettings()
    {
        gameObject.SetActive(false);
    }
}
