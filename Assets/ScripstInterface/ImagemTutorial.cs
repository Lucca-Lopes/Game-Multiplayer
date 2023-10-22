using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagemTutorial : MonoBehaviour
{
    public float timerDuration = 30.0f; 
    public GameObject imageObject1;
    public GameObject imageObject2;
    public GameObject imageObject3;
    public GameObject imageObject4;


    private float timer = 0.0f;
    private bool imageShown1 = false;
    private bool imageShown2 = false;
    private bool imageShown3 = false;
    private bool imageShown4 = false;
    
    void Update()
    {
        timer += Time.deltaTime;
        
       // Debug.Log(timer);
        Imagem1();
        Imagem2();
        Imagem3();
        Imagem4();
       


    }

    void Start()
    {

        // Certifique-se de que o objeto da imagem está desativado no início.
        if (imageObject1 != null)
        {
            imageObject1.SetActive(false);
        }
        if (imageObject2 != null)
        {
            imageObject2.SetActive(false);
        }
        if (imageObject3 != null)
        {
            imageObject3.SetActive(false);
        }
        if (imageObject4 != null)
        {
            imageObject4.SetActive(false);
        }
    }
    public void Imagem1()
    {
        if (timer >= 2 && !imageShown1)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject1.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown1 = true;
        }
    }
    public void Imagem2()
    {
        if (timer >= 4 && !imageShown2)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject2.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown2 = true;
        }
    }
    public void Imagem3()
    {
        if (timer >= 6 && !imageShown3)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject3.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown3 = true;
        }
    }
    public void Imagem4()
    {
        if (timer >= 8 && !imageShown4)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject4.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown4 = true;
        }
    }
   
}
