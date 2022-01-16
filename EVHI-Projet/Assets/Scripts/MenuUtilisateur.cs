using System.Collections;
using System.Collections.Generic;
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
    public Text textScoreJ1; // Le texte de score du joueur 1
    public Text textScoreJ2; // Le texte de score du joueur 2
    public Text textScoreJ3; // Le texte de score du joueur 3
    private Stopwatch timerPseudo; // Le chronomètre permettant de chronomètrer le temps que l'utilisateur met pour entrer son pseudo
    private string PseudoUtilisateur; // Le pseudo choisi par l'utilisateur
    private int NumJoueur; // Le numéro du joueur en cours de jeu ou qui souhaite changer son pseudo (trouvable également dans les PlayerPrefs)
    private Stopwatch timerSelection; // Le chronomètre permettant de mesurer le temps de sélection de l'utilisateur dans le menu
    private bool ChronoSelEnMarche; // Booléen qui indique si le chronomètre de sélection est en marche (true), ou en pause (false)
    private bool ChronoPseudoEnMarche; // Booléen qui indique si le chronomètre d'entrée de pseudo est en marche (true), ou en pause (false)
    private bool demarragePseudo; // Booléen qui indique si l'utilisateur est en train d'entrer son pseudo ou non
    public void GoMenuUtilisateur(){ //Retourner dans le menu utilisateur (pas dans le menu pseudos), appuyer sur Retour cache le panneau pseudo
        PanneauPseudo.SetActive(false); // On cache le panneau pseudo
        SetPseudos(); // On màj les pseudos
        SetScores(); // On affiche les bons scores lorsque l'on ouvre le menu utilisateurs
    }

    public void GoMenuPseudo(int NumJ){ // Permet d'accéder au menu pseudos
        // On arrête le chronomètre de sélection
        timerSelection.Stop();
        TimeSpan timeTaken = timerSelection.Elapsed; // On regarde le temps passé sur le chronomètre de sélection

        // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées) afin de l'enregistrer
        int days, hours, minutes, seconds, milliseconds;
        days = timeTaken.Days;
        hours = timeTaken.Hours;
        minutes = timeTaken.Minutes;
        seconds = timeTaken.Seconds;
        milliseconds = timeTaken.Milliseconds;
        // Temps passé en secondes :
        float floatTimeSpan = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
        // UnityEngine.Debug.Log("Temps de sélection du bouton changement de pseudo : " + floatTimeSpan + " secondes");

        PanneauPseudo.SetActive(true); // On affiche le panneau pseudo   

        // On lance le chronomètre de saisie de pseudo
        timerPseudo = new Stopwatch();
        ChronoPseudoEnMarche = true;
        demarragePseudo = true;
        timerPseudo.Start();

        // On fait en sorte que le champs de saisie soit automatiquement sélectionné (pas besoin de cliquer dessus)
        PseudoSaisi.GetComponent<InputField>().Select(); 
        // On "écoute" la saisie de l'utilisateur
        PseudoSaisi.GetComponent<InputField>().onEndEdit.AddListener(delegate { inputBetValue(PseudoSaisi.GetComponent<InputField>()); });   
        NumJoueur = NumJ; // On précise quel joueur a défini son pseudo
        PlayerPrefs.SetInt("NumJoueur", NumJ); // On précise quel joueur joue pour pouvoir associer les actions à ce joueur en particulier, et pour pouvoir cliquer sur jouer dans ce menu sans problèmes 

        // On enregistre le temps obtenu et on le sauvegarde
        UpdateTempsSelectionMenu(floatTimeSpan);
    }

    public void ValiderPseudo(){ // Appellée lorsque l'utilisateur valide son choix de pseudo
        // Calcul de la vitesse d'entrée de texte (de son pseudo) de l'utilisateur
        timerPseudo.Stop();
        demarragePseudo = false;
        TimeSpan timeTaken = timerPseudo.Elapsed;

        // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées) afin de l'enregistrer dans le gérant de l'hésitation
        int days, hours, minutes, seconds, milliseconds;
        days = timeTaken.Days;
        hours = timeTaken.Hours;
        minutes = timeTaken.Minutes;
        seconds = timeTaken.Seconds;
        milliseconds = timeTaken.Milliseconds;
        // Temps passé en secondes :
        float floatTimeSpan = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
        
        PlayerPrefs.SetFloat("TmpsEntreePseudoJ" + NumJoueur, floatTimeSpan); // On ajoute le temps de saisie du pseudo du joueur en cours de jeu pour pouvoir initialiser le temps d'entrée de texte
        
        string foo = "Temps d'entrée de pseudo du joueur " + NumJoueur + " = " + timeTaken.ToString(@"m\:ss\.fff"); // PB attention, timeTaken est un string mtn
        // UnityEngine.Debug.Log(foo);

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
        // UnityEngine.Debug.Log("Pseudo choisi : " + PseudoUtilisateur);
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

    public void SetScores(){ // Fonction permettant d'afficher les bons scores (grâce à la sauvegarde des scores dans les PlayerPrefs)
        // On affiche les scores précédemment sauvegardés
        if (PlayerPrefs.HasKey("ScoreJ1"))
        {
            textScoreJ1.text = PlayerPrefs.GetFloat("ScoreJ1").ToString();
        }else
        {
            textScoreJ1.text = "0"; // Affichage par défaut
        }
        if (PlayerPrefs.HasKey("ScoreJ2"))
        {
            textScoreJ2.text = PlayerPrefs.GetFloat("ScoreJ2").ToString();
        }else
        {
            textScoreJ2.text = "0"; // Affichage par défaut
        }
        if (PlayerPrefs.HasKey("ScoreJ3"))
        {
            textScoreJ3.text = PlayerPrefs.GetFloat("ScoreJ3").ToString();
        }else
        {
            textScoreJ3.text = "0"; // Affichage par défaut
        }       
    }

    public void AffichePopUpWarning(){ // Fonction permettant d'afficher le pop-up warning de l'écrasement des données utilisateur
        PopUpWarning.SetActive(true);
    }

    public void CachePopUpWarning(){ // Fonction permettant de cacher le pop-up warning de l'écrasement des données utilisateur
        PopUpWarning.SetActive(false);        
    }

    public void UpdateTempsSelectionMenu(float tmpsSelection){
        // On sauvegarde les données de temps de sélection dans les menus de ce joueur
        // On récupère l'ancienne liste de tempsSelectionMenu et on la complète avec les données récupérées
        var tempsSelectionMenu = new List<float>(); // Une liste de temps de sélection dans les menus permettant d'initialiser le niveau de l'utilisateur en terme de temps de sélection
    

        // On charge l'ancienne liste de tempsSelectionMenu du joueur concerné 
        UserSelectionMenu loadedDataSelection = DataSaver.loadData<UserSelectionMenu>("SelectionMenuJ" + PlayerPrefs.GetInt("NumJoueur"));

        if (loadedDataSelection == null || EqualityComparer<UserSelectionMenu>.Default.Equals(loadedDataSelection, default(UserSelectionMenu)))
        {
            // UnityEngine.Debug.Log("PAS DE DATA A LOAD SUR LES TEMPS DE SELECTION DANS LES MENUS");
        }else
        {
            tempsSelectionMenu = loadedDataSelection.tempsSelectionMenu;
            // UnityEngine.Debug.Log("Liste tempsSelectionMenu chargée (MenuUtilisateur) = " + tempsSelectionMenu + " de taille " + tempsSelectionMenu.Count);
        }

        // On ajoute les données récupérées à ce tour
        tempsSelectionMenu.Add(tmpsSelection);
        // UnityEngine.Debug.Log("Liste tempsSelectionMenu Concaténée (MenuUtilisateur) = " + tempsSelectionMenu + " de taille " + tempsSelectionMenu.Count);

        UserSelectionMenu saveDataSelectionMenu = new UserSelectionMenu();
        // PB on doit enregistrer toutes les stats
        saveDataSelectionMenu.tempsSelectionMenu = new List<float>();
        saveDataSelectionMenu.tempsSelectionMenu = tempsSelectionMenu;
        // Sauvegarde des données de selection dans les menus dans un fichier nommé SelectionMenuJ suivi du numéro du joueur
        DataSaver.saveData(saveDataSelectionMenu, "SelectionMenuJ" + PlayerPrefs.GetInt("NumJoueur"));
    }
    
    public void EffaceDonnees(){ // Associée au bouton d'écrasement des données utilisateur
        // On efface toutes les données du joueur en question
        DataSaver.deleteData("Stats_Joueur" + NumJoueur); // Efface les statistiques du joueur
        DataSaver.deleteData("Initialisation_Joueur" + NumJoueur); // Efface les données d'initialisation du joueur
        DataSaver.deleteData("TracesSelectJ" + NumJoueur); // Efface les traces de sélection du joueur
        DataSaver.deleteData("TracesTextJ" + NumJoueur); // Efface les traces d'entrée de texte du joueur
        DataSaver.deleteData("SelectionMenuJ" + NumJoueur); // Efface les données de selection dans les menus du joueur
        PlayerPrefs.DeleteKey("PseudoJ" + NumJoueur); // Efface son pseudo
        PlayerPrefs.DeleteKey("ScoreJ" + NumJoueur); // Efface son score
        PlayerPrefs.DeleteKey("TmpsEntreePseudoJ" + NumJoueur); // Efface le temps de saisie de son pseudo

        // On se rend sur la page de choix d'utilisateur (en cachant le warning)
        CachePopUpWarning(); 
        GoMenuUtilisateur();
    }

    // Fonction appelée lors de l'ouverture de la scène MenuUtilisateurs
    void OnEnable()
    {
        SetPseudos(); // On affiche les bons pseudos lorsque l'on ouvre le menu utilisateurs
        SetScores(); // On affiche les bons scores lorsque l'on ouvre le menu utilisateurs

        // On lance le chronomètre pour calculer la vitesse de selection de l'utilisateur
        timerSelection = new Stopwatch();
        ChronoSelEnMarche = true;
        ChronoPseudoEnMarche = false;
        demarragePseudo = false;
        timerSelection.Start();
    }

    void Update() // Fonction appelée toute les frames
    {
        //Detecter lorsqu'on appui sur la touche entrée pour valider le pseudo entré
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ValiderPseudo();                
        }

        // Si l'utilisateur ne regarde plus l'écran et que le chronomètre est en marche, on arrête le chronomètre
        if(ChronoSelEnMarche == true && PlayerPrefs.GetInt("ChronosEnPause") == 1){
            timerSelection.Stop();
            ChronoSelEnMarche = false;
        }
        // Lorsque l'utilisateur regarde de nouveau l'écran et que le chronomètre est arrêté, on relance le chronomètre     
        if(ChronoSelEnMarche == false && PlayerPrefs.GetInt("ChronosEnPause") == 0){
            timerSelection.Start(); // On reprend le chronomètre là où il s'était arrêté
            ChronoSelEnMarche = true;
        }   

        // Si l'utilisateur ne regarde plus l'écran et que le chronomètre est en marche, on arrête le chronomètre
        if(ChronoPseudoEnMarche == true && PlayerPrefs.GetInt("ChronosEnPause") == 1){
            timerPseudo.Stop();
            ChronoPseudoEnMarche = false;
        }
        // Lorsque l'utilisateur regarde de nouveau l'écran et que le chronomètre est arrêté, on relance le chronomètre     
        if(ChronoPseudoEnMarche == false && demarragePseudo == true && PlayerPrefs.GetInt("ChronosEnPause") == 0){
            timerPseudo.Start(); // On reprend le chronomètre là où il s'était arrêté
            ChronoPseudoEnMarche = true;
        }       
    }
}
