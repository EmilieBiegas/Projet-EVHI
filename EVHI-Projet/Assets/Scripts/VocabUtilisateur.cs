using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocabUtilisateur : MonoBehaviour
{
    public float[] probaAcquisition; //PB = new float[5]{0,0,0,0,0}; //Un tableau de proba d'acquisition de la même taille que QnA de QuizManager ou myQr de CsvReader
    public int[] nbRencontres; //PB = new int[5]{0,0,0,0,0}; //Un tableau du nombre de fois où le mot a été rencontré de la même taille que QnA de QuizManager ou myQr de CsvReader
    private const int nbMotVocab = 6;

    // Start is called before the first frame update
    public void Initialise()
    {
        probaAcquisition = new float[nbMotVocab]{0,0,0,0,0,0};
        nbRencontres = new int[nbMotVocab]{0,0,0,0,0,0};

        // Debug.Log("Taille des probas = "+ probaAcquisition.Length);
        // Debug.Log("nbMotVocab=" + nbMotVocab);
    }

    //PB a utiliser selon les cas
    public void setProbaAcquisition(int indiceQuestion, float valProba){
        probaAcquisition[indiceQuestion] = valProba;
    }

    public void UpdateNbRencontres(int indiceQuestion){
        nbRencontres[indiceQuestion] += 1;
    }
}
