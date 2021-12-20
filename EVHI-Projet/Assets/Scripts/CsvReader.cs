using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class CsvReader : MonoBehaviour
{    
    public TextAsset textAssetData;
    public List<QuestionReponse> myQr = new List<QuestionReponse>();
    private const int nbRep = 4;
    private const int nbColonne = nbRep + 2; // Une colonne question et une colonne réponse correcte en plus 

    // void Start(){
    //     //readCSV(); // readCSV est déjà appelé dans le Start de QuizManager
    // }

    public void readCSV(){
        // Debug.Log(textAssetData);
        string[] data = textAssetData.text.Split(new string[]{",","\n"}, StringSplitOptions.None);
        // Debug.Log(data.Length/nbColonne);
        int tableSize = data.Length / (nbColonne); //Il y a six colonnes dans le CSV, tableSize est le nombre de lignes de la table
        // myQr = new List<QuestionReponse>();

        for (int i = 0; i < tableSize; i++) // On parcourt les lignes (càd les différentes questions)
        {
            // Mise en ordre aléatoire des réponses
            int[] indices = new int[nbRep]; // Tableau qui associe la place de la réponse à l'indice de la réponse
            for (int l = 0; l < nbRep; l++)
            {
                indices[l] = l;
            }

            System.Random rnd = new System.Random(i); //PB random plus ou moins différent en fonction de la ligne, disons OK
            int indGood = rnd.Next(0, nbRep); // On tire au hasard l'indice de la bonne réponse
            indices[0] = indGood;
            indices[indGood] = 0;

            // Debug.Log(data.Length);
            // Debug.Log("__________________");
            // Debug.Log("Indices=");
            // Debug.Log(indices[0]);
            // Debug.Log(indices[1]);
            // Debug.Log(indices[2]);
            // Debug.Log(indices[3]);

            // Debug.Log(i);
            myQr.Add(new QuestionReponse() {Question = data[(nbColonne)*i], Reponses = new string[nbRep]{data[nbColonne*i+1+indices[0]], data[nbColonne*i+1+indices[1]],data[nbColonne*i+1+indices[2]],data[nbColonne*i+1+indices[3]]}, ReponseCorrecte = indGood});
            
            // Debug.Log("youpi");

            // for (int numQ = 0; i < nbRep; i++)
            // {
            //     myQr[i].Reponses[numQ] = data[nbColonne*i+1+numQ];                
            // }
            
            //myQr[i].ReponseCorrecte = int.Parse(data[nbColonne*i+5]);
        }

        // Debug.Log(myQr.Count);
        // Debug.Log(myQr[0].Question);
        // Debug.Log(myQr[0].Reponses);
        // Debug.Log(myQr[0].ReponseCorrecte);
    }
}
