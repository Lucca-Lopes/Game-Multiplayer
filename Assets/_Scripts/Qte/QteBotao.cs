using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QteBotao : MonoBehaviour
{
    [SerializeField] List<GameObject> previousObjects;
    [SerializeField] GameObject resultTxt, resultTxtParent;
    [SerializeField] Color missColor, okColor, greatColor;
    public float maxTimer, timer = 0;
    [Range(0, 1)]
    [SerializeField] float speed;
    [SerializeField] Color test, start, original, tap, disappear;
    [SerializeField] Image img;
    [SerializeField] TMP_Text txt;
    [SerializeField] Button button;
    QteStreak streak;
    float timer1 = 0, timer2 = 0;
    private bool isClicked = false;
    public float tempoParaDesaparecer = 5f; // Tempo em segundos para o objeto desaparecer
    private float tempoDecorrido = 0f;
    void Awake()
    {
        streak = new QteStreak();
    }

    void Update()
    {
        if (!button.IsInteractable())
            return;

        if (timer < maxTimer)
            timer += Time.deltaTime * speed;

        // Atualize o tempo decorrido
        tempoDecorrido += Time.deltaTime;

        // Verifique se o tempo decorrido atingiu o tempo para desaparecer
        if (tempoDecorrido >= tempoParaDesaparecer)
        {
            // Faça o objeto desaparecer (você pode destruí-lo ou desativá-lo)
            gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonDown(0) && !isClicked)
        {
            // Lance um raio a partir da posição do mouse para verificar se o objeto foi clicado.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Verifique se o objeto clicado é o mesmo objeto gerado durante o QTE.
                if (hit.collider.gameObject == gameObject)
                {
                    // Marque o objeto como clicado para evitar cliques repetidos.
                    isClicked = true;

                    // Faça o objeto desaparecer (você pode destruí-lo ou desativá-lo, dependendo da sua lógica).
                    gameObject.SetActive(false);

                    // Somar pontos ao QteStreak.
                    streak.AddStreak(1.0); // Você pode ajustar a quantidade de pontos conforme necessário.
                }
            }
        }

        if (!button.IsInteractable())
            return;

        if (timer < maxTimer)
            timer += Time.deltaTime * speed;

        if (timer < maxTimer * 1 / 5)
        {
            img.color = Color.Lerp(start, original, (timer / (maxTimer * 1 / 5)));
            txt.color = Color.Lerp(start, Color.black, timer / (maxTimer * 1 / 5));
            timer1 = timer;
        }
        else if (timer >= maxTimer * 1 / 2 && timer < maxTimer * 5 / 8)
        {
            img.color = Color.Lerp(original, tap, Mathf.Abs((timer - (maxTimer * 1 / 2)) / ((maxTimer * 5 / 8) - (maxTimer * 1 / 2))));
        }
        else if (timer >= maxTimer * 9 / 10)
        {
            //print($"{} of {}");
            img.color = Color.Lerp(tap, disappear, Mathf.Abs((timer - (maxTimer * 9 / 10)) / (maxTimer - (maxTimer * 9 / 10))));
            txt.color = Color.Lerp(Color.black, disappear, Mathf.Abs((timer - (maxTimer * 9 / 10)) / (maxTimer - (maxTimer * 9 / 10))));
        }
        if (timer >= maxTimer)
        {
            streak.ResetStreak();
            if (button.IsInteractable())
            {
                GameObject floatingTxt = Instantiate(resultTxt, this.gameObject.transform.position, Quaternion.identity, resultTxtParent.transform);
                floatingTxt.GetComponent<TMP_Text>().text = "Perdeu!";
                floatingTxt.GetComponent<TMP_Text>().color = missColor;
                Destroy(floatingTxt, 0.5f);
            }
            button.interactable = false;
        }
    }



    public void Tap()
    {
        button.interactable = false;
        img.color = disappear;
        txt.color = disappear;

        bool foundActivePo = false;
        foreach (GameObject po in previousObjects)
        {
            if (po.activeSelf && po.GetComponent<Button>().IsInteractable())
            {
                foundActivePo = true;
                po.GetComponent<QteBotao>().timer = maxTimer;
            }
        }
        if (foundActivePo)
        {
            streak.ResetStreak();
            foundActivePo = false;
        }

        if (timer >= maxTimer * 1 / 2 && timer < maxTimer * 9 / 10)
        {
            streak.AddStreak(1.25);

            GameObject floatingTxt = Instantiate(resultTxt, this.gameObject.transform.position, Quaternion.identity, resultTxtParent.transform);
            floatingTxt.GetComponent<TMP_Text>().text = "Ótimo";
            floatingTxt.GetComponent<TMP_Text>().color = greatColor;
            Destroy(floatingTxt, 0.5f);

        }
        else
        {
            streak.AddStreak(0.75);

            GameObject floatingTxt = Instantiate(resultTxt, this.gameObject.transform.position, Quaternion.identity, resultTxtParent.transform);
            floatingTxt.GetComponent<TMP_Text>().text = "Ok";
            floatingTxt.GetComponent<TMP_Text>().color = okColor;
            Destroy(floatingTxt, 0.5f);
        }
    }
}
