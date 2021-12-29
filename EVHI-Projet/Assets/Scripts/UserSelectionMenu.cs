// using System.Collections;
using System.Collections.Generic;
// using System;
using UnityEngine;

[System.Serializable]
// Classe permettant de rassembler les données à stocker concernant les temps de sélection dans les menus du joueur dans un seul type
public class UserSelectionMenu
{
    public List<float> tempsSelectionMenu; // Une liste de temps de sélection dans les menus permettant d'initialiser le niveau de l'utilisateur en terme de temps de sélection
}
