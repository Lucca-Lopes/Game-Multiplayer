using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResoluçaoJOgo : MonoBehaviour
{

    public void Resolution01()
    {
        Screen.SetResolution(1920, 1080, true);
    }
    public void Resolution02()
    {
        Screen.SetResolution(1366, 768, true);
    }
    public void Resolution03()
    {
        Screen.SetResolution(1280, 720, true);
    }
    public void Graficos01()
    {
        QualitySettings.SetQualityLevel(0, true);
    }
    public void Graficos02()
    {
        QualitySettings.SetQualityLevel(3, true);
    }
}
