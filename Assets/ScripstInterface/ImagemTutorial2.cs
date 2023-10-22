using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImagemTutorial2 : MonoBehaviour
{
    public float timerDuration = 30.0f;
    public GameObject imageObject5;
    public GameObject imageObject6;
    public GameObject imageObject7;
    public GameObject imageObject8;


    private float timer = 0.0f;
    private bool imageShown5 = false;
    private bool imageShown6 = false;
    private bool imageShown7 = false;
    private bool imageShown8 = false;

    void Update()
    {
        timer += Time.deltaTime;

        // Debug.Log(timer);
        Imagem5();
        Imagem6();
        Imagem7();
        Imagem8();



    }

    void Start()
    {

        // Certifique-se de que o objeto da imagem está desativado no início.
        if (imageObject5 != null)
        {
            imageObject5.SetActive(false);
        }
        if (imageObject6 != null)
        {
            imageObject6.SetActive(false);
        }
        if (imageObject7 != null)
        {
            imageObject7.SetActive(false);
        }
        if (imageObject8 != null)
        {
            imageObject8.SetActive(false);
        }
    }
    public void Imagem5()
    {
        if (timer >= 2 && !imageShown5)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject5.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown5 = true;
        }
    }
    public void Imagem6()
    {
        if (timer >= 4 && !imageShown6)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject6.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown6 = true;
        }
    }
    public void Imagem7()
    {
        if (timer >= 6 && !imageShown7)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject7.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown7 = true;
        }
    }
    public void Imagem8()
    {
        if (timer >= 8 && !imageShown8)
        {
            // Ative o objeto da imagem para torná-lo visível.
            imageObject8.SetActive(true);

            // Define a flag para indicar que a imagem foi exibida.
            imageShown8 = true;
        }
    }
}
