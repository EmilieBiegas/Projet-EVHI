using System.Diagnostics;
using System;
// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Text;

// Classe permettant de gérer le quiz
public class QuizManager : MonoBehaviour
{
    [SerializeField] private Color correctCol, wrongCol; // Les couleurs correspondant aux réponses fausses (rouge) et à la bonne (vert)
    public List<QuestionReponse> QnA; // Liste des questions/réponses
    public GameObject[] options; // Liste des 4 boutons pour le QCM
    public int QuestionCourrante; // Indice de la question courrante (càd qui est en train d'être posée)
    public Text TxtQuestionQCM; // Le texte qui formule la question pour le QCM (pour pouvoir le changer)
    public Text TxtQuestionEnt; // Le texte qui formule la question pour la réponse entière (pour pouvoir le changer)
    public CsvReader CsvLu; // L'objet qui va lire le CSV en entrée (déjà associé au fichier texte dans Unity)
    public VocabUtilisateur vocUt; // L'objet gérant les données associées au vocabulaire de l'utilisateur
    public GameObject NextButtonQCM; // Bouton "question suivante" pour les QCM
    public GameObject FicheButtonQCM; // Bouton permettant d'accéder à la fiche pour les QCM
    public GameObject NextButtonEntier; // Bouton "question suivante" pour les questions à réponse entière
    public GameObject FicheButtonEntier; // Bouton permettant d'accéder à la fiche pour les questions à réponse entière
    public GameObject PanneauFicheQCM; // Le panneau détaillant la fiche pour les QCM
    public GameObject PanneauFicheEntier; // Le panneau détaillant la fiche pour les questions à réponse entière
    public GameObject PanneauQEntier; // Le panneau contenant la question et le champs de saisie pour les questions à réponse entière
    public GameObject PanneauQCM; // Le panneau contenant la question et les réponses pour les QCM
    public GameObject OkButton; // Le bouton OK permettant de valider la réponse dans les questions à réponse entière
    public GameObject EntreeRep; // Le champs de saisie de la réponse dans les questions à réponse entière
    public GameObject BonneRep; // Le champs de saisie (en mode lecture) permettant d'afficher la bonne réponse lorsque l'utilisateur s'est trompé dans les questions à réponse entière
    public HesitationManager hesitationManager; // L'objet gérant l'hésitation de l'utilisateur
    public bool inInitialisation; // Indique si on est encore dans l'initialisation (true) ou non (false)
    public bool RepEntreeOK; // Indique si la réponse entrée est bonne (true) ou non (false)
    public int NbQuestAvantNouvelle; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré (modifié seulement lorsque le cycle en cours est terminé)
    public int NbQuestAvantNouvelleTemp; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré (modifié à chaque réponse de l'utilisateur)
    // NbQuestAvantNouvelle doit être non nul mais peut être supérieur à 0 (exprime le nb de questions ancienne avant d'en avoir une nouvelle) ou inférieur à 0 (exprime -le nb de questions nouvelle avant d'en avoir une ancienne)
    // PB NbQuestAvantNouvelle peut etre négatif pour avoir plusieurs nouvelles questions avant une ancienne
    // PB a modifier au cours du temps et du NbQuestionsTotales
    public int NbAncienneQuestion; // Le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'un nouveau mot
    public int NbAncienneQuestionTemp; // Pour permettre de mettre à jour NbAncienneQuestion que quand l'utilisateur a répondu et pas avant (s'il a juste vu la question)
    public int NbNouvelleQuestion; // Le nombre de questions posées sur des mots non encore rencontrés depuis la dernière rencontre d'un ancien mot
    public int NbNouvelleQuestionTemp; // Pour permettre de mettre à jour NbNouvelleQuestion que quand l'utilisateur a répondu et pas avant (s'il a juste vu la question)
    public int NbQuestionsTotales; // Le nombre de questions rencontrées au total // PB ou depuis la dernière màj de NbQuestAvantNouvelle? // PB utile ?
    public bool inQCM; // Indique si la question à laquelle on vient de répondre est un QCM (true) ou non (false)
    public List<int> IndQuestNonRencontrees; // Indices des questions non encores posées
    private Stopwatch timer; // Le chronomètre permettant de mesurer le temps de sélection ou d'entrée de texte de l'utilisateur
    private string ReponseUtilisateur; // La réponse entrée par l'utilisateur lors d'une question à réponse entière
    public string TypeQuestion; // Le type de la question en cours (QCM ou Entier)
    private int nbCarRep; // Le nombre de caractères entrés par l'utilisateur pour sa réponse en champs de saisie lors d'une question à réponse entière
    private bool loadAnciennePartie; // Booléen qui indique si on a load une ancienne partie (true) ou non (false)
    private bool Repondu; // Booléen qui indique si l'utilisateur a répondu à la question courrante ou non
    private bool ChronoEnMarche; // Booléen qui indique si le chronomètre est en marche (true), ou en pause (false)
    // PB il y a d'autres attributs plus bas au dessus de l'initialisation
    void Start(){ // Pour l'initialisation
        // UnityEngine.Debug.Log("START DU QUIZ MANAGER");

        // On commence par lire le CSV contenant les mots de vocabulaire
        CsvLu.readCSV();
        QnA = CsvLu.myQr; // On récupère la liste de question/réponses obtenue avec le CsvReader

        // Le nombre de mots de vocabulaire est donné par la taille de la liste de question/réponses
        PlayerPrefs.SetInt("NbMotsVocab", QnA.Count); // On enregistre cette donnée dans les PlayerPrefs afin de pouvoir y accéder dans tous les scripts
        UnityEngine.Debug.Log("INITIALISATION DU NOMBRE DE MOT DE VOCABULAIRE = " + QnA.Count);

        // Pour le calcul de la vitesse de sélection 
        timer = new Stopwatch();
        ChronoEnMarche = true;
        
        Initialisation();
    }

    // Fonction permettant d'initialiser les valeurs des attributs pour l'initialisation dans le cas d'un tout nouveau joueur
    void InitialiseUserInitialisation(){
        // On initialise les attributs caractérisant la dynamique du quiz
        NbQuestAvantNouvelle = 1; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré (modifié à chaque fois que le cycle en cours se termine)
        NbQuestAvantNouvelleTemp = 1; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré (modifié à chaque réponse de l'utilisateur)
        NbAncienneQuestion = 0; // Le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'un nouveau mot
        NbNouvelleQuestion = 0; // Le nombre de questions posées sur des mots pas encore rencontrés depuis la dernière rencontre d'un ancien mot
        NbQuestionsTotales = 0; // Le nombre de questions rencontrées au total // PB ou depuis la dernière màj de NbQuestAvantNouvelle?

        // Instantiation des variables pour l'initialisation
        nbBienRep = 0; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à la même question en entier
        nbMalRep = 0; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à mal répondu à la même question en entier
        numIte = 0; // Numéro de l'itération dans l'initialisation

        // On initialise les indices des questions non encore rencontrées
        IndQuestNonRencontrees = new List<int>();
        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            IndQuestNonRencontrees.Add(i);
        }

        inInitialisation = true; // On commence l'initialisation 
        QuestionCourrante = 0; // Initialisation, la valeur va être modifiée très prochainement
    }

    // Fonction appellée lorsque l'utilisateur vient de répondre à un QCM
    public void ReponduQCM(bool correct){ // correct vaut true si l'utilisateur a donné la bonne réponse et false sinon
        // Calcul de la vitesse de clic de l'utilisateur
        timer.Stop(); // On arrête le chronomètre
        vocUt.dateDerniereRencontre[QuestionCourrante] = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"); // On met à jour la date de dernière rencontre du mot
        Repondu = true; // On a répondu à la question
        TimeSpan timeTaken = timer.Elapsed; // On regarde le temps passé sur le chronomètre
        
        // // PB pour l affichage : OK c'est bien le temps de sélection !
        // string foo = "Temps de sélection: " + timeTaken.ToString(@"m\:ss\.fff"); // PB Attention, timeTaken est un string mtn
        // UnityEngine.Debug.Log(foo);

        // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées) afin de l'enregistrer dans le gérant de l'hésitation
        int days, hours, minutes, seconds, milliseconds;
        days = timeTaken.Days;
        hours = timeTaken.Hours;
        minutes = timeTaken.Minutes;
        seconds = timeTaken.Seconds;
        milliseconds = timeTaken.Milliseconds;
        // Temps passé en secondes :
        float floatTimeSpan = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
        UnityEngine.Debug.Log("Temps de sélection : " + floatTimeSpan + "secondes");
        // On enregistre ce temps (trace) dans le gérant d'hésitation
        hesitationManager.vitessesSelection[QuestionCourrante].Add(new Tuple<float, bool>(floatTimeSpan, correct));
        // UnityEngine.Debug.Log("vitessesSelection[" + QuestionCourrante + "] = " + hesitationManager.vitessesSelection[QuestionCourrante]);
        // hesitationManager.ComparaisonVitesseSelection();

        inQCM = true; // On indique que la réponse à laquelle on vient de répondre est un QCM
        // On indique que l'on vient de rencontrer cette question, on enlève donc l'indice de cette question à la liste d'indice des questions non encore rencontrées
        if (IndQuestNonRencontrees.Contains(QuestionCourrante)) // On ne met ça que quand on a répondu à un QCM puisqu'on répond toujours à un QCM avant de répondre à la question entière PB si ça change
        {
            IndQuestNonRencontrees.Remove(QuestionCourrante);
            // UnityEngine.Debug.Log("Enleve l'indice " + QuestionCourrante + " de IndQuestNonRencontrees");
        }

        // UnityEngine.Debug.Log("IndQuestNonRencontrees = " + IndQuestNonRencontrees + " de taille "+ IndQuestNonRencontrees.Count);
        vocUt.UpdateNbRencontres(QuestionCourrante); // On indique que l'on a rencontré une fois de plus la question
        NbQuestionsTotales += 1; // On indique que l'on a rencontré une question de plus
        NbAncienneQuestion = NbAncienneQuestionTemp; // On màj le NbAncienneQuestion mtn que l'utilisateur a répondu
        NbNouvelleQuestion = NbNouvelleQuestionTemp; // On màj le NbNouvelleQuestion mtn que l'utilisateur a répondu
        float hesite = hesitationManager.EstimationHesitationQCM(floatTimeSpan); // On estime l'hésitation de l'utilisateur
        vocUt.UpdateProbaAcquisitionQCM(QuestionCourrante, correct, hesite); // On met à jour les probas d'acquisition 

        // On affiche la bonne réponse en vert et les autres en rouge
        for (int i = 0; i < options.Length; i++)
        {
            // Rendre les boutons non cliquables
            options[i].GetComponent<Button>().interactable = false;
            if (options[i].GetComponent<ReponseScript>().isCorrect == true)
            {
                // Change la couleur du bouton (en mode non cliquable)
                var colors = options[i].GetComponent<Button>().colors;
                colors.disabledColor = correctCol;
                options[i].GetComponent<Button>().colors = colors;
            }else
            {
                // Change la couleur du bouton (en mode non cliquable)
                var colors = options[i].GetComponent<Button>().colors;
                colors.disabledColor = wrongCol;
                options[i].GetComponent<Button>().colors = colors;
            }
        }

        if (inInitialisation && RepEntreeOK == false)
        {
            numIte += 1; // L'utilisateur a fini de répondre à cette question et on peut donc incrémenter le numéro de l'itération de l'initialisation
        }

        // On affiche les boutons "suivant" permettant de passer à la question suivante et "fiche d'aide" permettant de consulter la fiche
        NextButtonQCM.SetActive (true);
        FicheButtonQCM.SetActive (true);
    }

    // Fonction appellée lorsque l'utilisateur vient de répondre à une question à réponse entière
    public void ReponduEntier(){
        // Calcul de la vitesse d'entrée de texte de l'utilisateur
        timer.Stop(); // On arrête le chronomètre
        vocUt.dateDerniereRencontre[QuestionCourrante] = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"); // On met à jour la date de dernière rencontre du mot
        Repondu = true; // On a répondu à la question
        TimeSpan timeTaken = timer.Elapsed; // On regarde le temps passé sur le chronomètre
        // // PB pour l affichage : OK c'est bien le temps d'entrée de texte et le nombre de caractères entrés !
        // string foo = "Temps d'entrée de texte: " + timeTaken.ToString(@"m\:ss\.fff") + "sur " + nbCarRep + " caractères"; // PB Attention, timeTaken est un string mtn
        // UnityEngine.Debug.Log(foo);

        // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées) afin de l'enregistrer dans le gérant de l'hésitation
        int days, hours, minutes, seconds, milliseconds;
        days = timeTaken.Days;
        hours = timeTaken.Hours;
        minutes = timeTaken.Minutes;
        seconds = timeTaken.Seconds;
        milliseconds = timeTaken.Milliseconds;
        // Temps passé en secondes :
        float floatTimeSpan = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
        UnityEngine.Debug.Log("Temps d'entrée de texte : " + floatTimeSpan + "secondes");
        // UnityEngine.Debug.Log("vitessesEntreeTexte[" + QuestionCourrante + "] = " + hesitationManager.vitessesEntreeTexte[QuestionCourrante]);
        // hesitationManager.ComparaisonVitesseEntreeTexte(nbCarRep);

        inQCM = false; // On indique que la réponse à laquelle on vient de répondre n'est pas un QCM mais une question entière
        vocUt.UpdateNbRencontres(QuestionCourrante); // On indique que l'on a rencontré une fois de plus la question
        NbQuestionsTotales += 1; // On indique que l'on a rencontré une question de plus
        NbAncienneQuestion = NbAncienneQuestionTemp; // On màj le NbAncienneQuestion mtn que l'utilisateur a répondu
        NbNouvelleQuestion = NbNouvelleQuestionTemp; // On màj le NbNouvelleQuestion mtn que l'utilisateur a répondu

        // On regarde si l'utilisateur a entré la bonne réponse
        var bonneRepEntree = (ReponseUtilisateur.ToLower() == QnA[QuestionCourrante].ReponseCorrecte.ToLower());  // On met les deux chaînes en minuscule pour qu'elles soient comparables
        // UnityEngine.Debug.Log("Meme rep entree ? " + bonneRepEntree);
        if (bonneRepEntree == false)
        {
            if (ReponseUtilisateur.Length > 3)
            {
                if (QnA[QuestionCourrante].ReponseCorrecte.ToLower().Substring(0,2) == "to")
                {
                    // UnityEngine.Debug.Log("Bonne réponse commence par to");
                    bonneRepEntree = (ReponseUtilisateur.ToLower() == QnA[QuestionCourrante].ReponseCorrecte.ToLower().Substring(3));
                }

                if (ReponseUtilisateur.ToLower().Substring(0,2) == "to")
                {
                    // UnityEngine.Debug.Log("Réponse donnée commence par to");
                    bonneRepEntree = (ReponseUtilisateur.ToLower().Substring(3) == QnA[QuestionCourrante].ReponseCorrecte.ToLower());
                }
            }
        }
        // On enregistre ce temps (trace) dans le gérant d'hésitation
        hesitationManager.AjoutVitesseEntreeTexte(QuestionCourrante, floatTimeSpan, nbCarRep, bonneRepEntree);

        // UnityEngine.Debug.Log("QnA[QuestionCourrante].ReponseCorrecte = " + QnA[QuestionCourrante].ReponseCorrecte + " et ReponseUtilisateur = " + ReponseUtilisateur); // OK, .ToLower et .Sustring ne modifient pas la chaîne d'origine
        // UnityEngine.Debug.Log("Meme rep entree sans le to ? " + bonneRepEntree);

        // On affiche le carré en vert si l'utilisateur a entré la bonne réponse et sinon on affiche en rouge son carré et la bonne réponse en vert
        if (bonneRepEntree)
        {
            UnityEngine.Debug.Log("BONNE REPONSE !");
            float hesite = hesitationManager.EstimationHesitationEntier(floatTimeSpan, nbCarRep); // On estime l'hésitation de l'utilisateur
            vocUt.UpdateProbaAcquisitionQEntier(QuestionCourrante, true, hesite); // On met à jour les probas d'acquisition
            RepEntreeOK = true; // On indique que l'utilisateur a donné la bonne réponse
            DecrementeNbQuestAvantNouvelle(); // On augmente l'étendu de l'apprentissage (plutôt que le renforcement des connaissances)

            // Change la couleur du bouton (en mode non cliquable)
            var colors = EntreeRep.GetComponent<InputField>().colors;
            colors.disabledColor = correctCol;
            EntreeRep.GetComponent<InputField>().colors = colors;
            // Rendre le champs de saisie et le bouton OK non cliquable
            EntreeRep.GetComponent<InputField>().interactable = false;
            OkButton.GetComponent<Button>().interactable = false;
        }else
        {
            UnityEngine.Debug.Log("MAUVAISE REPONSE !");
            float hesite = hesitationManager.EstimationHesitationEntier(floatTimeSpan, nbCarRep); // On estime l'hésitation de l'utilisateur
            vocUt.UpdateProbaAcquisitionQEntier(QuestionCourrante, false, hesite); // On met à jour les probas d'acquisition
            RepEntreeOK = false; // On indique que l'utilisateur a donné la mauvaise réponse
            IncrementeNbQuestAvantNouvelle(); // PB on augmente le renforcemment des connaissances (càd augmenter le nombre de questions déjà vues nécessaires avant une nouvelle question)

            // On met la réponse rentrée en rouge et on affiche un deuxième carré contenant la bonne réponse en vert
            var colors = EntreeRep.GetComponent<InputField>().colors;
            colors.disabledColor = wrongCol;
            EntreeRep.GetComponent<InputField>().colors = colors;

            colors = BonneRep.GetComponent<InputField>().colors;
            colors.disabledColor = correctCol;
            BonneRep.GetComponent<InputField>().colors = colors;

            // Afficher le bouton bonne réponse contenant la bonne réponse dans ce cas 
            BonneRep.SetActive(true);
            BonneRep.GetComponent<InputField>().text = QnA[QuestionCourrante].ReponseCorrecte;

            // Rendre les champs de saisie et le bouton OK non cliquable
            EntreeRep.GetComponent<InputField>().interactable = false;
            BonneRep.GetComponent<InputField>().interactable = false;
            OkButton.GetComponent<Button>().interactable = false;
        }

        if (inInitialisation)
        {
            numIte += 1; // L'utilisateur a fini de répondre à cette question et on peut donc incrémenter le numéro de l'itération de l'initialisation
            // UnityEngine.Debug.Log("Iteration " + numIte + " Entier");
            if (RepEntreeOK)
            {
                // UnityEngine.Debug.Log("Iteration " + numIte + " a répondu à la question entière CORRECTEMENT");
                nbBienRep += 1;
                // PB HERE ajouter (le temps de sélection, d'entrée de texte et) les données occulomètriques de cette question dans les données "sûr"
            }else
            {
                // UnityEngine.Debug.Log("Iteration " + numIte + " a MAL répondu à la question entière");
                nbMalRep += 1;
                // PB ajouter (le temps de sélection, d'entrée de texte et) les données occulomètriques de cette question dans les données "hesite"
            }
        }
        
        // On affiche les boutons "suivant" permettant de passer à la question suivante et "fiche d'aide" permettant de consulter la fiche
        NextButtonEntier.SetActive (true);
        FicheButtonEntier.SetActive (true);
    }


    // Appellée lorsqu'on souhaite passer à la question suivante (à l'appui du bouton "suivant")
    public void questionSuivante(){
        // On cache les boutons fiche, suivant et bonne réponse (dans le cas QCM comme Entier)
        NextButtonQCM.SetActive (false);
        FicheButtonQCM.SetActive (false);
        NextButtonEntier.SetActive (false);
        FicheButtonEntier.SetActive (false);
        BonneRep.SetActive(false);

        // On réactive les 4 boutons de choix (pour QCM), le champs de saisie de réponse et le bouton OK (pour Entier)
        for (int i = 0; i < options.Length; i++)
        {
            // Rendre les boutons cliquables de nouveau
            options[i].GetComponent<Button>().interactable = true;
        }
        EntreeRep.GetComponent<InputField>().interactable = true;
        OkButton.GetComponent<Button>().interactable = true;
        
        // On affiche la question suivante (en fonction de si on est dans l'initialisation ou non avec un QCM ou non)
        if (inInitialisation)
        {
            if (inQCM)
            {
                // UnityEngine.Debug.Log("Répondu à la question QCM");
                InitialisationReponduQCM();
            }else
            {
                // UnityEngine.Debug.Log("Répondu à la question entière");
                Initialisation(); // On continue l'initialisation
            }
            
        }else
        {
            genererQuestion(); // On génère la question suivante
        }
    }

    // Fonction permettant de choisir une question et de la générer
    void genererQuestion(){
        // UnityEngine.Debug.Log("On génère normalement désormais");
        /** PB Choisir la question posée */
        // UnityEngine.Debug.Log("ON CHOISIT LA QUESTION POSEE NbQuestAvantNouvelle = " + NbQuestAvantNouvelle + "NbQuestAvantNouvelleTemp = " + NbQuestAvantNouvelleTemp + " NbAncienneQuestion = " + NbAncienneQuestion + " NbNouvelleQuestion = " + NbNouvelleQuestion);

        // NbQuestAvantNouvelle doit être non nul mais peut être supérieur à 0 (exprime le nb de questions ancienne avant d'en avoir une nouvelle) ou inférieur à 0 (exprime -le nb de questions nouvelle avant d'en avoir une ancienne)
        if (NbQuestAvantNouvelle > 0)
        {
            if(NbAncienneQuestion < NbQuestAvantNouvelle)
            {
                // Dans ce cas, on pose encore une question sur un mot déjà rencontré
                NbAncienneQuestionTemp = NbAncienneQuestion + 1; // On ne le change que si l'utilisateur a répondu à la question et pas tout de suite

                // On choisit la question déjà rencontrée avec la plus faible proba d'acquisition
                // Calculer les probabilités d'acquisition actuelles (selon le temps passé depuis la dernière rencontre du mot et le nombre de rencontres de ce mot)
                float[] probaAcquisActuelles = new float[PlayerPrefs.GetInt("NbMotsVocab")];
                probaAcquisActuelles = vocUt.UpdateProbaAcquisitionOubli();

                // On cherche l'indice de la question avec la plus faible proba d'acquisition actuelle
                var minProba = -1.0;
                var minInd = -1;
                for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
                {
                    if (vocUt.nbRencontres[i] > 0 && i != QuestionCourrante) // On ne considère la question que si elle a déjà été posée et qu'elle est différente de la dernière question posée
                    {
                        if (minInd == -1) // On initialise le minimum
                        {
                            minProba = probaAcquisActuelles[i];
                            minInd = i;
                        }
                        if (minProba > probaAcquisActuelles[i])
                        {
                            minProba = probaAcquisActuelles[i];
                            minInd = i;
                        }
                    }
                }

                if (minInd == -1) // Si aucune question n'a encore été posée
                {
                    NbAncienneQuestionTemp = NbAncienneQuestion - 1; // On n'incrémente pas le nombre de questions anciennes
                    if (QuestionCourrante != 0)
                    {
                        minInd = 0; // On pose la première question // PB à changer ?
                    }else
                    {
                        minInd = 1; // On pose la seconde question // PB à changer ?
                    }
                }
                QuestionCourrante = minInd;

            }else // Si NbAncienneQuestion >= NbQuestAvantNouvelle
            {
                // Dans ce cas, on pose une question sur un mot non encore rencontré
                NbAncienneQuestionTemp = 0; // On réinitialise le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'une nouvelle question
                
                // List<int> IndQuestNonRencontrees = new List<int>(); // Indices des questions non encores posées
                // // PB pour gain de temps, on peut plutot initialiser a tous les indices et les enlever a chaque fois qu'on rencontre une nouvelle question
                // for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
                // {
                //     if (vocUt.nbRencontres[i] == 0) // Si on n'a jamais rencontré la question, on l'ajoute à la liste
                //     {
                //         IndQuestNonRencontrees.Add(i);
                //     }
                // }

                // UnityEngine.Debug.Log("Ensemble des questions non encore rencontrées :");
                // for (int i = 0; i < IndQuestNonRencontrees.Count; i++)
                // {
                //     UnityEngine.Debug.Log(IndQuestNonRencontrees[i]);
                // }

                if (IndQuestNonRencontrees.Count == 0) // Si toutes les questions ont déjà été posées, on prend celle de proba d'acquisition minimum
                {
                    // Calculer les probabilités d'acquisition actuelles (selon le temps passé depuis la dernière rencontre du mot et le nombre de rencontres de ce mot)
                    float[] probaAcquisActuelles = new float[PlayerPrefs.GetInt("NbMotsVocab")];
                    probaAcquisActuelles = vocUt.UpdateProbaAcquisitionOubli();

                    // On cherche l'indice de la question avec la plus faible proba d'acquisition actuelle
                    var minProba = -1.0;
                    var minInd = -1;
                    // On choisit la question avec la proba d'acquisition la plus faible parmi toutes sauf la question qui vient d'être posée
                    for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
                    {
                        if (i != QuestionCourrante) // On ne considère la question que si elle est différente de la dernière question posée
                        {
                            if (minInd == -1) // On initialise le minimum
                            {
                                minProba = probaAcquisActuelles[i];
                                minInd = i;
                            }
                            if (minProba > probaAcquisActuelles[i])
                            {
                                minProba = probaAcquisActuelles[i];
                                minInd = i;
                            }
                        }
                    }
                    QuestionCourrante = minInd;
                }else 
                {
                    // On choisit au hasard une question non rencontrée
                    System.Random rnd = new System.Random();
                    int IndListe  = rnd.Next(0, IndQuestNonRencontrees.Count);  // Créer un numéro entre 0 et la taille de la liste
                
                    QuestionCourrante =  IndQuestNonRencontrees[IndListe];
                }

                NbQuestAvantNouvelle = NbQuestAvantNouvelleTemp; // On met à jour les caractèristiques du nouveau cycle maintenant que l'on en commence un nouveau (et pas avant)  
                // On met à jour les niveaux de l'utilisateur en prenant en compte l'ensemble des traces récoltées
                hesitationManager.MajNivEntreeeTexteTrace();
                hesitationManager.MajNivSelectionTraces();
            }
        }

        // NbQuestAvantNouvelle doit être non nul mais peut être supérieur à 0 (exprime le nb de questions ancienne avant d'en avoir une nouvelle) ou inférieur à 0 (exprime -le nb de questions nouvelle avant d'en avoir une ancienne)
        if (NbQuestAvantNouvelle < 0)
        {
            if(NbNouvelleQuestion < (-1)*NbQuestAvantNouvelle)
            {
                // Dans ce cas, on pose encore une question sur un mot non encore rencontré
                NbNouvelleQuestionTemp = NbNouvelleQuestion + 1; // On ne le change que si l'utilisateur a répondu à la question et pas tout de suite
            
                // List<int> IndQuestNonRencontrees = new List<int>(); // Indices des questions non encores posées
                // // PB pour gain de temps, on peut plutot initialiser a tous les indices et les enlever a chaque fois qu'on rencontre une nouvelle question
                // for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
                // {
                //     if (vocUt.nbRencontres[i] == 0) // Si on n'a jamais rencontré la question, on l'ajoute à la liste
                //     {
                //         IndQuestNonRencontrees.Add(i);
                //     }
                // }

                if (IndQuestNonRencontrees.Count == 0) // Si toutes les questions ont déjà été posées, on prend celle de proba d'acquisition minimum
                {
                    // Calculer les probabilités d'acquisition actuelles (selon le temps passé depuis la dernière rencontre du mot et le nombre de rencontres de ce mot)
                    float[] probaAcquisActuelles = new float[PlayerPrefs.GetInt("NbMotsVocab")];
                    probaAcquisActuelles = vocUt.UpdateProbaAcquisitionOubli();

                    // On cherche l'indice de la question avec la plus faible proba d'acquisition actuelle
                    var minProba = -1.0;
                    var minInd = -1;
                    // On choisit la question avec la proba d'acquisition la plus faible parmi toutes les questions différentes de la dernière posée
                    for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
                    {
                        if (i != QuestionCourrante) // On ne considère la question que si elle est différente de la dernière question posée
                        {
                            if (minInd == -1) // On initialise le minimum
                            {
                                minProba = probaAcquisActuelles[i];
                                minInd = i;
                            }
                            if (minProba > probaAcquisActuelles[i])
                            {
                                minProba = probaAcquisActuelles[i];
                                minInd = i;
                            }
                        }
                    }
                    QuestionCourrante = minInd;
                }else
                {
                    // On choisit au hasard une question non rencontrée
                    System.Random rnd = new System.Random();
                    int IndListe  = rnd.Next(0, IndQuestNonRencontrees.Count);  // Créer un numéro entre 0 et la taille de la liste
                
                    QuestionCourrante =  IndQuestNonRencontrees[IndListe];
                }

            }else // Si (-1)*NbNouvelleQuestion >= NbQuestAvantNouvelle
            {
                // Dans ce cas, on pose une question sur un mot déjà rencontré
                NbNouvelleQuestionTemp = 0; // On réinitialise le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'une nouvelle question
                
                // On choisit la question déjà rencontrée avec la plus faible proba d'acquisition
                // Calculer les probabilités d'acquisition actuelles (selon le temps passé depuis la dernière rencontre du mot et le nombre de rencontres de ce mot)
                float[] probaAcquisActuelles = new float[PlayerPrefs.GetInt("NbMotsVocab")];
                probaAcquisActuelles = vocUt.UpdateProbaAcquisitionOubli();

                // On cherche l'indice de la question avec la plus faible proba d'acquisition actuelle
                var minProba = -1.0;
                var minInd = -1;
                for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
                {
                    if (vocUt.nbRencontres[i] > 0 && i != QuestionCourrante) // On ne considère la question que si elle a déjà été posée et qu'elle est différente de la dernière question posée
                    {
                        if (minInd == -1) // On initialise le minimum
                        {
                            minProba = probaAcquisActuelles[i];
                            minInd = i;
                        }
                        if (minProba > probaAcquisActuelles[i])
                        {
                            minProba = probaAcquisActuelles[i];
                            minInd = i;
                        }
                    }
                }

                if (minInd == -1) // Si aucune question n'a encore été posée
                {
                    NbAncienneQuestionTemp = NbAncienneQuestion - 1; // On n'incrémente pas le nombre de questions anciennes
                    if (QuestionCourrante != 0)
                    {
                        minInd = 0; // On pose la première question // PB à changer ?
                    }else
                    {
                        minInd = 1; // On pose la seconde question // PB à changer ?
                    }
                }
                QuestionCourrante = minInd;   
                NbQuestAvantNouvelle = NbQuestAvantNouvelleTemp; // On met à jour les caractèristiques du nouveau cycle maintenant que l'on en commence un nouveau (et pas avant)         
                // On met à jour les niveaux de l'utilisateur en prenant en compte l'ensemble des traces récoltées
                hesitationManager.MajNivEntreeeTexteTrace();
                hesitationManager.MajNivSelectionTraces();
            }
        }
        // Test choisir la question la moins posée //PB a changer plus tard
        // // UnityEngine.Debug.Log("On cherche la question suivante :");
        // var minNbPosees = vocUt.nbRencontres[0];
        // var minInd = 0;
        // // UnityEngine.Debug.Log("Initialisation :" + minNbPosees);
        // // UnityEngine.Debug.Log("Taille vocUt.nbRencontre ="+vocUt.nbRencontres.Length);
        // // UnityEngine.Debug.Log("QnA.Count="+QnA.Count);
        // for (int i = 0; i < QnA.Count; i++)
        // {
        //     // UnityEngine.Debug.Log("On compare min actuel =" + minNbPosees + " avec " + vocUt.nbRencontres[i]);
        //     // Debug.Log("i="+i);
        //     if (minNbPosees > vocUt.nbRencontres[i])
        //     {
        //         minNbPosees = vocUt.nbRencontres[i];
        //         minInd = i;
        //     }
        // }
        // QuestionCourrante = minInd;


        // Si la question a déjà été rencontrée et que l'utilisateur avait bien répondu à ce moment là, on propose alors la question sous forme Entier (et pas QCM) //PB a changer plus tard
        // if (vocUt.nbRencontres[QuestionCourrante] > 0 && vocUt.probaAcquisition[QuestionCourrante] > 0.75) 
        // {
        //     TypeQuestion = "Entier";
        // }else
        // {
        //     TypeQuestion = "QCM";
        // }

        // Si la question a déjà été rencontrée et que l'utilisateur s'est amélioré sur ce mot, on propose alors la question sous forme Entier (et pas QCM) //PB a changer plus tard
        if (vocUt.nbRencontres[QuestionCourrante] > 0 && hesitationManager.Amelioration(QuestionCourrante)) 
        {
            TypeQuestion = "Entier";
        }else
        {
            TypeQuestion = "QCM";
        }

        // On affiche la question sélectionnée
        afficherPanneauEnFctTypeQuestion();
    }

    // Fonction permettant d'afficher la question sélectionnée et de lancer le chronomètre de temps de réponse
    void afficherPanneauEnFctTypeQuestion(){
        if(TypeQuestion == "QCM"){  // La question choisie est un QCM
            // On affiche le panneau de QCM et pas de question Entier
            PanneauQEntier.SetActive(false);
            PanneauQCM.SetActive(true);

            TxtQuestionQCM.text = "Traduisez le mot : \n" + QnA[QuestionCourrante].Question; //La question est bien celle sélectionnée
            genererReponses(); // On génère les réponses possibles de ce QCM
        }else
        {
            // La question choisie est une question dont la réponse doit être entrée entièrement
            // On affiche le panneau de question Entier et pas de QCM
            PanneauQEntier.SetActive(true);
            PanneauQCM.SetActive(false);

            // On fait en sorte que le champs de saisie soit automatiquement sélectionné (pas besoin de cliquer dessus)
            EntreeRep.GetComponent<InputField>().Select();
            
            // Pour pouvoir lire ce qu'écrit l'utilisateur
            EntreeRep.GetComponent<InputField>().onEndEdit.AddListener(delegate { inputBetValue(EntreeRep.GetComponent<InputField>()); });

            TxtQuestionEnt.text = "Traduisez le mot : \n" + QnA[QuestionCourrante].Question; //La question est bien celle sélectionnée
        }

        Repondu = false; // On n'a pas encore répondu à la question
        // On lance le timer pour la prochaine question
        timer.Reset();
        timer.Start();
    }

    // Fonction permettant de générer les réponses pour le QCM et de leur associer la véracité de la réponse
    void genererReponses(){
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<ReponseScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[QuestionCourrante].Reponses[i];
            if(QnA[QuestionCourrante].IndReponseCorrecte == i){ 
                options[i].GetComponent<ReponseScript>().isCorrect = true;
                // UnityEngine.Debug.Log("Indice de la réponse correcte :" + QnA[QuestionCourrante].IndReponseCorrecte);
            } 
        }
    }

    // Fonction d'affichage de la fiche d'aide
    public void AfficherFiche(){
        if (TypeQuestion == "QCM")
        {
            PanneauFicheQCM.SetActive(true);
        }else
        {
            PanneauFicheEntier.SetActive(true);
            // On doit cacher le champs de saisie de réponse, le champs de la bonne réponse et le bouton OK
            EntreeRep.SetActive(false);
            OkButton.SetActive(false);
            BonneRep.SetActive(false);
        }
    }

    // Fonction pour cacher la fiche d'aide
    public void CacherFiche(){
        if (TypeQuestion == "QCM")
        {
            PanneauFicheQCM.SetActive(false);
        }else
        {
            PanneauFicheEntier.SetActive(false);
            // On doit faire réapparaître le champs de saisie de réponse et le bouton OK
            EntreeRep.SetActive(true);
            // OkButton.SetActive(true); // PB a enlever si on veut remettre le bouton OK
            // On doit aussi faire réapparaître le champs de la bonne réponse si il était précedemment affiché (càd si l'utilisateur avait mal répondu)
            if (RepEntreeOK == false)
            {
                BonneRep.SetActive(true); // Le réafficher seulement si on avait entré une mauvaise réponse                
            }
        }
    }

    // Fonctions des questions sans choix (on doit entrer la réponse à la main)
    public void inputBetValue(InputField userInput) // Fonction permettant de lire la saisie de l'utilisateur
    {
        ReponseUtilisateur = userInput.text;
        nbCarRep = userInput.text.Length + 1; // Le nombre de caractères entrés par l'utilisateur + la touche "entrée"
        // UnityEngine.Debug.Log(userInput.text);
    }

    // Fonction pour incrémenter le nombre d'ancienne question rencontré
    public void IncrementeNbQuestAvantNouvelle(){
        // NbQuestAvantNouvelle doit être non nul mais peut être supérieur à 0 (exprime le nb de questions ancienne avant d'en avoir une nouvelle) ou inférieur à 0 (exprime -le nb de questions nouvelle avant d'en avoir une ancienne)
        
        if (NbQuestAvantNouvelleTemp < 10) // Doit être entre -10 et 10
        {
            NbQuestAvantNouvelleTemp += 1;
        }

        if (NbQuestAvantNouvelleTemp == 0) 
        {
            NbQuestAvantNouvelleTemp = 1;
        }
    }

    // Fonction pour décrémenter le nombre d'ancienne question rencontré
    public void DecrementeNbQuestAvantNouvelle(){
        // NbQuestAvantNouvelle doit être non nul mais peut être supérieur à 0 (exprime le nb de questions ancienne avant d'en avoir une nouvelle) ou inférieur à 0 (exprime -le nb de questions nouvelle avant d'en avoir une ancienne)

        if (NbQuestAvantNouvelleTemp > -10) // Doit être entre -10 et 10
        {
            NbQuestAvantNouvelleTemp -= 1;
        }

        if (NbQuestAvantNouvelleTemp == 0) 
        {
            NbQuestAvantNouvelleTemp = -1;
        }
    }

    // Des attributs pour l'initialisation
    public int nbBienRep; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à la même question en entier
    public int nbMalRep; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à mal répondu à la même question en entier
    public int numIte; // Numéro de l'itération dans l'initialisation
    public void Initialisation(){ // Permet de faire l'initialisation afin de cibler le niveau de l'utilisateur en lui posant les premières questions
        var nbIteMax = 4; // Nombre maximum d'itérations avant de génerer les questions "normalement" 
        //PB prendre en compte si on n a pas assez de donnees sur tous les cas possibles

        UnityEngine.Debug.Log("Initialisation ite " + numIte + " avec nbBienRep = " + nbBienRep + "et nbMalRep = " + nbMalRep);

        // PB sauvegarder les données d'hésitation de l'utilisateur grâce à cette initialisation
        // On fait l'initialisation jusqu'à ce que l'on ai rencontré une fois les deux cas de figure ou jusqu'à un nombre défini d'itération
        if((nbBienRep < 1 || nbMalRep < 1) && numIte <= nbIteMax) // PB ajouter un nbr d'itération max qd même
        {
            // UnityEngine.Debug.Log("Toujours dans initialisation");
            if (loadAnciennePartie)
            {
                loadAnciennePartie = false; // On a chargé la dernière partie, c'est bon 
                // UnityEngine.Debug.Log("On LOAD une ancienne partie");
                if (inQCM && RepEntreeOK) // Dans ce cas on doit poser la même question que la dernière rencontrée en entier
                {
                    // Si l'utilisateur clique sur la bonne réponse, on lui repose la question sous forme d'écriture complète
                    TypeQuestion = "Entier";
                    // On garde la même QuestionCourrante

                    // On affiche la question choisie
                    afficherPanneauEnFctTypeQuestion();
                }else // Si on a mal répondu au QCM ou que la dernière question était une question entière, on pose un nouveau QCM
                {
                    Initialisation();
                }
            }else
            {
                // Sinon, on commence toujours par poser la question sous forme de QCM
                // UnityEngine.Debug.Log("Iteration " + numIte + " QCM");
                TypeQuestion = "QCM";
                QuestionCourrante = ((int)Math.Floor((double)numIte*(PlayerPrefs.GetInt("NbMotsVocab")-1)/nbIteMax)); // On prend des mots espacés dans la base de question (du mot numéro 0 au dernier d'indice (nbMotVocab-1) puisque les indices commencent à 0)
                
                // On affiche la question choisie
                afficherPanneauEnFctTypeQuestion();
            }
            
        }else
        {
            inInitialisation = false; // On n'est plus dans l'initialisation
            // UnityEngine.Debug.Log("On quitte l'initialisation");
            // Une fois l'initialisation terminée, on génère les questions comme expliqué dans le cahier des charges
            genererQuestion();
        }  
    }

    // Fonction appelée lorsque l'utilisateur est encore dans la phase d'initialisation et qu'il a répondu à un QCM (après avoir cliqué sur le bouton suivant)
    void InitialisationReponduQCM(){
        if (RepEntreeOK)
        {   // PB Peut etre enregistrer les questions auquelles il a bien répondu et lui demander de les écrire en entier après pour pas faire à la suite à chaque fois
            // UnityEngine.Debug.Log("Iteration " + numIte + " a répondu au QCM CORRECTEMENT");
            // Si l'utilisateur clique sur la bonne réponse, on lui repose la question sous forme d'écriture complète
            TypeQuestion = "Entier";
            // On garde la même QuestionCourrante

            // On affiche la question choisie
            afficherPanneauEnFctTypeQuestion();
        }else{
            // UnityEngine.Debug.Log("Iteration " + numIte + " a MAL répondu au QCM");
            // Si l'utilisateur répond faux c'est qu'il hésitait (PB à prendre en compte même si il répond faux au QCM)
            Initialisation(); // On continue l'initialisation
        }
    }

    void Update() // Fonction appelée toute les frames
    {
        //Detecter lorsqu'on appui sur la touche entrée 
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (TypeQuestion == "Entier" && Repondu == false) // Si on n'a pas encore répondu à la question entière, ça valide la réponse quand on appui sur entrée
            {
                //Lorsque l'on appui sur la touche entrée, ça valide la réponse entrée
                ReponduEntier();      
            }else{
                if (Repondu) // Si on appui sur entrée lorsqu'on a déjà répondu à la question (entière ou QCM), ça passe à la question suivante
                {
                    questionSuivante();    
                } 
            }               
        } 

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

    // Fonction appelée lors de l'ouverture de la scène QCM
    void OnEnable()
    {
        // On charge les données sauvegardées
        // UnityEngine.Debug.Log("ON ENABLE QuizManager : ON LOAD DATA");

        // Chargement des données de l'utilisateur en question
        if (PlayerPrefs.HasKey("NumJoueur")) // Normalement toujours vrai
        {
            var NumJoueur = PlayerPrefs.GetInt("NumJoueur"); // On a le num du joueur qui joue, il faut ensuite charger ses données
            // UnityEngine.Debug.Log("Joueur en train de jouer : " + NumJoueur);
            
            // *****
            // On charge les données statistiques du joueur concerné 
            UserStats loadedData = DataSaver.loadData<UserStats>("Stats_Joueur" + PlayerPrefs.GetInt("NumJoueur"));

            // UnityEngine.Debug.Log("PASSE LE LOAD, loadedData = " + loadedData);
            if (loadedData == null || EqualityComparer<UserStats>.Default.Equals(loadedData, default(UserStats)))// PB || loadedData.vocabUtilisateur == null) // ou ""
            {
                // UnityEngine.Debug.Log("PAS DE DATA A LOAD");
                // UnityEngine.Debug.Log("Aucune donnée statistique associée à ce joueur pour l'instant");
                // On initialise les données de vocabulaire de l'utilisateur
                vocUt.Initialise();
                hesitationManager.NivDefaut();
                hesitationManager.oculometreManager.InitialiseListesOcu();
            }else
            {
                // Si il y a quelque chose dans les données chargées, on les charge
                // UnityEngine.Debug.Log("Proba d'acquisition récupéré = " + loadedData.probaAcquisition);
                // UnityEngine.Debug.Log("NbRencontres récupéré = " + loadedData.nbRencontres);
                // UnityEngine.Debug.Log("dateDerniereRencontre récupéré = " + loadedData.dateDerniereRencontre);
                // UnityEngine.Debug.Log("NBMOTVOCAB LOAD QUIZMANAGER : = " + PlayerPrefs.GetInt("NbMotsVocab"));

                // Mise à jour des données du joueur
                vocUt.probaAcquisition = new float[PlayerPrefs.GetInt("NbMotsVocab")];
                vocUt.nbRencontres = new int[PlayerPrefs.GetInt("NbMotsVocab")];
                vocUt.dateDerniereRencontre = new string[PlayerPrefs.GetInt("NbMotsVocab")];

                Array.Copy(loadedData.probaAcquisition, vocUt.probaAcquisition, PlayerPrefs.GetInt("NbMotsVocab"));
                Array.Copy(loadedData.nbRencontres, vocUt.nbRencontres, PlayerPrefs.GetInt("NbMotsVocab"));
                // PB sans copy : vocUt.nbRencontres = loadedData.nbRencontres;
                // PB sans copy : vocUt.dateDerniereRencontre = loadedData.dateDerniereRencontre;

                // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre == null " + loadedData.dateDerniereRencontre == null);
                // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre = " + loadedData.dateDerniereRencontre);
                // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre.Length = " + loadedData.dateDerniereRencontre.Length);

                for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
                {
                    if (loadedData.dateDerniereRencontre[i] != null) // On ne charge que les données qui existent (les autres restent vides mais on ne les consultera pas donc pas de problèmes)
                    {
                        vocUt.dateDerniereRencontre[i] = loadedData.dateDerniereRencontre[i]; 
                        // UnityEngine.Debug.Log("Date derniere rencontres [" + i + "] =" + loadedData.dateDerniereRencontre[i]);
                        // UnityEngine.Debug.Log("Date derniere rencontres VocUt [" + i + "] =" + vocUt.dateDerniereRencontre[i]);
                    
                    }else
                    {
                        // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre[" + i + "] est null");
                    } 
                }

                hesitationManager.nivSelection = loadedData.nivSelection;
                hesitationManager.nivEntreeTexte = loadedData.nivEntreeTexte;
                hesitationManager.oculometreManager.occulaireHesite = new List<Vector2>(loadedData.occulaireHesite);
                hesitationManager.oculometreManager.occulaireSur = new List<Vector2>(loadedData.occulaireSur);
                UnityEngine.Debug.Log("Liste hesitation : "+hesitationManager.oculometreManager.occulaireHesite);
            }
            
            // Affichage des données chargées
            // for (int i = 0; i < nbMotVocab; i++)
            // {
            //     // UnityEngine.Debug.Log("Proba d'acquisition [" + i + "] =" + loadedData.probaAcquisition[i]);
            //     // UnityEngine.Debug.Log("Proba d'acquisition VocUt [" + i + "] =" + vocUt.probaAcquisition[i]);
            //     // UnityEngine.Debug.Log("Nb rencontres [" + i + "] =" + loadedData.nbRencontres[i]);
            //     // UnityEngine.Debug.Log("Nb rencontres VocUt [" + i + "] =" + vocUt.nbRencontres[i]);

            //     // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre = " + loadedData.dateDerniereRencontre);
            //     // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre.Length = " + loadedData.dateDerniereRencontre.Length);
            //     if (loadedData.dateDerniereRencontre[i] != null) // PB Attention, dateDerniereRencontre peut être null
            //     {
            //         // DateTime copyDateDerniereRencontre = loadedData.dateDerniereRencontre[i]; // Pour ne pas transformer loadedData.dateDerniereRencontre[i] en string
            //         // DateTime copyDateDerniereRencontreVoc = vocUt.dateDerniereRencontre[i]; // Pour ne pas transformer vocUtData.dateDerniereRencontre[i] en string
            //         // UnityEngine.Debug.Log("Date derniere rencontres [" + i + "] =" + copyDateDerniereRencontre.ToString("MM/dd/yyyy HH:mm:ss"));
            //         // UnityEngine.Debug.Log("Date derniere rencontres VocUt [" + i + "] =" + copyDateDerniereRencontreVoc.ToString("MM/dd/yyyy HH:mm:ss"));
            //     }
            // }


            // *****
            // On charge les données d'initialisation du joueur concerné 
            // UnityEngine.Debug.Log("ON CHARGE LES DONNEES D INITIALISATION");
            UserInitialisation loadedDataInit = DataSaver.loadData<UserInitialisation>("Initialisation_Joueur" + PlayerPrefs.GetInt("NumJoueur"));

            // UnityEngine.Debug.Log("LoadedData = " + loadedDataInit);
            if (loadedDataInit == null || EqualityComparer<UserInitialisation>.Default.Equals(loadedDataInit, default(UserInitialisation)))
            {
                // UnityEngine.Debug.Log("Aucune donnée d'initialisation associée à ce joueur pour l'instant");
                // On indique que l'on n'a pas load d'ancienne partie (c'est une toute nouvelle partie)
                loadAnciennePartie = false;
                // On initialise les attributs du tout nouvel utilisateur pour qu'il puisse commencer sa partie
                InitialiseUserInitialisation();
            }else
            {
                // UnityEngine.Debug.Log("Chargement des données d'initialisation associée à ce joueur...");
                // Si il y a quelque chose dans les données chargées, on les charge
                // Mise à jour des données du joueur
                nbBienRep = loadedDataInit.nbBienRep;
                nbMalRep = loadedDataInit.nbMalRep;
                numIte = loadedDataInit.numIte;
                inInitialisation = loadedDataInit.inInitialisation;
                RepEntreeOK = loadedDataInit.RepEntreeOK;
                NbQuestAvantNouvelle = loadedDataInit.NbQuestAvantNouvelle;
                NbAncienneQuestion = loadedDataInit.NbAncienneQuestion;
                NbNouvelleQuestion = loadedDataInit.NbNouvelleQuestion;
                NbQuestionsTotales = loadedDataInit.NbQuestionsTotales;
                inQCM = loadedDataInit.inQCM;
                QuestionCourrante = loadedDataInit.QuestionCourrante; 
                TypeQuestion = loadedDataInit.TypeQuestion; 
                NbAncienneQuestionTemp = loadedDataInit.NbAncienneQuestionTemp;
                NbNouvelleQuestionTemp = loadedDataInit.NbNouvelleQuestionTemp;
                IndQuestNonRencontrees = new List<int>(); 
                IndQuestNonRencontrees = loadedDataInit.IndQuestNonRencontrees; // PB Copy de list

                // UnityEngine.Debug.Log("IndQuestNonRencontrees CHARGE = " + IndQuestNonRencontrees + " de taille " + IndQuestNonRencontrees.Count);

                // On indique que l'on a load une ancienne partie (possiblement en cours d'initialisation)
                loadAnciennePartie = true;  
            }
            // UnityEngine.Debug.Log("nbBienRep =" + nbBienRep + "; nbMalRep = " + nbMalRep + "; numIte = " + numIte + "; inInitialisation = " + inInitialisation + "; RepEntreeOK = " + RepEntreeOK + "; NbQuestAvantNouvelle = " + NbQuestAvantNouvelle + "; NbAncienneQuestion = " + NbAncienneQuestion + "; NbNouvelleQuestion = " + NbNouvelleQuestion + "; NbQuestionsTotales = " + NbQuestionsTotales + "; inQCM = " + inQCM + "; QuestionCourrante = " + QuestionCourrante + "; TypeQuestion = " + TypeQuestion + "; NbAncienneQuestionTemp = " + NbAncienneQuestionTemp + "; NbNouvelleQuestionTemp = " + NbNouvelleQuestionTemp);
            // UnityEngine.Debug.Log("loadAnciennePartie = " + loadAnciennePartie);

            // *****
            // On charge les données historiques de trace d'hésitation du joueur concerné 
            hesitationManager.vitessesSelection = DataSaver.ChargerTraces("TracesSelectJ"+ PlayerPrefs.GetInt("NumJoueur"));
            hesitationManager.vitessesEntreeTexte = DataSaver.ChargerTraces("TracesTextJ"+ PlayerPrefs.GetInt("NumJoueur"));


            /** PB
            // UnityEngine.Debug.Log("ON CHARGE LES DONNEES DE TRACE");
            UserTraces loadedDataTrace = DataSaver.loadData<UserTraces>("Traces_Joueur" + PlayerPrefs.GetInt("NumJoueur")); 
            // UserTraces loadedDataTrace = DataSaver.ReadFromJsonFile<UserTraces>("Traces_Joueur" + PlayerPrefs.GetInt("NumJoueur")); // PB
            // UnityEngine.Debug.Log("LoadedData = " + loadedDataInit);
            if (loadedDataTrace == null || EqualityComparer<UserTraces>.Default.Equals(loadedDataTrace, default(UserTraces)))
            {
                // UnityEngine.Debug.Log("Aucune donnée de trace associée à ce joueur pour l'instant");
                // On initialise les attributs du tout nouvel utilisateur pour qu'il puisse commencer sa partie
                hesitationManager.ListesDefaut(); // On met les listes par défaut 
            }else
            {
                UnityEngine.Debug.Log("PB LOADED DATA TRACE : "+loadedDataTrace);
                UnityEngine.Debug.Log("PB LOADED DATA TRACE VITESSE SELECTION : "+loadedDataTrace.vitessesSelection);
                UnityEngine.Debug.Log("PB LOADED DATA TRACE VITESSE ENTREE TEXT : "+loadedDataTrace.vitessesEntreeTexte);
                // UnityEngine.Debug.Log("Chargement des données de trace associée à ce joueur...");
                // Si il y a quelque chose dans les données chargées, on les charge
                // Mise à jour des données du joueur
                hesitationManager.ListesDefaut(); // On met les listes par défaut
                if (loadedDataTrace.vitessesSelection==null)
                {
                    UnityEngine.Debug.Log("PROBLEME, VITESSE SELECTION NULL");
                }
                for (int i = 0; i < loadedDataTrace.vitessesSelection.Length; i++)
                {
                    if (loadedDataTrace.vitessesSelection[i].Count>0)
                    {
                        hesitationManager.vitessesSelection[i] = new List<Tuple<float, bool>>(loadedDataTrace.vitessesSelection[i]); 
                    }
                }
                for (int i = 0; i < loadedDataTrace.vitessesEntreeTexte.Length; i++)
                {
                    if (loadedDataTrace.vitessesEntreeTexte[i].Count>0)
                    {
                        hesitationManager.vitessesEntreeTexte[i] = new List<Tuple<float, bool>>(loadedDataTrace.vitessesEntreeTexte[i]); 
                    }
                }
            } **/

            // *****
            // On initialise le niveau de vitesse d'entrée de texte de l'utilisateur avec les données récupérées lors de sa saisie de pseudo 
            if (PlayerPrefs.HasKey("TmpsEntreePseudoJ" + NumJoueur))
            {
                var temps = PlayerPrefs.GetFloat("TmpsEntreePseudoJ" + NumJoueur);
                hesitationManager.MajNivEntreeeTextePseudo(temps, PlayerPrefs.GetString("PseudoJ" + NumJoueur).Length + 1); // Le nombre de caractères du pseudo + la touche entrée
            }else
            {
                // UnityEngine.Debug.Log("Pas de pseudo associé à ce joueur");
            }

            // *****
            // On initialise le niveau de vitesse de sélection de l'utilisateur avec les données récupérées lors de sa navigation dans les menus 
            // On charge la liste de tempsSelectionMenu du joueur concerné 
            UserSelectionMenu loadedDataSelection = DataSaver.loadData<UserSelectionMenu>("SelectionMenuJ" + PlayerPrefs.GetInt("NumJoueur"));

            if (loadedDataSelection == null || EqualityComparer<UserSelectionMenu>.Default.Equals(loadedDataSelection, default(UserSelectionMenu)))
            {
                // UnityEngine.Debug.Log("PAS DE DATA A LOAD SUR LES TEMPS DE SELECTION DANS LES MENUS");
            }else
            {
                var tempsSelectionMenuTemp = new List<float>();
                tempsSelectionMenuTemp = loadedDataSelection.tempsSelectionMenu;

                // UnityEngine.Debug.Log("INITIALISATION DES TEMPS DE SELECTION DANS LES MENUS : " + tempsSelectionMenuTemp.Count + " données");

                // On initialise le niveau de l'utilisateur en ce qui concerne le temps de sélection
                for (int i = 0; i < tempsSelectionMenuTemp.Count; i++)
                {
                    hesitationManager.MajNivSelectionMenu(tempsSelectionMenuTemp[i]);
                }
                // On retire les temps de sélection dans les menus puisqu'ils sont déjà traités
                DataSaver.deleteData("SelectionMenuJ" + NumJoueur); // Efface les données de selection dans les menus du joueur
            }

        }else
        {
            UnityEngine.Debug.Log("PB PROBLEME, JOUEUR NON PRECISE !!");
        }
    }
}

//PB Enlever les Debug et les PB