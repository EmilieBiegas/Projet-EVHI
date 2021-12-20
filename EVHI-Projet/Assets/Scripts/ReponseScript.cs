using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReponseScript : MonoBehaviour
{
    public bool isCorrect = false;
    public QuizManager quizManager;

    public void Reponse(){
        if(isCorrect){
            Debug.Log("Reponse correcte");
            //PB mettre bouton en vert
            quizManager.Repondu();
        }else
        {
            Debug.Log("Reponse incorrecte");
            //PB mettre bouton de la bonne réponse en vert
            quizManager.Repondu();
        }
    }
    
    //Pour revenir au menu principal lorsque l'on appuie sur le bouton "retour"
    public void BackMenu(){
        SceneManager.LoadScene("MainMenu"); //On load la scène de menu principal
    }
}
