// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Classe permettant de gérer la réponse de l'utilisateur à un QCM
public class ReponseScript : MonoBehaviour
{
    public bool isCorrect; // Indique si la réponse indiquée par l'utilisateur est correcte ou non
    public QuizManager quizManager; // Gérant de quiz associé

    public void ReponseQCM(){ // Appellée lorsque l'utilisateur répond à un QCM
        if(isCorrect){
            Debug.Log("Reponse correcte");
            quizManager.ReponduQCM(true); // On traite la réponse de l'utilisateur dans le quizManager (affichage des couleurs etc.)
            quizManager.DecrementeNbQuestAvantNouvelle(); // On augmente l'approfondissement des connaissances
            quizManager.RepEntreeOK = true; // Indiquer dans quizManager que l'utilisateur a bien répondu
        }else
        {
            Debug.Log("Reponse incorrecte");
            quizManager.ReponduQCM(false); // On traite la réponse de l'utilisateur dans le quizManager (affichage des couleurs etc.)
            quizManager.IncrementeNbQuestAvantNouvelle(); // On augmente le renforcemment des connaissances
            quizManager.RepEntreeOK = false; // Indiquer dans quizManager que l'utilisateur a mal répondu
        }
    }
    
    //Pour revenir au menu principal lorsque l'on appuie sur le bouton "retour"
    public void BackMenu(){
        SceneManager.LoadScene("MainMenu"); //On load la scène de menu principal
    }
}
