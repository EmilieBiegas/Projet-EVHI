// using System.Collections;
// using System.Collections.Generic;
//using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // private int NumJoueur;
    // public GameObject menuUt; // PB c'était pour le log des pseudos
    public GameObject vocabUt; // PB ct pour la sauvegarde des données
    public void PlayGame(int NumJ){
        PlayerPrefs.SetInt("NumJoueur", NumJ); // On précise quel joueur joue //PB pour pvr récupérer ses données à lui
        SceneManager.LoadScene("QCM"); //On load la scène des QCM à 4 choix
    }

    public void PlayGameApres(){ // Utilisé lorsque l'on accède au jeu par le menu de changement de pseudo (on a donc déjà mis à jour NumJoueur)
        SceneManager.LoadScene("QCM"); //On load la scène des QCM à 4 choix
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void GoMenuParametres(){
        SceneManager.LoadScene("MenuParametres"); //On load la scène des paramètres
    }

    public void GoMenuUtilisateurs(){
        SceneManager.LoadScene("MenuUtilisateurs"); //On load la scène de menu utilisateurs 
        // menuUt.GetComponents<MenuUtilisateur>().SetPseudos(); // PB c'était pour le log des pseudos
    }

    public void BackMenu(){
        SceneManager.LoadScene("MainMenu"); //On load la scène de menu principal
    }

    public void BackMenuFromQuestion(){
        UnityEngine.Debug.Log("DANS BACK MENU, ON SAVE DATA :");

        // On enregistre les données utilisateur
        UserStats saveData = new UserStats();
        // saveData.vocabUtilisateur = gameObject.AddComponent(typeof(VocabUtilisateur)) as VocabUtilisateur; //new VocabUtilisateur(); // PB engendre le warning de new à un monobehaviour
        
        // PB on doit enregistrer toutes les stats
        saveData.probaAcquisition = vocabUt.GetComponents<VocabUtilisateur>()[0].probaAcquisition; // PB pq [0] ??
        saveData.nbRencontres = vocabUt.GetComponents<VocabUtilisateur>()[0].nbRencontres;

        // Sauvegarde des données de UserStats dans un fichier nommé Joueur suivi du numéro du joueur
        DataSaver.saveData(saveData, "Joueur" + PlayerPrefs.GetInt("NumJoueur"));

        // Affichage des données sauvegardées
        for (int i = 0; i < 6; i++)
        {
            // UnityEngine.Debug.Log("Proba d'acquisition [" + i + "] =" + saveData.probaAcquisition[i]);
            UnityEngine.Debug.Log("Nb rencontres [" + i + "] =" + saveData.nbRencontres[i]);
        }

        //On load la scène de menu principal
        SceneManager.LoadScene("MainMenu"); 
    }
    
}
