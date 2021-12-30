// using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[System.Serializable]
// Classe permettant de rassembler les données à stocker concernant les traces du joueur (ensemble des vitesses de sélection et d'entrée de texte) dans un seul type
public class UserTraces
{
    // PB préciser la véracité de la réponse associée // PB sauvegarder cette liste de vitesse
    public List<Tuple<float, bool>>[] vitessesSelection; // Tableau de liste : chaque case du tableau correspond à un mot de vocabulaire, pour chaque mot on a une liste de tuple dont le premier élément est le temps mis et le second est la véracité de la réponse (à chaque fois qu'on a rencontré ce mot)
    public List<Tuple<float, bool>>[] vitessesEntreeTexte; // Tableau de liste : chaque case du tableau correspond à un mot de vocabulaire, pour chaque mot on a une liste de tuple dont le premier élément est le temps mis et le second est la véracité de la réponse (à chaque fois qu'on a rencontré ce mot)  
}
