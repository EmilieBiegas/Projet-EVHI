[System.Serializable] 
// Classe représentant les explications
public class Explication
{
	public string mot;
    public string nature; // Nature de mot possible: "nom","adjectif","verbe","adverbe"
    public string exemple; // Utilisation du mot dans son contexte
    public string definition;
}
