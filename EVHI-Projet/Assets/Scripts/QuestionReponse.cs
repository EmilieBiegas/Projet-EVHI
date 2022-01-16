[System.Serializable] 
// Classe représentant les question / réponses
public class QuestionReponse
{
    public string Question;
    public string[] Reponses;
    public int IndReponseCorrecte; // Attention, commence à 0
    public string ReponseCorrecte;
    public Explication explicationBonneReponse;
    public Explication[] explicationsSupplement;
    public FauxAmi fauxAmi;

}
