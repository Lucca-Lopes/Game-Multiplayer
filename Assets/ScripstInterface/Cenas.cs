using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Cenas : MonoBehaviour
{
    [SerializeField] private GameObject painelDesenvolvedor;
    [SerializeField] private GameObject painelMenuInicial;

    void Update()
    {
        AbrirPainel();
    }
    public void AbrirPainel()
    {
       if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
        {
            painelDesenvolvedor.SetActive(true);
        }    
    }
    public void AbreMenu()
    {
        painelDesenvolvedor.SetActive(false);
        painelMenuInicial.SetActive(true);
    }
    public void AbreCena1()
    {
        Debug.Log("A cena 1 foi aberta");
    }
    public void AbreCena2()
    {
        Debug.Log("A cena 2 foi aberta");
    }
    public void AbreCena3()
    {
        Debug.Log("A cena 3 foi aberta");
    }
    public void AbreCena4()
    {
        Debug.Log("A cena 4 foi aberta");
    }
    public void AbreCena5()
    {
        Debug.Log("A cena 5 foi aberta");

    }
    public void AbreCena6()
    {
        Debug.Log("A cena 6 foi aberta");
    }
    public void AbreCena7()
    {
        Debug.Log("A cena 7 foi aberta");
    }
    public void AbreCena8()
    {
        Debug.Log("A cena 8 foi aberta");
    }
}
