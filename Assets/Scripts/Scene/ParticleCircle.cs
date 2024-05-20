using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCircle : MonoBehaviour
{
    public class particleClass
    {
        public float ini_radiu = 0.0f;//初始化半径
        public float collect_radiu = 0.0f;//集合后的半径
        public float now_radiu = 0.0f;//粒子当前时刻半径，用于扩大缩小时与上两者比较

        public float angle = 0.0f;
        public particleClass(float radiu_, float angle_, float collect_)
        {
            ini_radiu = radiu_;
            angle = angle_;
            collect_radiu = collect_;
            now_radiu = radiu_;
        }
    }

    //创建粒子系统，
    public ParticleSystem particleSystem;
    //粒子数组
    private ParticleSystem.Particle[] particlesArray;
    //粒子属性数组
    private particleClass[] particleAttr;
    public int particleNum = 12000;

    //较宽的环的内外半径
    public float outMinRadius = 5.0f;
    public float outMaxRadius = 10.0f;

    //较窄的环(带缺口)的内外半径
    public float inMinRadius = 6.0f;
    public float inMaxRadius = 9.0f;

    private float speed;
    public float inputspeed;
    public int flag;
    private void OnEnable()
    {
        if (particleSystem != null)
        {
            particleSystem.Play();
        }
    }
    void Start()
    {
        flag = -1;
        particleAttr = new particleClass[particleNum];
        particlesArray = new ParticleSystem.Particle[particleNum];
        particleSystem.maxParticles = particleNum;
        particleSystem.Emit(particleNum);
        particleSystem.GetParticles(particlesArray);
        for (int i = 0; i < particleNum; i++)
        {
            //相应初始化操作，为每个粒子设置半径，角度
            float randomAngle;

            // 随机产生每个粒子距离中心的半径，同时粒子要集中在平均半径附近  
            float maxR, minR;

            if (i < particleNum * 5 / 12)//这部分粒子属于较宽的那个环
            {
                maxR = outMaxRadius;
                minR = outMinRadius;
                randomAngle = Random.Range(0.0f, 360.0f);
            }
            else//窄环带缺口，设置一半向0度集中、一半向180度集中，以便在90度和-90度形成两个对称缺口
            {
                maxR = inMaxRadius;
                minR = inMinRadius;
                float minAngle = Random.Range(-90f, 0.0f);
                float maxAngle = Random.Range(0.0f, 90f);
                float angle = Random.Range(minAngle, maxAngle);

                randomAngle = i % 2 == 0 ? angle : angle - 180;//利用对称性设置另一半粒子
            }

            float midRadius = (maxR + minR) / 2;

            float min = Random.Range(minR, midRadius);

            float max = Random.Range(midRadius, maxR);

            float randomRadius = Random.Range(min, max);

            float collectRadius;

            //注意设置平均半径外围的粒子缩小时移动的距离少一些
            if (randomRadius > midRadius)
                collectRadius = randomRadius - (randomRadius - midRadius) / 2;
            else
                collectRadius = randomRadius - (randomRadius - midRadius) * 3 / 4;

            //粒子属性设置
            particleAttr[i] = new particleClass(randomRadius, randomAngle, collectRadius);
            particlesArray[i].position = new Vector3(randomRadius * Mathf.Cos(randomAngle), randomRadius * Mathf.Sin(randomAngle), 0.0f);
        }
        //设置粒子
        particleSystem.SetParticles(particlesArray, particleNum);
    }


    void Update()
    {
        for (int i = 0; i < particleNum; i++)
        {
            //根据新的角度
            if (i > particleNum * 5 / 12)
                speed = inputspeed;
            else
                speed = inputspeed/2;
            particleAttr[i].angle -= speed;
            particleAttr[i].angle = particleAttr[i].angle % 360;
            float rad = particleAttr[i].angle / 180 * Mathf.PI;

            //判断需不需要缩放，改变粒子的暂存半径
            if (flag == 0)//需要向中间收缩
            {
                if (particleAttr[i].now_radiu > particleAttr[i].collect_radiu)
                {
                    //两层环的收缩速度不同
                    if (i < particleNum * 5 / 12)
                        particleAttr[i].now_radiu -= 3.0f * Time.deltaTime;
                    else
                        particleAttr[i].now_radiu -= 4.0f * Time.deltaTime;
                }
                else if (particleAttr[i].now_radiu < particleAttr[i].collect_radiu)
                {
                    if (i < particleNum * 5 / 12)
                        particleAttr[i].now_radiu += 2.0f * Time.deltaTime;
                    else
                        particleAttr[i].now_radiu += Time.deltaTime;
                }
            }
            else if (flag == 1)//扩大
            {
                if (particleAttr[i].now_radiu < particleAttr[i].ini_radiu)
                {
                    if (i < particleNum * 5 / 12)
                        particleAttr[i].now_radiu += 2.0f * Time.deltaTime;
                    else
                        particleAttr[i].now_radiu += 3.0f * Time.deltaTime;
                }
                else if (particleAttr[i].now_radiu > particleAttr[i].ini_radiu)
                {
                    if (i < particleNum * 5 / 12)
                        particleAttr[i].now_radiu -= 4.0f * Time.deltaTime;
                    else
                        particleAttr[i].now_radiu -= 4.0f * Time.deltaTime;
                }
            }

            //粒子新的位置
            particlesArray[i].position = new Vector3(particleAttr[i].now_radiu * Mathf.Cos(rad), particleAttr[i].now_radiu * Mathf.Sin(rad), 0f);
        }
        particleSystem.SetParticles(particlesArray, particleNum);
    }
}
