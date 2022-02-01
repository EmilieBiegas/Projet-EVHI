// using System.Collections;
using System.Collections.Generic;
// using System;
using UnityEngine;

[System.Serializable]
// Classe permettant de rassembler les données à stocker concernant l'initialisation dans un seul type
public class UserInitialisation
{
    public int nbBienRep; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à la même question en entier
    public int nbMalRep; // Nombre de fois où l'utilisateur a bien répondu au QCM puis à mal répondu à la même question en entier
    public int numIte; // Numéro de l'itération dans l'initialisation
    public bool inInitialisation; // Indique si on est encore dans l'initialisation (true) ou non (false)
    public bool RepEntreeOK; // Indique si la réponse entrée est bonne (true) ou non (false)
    public int NbQuestAvantNouvelle; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré (modifié seulement à la fin d'un cycle de réponse)
    public int NbQuestAvantNouvelleTemp; // Le nombre de questions nécessaires sur des mots déjà rencontrés avant une question sur un mot non encore rencontré (modifié à chaque réponse de l'utilisateur)
    public int NbAncienneQuestion; // Le nombre de questions posées sur des mots déjà rencontrés depuis la dernière rencontre d'un nouveau mot
    public int NbNouvelleQuestion; // Le nombre de questions posées sur des mots non encore rencontrés depuis la dernière rencontre d'un ancien mot
    public int NbQuestionsTotales; // Le nombre de questions rencontrées au total
    public bool inQCM; // Indique si la question à laquelle on vient de répondre est un QCM (true) ou non (false)
    
    // Pour retrouver la question posée dernièrement :
    public int QuestionCourrante; // Indice de la question courrante (càd qui est en train d'être posée)
    public string TypeQuestion; // Le type de la question en cours (QCM ou Entier)
    public int NbAncienneQuestionTemp; // Pour permettre de mettre à jour NbAncienneQuestion que quand l'utilisateur a répondu et pas avant (s'il a juste vu la question)
    public int NbNouvelleQuestionTemp; // Pour permettre de mettre à jour NbNouvelleQuestion que quand l'utilisateur a répondu et pas avant (s'il a juste vu la question)
    public List<int> IndQuestNonRencontrees; // Indices des questions non encores posées
}
