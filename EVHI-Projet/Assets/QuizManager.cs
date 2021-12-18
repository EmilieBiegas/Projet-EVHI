//using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Text;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private Color correctCol, wrongCol; 
    public List<QuestionReponse> QnA; //Liste des questions/réponses
    public GameObject[] options;
    public int QuestionCourrante; //Indice de la question courrante
    public Text TxtQuestion; 

    private void Start(){
        genererQuestion();
    }

    public void BienRepondu(){
        // PB options[QnA[QuestionCourrante].ReponseCorrecte].transform.GetChild(0).GetComponent<Button>().colors.normalColor = correctCol;
        // PB options[QnA[QuestionCourrante].ReponseCorrecte].transform.GetChild(0).GetComponent<Button>().GetComponent<Image>().color = correctCol;
        QnA.RemoveAt(QuestionCourrante); // PB on enlève la question courrante, nous on veut pouvoir revoir la question plus tard
        genererQuestion();
    }

    void genererReponses(){
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<ReponseScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[QuestionCourrante].Reponses[i];
            if(QnA[QuestionCourrante].ReponseCorrecte == i+1){ //+1 car i commence à 0 et ReponseCorrecte à 1
                options[i].GetComponent<ReponseScript>().isCorrect = true;
            } 
        }
    }

    void genererQuestion(){
        if(QnA.Count > 0){ // PB à enlever puisqu'on a tjr des questions nous, on ne les retire pas
            /** PB Choisir la question posée */
            QuestionCourrante = UnityEngine.Random.Range(0, QnA.Count); //Une question aléatoire entre 0 et le nb de questions
            TxtQuestion.text = QnA[QuestionCourrante].Question; //La question est bien celle sélectionnée
            genererReponses();
        }else
        {
            Debug.Log("Plus de question disponible");
        }
    }
}

//PB Enlever les Debug