using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFinishEvent : MonoBehaviour
{
    private Vector3 originPos;
    public List<GameObject> levelSwitches;

    public void MoveUp(float _distance)
    {
        originPos = gameObject.transform.position;
        gameObject.transform.position += new Vector3(0, _distance, 0);
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
}
