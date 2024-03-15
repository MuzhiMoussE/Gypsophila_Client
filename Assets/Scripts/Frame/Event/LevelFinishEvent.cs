using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LevelFinishEvent : MonoBehaviour
{
    private Vector3 originPos;
    private Vector3 targetPos;
    public List<GameObject> levelSwitches;
    public List<GameObject> sightSwitches;
    public void MoveUp(float _distance)
    {
        originPos = gameObject.transform.position;
        targetPos = gameObject.transform.position + new Vector3(0, _distance, 0);
        transform.position = targetPos;
    }
    public void MoveDown(float _distance)
    {
        
    }
    public void ReturnOriginPosition()
    {
        gameObject.transform.position = originPos;
    }
    public void LevelFinishStateForSwitches()
    {
        foreach (var switches in levelSwitches)
        {
            switches.GetComponent<LevelSwitch>().LevelFinishState();
        }
    }
    public void LevelFinishStateForSights()
    {
        foreach (var switches in sightSwitches)
        {
            switches.GetComponent<SightSwitch>().LevelFinishState();
        }
    }
}
