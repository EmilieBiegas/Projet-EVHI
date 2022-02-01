// using System.Collections;
using System.Collections.Generic;
// using System;
using UnityEngine;

[System.Serializable]
// Classe pour stocker l'ensemble de trace de reponse utilisateur. 
//List trace :separer par indice de mot :indice commence par 2; 0 : une bonne reponse;1:une mauvaise reponse; ex: 2001130104567...: pour le premier mot on a sequence de reponse: vrai vrai false false,pour le deuxieme mot : vrai false vrai.et pas de reponse pour 4,5,6,7...
//peut-etre transformer aux list<bool>[] dans la classe quizmanager en utilisant le methode transformeDataTraceToListTraceReponse,et tranformation autre sens:transformeListTraceReponseToDataTrace
//ces données utilisées pour prédire l'acquision de mot 
public class UserTracesVeraciteRep
{
    public List<int> trace;
}
