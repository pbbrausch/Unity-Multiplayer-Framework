using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    private void Awake()
    {
        FBPP.Start(new FBPPConfig());
    }
}

