using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
public class SketchUtility : MonoBehaviour
{
    RawImage rawImage;
    List<UniGif.GifTexture> gifTextures;
    //当前播放的索引
    int index = -1;
    //计时器
    float timer = 0;


    private void Awake()
    {
        //获取组件
        rawImage = GetComponent<RawImage>();
        //读取图片
        string gifpath = Application.dataPath + "/Demo.gif";
        Stream stream = new FileStream(gifpath, FileMode.Open, FileAccess.Read);
        byte[] bt = new byte[stream.Length];
        stream.Read(bt, 0, (int)stream.Length);

        //使用工具   协程
        StartCoroutine(UniGif.GetTextureListCoroutine(bt, Lodinggifpic));
    }


    //得到图片后调用   
    private void Lodinggifpic(List<UniGif.GifTexture> list, int count, int width, int hight)
    {
        //把得到的数组存起来
        gifTextures = list;
        index = 0;
    }


    private void Update()
    {
        //说明还没加载完，就什么也不干
        if (index == -1)
        {
            return;
        }
        //加载完后
        timer += Time.deltaTime;
        if (timer >= 0.05f)
        {
            //计时器归零
            timer = 0;
            //要处理越界问题   如果最后一帧 == list中的最后一张,则重新播放
            if (index > gifTextures.Count - 1)
            {
                index = 0;
            }
            else
            {
                //显示图片
                rawImage.texture = gifTextures[index++].m_texture2d;
            }
        }

    }
}
