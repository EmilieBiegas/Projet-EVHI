using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe permettant de gérer l'hésitation de l'utilisateur (au niveau de sa vitesse de sélection, d'entrée de texte)
public class HesitationManager : MonoBehaviour
{
    // PB préciser la véracité de la réponse associée // PB sauvegarder cette liste de vitesse
    public List<float>[] vitessesSelection; // Tableau de liste : chaque case du tableau correspond à un mot de vocabulaire, pour chaque mot on a la liste des temps mis (à chaque fois qu'on a rencontré ce mot)
    public List<float>[] vitessesEntreeTexte; // Tableau de liste : chaque case du tableau correspond à un mot de vocabulaire, pour chaque mot on a la liste des temps mis (à chaque fois qu'on a rencontré ce mot)
    public int nivSelection; // Défini le niveau en terme de vitesse de sélection de l'utilisateur
    public int nivEntreeTexte; // Défini le niveau en terme de vitesse d'entrée de texte de l'utilisateur
    // Paramètre alpha permettant de prendre plus ou moins en compte l'estimation d'hésitation par l'oculomètre et par la vitesse de sélection
    private const float alpha = 1; // PB Alpha à 1 : on ne prends en compte que l'hésitation déterminée par l'oculomètre et pas par la vitesse de sélection
    public OculometreManager oculometreManager;
    

    // PB les temps sont en seconde
    public static readonly string[] TousNiv = {"Best", "Good", "Avg+", "Avg-", "Bad+", "Bad-", "Worst"};
    // D'après le Keystroke-Level Model (KLM), on a les teps suivants :
    public static readonly float[] TmpsEntreeTexte = {0.08f, 0.12f, 0.20f, 0.28f, 0.5f, 0.75f, 1.2f}; // Le temps d'entrée de texte (d'un caractère) en fonction du niveau de l'utilisateur
    public static readonly float[] TmpsPointage = {0.8f, 1f, 1.1f, 1.3f, 1.4f, 1.5f, 1.6f}; // Le temps de pointage à la souris de 0.8 à 1.6 secondes en fonction du niveau de l'utilisateur
    const float TmpsHftK = 0.4f; // Le temps pour passer du clavier à un autre dispositif (souris) ou au statut inactif (pas sur le clavier ni sur le dispositif) et inversement
    const float TmpsMental = 1.35f; // Le temps de préparation mentale allant de 1.35 à 1.62 (temps nécessaire à l'utilisateur pour réflechir à sa décision)
    const float TmpsClicButton = 0.1f; // Le temps pour cliquer ou relaché un bouton (sur la souris)
    const float TmpsLectureQCM = 0f; // Le temps de lecture de la question QCM est estimé à PB secondes
    const float TmpsLectureEntier = 0f; // Le temps de lecture de la question à réponse entière est estimé à PB secondes
    private float TmpsAutourSelection = TmpsLectureQCM + TmpsMental + 2*TmpsClicButton; // Ce qui doit être ajouté au temps de pointage pour estimer le temps de réponse du QCM : pour répondre à un QCM on a TmpsLectureQCM + TmpsMental + TmpsPointage + 2*TmpsClicButton
    private float TmpsAutourEntreeTexte = TmpsLectureEntier + TmpsMental + TmpsHftK;// Ce qui doit être ajouté au temps d'entrée de texte pour estimer le temps de réponse entière : 
    // pour répondre à une question à réponse entière, on a TmpsLectureEntier + TmpsMental + (TmpsPointage[i] + 2*TmpsClicButton) + TmpsHftK + nbCarEntres*TmpsEntreeTexte (sans oublier la touche entrée) + (TmpsHftK + TmpsPointage + 2*TmpsClicButton) 
    // Première parenthèse : si on considère que l'utilisateur appuie sur la barre de saisie
    // Deuxième parenthèse : si on considère que l'utilisateur appuie sur la touche OK pour valider

    // PB Fits Law Pour pointage ??  

    public void ListesDefaut() // Initialisation des listes de vitesses de sélection et d'entrée de texte (à n'appeler que pour un nouvel utilisateur)
    {
        // Initialisation des listes de vitesses de sélection et d'entrée de texte
        vitessesSelection = new List<float>[PlayerPrefs.GetInt("NbMotsVocab")]; 
        vitessesEntreeTexte = new List<float>[PlayerPrefs.GetInt("NbMotsVocab")];
        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            vitessesSelection[i] = new List<float>();
            vitessesEntreeTexte[i] = new List<float>();
        }
    }

    public void NivDefaut(){ // Initialisation des niveaux de l'utilisateur (à n'appeler que pour un nouvel utilisateur)
        // Valeur par défaut des niveaux de l'utilisateur
        nivSelection = -1;
        nivEntreeTexte = -1;
    }

    public void ComparaisonVitesseSelection(){ // Fonction permettant d'afficher les différentes vitesses de selection selon le niveau de l'utilisateur
        string message = "vitessesSelection : ";
        for (int i = 0; i < TousNiv.Length; i++)
        {
           message += "pour " + TousNiv[i] + " est " + (TmpsAutourSelection + TmpsPointage[i]) + "; ";
        }
        UnityEngine.Debug.Log(message);
    }

    public void ComparaisonVitesseEntreeTexte(int nbCarEntres){ // Fonction permettant d'afficher les différentes vitesses d'entrée de texte selon le niveau de l'utilisateur
        string messageNormal = "vitessesEntreeTexte : ";
        string messageAppuiSaisie = "vitessesEntreeTexte avec appui sur barre saisie : ";
        string messageOK = "vitessesEntreeTexte avec appui sur touche OK : ";
        for (int i = 0; i < TousNiv.Length; i++)
        {
            messageNormal += "pour " + TousNiv[i] + " est " + (TmpsAutourEntreeTexte + nbCarEntres*TmpsEntreeTexte[i]) + "; ";
            messageAppuiSaisie += "pour " + TousNiv[i] + " est " + (TmpsAutourEntreeTexte + (TmpsPointage[i] + 2*TmpsClicButton) + nbCarEntres*TmpsEntreeTexte[i]) + "; ";
            messageOK += "pour " + TousNiv[i] + " est " + (TmpsAutourEntreeTexte + nbCarEntres*TmpsEntreeTexte[i] + (TmpsHftK + TmpsPointage[i] + 2*TmpsClicButton)) + "; ";
        }
        UnityEngine.Debug.Log(messageNormal);
        UnityEngine.Debug.Log(messageAppuiSaisie);
        UnityEngine.Debug.Log(messageOK);
    }

    public float EstimationHesitationSelection(float temps){ // Fonction estimant et retournant la probabilité d'hésitation de l'utilisateur en fonction de la vitesse de sélection
        // PB attention, il faut que nivSelection soit déjà défini qd on arrive ici (!=-1)

        MajNivSelection(temps); // On met à jour le niveau de l'utilisateur en fonction du temps qu'il a mis à répondre
        UnityEngine.Debug.Log("Niveau de sélection de l'utilisateur = " + nivSelection);
        float tempsPredit = TmpsAutourSelection + TmpsPointage[nivSelection];

        if (temps<tempsPredit)
        {
            UnityEngine.Debug.Log("HESITATION DE SELECTION ESTIMEE = 0");
            return 0; // Si il met moins de temps que ce qui est prévu, il ne présente aucune hésitation
        }
        // PB peut etre le mettre avant MajNiv?
        float hesitation = ((temps - tempsPredit)/temps); // PB entre 0 et 1 OK
        // ((temps - tempsPredit)/temps) = 1-tempsPredit/temps or, tempsPredit<temps d'où tempsPredit/temps<1 et >0 comme les deux temps sont positifs

        
        UnityEngine.Debug.Log("HESITATION DE SELECTION ESTIMEE = " + hesitation); 

        return hesitation;
    }

    public float EstimationHesitationQCM(float temps){ // Fonction estimant et retournant la probabilité d'hésitation de l'utilisateur (lorsqu'il a répondu à un QCM)
        // Nous devons prendre en compte la vitesse de sélection et les données oculomètriques
        float oculometre = oculometreManager.EstimationHesitationOculometre();
        float selection = EstimationHesitationSelection(temps);
        return alpha*oculometre + (1-alpha)*selection;
    }

    public float EstimationHesitationEntier(float temps, int NbCar){ // Fonction estimant et retournant la probabilité d'hésitation de l'utilisateur (lorsqu'il a répondu à une question à réponse entière)
        // Nous n'avons que la vitesse d'entrée de texte à prendre en compte

        // PB attention, il faut que nivEntreeTexte soit déjà défini qd on arrive ici (!=-1)

        MajNivEntreeeTexte(temps, NbCar); // On met à jour le niveau de l'utilisateur en fonction du temps qu'il a mis à répondre
        
        UnityEngine.Debug.Log("Niveau d'entrée de texte de l'utilisateur = " + nivEntreeTexte);
        float tempsPredit = TmpsAutourEntreeTexte + NbCar*TmpsEntreeTexte[nivEntreeTexte];

        if (temps<tempsPredit)
        {
            UnityEngine.Debug.Log("HESITATION D ENTREE DE TEXTE ESTIMEE = 0");
            return 0; // Si il met moins de temps que ce qui est prévu, il ne présente aucune hésitation
        }
        // PB peut etre le mettre avant MajNiv?
        float hesitation = ((temps - tempsPredit)/temps); // PB entre 0 et 1 OK
        // ((temps - tempsPredit)/temps) = 1-tempsPredit/temps or, tempsPredit<temps d'où tempsPredit/temps<1 et >0 comme les deux temps sont positifs

        
        UnityEngine.Debug.Log("HESITATION D ENTREE DE TEXTE ESTIMEE = " + hesitation); 

        return hesitation;
    }

    public void MajNivSelection(float temps){ // Fonction permettant de mettre à jour le niveau de l'utilisateur en ce qui concerne la vitesse de sélection
        // On met à jour le niveau de sélection de l'utilisateur dès que celui-ci sélectionne une réponse
        // On compare alors le temps qu'il a mis avec le temps prédit 
        
        if (nivSelection == -1) // Si on n'a pas encore initialisé le niveau de l'utilisateur
        {
            nivSelection = TousNiv.Length-1; // On commence par mettre le pire niveau à l'utilisateur (cela va se mettre à jour juste après)
        }
        
        while (nivSelection > 0 && temps < TmpsAutourSelection + TmpsPointage[nivSelection]) // S'il sélectionne plus vite que son temps prédit et que son niveau n'est pas le meilleur, c'est peut-être qu'il a un meilleur niveau
        {
            // Si son niveau n'est pas le meilleur, on augmente de 1 le rang de son niveau
            nivSelection -= 1;
        }
    }

    public void MajNivEntreeeTexte(float temps, int nbCarEntres){ // Fonction permettant de mettre à jour le niveau de l'utilisateur en ce qui concerne la vitesse d'entrée de texte
        // On met à jour le niveau d'entrée de texte de l'utilisateur dès que celui-ci entre du texte
        // On compare alors le temps qu'il a mis avec le temps prédit 
        
        if (nivEntreeTexte == -1) // Si on n'a pas encore initialisé le niveau de l'utilisateur
        {
            nivEntreeTexte = TousNiv.Length-1; // On commence par mettre le pire niveau à l'utilisateur (cela va se mettre à jour juste après)
        }

        while (nivEntreeTexte > 0 && temps < TmpsAutourEntreeTexte + nbCarEntres*TmpsEntreeTexte[nivEntreeTexte]) // S'il écrit plus vite que son temps prédit et que son niveau n'est pas le meilleur, c'est peut-être qu'il a un meilleur niveau
        {
            // Si son niveau n'est pas le meilleur, on augmente de 1 le rang de son niveau
            nivEntreeTexte -= 1;
        }
    }

    // PB a utiliser
    public void MajNivSelectionMenu(float temps){ // Fonction permettant de mettre à jour le niveau de l'utilisateur en ce qui concerne la vitesse de sélection lorsqu'il navigue dans les menus
        // On met à jour le niveau de sélection de l'utilisateur dès que celui-ci sélectionne un bouton
        // On compare alors le temps qu'il a mis avec le temps prédit par son niveau
        
        if (nivSelection == -1) // Si on n'a pas encore initialisé le niveau de l'utilisateur
        {
            nivSelection = TousNiv.Length-1; // On commence par mettre le pire niveau à l'utilisateur (cela va se mettre à jour juste après)
        }
        // PB temps mental a enlever ?
        while (nivSelection > 0 && temps < TmpsMental + TmpsPointage[nivSelection] + 2*TmpsClicButton) // S'il sélectionne plus vite que son temps prédit et que son niveau n'est pas le meilleur, c'est peut-être qu'il a un meilleur niveau
        {
            // Si son niveau n'est pas le meilleur, on augmente de 1 le rang de son niveau
            nivSelection -= 1;
        }
        
        // UnityEngine.Debug.Log("Niveau de selection du joueur selon sa sélection dans les menus est " + nivEntreeTexte);
    }

    public void MajNivEntreeeTextePseudo(float temps, int nbCarEntres){ // Fonction permettant de mettre à jour le niveau de l'utilisateur en ce qui concerne la vitesse d'entrée de texte lorsqu'il entre son pseudo
        // On met à jour le niveau d'entrée de texte de l'utilisateur dès que celui-ci entre son pseudo
        // On compare alors le temps qu'il a mis avec le temps prédit
        
        if (nivEntreeTexte == -1) // Si on n'a pas encore initialisé le niveau de l'utilisateur
        {
            nivEntreeTexte = TousNiv.Length-1; // On commence par mettre le pire niveau à l'utilisateur (cela va se mettre à jour juste après)
        }
        // PB temps mental a enlever ?
        while (nivEntreeTexte > 0 && temps < TmpsMental + nbCarEntres*TmpsEntreeTexte[nivEntreeTexte]) // S'il écrit plus vite que son temps prédit et que son niveau n'est pas le meilleur, c'est peut-être qu'il a un meilleur niveau
        {
            // Si son niveau n'est pas le meilleur, on augmente de 1 le rang de son niveau
            nivEntreeTexte -= 1;
        }

        // UnityEngine.Debug.Log("Niveau d'entrée de texte du joueur selon l'entrée de son pseudo est " + nivEntreeTexte);
    }
}