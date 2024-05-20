using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCircle : MonoBehaviour
{
    public class particleClass
    {
        public float ini_radiu = 0.0f;//��ʼ���뾶
        public float collect_radiu = 0.0f;//���Ϻ�İ뾶
        public float now_radiu = 0.0f;//���ӵ�ǰʱ�̰뾶������������Сʱ�������߱Ƚ�

        public float angle = 0.0f;
        public particleClass(float radiu_, float angle_, float collect_)
        {
            ini_radiu = radiu_;
            angle = angle_;
            collect_radiu = collect_;
            now_radiu = radiu_;
        }
    }

    //��������ϵͳ��
    public ParticleSystem particleSystem;
    //��������
    private ParticleSystem.Particle[] particlesArray;
    //������������
    private particleClass[] particleAttr;
    public int particleNum = 12000;

    //�Ͽ�Ļ�������뾶
    public float outMinRadius = 5.0f;
    public float outMaxRadius = 10.0f;

    //��խ�Ļ�(��ȱ��)������뾶
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
            //��Ӧ��ʼ��������Ϊÿ���������ð뾶���Ƕ�
            float randomAngle;

            // �������ÿ�����Ӿ������ĵİ뾶��ͬʱ����Ҫ������ƽ���뾶����  
            float maxR, minR;

            if (i < particleNum * 5 / 12)//�ⲿ���������ڽϿ���Ǹ���
            {
                maxR = outMaxRadius;
                minR = outMinRadius;
                randomAngle = Random.Range(0.0f, 360.0f);
            }
            else//խ����ȱ�ڣ�����һ����0�ȼ��С�һ����180�ȼ��У��Ա���90�Ⱥ�-90���γ������Գ�ȱ��
            {
                maxR = inMaxRadius;
                minR = inMinRadius;
                float minAngle = Random.Range(-90f, 0.0f);
                float maxAngle = Random.Range(0.0f, 90f);
                float angle = Random.Range(minAngle, maxAngle);

                randomAngle = i % 2 == 0 ? angle : angle - 180;//���öԳ���������һ������
            }

            float midRadius = (maxR + minR) / 2;

            float min = Random.Range(minR, midRadius);

            float max = Random.Range(midRadius, maxR);

            float randomRadius = Random.Range(min, max);

            float collectRadius;

            //ע������ƽ���뾶��Χ��������Сʱ�ƶ��ľ�����һЩ
            if (randomRadius > midRadius)
                collectRadius = randomRadius - (randomRadius - midRadius) / 2;
            else
                collectRadius = randomRadius - (randomRadius - midRadius) * 3 / 4;

            //������������
            particleAttr[i] = new particleClass(randomRadius, randomAngle, collectRadius);
            particlesArray[i].position = new Vector3(randomRadius * Mathf.Cos(randomAngle), randomRadius * Mathf.Sin(randomAngle), 0.0f);
        }
        //��������
        particleSystem.SetParticles(particlesArray, particleNum);
    }


    void Update()
    {
        for (int i = 0; i < particleNum; i++)
        {
            //�����µĽǶ�
            if (i > particleNum * 5 / 12)
                speed = inputspeed;
            else
                speed = inputspeed/2;
            particleAttr[i].angle -= speed;
            particleAttr[i].angle = particleAttr[i].angle % 360;
            float rad = particleAttr[i].angle / 180 * Mathf.PI;

            //�ж��費��Ҫ���ţ��ı����ӵ��ݴ�뾶
            if (flag == 0)//��Ҫ���м�����
            {
                if (particleAttr[i].now_radiu > particleAttr[i].collect_radiu)
                {
                    //���㻷�������ٶȲ�ͬ
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
            else if (flag == 1)//����
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

            //�����µ�λ��
            particlesArray[i].position = new Vector3(particleAttr[i].now_radiu * Mathf.Cos(rad), particleAttr[i].now_radiu * Mathf.Sin(rad), 0f);
        }
        particleSystem.SetParticles(particlesArray, particleNum);
    }
}
