using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
public class UserStats
{
     public float[] probaAcquisition; // PB Défini le tableau de proba d'acquisition de chaque mot de vocabulaire
    public int[] nbRencontres; // PB Défini le tableau du nombre de rencontres de chaque mot de vocabulaire
    public int nivSelection; // PB Défini le niveau en terme de vitesse de sélection de l'utilisateur
    public int nivEntreeTexte; // PB Défini le niveau en terme de vitesse d'entrée de texte de l'utilisateur
    public float[][] probaAcquisNature; // PB Défini les connaissances de l'apprenant quant à la nature des mots
    public List<Vector2> occulaireHesite; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "hésite"
    public List<Vector2> occulaireSur; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "sûr"
    // PB ou alors donnees occulaire seulement les deux extremes puis entre les deux c'est des probas d'hésitation ?
}
