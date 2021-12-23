// using System.Collections;
using System.Collections.Generic;
// using System;
using UnityEngine;

[System.Serializable]
// Classe permettant de rassembler les données à stocker dans un seul type
public class UserStats
{
    public float[] probaAcquisition; // PB Défini le tableau de probabilité d'acquisition de chaque mot de vocabulaire
    public int[] nbRencontres; // PB Défini le tableau du nombre de rencontres de chaque mot de vocabulaire
    public string[] dateDerniereRencontre; // PB Défini le tableau de date de dernière rencontre de chaque mot de vocabulaire (en format string "MM/dd/yyyy HH:mm:ss")
    public int nivSelection; // PB Défini le niveau en terme de vitesse de sélection de l'utilisateur
    public int nivEntreeTexte; // PB Défini le niveau en terme de vitesse d'entrée de texte de l'utilisateur
    public float[][] probaAcquisNature; // PB Défini les connaissances de l'apprenant quant à la nature des mots
    public List<Vector2> occulaireHesite; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "hésite"
    public List<Vector2> occulaireSur; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "sûr"
    // PB ou alors donnees occulaire seulement les deux extremes puis entre les deux c'est des probas d'hésitation ?
}
