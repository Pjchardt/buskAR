using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataObject : MonoBehaviour
{
    public static DataObject Instance;

    public GameObject ARObjectToTrack;

    private void Awake()
    {
        Instance = this;
    }
}
