using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
// Classe permettant de rassembler les données à stocker concernant les statistiques du joueur dans un seul type
public class UserStats
{
    public float[] probaAcquisition; // Défini le tableau de probabilité d'acquisition de chaque mot de vocabulaire (de VocabUtilisateur)
    public int[] nbRencontres; // Défini le tableau du nombre de rencontres de chaque mot de vocabulaire (de VocabUtilisateur)
    public string[] dateDerniereRencontre; // Défini le tableau de date de dernière rencontre de chaque mot de vocabulaire (en format string "MM/dd/yyyy HH:mm:ss") (de VocabUtilisateur)
    public int nivSelection; // Défini le niveau en terme de vitesse de sélection de l'utilisateur (de HesitationManager)
    public int nivEntreeTexte; // Défini le niveau en terme de vitesse d'entrée de texte de l'utilisateur (de HesitationManager)
    public float[][] probaAcquisNature; // Défini les connaissances de l'apprenant quant à la nature des mots
    public List<Vector2> occulaireHesite; // Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "hésite" (de OculometreManager)
    public List<Vector2> occulaireSur; // Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "sûr" (de OculometreManager)
    public float[] list_probaL_init;//meme taille que myQr proba d'acquision au debut pour chaque mot
    public float[] list_proba_G;//proba guess
    public float[] list_proba_S;//proba slip
    public float[] list_proba_T;//proba de passer etat not learned vers learned
}
