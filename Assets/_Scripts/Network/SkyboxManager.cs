using UnityEngine;

public class SkyboxManager : MonoBehaviour
{
    public float rotateSpeed = 1f;
    public bool rotateSkybox = true;

    void Update()
    {
        if (rotateSkybox)
        {
            RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotateSpeed);
        }
    }
}