using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RewardWallScript : MonoBehaviour
{


    private int NumberOfRewardWalls;

    

    void Start()
    {
        NumberOfRewardWalls = getNumberOfRewardWalls();
    }




    public int getNumberOfRewardWalls()
    {
        return transform.childCount;
    }


    public GameObject getRewardWallWithIndex(int index)
    {
        if (index < 0)  index = 0;
        else if (index >= NumberOfRewardWalls ) index = NumberOfRewardWalls - 1;
        return transform.GetChild(index).gameObject;
    }




}
