using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.UI;
using Utility;

public class SketchUtility : SingletonMonoBase<SketchUtility>
{
    //图片序列->sprite动画

    [SerializeField]private float frame_number = 15;
    private List<Sprite> sprites = new List<Sprite>();
    private SpriteRenderer sketchmanRenderer;

    //回调事件
    public UnityEvent onCompleteEvent;
    private int index;
    private float _time;
    private float waittim;
    private int loadIndex = 0;

    private void SetInit(GameObject gameobject)
    {
        sketchmanRenderer = gameobject.GetComponent<SpriteRenderer>();
        _time = 0;
        index = 0;
        waittim = 1 / frame_number;
    }
    private void Play()//update中
    {
        _time += Time.deltaTime;
        if (_time >= waittim)
        {
            _time = 0;
            index++;
            if (index >= sprites.Count)
            {
                index = 0;
                //此处可添加结束回调函数
            }
            sketchmanRenderer.sprite = sprites[index];
        }
    }

    /// <summary>
    /// 加载本地图片为Texture
    /// </summary>
    /// <param name="filePath"></param>
    private bool LoadTextureFromFile(string filePath)
    {
        if(!File.Exists(filePath))
        {
            loadIndex = 0;
            return false;
            
        }
        //创建数据流并加载图片
        FileStream files = new FileStream(filePath, FileMode.Open);

        byte[] imgByte = new byte[files.Length];
        files.Read(imgByte, 0, imgByte.Length);
        files.Close();

        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(imgByte);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        sprites.Add(sprite);
        return true;
    }

    private void LoadSprite()
    {
        while(LoadTextureFromFile(Application.streamingAssetsPath + "/AnimSequence/test1/frame_" + loadIndex+".png"))
        {
            loadIndex += 10;
        }
    }
    public void SketchInit(GameObject gameObject)
    {
        SetInit(gameObject);
        LoadSprite();
    }
    public void StartReleasing()
    {
        Play();
    }
}
