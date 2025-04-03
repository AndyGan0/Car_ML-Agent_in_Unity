using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarTireTrailScript : MonoBehaviour
{
    public TrailRenderer[] tiremarks;


    public void startEmitting()
    {
        foreach (TrailRenderer tiremark in tiremarks)
        {
            tiremark.emitting = true;
        }
    }

    public void stopEmitting()
    {
        foreach (TrailRenderer tiremark in tiremarks)
        {
            tiremark.emitting = false;
        }
    }
}
