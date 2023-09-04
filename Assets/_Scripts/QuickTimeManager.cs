using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickTimeManager : MonoBehaviour
{

    [SerializeField] List<GameObject> quickTimesDisponiveis;
    public int numeroTotalQTE;

    [HideInInspector] public bool QTEativo = false;
    [HideInInspector] public bool QTEpermitido = false;
    [HideInInspector] public int numeroAtualQTE = 0;

    private void Update()
    {
        if(QTEpermitido && !QTEativo)
        {
            if (numeroAtualQTE < numeroTotalQTE)
            {
                numeroAtualQTE++;
                StartCoroutine("InstanciarQTE");
            }
            else
                QTEpermitido = false;
            
        }
    }

    public void IniciarQTE()
    {
        QTEpermitido = true;
        numeroAtualQTE = 0;
    }

    IEnumerator InstanciarQTE()
    {
        QTEativo = true;
        var managerOBJ = GameObject.FindGameObjectWithTag("QTEManager");
        var indexQTE = Random.Range(0, quickTimesDisponiveis.Count);
        Instantiate(quickTimesDisponiveis[indexQTE], this.transform);
        var botoesQTE = quickTimesDisponiveis[indexQTE].GetComponentsInChildren<QteBotao>();
        yield return new WaitForSeconds(botoesQTE.Length * 2.5f + 1.0f);
        QTEativo = false;
    }

}
