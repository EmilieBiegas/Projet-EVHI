using System.Diagnostics;
using System.Threading;
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
    public Text TxtQuestion; 
    public CsvReader CsvLu; 
    public TextAsset textData;
    public VocabUtilisateur vocUt; // PB à changer selon le profil de l'utilisateur
    public GameObject NextButton;
    public GameObject FicheButton;
    private Stopwatch timer;

    void Start(){
        // On commence par lire le CSV contenant les mots de vocabulaire
        CsvLu.textAssetData = textData; 
        CsvLu.readCSV();
        // Debug.Log("IMPORTANT:");
        // Debug.Log(CsvLu.myQr.Count);
        QnA = CsvLu.myQr;

        // On initialise les données de vocabulaire de l'utilisateur PB à faire seulement si c'est un nouveau joueur
        vocUt.Initialise();
        // Debug.Log(vocUt.probaAcquisition.Length);

        // Pour le calcul de la vitesse de sélection
        timer = new Stopwatch();
        timer.Start();
        genererQuestion();
    }

    public void Repondu(){
        // Calcul de la vitesse de clic de l'utilisateur
        timer.Stop();
        TimeSpan timeTaken = timer.Elapsed;
        // PB pour l affichage : OK c'est bien le temps de sélection !
        string foo = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
        UnityEngine.Debug.Log(foo);

        // On affiche la bonne réponse en vert et les autres en rouge
        var colorOrigin = options[0].GetComponent<Button>().colors.normalColor; // On garde la couleur d'origine des boutons
        for (int i = 0; i < options.Length; i++)
        {
            if (options[i].GetComponent<ReponseScript>().isCorrect == true)
            {
                // Change la couleur du bouton (en mode non cliquable)
                var colors = options[i].GetComponent<Button>().colors;
                colors.disabledColor = correctCol;
                options[i].GetComponent<Button>().colors = colors;
                // Rendre les boutons non cliquables
                options[i].GetComponent<Button>().interactable = false;
            }else
            {
                // Change la couleur du bouton (en mode non cliquable)
                var colors = options[i].GetComponent<Button>().colors;
                colors.disabledColor = wrongCol;
                options[i].GetComponent<Button>().colors = colors;
                // Rendre les boutons non cliquables
                options[i].GetComponent<Button>().interactable = false;
            }
        }
        // On affiche les boutons "suivant" permettant de passer à la question suivante et "fiche d'aide" permettant de consulter la fiche
        NextButton.SetActive (true);
        FicheButton.SetActive (true);
    }

    public void questionSuivante(){
        // On cache les boutons fiche et suivant 
        NextButton.SetActive (false);
        FicheButton.SetActive (false);

        // On réactive les 4 boutons de choix
        for (int i = 0; i < options.Length; i++)
        {
            // Rendre les boutons cliquables de nouveau
            options[i].GetComponent<Button>().interactable = true;
        }
        
        // On lance le timer pour la prochaine question
        timer.Reset();
        timer.Start();
        genererQuestion(); // On génère la question suivante
    }

    void genererReponses(){
        for (int i = 0; i < options.Length; i++)
        {
            options[i].GetComponent<ReponseScript>().isCorrect = false;
            options[i].transform.GetChild(0).GetComponent<Text>().text = QnA[QuestionCourrante].Reponses[i];
            if(QnA[QuestionCourrante].ReponseCorrecte == i){ 
                options[i].GetComponent<ReponseScript>().isCorrect = true;
            } 
        }
    }

    void genererQuestion(){
        // if(QnA.Count > 0){ // PB à enlever puisqu'on a tjr des questions nous, on ne les retire pas
            /** PB Choisir la question posée */

            // Test choisir la question la moins posée //PB a changer plus tard
            var minNbPosees = vocUt.nbRencontres[0];
            var minInd = 0;
            // UnityEngine.Debug.Log("Taille vocUt.nbRencontre ="+vocUt.nbRencontres.Length);
            // UnityEngine.Debug.Log("QnA.Count="+QnA.Count);
            for (int i = 0; i < QnA.Count; i++)
            {
                // Debug.Log("i="+i);
                if (minNbPosees > vocUt.nbRencontres[i])
                {
                    minNbPosees = vocUt.nbRencontres[i];
                    minInd = i;
                }
            }
            QuestionCourrante = minInd;

            //QuestionCourrante = UnityEngine.Random.Range(0, QnA.Count); //Une question aléatoire entre 0 et le nb de questions
            TxtQuestion.text = "Traduisez le mot : \n" + QnA[QuestionCourrante].Question; //La question est bien celle sélectionnée
            vocUt.UpdateNbRencontres(QuestionCourrante); // On indique que l'on a rencontré une fois de plus la question
            genererReponses();
        // }else
        // {
        //     Debug.Log("Plus de question disponible");
        // }
    }
}

//PB Enlever les Debug