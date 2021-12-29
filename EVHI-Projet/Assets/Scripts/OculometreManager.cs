using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OculometreManager : MonoBehaviour
{
    public List<Vector2> occulaireHesite; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "hésite" (de PB)
    public List<Vector2> occulaireSur; // PB Défini l'ensemble des données occulaires (vecteur à deux dimensions) de la classe "sûr" (de PB)
    // PB ou alors donnees occulaire seulement les deux extremes puis entre les deux c'est des probas d'hésitation ?
    public float MaxTempsDiff; // La différence maximum de temps passé sur un mot entre deux mots 
    public float NbBougRegard; // Le nombre de fois total où l’utilisateur a bougé son regard sur un autre mot divisé par le nombre de propositions (donc la somme du nombre de fois où il a regardé chaque mot divisé par le nombre de propositions)
    
    public float EstimationHesitationOculometre(){ // Fonction estimant et retournant la probabilité d'hésitation de l'utilisateur selon l'oculomètre
        // PB
        // On ajoute le point obtenu à la base de donnée
        // On estime l'hésitation de l'utilisateur
        // On remet les compteurs à 0
        return 1;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
