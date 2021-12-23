// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using UnityEngine.UI;

// Classe permettant de gérer le menu utilisateur
public class MenuUtilisateur : MonoBehaviour
{
    public GameObject PanneauPseudo; // La page de choix de pseudo / effacage de données
    public GameObject PopUpWarning; // Le pop-up qui apparaît lorsque l'on souhaite effacer des données utilisateur pour prévenir que c'est irréversible
    public GameObject PseudoSaisi; // Le champs de saisie du pseudo
    public GameObject ButtonJoueur1; // Le bouton de sélection du joueur 1
    public GameObject ButtonJoueur2; // Le bouton de sélection du joueur 2
    public GameObject ButtonJoueur3; // Le bouton de sélection du joueur 3
    private Stopwatch timerPseudo; // Le chronomètre permettant de chronomètrer le temps que l'utilisateur met pour entrer son pseudo
    private string PseudoUtilisateur; // Le pseudo choisi par l'utilisateur
    private int NumJoueur; // Le numéro du joueur en cours de jeu ou qui souhaite changer son pseudo (trouvable également dans les PlayerPrefs)

    public void GoMenuUtilisateur(){ //Retourner dans le menu utilisateur (pas dans le menu pseudos), appuyer sur Retour cache le panneau pseudo
        PanneauPseudo.SetActive(false); // On cache le panneau pseudo
        SetPseudos(); // On màj les pseudos
    }

    public void GoMenuPseudo(int NumJ){ // Permet d'accéder au menu pseudos
        PanneauPseudo.SetActive(true); // On affiche le panneau pseudo   

        // On lance le chronomètre
        timerPseudo = new Stopwatch();
        timerPseudo.Start();

        // On fait en sorte que le champs de saisie soit automatiquement sélectionné (pas besoin de cliquer dessus)
        PseudoSaisi.GetComponent<InputField>().Select(); 
        // On "écoute" la saisie de l'utilisateur
        PseudoSaisi.GetComponent<InputField>().onEndEdit.AddListener(delegate { inputBetValue(PseudoSaisi.GetComponent<InputField>()); });   
        NumJoueur = NumJ; // On précise quel joueur a défini son pseudo
        PlayerPrefs.SetInt("NumJoueur", NumJ); // On précise quel joueur joue pour pouvoir cliquer sur jouer dans ce menu sans problèmes
    }

    public void ValiderPseudo(){ // Appellée lorsque l'utilisateur valide son choix de pseudo
        // Calcul de la vitesse d'entrée de texte (de son pseudo) de l'utilisateur
        timerPseudo.Stop();
        TimeSpan timeTaken = timerPseudo.Elapsed;
        string foo = "Temps d'entrée de pseudo du joueur " + NumJoueur + " = " + timeTaken.ToString(@"m\:ss\.fff"); // PB s'en servir PB attention, timeTaken est un string mtn
        UnityEngine.Debug.Log(foo);

        // On met à jour le pseudo du joueur en question
        // Enregistrer le pseudo de l'utilisateur et le mettre dans l'emplacement sélectionné
        if (NumJoueur == 1)
        {
            ButtonJoueur1.GetComponentInChildren<Text>().text = PseudoUtilisateur;
            PlayerPrefs.SetString("PseudoJ1", PseudoUtilisateur); // Enregistrer le pseudo du joueur dans les PlayerPrefs pour pouvoir y accéder à chaque lancement de jeu
        }
        if (NumJoueur == 2)
        {
            ButtonJoueur2.GetComponentInChildren<Text>().text = PseudoUtilisateur;
            PlayerPrefs.SetString("PseudoJ2", PseudoUtilisateur); // Enregistrer le pseudo du joueur dans les PlayerPrefs pour pouvoir y accéder à chaque lancement de jeu
        }
        if (NumJoueur == 3)
        {
            ButtonJoueur3.GetComponentInChildren<Text>().text = PseudoUtilisateur;
            PlayerPrefs.SetString("PseudoJ3", PseudoUtilisateur); // Enregistrer le pseudo du joueur dans les PlayerPrefs pour pouvoir y accéder à chaque lancement de jeu
        }  
        UnityEngine.Debug.Log("Pseudo choisi : " + PseudoUtilisateur);
        // On se rend sur la page de choix d'utilisateur
        GoMenuUtilisateur(); 
    }

    public void inputBetValue(InputField userInput) // Fonction permettant de lire la saisie de l'utilisateur
    {
        PseudoUtilisateur = userInput.text;
    }

    public void SetPseudos(){ // Fonction permettant d'afficher les bons pseudos (grâce à la sauvegarde des pseudos dans les PlayerPrefs)
        // On affiche les pseudos précédemment sauvegardés
        if (PlayerPrefs.HasKey("PseudoJ1"))
        {
            ButtonJoueur1.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PseudoJ1");
        }else
        {
            ButtonJoueur1.GetComponentInChildren<Text>().text = "Joueur 1"; // Affichage par défaut
        }
        if (PlayerPrefs.HasKey("PseudoJ2"))
        {
            ButtonJoueur2.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PseudoJ2");
        }else
        {
            ButtonJoueur2.GetComponentInChildren<Text>().text = "Joueur 2"; // Affichage par défaut
        }
        if (PlayerPrefs.HasKey("PseudoJ3"))
        {
            ButtonJoueur3.GetComponentInChildren<Text>().text = PlayerPrefs.GetString("PseudoJ3");
        }else
        {
            ButtonJoueur3.GetComponentInChildren<Text>().text = "Joueur 3"; // Affichage par défaut
        }       
    }

    public void AffichePopUpWarning(){ // Fonction permettant d'afficher le pop-up warning de l'écrasement des données utilisateur
        PopUpWarning.SetActive(true);
    }

    public void CachePopUpWarning(){ // Fonction permettant de cacher le pop-up warning de l'écrasement des données utilisateur
        PopUpWarning.SetActive(false);        
    }
    
    public void EffaceDonnees(){ // Associée au bouton d'écrasement des données utilisateur
        // On efface toutes les données du joueur en question
        DataSaver.deleteData("Stats_Joueur" + NumJoueur); // Efface les statistiques du joueur
        DataSaver.deleteData("Initialisation_Joueur" + NumJoueur); // Efface les données d'initialisation du joueur
        PlayerPrefs.DeleteKey("PseudoJ" + NumJoueur); // Efface son pseudo

        // On se rend sur la page de choix d'utilisateur (en cachant le warning)
        CachePopUpWarning(); 
        GoMenuUtilisateur();
    }

    // Fonction appelée lors de l'ouverture de la scène MenuUtilisateurs
    void OnEnable()
    {
        SetPseudos(); // On affiche les bons pseudos lorsque l'on ouvre le menu utilisateurs
    }

    void Update()
    {
        //Detecter lorsqu'on appui sur la touche entrée pour valider le pseudo entré
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ValiderPseudo();                
        }
        
    }
}
