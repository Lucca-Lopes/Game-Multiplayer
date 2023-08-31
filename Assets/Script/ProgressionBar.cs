using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressionBar : MonoBehaviour
{
    public float duration = 20.0f;
    private float elapsedtime = 0.0f;
    public Image progressBar;
    
    private void Start()
    {
        progressBar = GetComponent<Image>();
    }

    // Update is called once per frame
   private  void Update()
    {
        if(elapsedtime < duration) 
        {
            elapsedtime += Time.deltaTime;
            float fillAmount =elapsedtime / duration;
            progressBar.fillAmount = fillAmount;
        }
    }
}
