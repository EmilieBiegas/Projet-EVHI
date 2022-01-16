using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Classe permettant de gérer l'hésitation de l'utilisateur concernant les données oculomètriques
public class OculometreManager : MonoBehaviour
{
    public List<Vector2> occulaireHesite; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "hésite" (de PB)
    public List<Vector2> occulaireSur; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "sûr" (de PB)
    // PB ou alors donnees occulaire seulement les deux extremes puis entre les deux c'est des probas d'hésitation ?
    public Tobii.Gaming.Examples.GazePointData.PrintGazePosition printGazePosition; // Permettant de gérer l'hésitation de l'utilisateur grâce aux données occulomètriques
    private const float beta = 0.5f; // Paramètre entre 0 et 1, Beta à 1 : on ne prends en compte que la classification des données oculomètriques et pas dutout l'écart de distance

    public void InitialiseListesOcu(){
        occulaireHesite = new List<Vector2>();
        occulaireSur = new List<Vector2>();
    }

    public float EstimationHesitationOculometre(){ // Fonction estimant et retournant la probabilité d'hésitation de l'utilisateur selon l'oculomètre
        // On récupère le point oculomètrique obtenu
        Vector2 pointOcu = printGazePosition.ObtenirPtOculometre();

        // On estime l'hésitation de l'utilisateur
        Tuple<float, string> tupleRes = ClassifyKNN(pointOcu);
        float EstimationHestation = tupleRes.Item1;
        string classeEstimee = tupleRes.Item2;
        UnityEngine.Debug.Log("Hesitation selon Oculomètre : " + EstimationHestation);

        // On ajoute le point obtenu à la base de donnée
        if (classeEstimee == "hesite")
        {
            occulaireHesite.Add(pointOcu);
        }else
        {
            occulaireSur.Add(pointOcu);
        }
        return EstimationHestation;
    }

    public Tuple<float, string> ClassifyKNN(Vector2 pointObtenu){
        // Fonction permettant de classifier un point en entrée et d'estimer la probabilité d'hésitation de l'utilisateur
        // Attention, pour utiliser cette fonction, il faut que les listes de vecteurs occulaireSur et occulaireHesite soient non vides
       
        // On compare la distance du point en entrée avec chacun des points des classes // PB comparer au point moyen ou au point le plus proche ?
        float distMinSur = -1; // La distance minimale du point avec la classe sur
        for(int i = 0; i < occulaireSur.Count; i++)
        {
            var disti = Vector2.Distance(pointObtenu, occulaireSur[i]); // On regarde la distance au point de la classe Sur
            if(distMinSur == -1)  
            {
                // On initialise la distance minimum
                distMinSur = disti;
            }
            
            if(distMinSur > disti)
            {
                // On met à jour la distance minimum
                distMinSur = disti;
            }
        }

        float distMinHesite = -1; // La distance minimale du point avec la classe hesite
        for(int i = 0; i < occulaireHesite.Count; i++)
        {
            var disti = Vector2.Distance(pointObtenu, occulaireHesite[i]); // On regarde la distance au point de la classe Hesite
            if(distMinHesite == -1)  
            {
                // On initialise la distance minimum
                distMinHesite = disti;
            }
            
            if(distMinHesite > disti)
            {
                // On met à jour la distance minimum
                distMinHesite = disti;
            }
        }
			
        // On estime la classe à laquelle appartient le point en entrée et on évalue la probabilité d'hésitation de l'utilisateur
        string classeEstimee = "";
        float ProbaHesitation = 0;
        if (distMinHesite < distMinSur)
        {
            classeEstimee = "hesite";
            ProbaHesitation = beta + (1-beta)*(1 - distMinHesite / distMinSur); // Entre 0 et 1 OK
        }else{
            classeEstimee = "sur";
            ProbaHesitation = (1-beta)*(distMinSur / distMinHesite); // Entre 0 et 1 OK
        }
        return new Tuple<float, string>(ProbaHesitation, classeEstimee);
    }

    public void AjoutePtSur(){ // Fonction ajoutant le point obtenu à la classe sûr (dans le cas où nous n'avions pas encore assez de données pour estimer l'hésitation avec knn)
        // On récupère le point oculomètrique obtenu
        Vector2 pointOcu = printGazePosition.ObtenirPtOculometre();

        // On ajoute le point obtenu à la base de donnée
        occulaireSur.Add(pointOcu);
    }

    public void AjoutePtHesite(){ // Fonction ajoutant le point obtenu à la classe hesite (dans le cas où nous n'avions pas encore assez de données pour estimer l'hésitation avec knn)
        // On récupère le point oculomètrique obtenu
        Vector2 pointOcu = printGazePosition.ObtenirPtOculometre();

        // On ajoute le point obtenu à la base de donnée
        occulaireHesite.Add(pointOcu);
    }       
}
