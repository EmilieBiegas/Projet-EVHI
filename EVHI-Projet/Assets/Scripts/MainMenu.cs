// using System.Collections;
// using System.Collections.Generic;
//using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
// using System;
// using System.IO;

// Classe permettant de gérer le menu principal
public class MainMenu : MonoBehaviour
{
    public GameObject vocabUt; // Pour la sauvegarde des données statistiques qui s'effectue lorsque l'on revient au menu depuis une question
    public GameObject quizManager; // Pour la sauvegarde des données d'initialisation qui s'effectue lorsque l'on revient au menu depuis une question
    private const int nbMotVocab = 6; // PB Le nombre de mots de vocabulaires disponibles
    public void PlayGame(int NumJ){ // Associé au bouton jouer
        PlayerPrefs.SetInt("NumJoueur", NumJ); // On précise quel joueur joue pour pouvoir récupérer ses données et définir les données de ce joueur
        SceneManager.LoadScene("QCM"); //On load la scène des QCM à 4 choix
    }

    public void PlayGameApres(){ // Utilisé lorsque l'on accède au jeu par le menu de changement de pseudo (on a donc déjà mis à jour NumJoueur en accédant au menu de pseudo)
        SceneManager.LoadScene("QCM"); //On load la scène des QCM à 4 choix
    }

    public void QuitGame(){ // Associé au bouton quitter
        Application.Quit();
    }

    public void GoMenuParametres(){ // Associé au bouton paramètres
        SceneManager.LoadScene("MenuParametres"); //On load la scène des paramètres
    }

    public void GoMenuUtilisateurs(){ // Lorsque l'on clique sur jouer depuis le menu principal, on choisit d'abord le profil que l'on souhaite jouer
        SceneManager.LoadScene("MenuUtilisateurs"); //On load la scène de menu utilisateurs 
    }

    public void BackMenu(){ // Associé au bouton menu accessible depuis un autre menu que le menu principal
        SceneManager.LoadScene("MainMenu"); //On load la scène de menu principal
    }

    public void BackMenuFromQuestion(){ // Appelée lorsque l'on retourne au menu principal depuis une question, on sauvegarde à ce moment les données du joueur
        UnityEngine.Debug.Log("DANS BACK MENU, ON SAVE DATA :");

        // On enregistre les données statistique utilisateur
        UserStats saveDataStat = new UserStats();
        // PB on doit enregistrer toutes les stats
        saveDataStat.probaAcquisition = vocabUt.GetComponents<VocabUtilisateur>()[0].probaAcquisition; // PB pq [0] ??
        saveDataStat.nbRencontres = vocabUt.GetComponents<VocabUtilisateur>()[0].nbRencontres;
        saveDataStat.dateDerniereRencontre = vocabUt.GetComponents<VocabUtilisateur>()[0].dateDerniereRencontre;
        // Sauvegarde des données de UserStats dans un fichier nommé Stats_Joueur suivi du numéro du joueur
        DataSaver.saveData(saveDataStat, "Stats_Joueur" + PlayerPrefs.GetInt("NumJoueur"));


        // On enregistre les données d'initialisation utilisateur
        UserInitialisation saveDataInit = new UserInitialisation();
        // PB on doit enregistrer toutes les stats
        saveDataInit.nbBienRep = quizManager.GetComponent<QuizManager>().nbBienRep;
        saveDataInit.nbMalRep = quizManager.GetComponent<QuizManager>().nbMalRep;
        saveDataInit.numIte  = quizManager.GetComponent<QuizManager>().numIte;
        saveDataInit.inInitialisation  = quizManager.GetComponent<QuizManager>().inInitialisation;
        saveDataInit.RepEntreeOK  = quizManager.GetComponent<QuizManager>().RepEntreeOK;
        saveDataInit.NbQuestAvantNouvelle  = quizManager.GetComponent<QuizManager>().NbQuestAvantNouvelle;
        saveDataInit.NbAncienneQuestion  = quizManager.GetComponent<QuizManager>().NbAncienneQuestion;
        saveDataInit.NbQuestionsTotales  = quizManager.GetComponent<QuizManager>().NbQuestionsTotales;
        saveDataInit.inQCM  = quizManager.GetComponent<QuizManager>().inQCM;
        saveDataInit.QuestionCourrante  = quizManager.GetComponent<QuizManager>().QuestionCourrante;
        saveDataInit.TypeQuestion  = quizManager.GetComponent<QuizManager>().TypeQuestion;
        saveDataInit.NbAncienneQuestionTemp  = quizManager.GetComponent<QuizManager>().NbAncienneQuestionTemp;
        // Sauvegarde des données de UserInitialisation dans un fichier nommé Initialisation_Joueur suivi du numéro du joueur
        DataSaver.saveData(saveDataInit, "Initialisation_Joueur" + PlayerPrefs.GetInt("NumJoueur"));

        // Affichage des données sauvegardées
        for (int i = 0; i < nbMotVocab; i++)
        {
            // UnityEngine.Debug.Log("Proba d'acquisition [" + i + "] =" + saveData.probaAcquisition[i]);
            // UnityEngine.Debug.Log("Nb rencontres [" + i + "] =" + saveData.nbRencontres[i]);
            if (saveDataStat.dateDerniereRencontre[i] != null) // PB Attention, dateDerniereRencontre peut être null ou vide ""
            {
                // DateTime copyDateDerniereRencontre = saveData.dateDerniereRencontre[i]; // Pour ne pas transformer saveData.dateDerniereRencontre[i] en string
                // UnityEngine.Debug.Log("Date derniere rencontres [" + i + "] =" + saveData.dateDerniereRencontre[i]);
                // UnityEngine.Debug.Log(saveData.dateDerniereRencontre[i] + " de type " + saveData.dateDerniereRencontre[i].GetType());
            }
        }

        //On load la scène de menu principal
        SceneManager.LoadScene("MainMenu"); 
    }
    
}
