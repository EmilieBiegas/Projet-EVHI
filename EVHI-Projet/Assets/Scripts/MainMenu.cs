using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Diagnostics;

// Classe permettant de gérer le menu principal
public class MainMenu : MonoBehaviour
{
    public VocabUtilisateur vocabUt; // Pour la sauvegarde des données statistiques qui s'effectue lorsque l'on revient au menu depuis une question
    public HesitationManager hesitationManager; // Pour la sauvegarde des données d'hésitation qui s'effectue lorsque l'on revient au menu depuis une question
    public QuizManager quizManager; // Pour la sauvegarde des données d'initialisation qui s'effectue lorsque l'on revient au menu depuis une question
    private Stopwatch timer; // Le chronomètre permettant de mesurer le temps de sélection de l'utilisateur dans le menu
    private bool ChronoEnMarche; // Booléen qui indique si le chronomètre est en marche (true), ou en pause (false)

    public void PlayGame(int NumJ){ // Associé au bouton jouer
        PlayerPrefs.SetInt("NumJoueur", NumJ); // On précise quel joueur joue pour pouvoir récupérer ses données et définir les données de ce joueur
        
        // On sauvegarde les données de temps de sélection dans les menus de ce joueur
        // On récupère l'ancienne liste de tempsSelectionMenu et on la complète avec les données récupérées
        var tempsSelectionMenuTemp = new List<float>();

        // On charge l'ancienne liste de tempsSelectionMenu du joueur concerné 
        UserSelectionMenu loadedDataSelectionJ = DataSaver.loadData<UserSelectionMenu>("SelectionMenuJ" + PlayerPrefs.GetInt("NumJoueur"));

        if (loadedDataSelectionJ == null || EqualityComparer<UserSelectionMenu>.Default.Equals(loadedDataSelectionJ, default(UserSelectionMenu)))
        {
            // UnityEngine.Debug.Log("PAS DE DATA A LOAD SUR LES TEMPS DE SELECTION DANS LES MENUS POUR CE JOUEUR");
        }else
        {
            tempsSelectionMenuTemp = loadedDataSelectionJ.tempsSelectionMenu;
            // UnityEngine.Debug.Log("Liste tempsSelectionMenu chargée (MainMenu) = " + tempsSelectionMenuTemp + " de taille " + tempsSelectionMenuTemp.Count);
        }

        // On ajoute les données récupérées à ce tour
        // On charge la liste de tempsSelectionMenu de ce tour 
        UserSelectionMenu loadedDataSelection = DataSaver.loadData<UserSelectionMenu>("SelectionMenu");

        if (loadedDataSelection == null || EqualityComparer<UserSelectionMenu>.Default.Equals(loadedDataSelection, default(UserSelectionMenu)))
        {
            // UnityEngine.Debug.Log("PAS DE DATA A LOAD SUR LES TEMPS DE SELECTION DANS LES MENUS");
        }else
        {
            tempsSelectionMenuTemp.AddRange(loadedDataSelection.tempsSelectionMenu);
            // UnityEngine.Debug.Log("Liste tempsSelectionMenu concaténée (MainMenu) = " + tempsSelectionMenuTemp + " de taille " + tempsSelectionMenuTemp.Count);
        }
        
        // UnityEngine.Debug.Log("Liste Concaténée (MainMenu) = " + tempsSelectionMenuTemp + " de taille " + tempsSelectionMenuTemp.Count);

        UserSelectionMenu saveDataSelectionMenu = new UserSelectionMenu();
        // PB on doit enregistrer toutes les stats
        saveDataSelectionMenu.tempsSelectionMenu = new List<float>();
        saveDataSelectionMenu.tempsSelectionMenu = tempsSelectionMenuTemp;
        // Sauvegarde des données de TracesUtilisateur dans un fichier nommé SelectionMenuJ suivi du numéro du joueur
        DataSaver.saveData(saveDataSelectionMenu, "SelectionMenuJ" + PlayerPrefs.GetInt("NumJoueur"));

        SceneManager.LoadScene("QCM"); //On load la scène des QCM à 4 choix
    }

    public void PlayGameApres(){ // Utilisé lorsque l'on accède au jeu par le menu de changement de pseudo (on a donc déjà mis à jour NumJoueur en accédant au menu de pseudo)
        SceneManager.LoadScene("QCM"); //On load la scène des QCM à 4 choix
    }

    public void QuitGame(){ // Associé au bouton quitter
        // On arrête le chronomètre // PB ne sert à rien puisque l'on quitte le jeu à ce moment
        // timer.Stop();
        // TimeSpan timeTaken = timer.Elapsed; // On regarde le temps passé sur le chronomètre

        // // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées) afin de l'enregistrer
        // int days, hours, minutes, seconds, milliseconds;
        // days = timeTaken.Days;
        // hours = timeTaken.Hours;
        // minutes = timeTaken.Minutes;
        // seconds = timeTaken.Seconds;
        // milliseconds = timeTaken.Milliseconds;
        // // Temps passé en secondes :
        // float floatTimeSpan = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
        // UnityEngine.Debug.Log("Temps de sélection du bouton paramètre : " + floatTimeSpan + " secondes");

        // // On enregistre le temps obtenu
        // tempsSelectionMenu.Add(floatTimeSpan);

        Application.Quit();
    }

    public void GoMenuParametres(){ // Associé au bouton paramètres
        // On arrête le chronomètre
        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed; // On regarde le temps passé sur le chronomètre

        // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées) afin de l'enregistrer
        int days, hours, minutes, seconds, milliseconds;
        days = timeTaken.Days;
        hours = timeTaken.Hours;
        minutes = timeTaken.Minutes;
        seconds = timeTaken.Seconds;
        milliseconds = timeTaken.Milliseconds;
        // Temps passé en secondes :
        float floatTimeSpan = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
        // UnityEngine.Debug.Log("Temps de sélection du bouton paramètre : " + floatTimeSpan + " secondes");

        // On enregistre le temps obtenu
        UpdateTempsSelectionMenu(floatTimeSpan);

        SceneManager.LoadScene("MenuParametres"); //On load la scène des paramètres
    }

    public void GoMenuUtilisateurs(){ // Lorsque l'on clique sur jouer depuis le menu principal, on choisit d'abord le profil que l'on souhaite jouer
        // On arrête le chronomètre
        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed; // On regarde le temps passé sur le chronomètre

        // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées) afin de l'enregistrer
        int days, hours, minutes, seconds, milliseconds;
        days = timeTaken.Days;
        hours = timeTaken.Hours;
        minutes = timeTaken.Minutes;
        seconds = timeTaken.Seconds;
        milliseconds = timeTaken.Milliseconds;
        // Temps passé en secondes :
        float floatTimeSpan = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
        // UnityEngine.Debug.Log("Temps de sélection du bouton utilisateur : " + floatTimeSpan + " secondes");

        // On enregistre le temps obtenu
        UpdateTempsSelectionMenu(floatTimeSpan);

        SceneManager.LoadScene("MenuUtilisateurs"); //On load la scène de menu utilisateurs 
    }

    public void BackMenu(){ // Associé au bouton menu accessible depuis un autre menu que le menu principal
        SceneManager.LoadScene("MainMenu"); //On load la scène de menu principal
    }

    public void BackMenuFromQuestion(){ // Appelée lorsque l'on retourne au menu principal depuis une question, on sauvegarde à ce moment les données du joueur
        // On enregistre le score du joueur en cours de jeu
        PlayerPrefs.SetFloat("ScoreJ" + PlayerPrefs.GetInt("NumJoueur"), hesitationManager.Score());
        // On enregistre les données statistique utilisateur
        UserStats saveDataStat = new UserStats();
        // PB on doit enregistrer toutes les stats
        saveDataStat.probaAcquisition = vocabUt.probaAcquisition;
        saveDataStat.nbRencontres = vocabUt.nbRencontres;
        saveDataStat.dateDerniereRencontre = vocabUt.dateDerniereRencontre;
        saveDataStat.nivSelection = hesitationManager.nivSelection; 
        saveDataStat.nivEntreeTexte = hesitationManager.nivEntreeTexte;
        saveDataStat.occulaireHesite = hesitationManager.oculometreManager.occulaireHesite; // PB liste copy()?
        saveDataStat.occulaireSur = hesitationManager.oculometreManager.occulaireSur;
        // Sauvegarde des données de UserStats dans un fichier nommé Stats_Joueur suivi du numéro du joueur
        DataSaver.saveData(saveDataStat, "Stats_Joueur" + PlayerPrefs.GetInt("NumJoueur"));


        // On enregistre les données d'initialisation utilisateur
        UserInitialisation saveDataInit = new UserInitialisation();
        // PB on doit enregistrer toutes les stats
        saveDataInit.nbBienRep = quizManager.nbBienRep;
        saveDataInit.nbMalRep = quizManager.nbMalRep;
        saveDataInit.numIte  = quizManager.numIte;
        saveDataInit.inInitialisation  = quizManager.inInitialisation;
        saveDataInit.RepEntreeOK  = quizManager.RepEntreeOK;
        saveDataInit.NbQuestAvantNouvelle  = quizManager.NbQuestAvantNouvelle;
        saveDataInit.NbQuestAvantNouvelleTemp  = quizManager.NbQuestAvantNouvelleTemp;
        saveDataInit.NbAncienneQuestion  = quizManager.NbAncienneQuestion;
        saveDataInit.NbNouvelleQuestion  = quizManager.NbNouvelleQuestion;
        saveDataInit.NbQuestionsTotales  = quizManager.NbQuestionsTotales;
        saveDataInit.inQCM  = quizManager.inQCM;
        saveDataInit.QuestionCourrante  = quizManager.QuestionCourrante;
        saveDataInit.TypeQuestion  = quizManager.TypeQuestion;
        saveDataInit.NbAncienneQuestionTemp  = quizManager.NbAncienneQuestionTemp;
        saveDataInit.NbNouvelleQuestionTemp  = quizManager.NbNouvelleQuestionTemp;
        saveDataInit.IndQuestNonRencontrees = quizManager.IndQuestNonRencontrees; // PB copy de liste ??
        // Sauvegarde des données de UserInitialisation dans un fichier nommé Initialisation_Joueur suivi du numéro du joueur
        DataSaver.saveData(saveDataInit, "Initialisation_Joueur" + PlayerPrefs.GetInt("NumJoueur"));


        // On enregistre les données de traces utilisateur
        DataSaver.SauvegarderTraces(hesitationManager.vitessesSelection, "TracesSelectJ"+ PlayerPrefs.GetInt("NumJoueur"));
        DataSaver.SauvegarderTraces(hesitationManager.vitessesEntreeTexte, "TracesTextJ"+ PlayerPrefs.GetInt("NumJoueur"));


        /** PB  UserTraces saveDataTrace = new UserTraces();
        // Initialisation des listes de vitesses de sélection et d'entrée de texte
        saveDataTrace.vitessesSelection = new List<Tuple<float, bool>>[PlayerPrefs.GetInt("NbMotsVocab")]; 
        saveDataTrace.vitessesEntreeTexte = new List<Tuple<float, bool>>[PlayerPrefs.GetInt("NbMotsVocab")];
        // On fait une copie en profondeur
        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            if (hesitationManager.vitessesSelection[i].Count>0)
            {
                saveDataTrace.vitessesSelection[i] = new List<Tuple<float, bool>>(hesitationManager.vitessesSelection[i]);
            }else{
                saveDataTrace.vitessesSelection[i] = new List<Tuple<float, bool>>();
            }
            
            if (hesitationManager.vitessesEntreeTexte[i].Count>0)
            {
                saveDataTrace.vitessesEntreeTexte[i] = new List<Tuple<float, bool>>(hesitationManager.vitessesEntreeTexte[i]);
            }else{
                saveDataTrace.vitessesEntreeTexte[i] = new List<Tuple<float, bool>>();
            }
        }
        // UnityEngine.Debug.Log("VITESSE SELECTION sauvegardée de taille : "+saveDataTrace.vitessesSelection.Length);
        // Sauvegarde des données de UserTraces dans un fichier nommé Traces_Joueur suivi du numéro du joueur
        DataSaver.saveData(saveDataTrace, "Traces_Joueur" + PlayerPrefs.GetInt("NumJoueur")); 
        // DataSaver.WriteToJsonFile<UserTraces>("Traces_Joueur" + PlayerPrefs.GetInt("NumJoueur"), saveDataTrace); // PB 

        // Affichage des données sauvegardées
        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            if (saveDataTrace.vitessesSelection[i].Count > 0) 
            {
                // DateTime copyDateDerniereRencontre = saveData.dateDerniereRencontre[i]; // Pour ne pas transformer saveData.dateDerniereRencontre[i] en string
                UnityEngine.Debug.Log("Sauvegarde vitessesSelection [" + i + "] =" + saveDataTrace.vitessesSelection[i][0]);
                // UnityEngine.Debug.Log(saveData.dateDerniereRencontre[i] + " de type " + saveData.dateDerniereRencontre[i].GetType());
            }
        } **/

        //On load la scène de menu principal
        SceneManager.LoadScene("MainMenu"); 
    }

    public void UpdateTempsSelectionMenu(float tmpsSelection){
        // On sauvegarde les données de temps de sélection dans les menus de ce joueur
        // On récupère l'ancienne liste de tempsSelectionMenu et on la complète avec les données récupérées
        var tempsSelectionMenu = new List<float>(); // Une liste de temps de sélection dans les menus permettant d'initialiser le niveau de l'utilisateur en terme de temps de sélection
    

        // On charge l'ancienne liste de tempsSelectionMenu  
        UserSelectionMenu loadedDataSelection = DataSaver.loadData<UserSelectionMenu>("SelectionMenu"); // On ne sait pas qui est en train de jouer pour l'instant

        if (loadedDataSelection == null || EqualityComparer<UserSelectionMenu>.Default.Equals(loadedDataSelection, default(UserSelectionMenu)))
        {
            // UnityEngine.Debug.Log("PAS DE DATA A LOAD SUR LES TEMPS DE SELECTION DANS LES MENUS");
        }else
        {
            tempsSelectionMenu = loadedDataSelection.tempsSelectionMenu;
            // UnityEngine.Debug.Log("Liste tempsSelectionMenu chargée (MainMenu) = " + tempsSelectionMenu + " de taille " + tempsSelectionMenu.Count);
        }

        // On ajoute les données récupérées à ce tour
        tempsSelectionMenu.Add(tmpsSelection);
        // UnityEngine.Debug.Log("Liste tempsSelectionMenu Concaténée (MainMenu) = " + tempsSelectionMenu + " de taille " + tempsSelectionMenu.Count);

        UserSelectionMenu saveDataSelectionMenu = new UserSelectionMenu();
        // PB on doit enregistrer toutes les stats
        saveDataSelectionMenu.tempsSelectionMenu = new List<float>();
        saveDataSelectionMenu.tempsSelectionMenu = tempsSelectionMenu;
        // Sauvegarde des données de selection dans les menus dans un fichier nommé SelectionMenu
        DataSaver.saveData(saveDataSelectionMenu, "SelectionMenu");
    }

    // Fonction appelée lors de l'ouverture de la scène MainMenu
    void OnEnable(){  
        // On lance le chronomètre pour calculer la vitesse de selection de l'utilisateur
        timer = new Stopwatch();
        ChronoEnMarche = true;
        timer.Start();
    }
    
    void Update(){ // Fonction appelée toute les frames
        // Si l'utilisateur ne regarde plus l'écran et que le chronomètre est en marche, on arrête le chronomètre
        if(ChronoEnMarche == true && PlayerPrefs.GetInt("ChronosEnPause") == 1){
            timer.Stop();
            ChronoEnMarche = false;
        }
        // Lorsque l'utilisateur regarde de nouveau l'écran et que le chronomètre est arrêté, on relance le chronomètre     
        if(ChronoEnMarche == false && PlayerPrefs.GetInt("ChronosEnPause") == 0){
            timer.Start(); // On reprend le chronomètre là où il s'était arrêté
            ChronoEnMarche = true;
        } 
    }
}
