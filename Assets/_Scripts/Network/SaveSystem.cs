using UnityEngine;
using UnityEngine.Audio;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem instance;

    private void Awake()
    {
        if (instance != null) { Destroy(this); return; }
       
        instance = this;
        FBPP.Start(new FBPPConfig());
    }
}