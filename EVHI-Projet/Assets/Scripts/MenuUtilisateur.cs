// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
// using System.Diagnostics;
// using System;
using UnityEngine.UI;

public class MenuUtilisateur : MonoBehaviour
{
    public GameObject PanneauPseudo;
    public GameObject PopUpWarning;
    public GameObject PseudoSaisi;
    public GameObject ButtonJoueur1; // Le bouton de sélection du joueur PB à changer entre J1, J2 et J3
    public GameObject ButtonJoueur2; // Le bouton de sélection du joueur PB à changer entre J1, J2 et J3
    public GameObject ButtonJoueur3; // Le bouton de sélection du joueur PB à changer entre J1, J2 et J3
    private string PseudoUtilisateur;
    private int NumJoueur;

    // Enregistrer le pseudo de l'utilisateur et le mettre dans l'emplacement sélectionné

    // Appuyer sur Retour cache le panneau 
    public void GoMenuUtilisateur(){
        PanneauPseudo.SetActive(false);
        SetPseudos();
    }

    public void GoMenuPseudo(int NumJ){
        PanneauPseudo.SetActive(true);    
        // On fait en sorte que le champs de saisie soit automatiquement sélectionné (pas besoin de cliquer dessus)
        PseudoSaisi.GetComponent<InputField>().Select(); 
        PseudoSaisi.GetComponent<InputField>().onEndEdit.AddListener(delegate { inputBetValue(PseudoSaisi.GetComponent<InputField>()); });   
        NumJoueur = NumJ; // On précise quel joueur a défini son pseudo
        PlayerPrefs.SetInt("NumJoueur", NumJ); // On précise quel joueur joue pour pouvoir cliquer sur jouer dans ce menu sans problème
    }

    public void ValiderPseudo(){
        // On met à jour le pseudo du joueur en question
        if (NumJoueur == 1)
        {
            ButtonJoueur1.GetComponentInChildren<Text>().text = PseudoUtilisateur;
            PlayerPrefs.SetString("PseudoJ1", PseudoUtilisateur);
        }
        if (NumJoueur == 2)
        {
            ButtonJoueur2.GetComponentInChildren<Text>().text = PseudoUtilisateur;
            PlayerPrefs.SetString("PseudoJ2", PseudoUtilisateur);
        }
        if (NumJoueur == 3)
        {
            ButtonJoueur3.GetComponentInChildren<Text>().text = PseudoUtilisateur;
            PlayerPrefs.SetString("PseudoJ3", PseudoUtilisateur);
        }  
        Debug.Log("Pseudo choisi : " + PseudoUtilisateur);
        // On se rend sur la page de choix d'utilisateur
        GoMenuUtilisateur(); 
    }

    public void inputBetValue(InputField userInput)
    {
        PseudoUtilisateur = userInput.text;
    }

    public void SetPseudos(){
        // On met les pseudos précédemment sauvegardés
        if (PlayerPrefs.HasKey("PseudoJ1"))
        {
            ButtonJoueur1.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PseudoJ1");
        }else
        {
            ButtonJoueur1.GetComponentInChildren<Text>().text = "Joueur 1";
        }
        if (PlayerPrefs.HasKey("PseudoJ2"))
        {
            ButtonJoueur2.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PseudoJ2");
        }else
        {
            ButtonJoueur2.GetComponentInChildren<Text>().text = "Joueur 2";
        }
        if (PlayerPrefs.HasKey("PseudoJ3"))
        {
            ButtonJoueur3.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PseudoJ3");
        }else
        {
            ButtonJoueur3.GetComponentInChildren<Text>().text = "Joueur 3";
        }       
    }

    public void AffichePopUpWarning(){
        PopUpWarning.SetActive(true);
    }

    public void CachePopUpWarning(){
        PopUpWarning.SetActive(false);        
    }
    public void EffaceDonnees(){
        // On efface toutes les données du joueur en question
        DataSaver.deleteData("Joueur" + NumJoueur); // Efface les statistiques du joueur
        PlayerPrefs.DeleteKey("PseudoJ" + NumJoueur); // Efface son pseudo

        // On se rend sur la page de choix d'utilisateur
        CachePopUpWarning(); 
        GoMenuUtilisateur();
    }

    // Fonction appelée lors de l'ouverture de la scène MenuUtilisateurs
    void OnEnable()
    {
        SetPseudos();
    }
}

// PB enregistrer les pseudos des joueurs avec leurs données pour pvr les charger de nouveau
