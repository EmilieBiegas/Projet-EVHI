using System.Diagnostics;
using System;
//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using System.Text;

public class QuizManager : MonoBehaviour
{
    [SerializeField] private Color correctCol, wrongCol;
    public List<QuestionReponse> QnA; //Liste des questions/réponses
    public GameObject[] options;
    public int QuestionCourrante; //Indice de la question courrante
    public Text TxtQuestionQCM; 
    public Text TxtQuestionEnt; 
    public CsvReader CsvLu; 
    public TextAsset textData;
    public VocabUtilisateur vocUt; // PB à changer selon le profil de l'utilisateur
    public GameObject NextButtonQCM;
    public GameObject FicheButtonQCM;
    public GameObject NextButtonEntier;
    public GameObject FicheButtonEntier;
    public GameObject PanneauFicheQCM;
    public GameObject PanneauFicheEntier;
    public GameObject PanneauQEntier;
    public GameObject PanneauQCM;
    public GameObject OkButton;
    public GameObject EntreeRep;
    public GameObject BonneRep;
    private Stopwatch timer;
    private string ReponseUtilisateur;
    private string TypeQuestion;
    private bool RepEntreeOK;
    private int nbCarRep;
    // private int NumJoueur; // Le numéro du joueur qui joue actuellement (entre 1 et 3 inclus)

    void Start(){
        // On commence par lire le CSV contenant les mots de vocabulaire
        CsvLu.textAssetData = textData; 
        CsvLu.readCSV();
        // Debug.Log("IMPORTANT:");
        // Debug.Log(CsvLu.myQr.Count);
        QnA = CsvLu.myQr;

        // Pour le calcul de la vitesse de sélection
        timer = new Stopwatch();
        timer.Start();
        genererQuestion();
    }

    public void ReponduQCM(){
        // Calcul de la vitesse de clic de l'utilisateur
        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed;
        // PB pour l affichage : OK c'est bien le temps de sélection !
        string foo = "Temps de sélection: " + timeTaken.ToString(@"m\:ss\.fff");
        
        UnityEngine.Debug.Log(foo);

        vocUt.UpdateNbRencontres(QuestionCourrante); // On indique que l'on a rencontré une fois de plus la question

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
        // On affiche les boutons "suivant" permettant de passer à la question suivante et "fiche d'aide" permettant de consulter la fiche
        NextButtonQCM.SetActive (true);
        FicheButtonQCM.SetActive (true);
    }

    public void ReponduEntier(){
        // Calcul de la vitesse d'entrée de texte de l'utilisateur
        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed;
        // PB pour l affichage : OK c'est bien le temps d'entrée de texte et le nombre de caractères entrés !
        string foo = "Temps d'entrée de texte: " + timeTaken.ToString(@"m\:ss\.fff") + "sur " + nbCarRep + " caractères";;
        UnityEngine.Debug.Log(foo);

        vocUt.UpdateNbRencontres(QuestionCourrante); // On indique que l'on a rencontré une fois de plus la question

        // On affiche le carré en vert si l'utilisateur a entré la bonne réponse et sinon on affiche en rouge son carré et la bonne réponse en vert
        if (ReponseUtilisateur == QnA[QuestionCourrante].ReponseCorrecte)
        {
            UnityEngine.Debug.Log("BONNE REPONSE !");
            RepEntreeOK = true;
            // Change la couleur du bouton (en mode non cliquable)
            var colors = EntreeRep.GetComponent<InputField>().colors;
            colors.disabledColor = correctCol;
            EntreeRep.GetComponent<InputField>().colors = colors;
            // Rendre le champs de saisie non cliquable
            EntreeRep.GetComponent<InputField>().interactable = false;
            OkButton.GetComponent<Button>().interactable = false;
        }else
        {
            UnityEngine.Debug.Log("MAUVAISE REPONSE !");
            RepEntreeOK = false;
            // On met la réponse rentrée en rouge et on affiche un deuxième carré contenant la bonne réponse
            var colors = EntreeRep.GetComponent<InputField>().colors;
            colors.disabledColor = wrongCol;
            EntreeRep.GetComponent<InputField>().colors = colors;

            colors = BonneRep.GetComponent<InputField>().colors;
            colors.disabledColor = correctCol;
            BonneRep.GetComponent<InputField>().colors = colors;

            // Afficher le bouton bonne réponse dans ce cas contenant la bonne réponse
            BonneRep.SetActive(true);
            BonneRep.GetComponent<InputField>().text = QnA[QuestionCourrante].ReponseCorrecte;

            // Rendre le champs de saisie et le bouton OK non cliquable
            EntreeRep.GetComponent<InputField>().interactable = false;
            BonneRep.GetComponent<InputField>().interactable = false;
            OkButton.GetComponent<Button>().interactable = false;
        }

        // On affiche les boutons "suivant" permettant de passer à la question suivante et "fiche d'aide" permettant de consulter la fiche
        NextButtonEntier.SetActive (true);
        FicheButtonEntier.SetActive (true);
    }


    public void questionSuivante(){
        // On cache les boutons fiche, suivant et bonne réponse
        NextButtonQCM.SetActive (false);
        FicheButtonQCM.SetActive (false);
        NextButtonEntier.SetActive (false);
        FicheButtonEntier.SetActive (false);
        BonneRep.SetActive(false);

        // On réactive les 4 boutons de choix et le champs de saisie de réponse
        for (int i = 0; i < options.Length; i++)
        {
            // Rendre les boutons cliquables de nouveau
            options[i].GetComponent<Button>().interactable = true;
        }
        EntreeRep.GetComponent<InputField>().interactable = true;
        OkButton.GetComponent<Button>().interactable = true;
        
        // On lance le timer pour la prochaine question
        timer.Reset();
        timer.Start();
        genererQuestion(); // On génère la question suivante
    }

    void genererQuestion(){

        /** PB Choisir la question posée */

        // Test choisir la question la moins posée //PB a changer plus tard
        UnityEngine.Debug.Log("On cherche la question suivante :");
        var minNbPosees = vocUt.nbRencontres[0];
        var minInd = 0;
        UnityEngine.Debug.Log("Initialisation :" + minNbPosees);
        // UnityEngine.Debug.Log("Taille vocUt.nbRencontre ="+vocUt.nbRencontres.Length);
        // UnityEngine.Debug.Log("QnA.Count="+QnA.Count);
        for (int i = 0; i < QnA.Count; i++)
        {
            UnityEngine.Debug.Log("On compare min actuel =" + minNbPosees + " avec " + vocUt.nbRencontres[i]);
            // Debug.Log("i="+i);
            if (minNbPosees > vocUt.nbRencontres[i])
            {
                minNbPosees = vocUt.nbRencontres[i];
                minInd = i;
            }
        }
        QuestionCourrante = minInd;

        // Test si la question a déjà été posée, on propose d'entrée la réponse entièrement //PB a changer plus tard
        if (vocUt.nbRencontres[minInd] > 0) 
        {
            TypeQuestion = "Entier";
        }else
        {
            TypeQuestion = "QCM";
        }

        if(TypeQuestion == "QCM"){ 
            PanneauQEntier.SetActive(false);
            PanneauQCM.SetActive(true);

            //QuestionCourrante = UnityEngine.Random.Range(0, QnA.Count); //Une question aléatoire entre 0 et le nb de questions
            TxtQuestionQCM.text = "Traduisez le mot : \n" + QnA[QuestionCourrante].Question; //La question est bien celle sélectionnée
            genererReponses();
        }else
        {
            // Question dont la réponse doit être entrée entièrement
            PanneauQEntier.SetActive(true);
            PanneauQCM.SetActive(false);

            // On fait en sorte que le champs de saisie soit automatiquement sélectionné (pas besoin de cliquer dessus)
            EntreeRep.GetComponent<InputField>().Select();
            
            // Pour pouvoir lire ce qu'écrit l'utilisateur
            EntreeRep.GetComponent<InputField>().onEndEdit.AddListener(delegate { inputBetValue(EntreeRep.GetComponent<InputField>()); });

            TxtQuestionEnt.text = "Traduisez le mot : \n" + QnA[QuestionCourrante].Question; //La question est bien celle sélectionnée
        }
    }

    void genererReponses(){
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<ReponseScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[QuestionCourrante].Reponses[i];
            if(QnA[QuestionCourrante].IndReponseCorrecte == i){ 
                options[i].GetComponent<ReponseScript>().isCorrect = true;
            } 
        }
    }

    // Fonctions d'affichage de la fiche d'aide
    public void AfficherFiche(){
        if (TypeQuestion == "QCM")
        {
            PanneauFicheQCM.SetActive(true);
        }else
        {
            PanneauFicheEntier.SetActive(true);
            // On doit cacher le champs de saisie de réponse, le camps de la bonne réponse et le bouton OK
            EntreeRep.SetActive(false);
            OkButton.SetActive(false);
            BonneRep.SetActive(false);
        }
    }
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
            if (RepEntreeOK == false)
            {
                BonneRep.SetActive(true); // Le réafficher seulement si on avait entré une mauvaise réponse                
            }
        }
    }

    // Fonctions des questions sans choix (on doit entrer la réponse à la main)
    public void inputBetValue(InputField userInput)
    {
        ReponseUtilisateur = userInput.text;
        nbCarRep = userInput.text.Length;
        // UnityEngine.Debug.Log(userInput.text);
    }

    void Update()
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
        UnityEngine.Debug.Log("ON ENABLE QuizManager : ON LOAD DATA");

        // Chargement des données de l'utilisateur en question
        if (PlayerPrefs.HasKey("NumJoueur")) // Normalement toujours vrai
        {
            var NumJoueur = PlayerPrefs.GetInt("NumJoueur"); // On a le num du joueur qui joue, il faut ensuite charger ses données
            UnityEngine.Debug.Log("Joueur en train de jouer : " + NumJoueur);

            // PB on a changé Userstat en mettant direct les probaAcquis et nbRencontre plutot que'un vocabUtilisateur
            //UserStats loadedData = new UserStats(); 
            //loadedData.vocabUtilisateur = gameObject.AddComponent(typeof(VocabUtilisateur)) as VocabUtilisateur; // loadedData.vocabUtilisateur = new VocabUtilisateur();
            // On charge les données du joueur concerné // PB HERE en cours
            UserStats loadedData = DataSaver.loadData<UserStats>("Joueur" + PlayerPrefs.GetInt("NumJoueur"));

            UnityEngine.Debug.Log("PASSE LE LOAD, loadedData = " + loadedData);
            if (loadedData == null )// PB || loadedData.vocabUtilisateur == null)
            {
                UnityEngine.Debug.Log("PAS DE DATA A LOAD");
                UnityEngine.Debug.Log("Aucune donnée associée à ce joueur pour l'instant");
                // On initialise les données de vocabulaire de l'utilisateur PB à faire seulement si c'est un nouveau joueur
                vocUt.Initialise();
                return;
            }
            UnityEngine.Debug.Log("Proba d'acquisition récupéré = " + loadedData.probaAcquisition);
            UnityEngine.Debug.Log("NbRencontres récupéré = " + loadedData.nbRencontres);

            // Mise à jour des données du joueur
            vocUt.probaAcquisition = new float[6];
            vocUt.nbRencontres = new int[6];
            Array.Copy(loadedData.probaAcquisition,  vocUt.probaAcquisition, 6);
            Array.Copy(loadedData.nbRencontres,  vocUt.nbRencontres, 6);
            // PB sans copy : vocUt.nbRencontres = loadedData.nbRencontres;

            // Affichage des données chargées
            for (int i = 0; i < 6; i++)
            {
                // UnityEngine.Debug.Log("Proba d'acquisition [" + i + "] =" + loadedData.probaAcquisition[i]);
                // UnityEngine.Debug.Log("Proba d'acquisition VocUt [" + i + "] =" + vocUt.probaAcquisition[i]);
                UnityEngine.Debug.Log("Nb rencontres [" + i + "] =" + loadedData.nbRencontres[i]);
                UnityEngine.Debug.Log("Nb rencontres VocUt [" + i + "] =" + vocUt.nbRencontres[i]);
            }
        }else
        {
            UnityEngine.Debug.Log("PB PROBLEME, JOUEUR NON PRECISE !!");
        }
    }
}

//PB Enlever les Debug