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
    public bool inInitialisation; // Indique si on est encore dans l'initialisation (true) ou non (false)
    public bool RepEntreeOK; // Indique si la réponse entrée est bonne (true) ou non (false)
    // PB il faut mémoriser NbQuestAvantNouvelle, NbAncienneQuestion et le nbr de questions totales surement
    public int NbQuestAvantNouvelle; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré
    // PB a modifier au cours du temps et du NbQuestionsTotales
    public int NbAncienneQuestion; // Le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'un nouveau mot
    public int NbAncienneQuestionTemp; // Pour permettre de mettre à jour NbAncienneQuestion que quand l'utilisateur a répondu et pas avant (s'il a juste vu la question)
    public int NbQuestionsTotales; // Le nombre de questions rencontrées au total // PB ou depuis la dernière màj de NbQuestAvantNouvelle?
    public bool inQCM; // Indique si la question à laquelle on vient de répondre est un QCM (true) ou non (false)
    private Stopwatch timer; // Le chronomètre permettant de mesurer le temps de sélection ou d'entrée de texte de l'utilisateur
    private string ReponseUtilisateur; // La réponse entrée par l'utilisateur lors d'une question à réponse entière
    public string TypeQuestion; // Le type de la question en cours (QCM ou Entier)
    private int nbCarRep; // Le nombre de caractères entrés par l'utilisateur pour sa réponse en champs de saisie lors d'une question à réponse entière
    private const int nbMotVocab = 6; // PB Le nombre de mots de vocabulaires disponibles // PB a changer = QnA.Count à tester dans Start ? et chercher QnA.Count remplacer par nbMotVocab
    private bool loadAnciennePartie; // Booléen qui indique si on a load une ancienne partie (true) ou non (false)
    // PB il y a d'autres attributs plus bas au dessus de l'initialisation
    void Start(){ // Pour l'initialisation
        // On commence par lire le CSV contenant les mots de vocabulaire
        CsvLu.readCSV();

        // Debug.Log("IMPORTANT:");
        // Debug.Log(CsvLu.myQr.Count);
        QnA = CsvLu.myQr; // On récupère la liste de question/réponses obtenue avec le CsvReader

        // Pour le calcul de la vitesse de sélection 
        timer = new Stopwatch();
        
        Initialisation();
    }

    // Fonction permettant d'initialiser les valeurs des attributs pour l'initialisation dans le cas d'un tout nouveau joueur
    void InitialiseUserInitialisation(){
        // On initialise les attributs caractérisant la dynamique du quiz
        NbQuestAvantNouvelle = 1; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré
        NbAncienneQuestion = 0; // Le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'un nouveau mot
        NbQuestionsTotales = 0; // Le nombre de questions rencontrées au total // PB ou depuis la dernière màj de NbQuestAvantNouvelle?

        // Instantiation des variables pour l'initialisation
        nbBienRep = 0; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à la même question en entier
        nbMalRep = 0; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à mal répondu à la même question en entier
        numIte = 0; // Numéro de l'itération dans l'initialisation

        inInitialisation = true; // On commence l'initialisation 
    }

    // Fonction appellée lorsque l'utilisateur vient de répondre à un QCM
    public void ReponduQCM(bool correct){ // correct vaut true si l'utilisateur a donné la bonne réponse et false sinon
        // Calcul de la vitesse de clic de l'utilisateur
        timer.Stop();
        vocUt.dateDerniereRencontre[QuestionCourrante] = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"); // On met à jour la date de dernière rencontre du mot
        TimeSpan timeTaken = timer.Elapsed;
        // PB pour l affichage : OK c'est bien le temps de sélection !
        string foo = "Temps de sélection: " + timeTaken.ToString(@"m\:ss\.fff"); // PB Attention, timeTaken est un string mtn
        UnityEngine.Debug.Log(foo);

        inQCM = true; // On indique que la réponse à laquelle on vient de répondre est un QCM

        UnityEngine.Debug.Log("Question courrante = " + QuestionCourrante);
        vocUt.UpdateNbRencontres(QuestionCourrante); // On indique que l'on a rencontré une fois de plus la question
        NbQuestionsTotales += 1; // On indique que l'on a rencontré une question de plus
        NbAncienneQuestion = NbAncienneQuestionTemp; // On màj le NbAncienneQuestion mtn que l'utilisateur a répondu
        vocUt.UpdateProbaAcquisitionQCM(QuestionCourrante, correct, 1); // On met à jour les probas d'acquisition // PB paramètre hésite à changer

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
        timer.Stop();
        vocUt.dateDerniereRencontre[QuestionCourrante] = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"); // On met à jour la date de dernière rencontre du mot
        TimeSpan timeTaken = timer.Elapsed;
        // PB pour l affichage : OK c'est bien le temps d'entrée de texte et le nombre de caractères entrés !
        string foo = "Temps d'entrée de texte: " + timeTaken.ToString(@"m\:ss\.fff") + "sur " + nbCarRep + " caractères"; // PB Attention, timeTaken est un string mtn
        UnityEngine.Debug.Log(foo);

        inQCM = false; // On indique que la réponse à laquelle on vient de répondre n'est pas un QCM mais une question entière
        vocUt.UpdateNbRencontres(QuestionCourrante); // On indique que l'on a rencontré une fois de plus la question
        NbQuestionsTotales += 1; // On indique que l'on a rencontré une question de plus
        NbAncienneQuestion = NbAncienneQuestionTemp; // On màj le NbAncienneQuestion mtn que l'utilisateur a répondu

        // On affiche le carré en vert si l'utilisateur a entré la bonne réponse et sinon on affiche en rouge son carré et la bonne réponse en vert
        if (ReponseUtilisateur == QnA[QuestionCourrante].ReponseCorrecte)
        {
            UnityEngine.Debug.Log("BONNE REPONSE !");
            vocUt.UpdateProbaAcquisitionQEntier(QuestionCourrante, true, 1); // On met à jour les probas d'acquisition // PB paramètre hésite à changer
            RepEntreeOK = true; // On indique que l'utilisateur a donné la bonne réponse
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
            vocUt.UpdateProbaAcquisitionQEntier(QuestionCourrante, false, 1); // On met à jour les probas d'acquisition // PB paramètre hésite à changer
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
            UnityEngine.Debug.Log("Iteration " + numIte + " Entier");
            if (RepEntreeOK)
            {
                UnityEngine.Debug.Log("Iteration " + numIte + " a répondu à la question entière CORRECTEMENT");
                nbBienRep += 1;
            }else
            {
                UnityEngine.Debug.Log("Iteration " + numIte + " a MAL répondu à la question entière");
                nbMalRep += 1;
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
                UnityEngine.Debug.Log("Répondu à la question QCM");
                InitialisationReponduQCM();
            }else
            {
                UnityEngine.Debug.Log("Répondu à la question entière");
                Initialisation(); // On continue l'initialisation
            }
            
        }else
        {
            genererQuestion(); // On génère la question suivante
        }
    }

    // Fonction permettant de choisir une question et de la générer
    void genererQuestion(){
        UnityEngine.Debug.Log("On génère normalement désormais");
        /** PB Choisir la question posée */
        if (NbAncienneQuestion < NbQuestAvantNouvelle)
        {
            // Dans ce cas, on pose encore une question sur un mot déjà rencontré
            NbAncienneQuestionTemp = NbAncienneQuestion + 1; // On ne le change que si l'utilisateur a répondu à la question et pas tout de suite

            // On choisit la question déjà rencontrée avec la plus faible proba d'acquisition
            // Calculer les probabilités d'acquisition actuelles (selon le temps passé depuis la dernière rencontre du mot et le nombre de rencontres de ce mot)
            float[] probaAcquisActuelles = new float[nbMotVocab];
            probaAcquisActuelles = vocUt.UpdateProbaAcquisitionPowLawPrac();

            // On cherche l'indice de la question avec la plus faible proba d'acquisition actuelle
            var minProba = -1.0;
            var minInd = -1;
            for (int i = 0; i < nbMotVocab; i++)
            {
                if (vocUt.nbRencontres[i] > 0) // On ne considère la question que si elle a déjà été posée
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
                minInd = 0; // On pose la première question // PB à changer ?
            }
            QuestionCourrante = minInd;

        }else
        {
            // Dans ce cas, on pose une question sur un mot non encore rencontré
            NbAncienneQuestionTemp = 0; // On réinitialise le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'une nouvelle question
            
            List<int> IndQuestNonRencontrees = new List<int>(); // Indices des questions non encores posées
            // PB pour gain de temps, on peut plutot initialiser a tous les indices et les enlever a chaque fois qu'on rencontre une nouvelle question
            for (int i = 0; i < nbMotVocab; i++)
            {
                if (vocUt.nbRencontres[i] == 0) // Si on n'a jamais rencontré la question, on l'ajoute à la liste
                {
                    IndQuestNonRencontrees.Add(i);
                }
            }

            // UnityEngine.Debug.Log("Ensemble des questions non encore rencontrées :");
            // for (int i = 0; i < IndQuestNonRencontrees.Count; i++)
            // {
            //     UnityEngine.Debug.Log(IndQuestNonRencontrees[i]);
            // }

            if (IndQuestNonRencontrees.Count == 0) // Si toutes les questions ont déjà été posées, on prend celle de proba d'acquisition minimum
            {
                // Calculer les probabilités d'acquisition actuelles (selon le temps passé depuis la dernière rencontre du mot et le nombre de rencontres de ce mot)
                float[] probaAcquisActuelles = new float[nbMotVocab];
                probaAcquisActuelles = vocUt.UpdateProbaAcquisitionPowLawPrac();

                // On cherche l'indice de la question avec la plus faible proba d'acquisition actuelle
                var minProba = -1.0;
                var minInd = -1;
                // On choisit la question avec la proba d'acquisition la plus faible parmi toutes
                for (int i = 0; i < nbMotVocab; i++)
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
                QuestionCourrante = minInd;
            }else
            {
                // On choisit au hasard une question non rencontrée
                System.Random rnd = new System.Random();
                int IndListe  = rnd.Next(0, IndQuestNonRencontrees.Count);  // Créer un numéro entre 0 et la taille de la liste
            
                QuestionCourrante =  IndQuestNonRencontrees[IndListe];
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
        if (vocUt.nbRencontres[QuestionCourrante] > 0 && vocUt.probaAcquisition[QuestionCourrante] > 0.75) 
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
            OkButton.SetActive(true);
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
        nbCarRep = userInput.text.Length;
        // UnityEngine.Debug.Log(userInput.text);
    }

    // Fonction pour incrémenter le nombre d'ancienne question rencontré
    public void IncrementeNbQuestAvantNouvelle(){ // PB peut etre ajouter des détails comme la taille de 10 dans le cahier des charges
        NbQuestAvantNouvelle += 1;
    }

    // Des attributs pour l'initialisation
    public int nbBienRep; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à la même question en entier
    public int nbMalRep; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à mal répondu à la même question en entier
    public int numIte; // Numéro de l'itération dans l'initialisation
    public void Initialisation(){ // Permet de faire l'initialisation afin de cibler le niveau de l'utilisateur en lui posant les premières questions
        // PB prévoir une sauvegarde si l'utilisateur quitte avant la fin de l'initialisation (on doit récupérer bcp de choses : nbBienRep, nbMalRep, numIte, inQCM, ...)
        var nbIteMax = 4; // Nombre maximum d'itérations avant de génerer les questions "normalement" 
        //PB prendre en compte si on n a pas assez de donnees sur tous les cas possibles

        UnityEngine.Debug.Log("Initialisation ite " + numIte + " avec nbBienRep = " + nbBienRep + "et nbMalRep = " + nbMalRep);

        // PB sauvegarder les données d'hésitation de l'utilisateur grâce à cette initialisation
        // On fait l'initialisation jusqu'à ce que l'on ai rencontré une fois les deux cas de figure ou jusqu'à un nombre défini d'itération
        if((nbBienRep < 1 || nbMalRep < 1) && numIte <= nbIteMax) // PB ajouter un nbr d'itération max qd même
        {
            UnityEngine.Debug.Log("Toujours dans initialisation");
            if (loadAnciennePartie)
            {
                loadAnciennePartie = false; // On a chargé la dernière partie, c'est bon 
                UnityEngine.Debug.Log("On LOAD une ancienne partie");
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
                UnityEngine.Debug.Log("Iteration " + numIte + " QCM");
                TypeQuestion = "QCM";
                QuestionCourrante = ((int)numIte*(nbMotVocab-1)/nbIteMax); // On prend des mots espacés dans la base de question (du mot numéro 0 au dernier d'indice (nbMotVocab-1) puisque les indices commencent à 0)
                
                // On affiche la question choisie
                afficherPanneauEnFctTypeQuestion();
            }
            
        }else
        {
            inInitialisation = false; // On n'est plus dans l'initialisation
            UnityEngine.Debug.Log("On quitte l'initialisation");
            // Une fois l'initialisation terminée, on génère les questions comme expliqué dans le cahier des charges
            genererQuestion();
        }  
    }

    // Fonction appelée lorsque l'utilisateur est encore dans la phase d'initialisation et qu'il a répondu à un QCM (après avoir cliqué sur le bouton suivant)
    void InitialisationReponduQCM(){
        if (RepEntreeOK)
        { // PB Peut etre enregistrer les questions auquelles il a bien répondu et lui demander de les écrire en entier après pour pas faire à la suite à chaque fois
            UnityEngine.Debug.Log("Iteration " + numIte + " a répondu au QCM CORRECTEMENT");
            // Si l'utilisateur clique sur la bonne réponse, on lui repose la question sous forme d'écriture complète
            TypeQuestion = "Entier";
            // On garde la même QuestionCourrante

            // On affiche la question choisie
            afficherPanneauEnFctTypeQuestion();
        }else{
            UnityEngine.Debug.Log("Iteration " + numIte + " a MAL répondu au QCM");
            // Si l'utilisateur répond faux c'est qu'il hésitait (PB à prendre en compte même si il répond faux au QCM)
            Initialisation(); // On continue l'initialisation
        }
    }

    void Update() // Fonction appelée toute les frame
    {
        if (TypeQuestion == "Entier")
        {
            //Detecter lorsqu'on appui sur la touche entrée pour valider la réponse entrée
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ReponduEntier();                
            }
        }
    }

    // Fonction appelée lors de l'ouverture de la scène QCM
    void OnEnable()
    {
        // On charge les données sauvegardées
        UnityEngine.Debug.Log("ON ENABLE QuizManager : ON LOAD DATA");

        // Chargement des données de l'utilisateur en question
        if (PlayerPrefs.HasKey("NumJoueur")) // Normalement toujours vrai
        {
            var NumJoueur = PlayerPrefs.GetInt("NumJoueur"); // On a le num du joueur qui joue, il faut ensuite charger ses données
            UnityEngine.Debug.Log("Joueur en train de jouer : " + NumJoueur);

            // PB on a changé Userstat en mettant direct les probaAcquis, nbRencontre et dateDerniereRencontre plutot que'un vocabUtilisateur
            // PB UserStats loadedData = new UserStats(); 
            // PB loadedData.vocabUtilisateur = gameObject.AddComponent(typeof(VocabUtilisateur)) as VocabUtilisateur; // loadedData.vocabUtilisateur = new VocabUtilisateur();
            
            // *****
            // On charge les données statistiques du joueur concerné 
            UserStats loadedData = DataSaver.loadData<UserStats>("Stats_Joueur" + PlayerPrefs.GetInt("NumJoueur"));

            // UnityEngine.Debug.Log("PASSE LE LOAD, loadedData = " + loadedData);
            if (loadedData == null || EqualityComparer<UserStats>.Default.Equals(loadedData, default(UserStats)))// PB || loadedData.vocabUtilisateur == null) // ou ""
            {
                UnityEngine.Debug.Log("PAS DE DATA A LOAD");
                UnityEngine.Debug.Log("Aucune donnée statistique associée à ce joueur pour l'instant");
                // On initialise les données de vocabulaire de l'utilisateur
                vocUt.Initialise();
            }else
            {
                // Si il y a quelque chose dans les données chargées, on les charge
                // UnityEngine.Debug.Log("Proba d'acquisition récupéré = " + loadedData.probaAcquisition);
                // UnityEngine.Debug.Log("NbRencontres récupéré = " + loadedData.nbRencontres);
                // UnityEngine.Debug.Log("dateDerniereRencontre récupéré = " + loadedData.dateDerniereRencontre);

                // Mise à jour des données du joueur
                vocUt.probaAcquisition = new float[nbMotVocab];
                vocUt.nbRencontres = new int[nbMotVocab];
                vocUt.dateDerniereRencontre = new string[nbMotVocab];

                Array.Copy(loadedData.probaAcquisition, vocUt.probaAcquisition, nbMotVocab);
                Array.Copy(loadedData.nbRencontres, vocUt.nbRencontres, nbMotVocab);
                // PB sans copy : vocUt.nbRencontres = loadedData.nbRencontres;
                // PB sans copy : vocUt.dateDerniereRencontre = loadedData.dateDerniereRencontre;

                // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre == null " + loadedData.dateDerniereRencontre == null);
                // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre = " + loadedData.dateDerniereRencontre);
                // UnityEngine.Debug.Log("loadedData.dateDerniereRencontre.Length = " + loadedData.dateDerniereRencontre.Length);

                for (int i = 0; i < nbMotVocab; i++)
                {
                    if (loadedData.dateDerniereRencontre[i] != null) // On ne charge que les données qui existent (les autres restent vides mais on ne les consultera pas donc pas de problèmes)
                    {
                        vocUt.dateDerniereRencontre[i] = loadedData.dateDerniereRencontre[i]; 
                        // UnityEngine.Debug.Log("Date derniere rencontres [" + i + "] =" + loadedData.dateDerniereRencontre[i]);
                        // UnityEngine.Debug.Log("Date derniere rencontres VocUt [" + i + "] =" + vocUt.dateDerniereRencontre[i]);
                    
                    }else
                    {
                        UnityEngine.Debug.Log("loadedData.dateDerniereRencontre[" + i + "] est null");
                    } 
                }
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
            UnityEngine.Debug.Log("ON CHARGE LES DONNEES D INITIALISATION");
            UserInitialisation loadedDataInit = DataSaver.loadData<UserInitialisation>("Initialisation_Joueur" + PlayerPrefs.GetInt("NumJoueur"));

            UnityEngine.Debug.Log("LoadedData = " + loadedDataInit);
            if (loadedDataInit == null || EqualityComparer<UserInitialisation>.Default.Equals(loadedDataInit, default(UserInitialisation)))
            {
                UnityEngine.Debug.Log("Aucune donnée d'initialisation associée à ce joueur pour l'instant");
                // On indique que l'on n'a pas load d'ancienne partie (c'est une toute nouvelle partie)
                loadAnciennePartie = false;
                // On initialise les attributs du tout nouvel utilisateur pour qu'il puisse commencer sa partie
                InitialiseUserInitialisation();
            }else
            {
                UnityEngine.Debug.Log("Chargement des données d'initialisation associée à ce joueur...");
                // Si il y a quelque chose dans les données chargées, on les charge
                // Mise à jour des données du joueur
                nbBienRep = loadedDataInit.nbBienRep;
                nbMalRep = loadedDataInit.nbMalRep;
                numIte = loadedDataInit.numIte;
                inInitialisation = loadedDataInit.inInitialisation;
                RepEntreeOK = loadedDataInit.RepEntreeOK;
                NbQuestAvantNouvelle = loadedDataInit.NbQuestAvantNouvelle;
                NbAncienneQuestion = loadedDataInit.NbAncienneQuestion;
                NbQuestionsTotales = loadedDataInit.NbQuestionsTotales;
                inQCM = loadedDataInit.inQCM;
                QuestionCourrante = loadedDataInit.QuestionCourrante; 
                TypeQuestion = loadedDataInit.TypeQuestion; 
                NbAncienneQuestionTemp = loadedDataInit.NbAncienneQuestionTemp;

                // On indique que l'on a load une ancienne partie (possiblement en cours d'initialisation)
                loadAnciennePartie = true;  
            }

            UnityEngine.Debug.Log("nbBienRep =" + nbBienRep + "; nbMalRep = " + nbMalRep + "; numIte = " + numIte + "; inInitialisation = " + inInitialisation + "; RepEntreeOK = " + RepEntreeOK + "; NbQuestAvantNouvelle = " + NbQuestAvantNouvelle + "; NbAncienneQuestion = " + NbAncienneQuestion + "; NbQuestionsTotales = " + NbQuestionsTotales + "; inQCM = " + inQCM + "; QuestionCourrante = " + QuestionCourrante + "; TypeQuestion = " + TypeQuestion + "; NbAncienneQuestionTemp = " + NbAncienneQuestionTemp);
            UnityEngine.Debug.Log("loadAnciennePartie = " + loadAnciennePartie);
        }else
        {
            UnityEngine.Debug.Log("PB PROBLEME, JOUEUR NON PRECISE !!");
        }
    }
}

//PB Enlever les Debug et les PB