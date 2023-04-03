using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Customization : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Slider[] sliders;
    [SerializeField] private Material temp;

    private Material mat;
    private bool set;

    public void OnEnable()
    {
        if (!set)
        {
            mat = new(temp);
            set = true;
        }

        mat.color = new Color(FBPP.GetFloat("colorR") / 255, FBPP.GetFloat("colorG") / 255, FBPP.GetFloat("colorB") / 255);
        meshRenderer.material = mat;

        sliders[0].value = FBPP.GetFloat("colorR");
        sliders[1].value = FBPP.GetFloat("colorG");
        sliders[2].value = FBPP.GetFloat("colorB");
    }
    
    public void OnSliderChanged()
    {
        mat.color = new Color(sliders[0].value / 255, sliders[1].value / 255, sliders[2].value / 255);
        meshRenderer.material = mat;
    }

    public void Save()
    {
        FBPP.SetFloat("colorR", sliders[0].value);
        FBPP.SetFloat("colorG", sliders[1].value);
        FBPP.SetFloat("colorB", sliders[2].value);
    }
}
