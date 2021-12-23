// using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// using System.Linq; // PB régler les commentaires dans les using

// Classe permettant de lire un CSV de question/réponses et de récupérer la liste des question/réponses
public class CsvReader : MonoBehaviour
{    
    public TextAsset textAssetData; // Le CSV à lire
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

            System.Random rnd = new System.Random(i); //PB random plus ou moins différent en fonction de la ligne, disons OK
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
}
