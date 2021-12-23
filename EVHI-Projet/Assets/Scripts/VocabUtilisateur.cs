using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

// Classe permettant de gérer les données associées au vocabulaire de l'utilisateur (statistiques sur son apprentissage et sur son interaction avec le jeu)
public class VocabUtilisateur : MonoBehaviour
{
    public float[] probaAcquisition; //Un tableau de proba d'acquisition de la même taille que QnA de QuizManager ou myQr de CsvReader (càd le nombre de mots de vocabulaire)
    public int[] nbRencontres; //Un tableau du nombre de fois où le mot a été rencontré de la même taille que QnA de QuizManager ou myQr de CsvReader (càd le nombre de mots de vocabulaire)
    public string[] dateDerniereRencontre; //Un tableau de date de la dernière rencontre au format "MM/dd/yyyy HH:mm:ss" pour pouvoir appliquer la Power Law of Practice
    private const int nbMotVocab = 6; // PB Le nombre de mots de vocabulaires disponibles
    // Paramètre beta permettant de prendre plus ou moins en compte la correction de la réponse donnée et l'hésitation de l'utilisateur dans la proba d'acquisition du mot de vocabulaire
    private const float beta = 1; // PB Beta à 1 : on ne prends en compte que la correction de la réponse donnée et pas l'hésitation

    public void Initialise() // Initialisation des statistiques (à n'appeler que pour un nouvel utilisateur)
    {
        probaAcquisition = new float[nbMotVocab]{0,0,0,0,0,0}; // PB si pas 6
        nbRencontres = new int[nbMotVocab]{0,0,0,0,0,0}; // PB si pas 6
        dateDerniereRencontre = new string[nbMotVocab]; //PB {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"), DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}; 
        // PB Penser à regarder le dateDerniereRencontre seulement si nbRencontres > 0 !!

        // Debug.Log("Taille des probas = "+ probaAcquisition.Length);
        // Debug.Log("nbMotVocab=" + nbMotVocab);
    }

    //PB a utiliser selon les cas
    // public void setProbaAcquisition(int indiceQuestion, float valProba){
    //     probaAcquisition[indiceQuestion] = valProba;
    // }

    // Fonction qui màj la proba d'acquisition lorsque l'on répond à un QCM // PB à adapter
    public void UpdateProbaAcquisitionQCM(int indiceQuestion, bool correct, float hesite){
        // correct vaut true si l'utilisateur a donné la bonne réponse et false sinon
        // hesite est déterminé par la probabilité d’hésitation de l’utilisateur, et vaut 1 si l’utilisateur hésite totalement (d’où le 1-cette proba)
        int intCorrect = correct ? 1 : 0; // Vaut 1 si true et 0 si false
        
        probaAcquisition[indiceQuestion] = beta*intCorrect + (1-beta)*(1-hesite);  
    }

    // Fonction qui màj la proba d'acquisition lorsque l'on répond à un Questionnaire à réponse entière // PB à adapter
    public void UpdateProbaAcquisitionQEntier(int indiceQuestion, bool correct, float hesite){ 
        // correct vaut true si l'utilisateur a donné la bonne réponse et false sinon
        // hesite est déterminé par la probabilité d’hésitation de l’utilisateur, et vaut 1 si l’utilisateur hésite totalement (d’où le 1-cette proba)
        int intCorrect = correct ? 1 : 0; // Vaut 1 si true et 0 si false
        
        probaAcquisition[indiceQuestion] = beta*intCorrect + (1-beta)*(1-hesite);  // PB peut etre un autre beta ?          
        
    }

    // Fonction qui retourne les probas d'acquisition actuelle en fonction du temps passé depuis la dernière rencontre du mot et de la proba d'acquisition enregistrée à cette date
    public float[] UpdateProbaAcquisitionPowLawPrac(){ // PB changer nom
        // PB Estimer chaque proba d'acquisition au temps actuel en fonction de la power Law of Practice 
        float[] newProbaAcquisition = new float[nbMotVocab];
        Array.Copy(probaAcquisition, newProbaAcquisition, nbMotVocab); // On fait une copy pour ne pas perdre les valeurs de proba d'acquisition au temps de la dernière rencontre

        // On converti les dates en DateTime depuis le type string
        DateTime[] DerniereRencontreDATE = new DateTime[nbMotVocab];
        TimeSpan[] TempsEntreDerniereDateEtAuj = new TimeSpan[nbMotVocab]; // Temps écoulé depuis la dernière rencontre du mot
        float[] floatTimeSpan = new float[nbMotVocab]; // Temps écoulé depuis la dernière rencontre du mot en float (en seconde par ex.)
        // IFormatProvider culture = new CultureInfo("en-US", true); // PB
        // DateTimeFormatInfo culture = new DateTimeFormatInfo(); //"MM/dd/yyyy HH:mm:ss" // PB

        for (int i = 0; i < nbMotVocab; i++)
        {
            if (dateDerniereRencontre[i] != null && dateDerniereRencontre[i] != "")
            {
                // UnityEngine.Debug.Log("dateDerniereRencontre[i]=" + dateDerniereRencontre[i] + " de type " + dateDerniereRencontre[i].GetType());
                // UnityEngine.Debug.Log("culture=" + culture + " de type " + culture.GetType()); 
                // PB DateTime convertedDate = System.DateTime.ParseExact(dateDerniereRencontre[i], "MM/dd/yyyy HH:mm:ss", null);
                // UnityEngine.Debug.Log("DateTime.Parse(dateDerniereRencontre[i], culture)=" + convertedDate + " de type " + convertedDate.GetType());
                
                // On récupère la date de dernière rencontre en type DateTime
                DerniereRencontreDATE[i] = System.DateTime.ParseExact(dateDerniereRencontre[i], "MM/dd/yyyy HH:mm:ss", null);
                // UnityEngine.Debug.Log("Date obtenue : " + DerniereRencontreDATE[i]);  
                
                // On calcule le temps écoulé entre ce temps et maintenant
                TempsEntreDerniereDateEtAuj[i] = DateTime.Now - DerniereRencontreDATE[i]; // Sous forme HH:mm:ss
                // UnityEngine.Debug.Log("Duree depuis la derniere rencontre obtenue : " + TempsEntreDerniereDateEtAuj[i]);  

                // On transforme la durée obtenue en float (nombre de secondes/minutes/heures écoulées)
                int days, hours, minutes, seconds, milliseconds;
                days = TempsEntreDerniereDateEtAuj[i].Days;
                hours = TempsEntreDerniereDateEtAuj[i].Hours;
                minutes = TempsEntreDerniereDateEtAuj[i].Minutes;
                seconds = TempsEntreDerniereDateEtAuj[i].Seconds;
                milliseconds = TempsEntreDerniereDateEtAuj[i].Milliseconds;
                // UnityEngine.Debug.Log("Temps : " + days + " jours, " + hours + " heures, " + minutes + " minutes, " + seconds + " secondes, " + milliseconds + " millisecondes");
                // UnityEngine.Debug.Log("Temps en secondes : " + ((float)days*24*3600) + "(jours) + " + ((float)hours*3600) + "(heures) + " + ((float)minutes*60) + "(minutes) + " + (float)seconds + "(secondes) + " + ((float)milliseconds/1000) + "(millisecondes)");
                // Temps obtenu en secondes :
                floatTimeSpan[i] = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
                // floatTimeSpan[i] /= 60; // On met le résultat en minutes plutôt // PB
                // floatTimeSpan[i] /= 3600; // On met le résultat en heures plutôt // PB
                floatTimeSpan[i] /= 24*3600; // On met le résultat en jours plutôt comme c'est plus cohérent pour la fonction d'oubli // PB
                // UnityEngine.Debug.Log("Temps passé en jours :" + floatTimeSpan[i]);

                // On applique la fonction choisie (ici, courbe de l'oubli)
                float ValApresFct = CourbeOubli(floatTimeSpan[i], nbRencontres[i]);
                // UnityEngine.Debug.Log("Après courbe de l'oubli : " + CourbeOubli(floatTimeSpan[i], nbRencontres[i]) + "avec proba acquis donne :" + ValApresFct*probaAcquisition[i]);
                // UnityEngine.Debug.Log("Après courbe de l'oubli +1: " + CourbeOubli(floatTimeSpan[i], nbRencontres[i]+1));
                // UnityEngine.Debug.Log("Après courbe de l'oubli +2: " + CourbeOubli(floatTimeSpan[i], nbRencontres[i]+2));
                // UnityEngine.Debug.Log("Après courbe de l'oubli +3: " + CourbeOubli(floatTimeSpan[i], nbRencontres[i]+3)); // OK ça a bien l'allure souhaitée

                // On met à jour la proba d'acquisition actuelle (toujours située entre 0 et 1)
                newProbaAcquisition[i] = ValApresFct * probaAcquisition[i]; // On prend également en compte la proba d'acquisition déjà établie au dernier temps de rencontre du mot
            }else
            {
                // UnityEngine.Debug.Log("dateDerniereRencontre[" + i + "] est null ou vide");
                // Dans ce cas, aucun changement pour la proba d'acquisition
            }      
        }
        
        return newProbaAcquisition;
    }

    // Fonction choisie pour caractériser l'oubli en fonction du temps et du nombre de rappels de la connaissance
    private float CourbeOubli(float t, int F){ // t la durée entre la dernière rencontre du mot et maintenant, F le nombre de rencontres du mot
        // Retourne une valeur entre 0 et 1 comme t > 0 et F > 0 soit t/F > 0 donc -t/F va de -inf à 0 et donc exp(-t/F) va de 0 à 1
        return (float)Math.Exp(-t/F);
    }

    public void UpdateNbRencontres(int indiceQuestion){ // Fonction permettant d'indiquer que l'on a rencontré une fois de plus la question d'indice indiceQuestion
        nbRencontres[indiceQuestion] += 1;
    }
}
