// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using System.Linq; 

// Classe permettant de lire un CSV de question/réponses et de récupérer la liste des question/réponses
public class CsvReader : MonoBehaviour
{    
    public TextAsset textAssetData; // Le CSV à lire en format texte
    public TextAsset textExplications;
    public TextAsset textFauxAmis;
    public List<QuestionReponse> myQr = new List<QuestionReponse>(); // La liste des question/réponses lu d'après le CSV
    private const int nbRep = 4; // Le nombre de proposition de réponse dans chaque ligne du CSV
    private const int nbColonne = nbRep + 2; // Le nombre de colonne par mot de vocabulaire : une colonne question et une colonne réponse correcte en plus 

    public void readCSV(){
        // Debug.Log(textAssetData);
        string[] data = textAssetData.text.Split(new string[]{",","\n"}, StringSplitOptions.None); // Le CSV sous forme de float
        // Debug.Log(data.Length/nbColonne);
        int tableSize = data.Length / (nbColonne); // Le nombre de lignes de la table, càd le nombre de mot de vocabulaire

        // On met en forme les différentes question/réponses
        for (int i = 0; i < tableSize; i++) // On parcourt les lignes (càd les différentes questions)
        {
            // Mise en ordre aléatoire des réponses
            int[] indices = new int[nbRep]; // Tableau qui associe la place de la réponse à l'indice de la réponse
            for (int l = 0; l < nbRep; l++)
            {
                indices[l] = l; // Ils sont tous à la même place que l'ordre dans lequel ils sont cités dans le CSV
            }

            System.Random rnd = new System.Random(i); // random plus ou moins différent en fonction de la ligne, disons OK
            int indGood = rnd.Next(0, nbRep); // On tire au hasard l'indice de la bonne réponse (qui est au début dans la première colonne)
            indices[0] = indGood; // On inverse l'indice de la première colonne avec l'indice tiré pour que la bonne réponse soit placée à un indice aléatoire
            indices[indGood] = 0;

            // Debug.Log(data.Length);
            // Debug.Log("__________________");
            // Debug.Log("Indices=");
            // Debug.Log(indices[0]);
            // Debug.Log(indices[1]);
            // Debug.Log(indices[2]);
            // Debug.Log(indices[3]);
            // Debug.Log(i);

            // On ajoute donc la question/réponses tout juste lue (avec la place de la bonne réponse modifiée)
            myQr.Add(new QuestionReponse() {Question = data[(nbColonne)*i], Reponses = new string[nbRep]{data[nbColonne*i+1+indices[0]], data[nbColonne*i+1+indices[1]],data[nbColonne*i+1+indices[2]],data[nbColonne*i+1+indices[3]]}, ReponseCorrecte = data[nbColonne*i+1], IndReponseCorrecte = indGood});
        }

        // Debug.Log(myQr.Count);
        // Debug.Log(myQr[0].Question);
        // Debug.Log(myQr[0].Reponses);
        // Debug.Log(myQr[0].ReponseCorrecte);
    }
    public void readCSVExplications(){
        // On ajoute l'explication de bonne reponse en courant et explications supplements pour les autres reponses s'il existe
        string[] data = textExplications.text.Split(new string[]{"\n"}, StringSplitOptions.None); // separer les lignes
        //Debug.Log("csvexplications"+data.Length);
        if(data.Length!=myQr.Count){
            Debug.Log(myQr.Count);
            Debug.Log(data.Length);
            Debug.Log("problem: taille CSVExplications et différent de myQr");
        }
        
        for (int i = 0; i < data.Length; i++) // On parcourt les lignes (càd les différentes questions)
        {
            string[] explications=data[i].Split(',');//separer chaque case d'une ligne
            if(explications.Length==0){
                Debug.Log("problem: csvexplications ligne erreur");
            }
            string[] explicationBR=explications[0].Split('\\');//PBseparer mot\\nature\\exemple d'une explication
            //Debug.Log("explications taille"+explications.Length+explications[0]);
            //Debug.Log("explicationBR"+explicationBR+"taille"+explicationBR.Length+"first:  "+explicationBR[1]+explicationBR[2]);
            //for(int k=0;k<explicationBR.Length;k++){
            //    Debug.Log("explicationBR"+explicationBR[k]);
            //}
            //ajouter l'explication de bonne reponse
            myQr[i].explicationBonneReponse=new Explication(){mot=explicationBR[0],nature=explicationBR[1],definition=explicationBR[2],exemple=explicationBR[3]};//copy???
            //existe explicationSupplement pour les autres reponses
            if(explications.Length>1){
                //ajouter explicationSupplement pour les autres reponses
                myQr[i].explicationsSupplement=new Explication[explications.Length-1];
                for(int j=1;j<explications.Length;j++){
                    string[] explication=explications[j].Split('\\');//PB???????????? ca marche mais je ne comprends pas , j'ecris \ dans le fichier mais il bien separer avec \\,si je mets \\ dans csv ca me donne de case vide
                    myQr[i].explicationsSupplement[j-1]=new Explication(){mot=explication[0],nature=explication[1],definition=explication[2],exemple=explication[3]};
                    
                    //Debug.Log("!!!!!!! "+myQr[i].explicationsSupplement[j-1].mot);
                    
                }
            }
            
            
        }
    }
    public void readCSVFauxAmis(){
        //faux amis : ligne correpondant au myQr,faux amis,traduction
        string[] data = textFauxAmis.text.Split(new string[]{"\n"}, StringSplitOptions.None);
        for (int i = 0; i < data.Length; i++) // On parcourt les lignes 
        {        
            string[] ligne_fauxami=data[i].Split(',');//separer chaque case d'une ligne
            if(ligne_fauxami.Length==0){
                Debug.Log("problem: fauxAmis ligne erreur");
            }
            //Debug.Log("taille faux amis "+ligne_fauxami.Length);
            //for(int j=0;j<ligne_fauxami.Length;j++){
            //    Debug.Log("faux amis "+ligne_fauxami[j]);
            //}
            
            //ajouter faux-ami pour le mot correpondant
            myQr[int.Parse(ligne_fauxami[0])].fauxAmi=new FauxAmi(){fauxami=ligne_fauxami[1],traduction=ligne_fauxami[2]} ;          
        }
    }
}
