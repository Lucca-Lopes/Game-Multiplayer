using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Placar : MonoBehaviour
{
    public Text placarTexto;
    public int placar = 0;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        placarTexto.text = "Placar:" + placar;
        SomaPonto();
        SubtraiPlacar();
    }
    public void SomaPonto()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            placar++;
        }
    }
    public void SubtraiPlacar()
    {
        if( Input.GetKeyDown(KeyCode.C))
        {
            placar--;
        }
    }
}
