using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// Classe permettant de gérer la fiche d'aide
public class FicheManager : MonoBehaviour
{
	public Text ContenuFicheAideQCM;
	public Text ContenuFicheAideQEnt;
	public  QuestionReponse QR;
	public int indReponseSelectionnee;
	public static readonly List<string> SUFFIXE_NOM_NOM=new List<string>(){"dom","ship","hood","ian","er","ism"};
	public static readonly List<string> SUFFIXE_NOM_ADJ=new List<string>(){"en","less","ish","ful","al","ly"};
	public static readonly List<string> SUFFIXE_NOM_VERBE=new List<string>(){"en"};
	public static readonly List<string> SUFFIXE_ADJ_NOM=new List<string>(){"dom","ness","ship","hood","ity"};
	public static readonly List<string> SUFFIXE_ADJ_VERBE=new List<string>(){"ize"};
	public static readonly List<string> SUFFIXE_ADJ_ADVERBE=new List<string>(){"ly"};
	public static readonly List<string> SUFFIXE_VERBE_NOM=new List<string>(){"er","ee","al","ence","ee"};
	public static readonly List<string> SUFFIXE_VERBE_ADJ=new List<string>(){"able","ing","ed","ive"};
	public string typeConfusion;
	public void updateContenuFicheAideQCM(){
		ContenuFicheAideQCM.text="explication for "+ QR.ReponseCorrecte;
		ContenuFicheAideQCM.text+="\n nature de mot : "+QR.explicationBonneReponse.nature;
		ContenuFicheAideQCM.text+="\n definition : "+QR.explicationBonneReponse.definition;
		ContenuFicheAideQCM.text+="\n exemple : "+QR.explicationBonneReponse.exemple;
		//si utilisateur ne selectionne pas la bonne reponse
		if(indReponseSelectionnee!=QR.IndReponseCorrecte){
			ContenuFicheAideQCM.text+="\n vous avez confondu avec : "+QR.Reponses[indReponseSelectionnee];
			//si il y a des informations supplementaire sur les autres reponses
			if(QR.explicationsSupplement!=null){
				//cherche s'il y a des informations supplementaire sur la reponse que l'utilisateur a selectionné
				for(int i=0;i<QR.explicationsSupplement.Length;i++){
					//afficher l'explication du mot l'utilisateur a selectionnée
					//Debug.Log("explicationsSupplement"+QR.explicationsSupplement.Length);
					if(QR.Reponses[indReponseSelectionnee]==QR.explicationsSupplement[i].mot){
						ContenuFicheAideQCM.text+="\n nature de mot : "+QR.explicationsSupplement[i].nature;
						ContenuFicheAideQCM.text+="\n definition : "+QR.explicationsSupplement[i].definition;
						ContenuFicheAideQCM.text+="\n exemple : "+QR.explicationsSupplement[i].exemple;
						break;
					}
				}
			}	
		}

	}
	public void updateContenuFicheAideQEnt(string ReponseUtilisateur){
		ContenuFicheAideQEnt.text="explication for "+ QR.ReponseCorrecte;
		ContenuFicheAideQEnt.text+="\n nature de mot "+QR.explicationBonneReponse.nature;
		ContenuFicheAideQEnt.text+="\n definition : "+QR.explicationBonneReponse.definition;
		ContenuFicheAideQEnt.text+="\n exemple : "+QR.explicationBonneReponse.exemple;
		//si utilisateur n'ecrit pas la bonne reponse
		if(ReponseUtilisateur!=QR.ReponseCorrecte){
			ContenuFicheAideQEnt.text+="\n confondu avec "+ReponseUtilisateur;
			//chercher si l'utilisateur se trompe avec le mot qui a l'explication
			if(QR.explicationsSupplement!=null){
				//cherche s'il y a des informations supplementaire sur la reponse que l'utilisateur a donnée
				for(int i=0;i<QR.explicationsSupplement.Length;i++){
					//afficher l'explication du mot l'utilisateur a taper
					//Debug.Log("explicationsSupplement"+QR.explicationsSupplement.Length);
					if(ReponseUtilisateur==QR.explicationsSupplement[i].mot){
						ContenuFicheAideQEnt.text+="\n nature de mot : "+QR.explicationsSupplement[i].nature;
						ContenuFicheAideQEnt.text+="\n definition : "+QR.explicationsSupplement[i].definition;
						ContenuFicheAideQEnt.text+="\n exemple : "+QR.explicationsSupplement[i].exemple;
						break;
					}
				}
			}
			//se tromper avec faux ami
			if(QR.fauxAmi!=null){
				if(ReponseUtilisateur==QR.fauxAmi.fauxami){
					ContenuFicheAideQEnt.text+="\n confondu avec fauxami\n traduction: "+QR.fauxAmi.traduction;
				}
			}
			//chercher si l'utilisateur se trompe sur la nature de mot
			if(detecterConfusionNature(ReponseUtilisateur)){
				ContenuFicheAideQEnt.text+="\n confondre la nature de mot ("+typeConfusion+")";
			}
		}



	}
	public bool detecterConfusionNature(string ReponseUtilisateur){
		Debug.Log("detecterConfusionNature");
		char[] charactersRU=ReponseUtilisateur.ToCharArray();//characteres reponse utilisateur
		char[] charactersRC=QR.ReponseCorrecte.ToCharArray();//characteres reponse correcte
		Debug.Log("charactersRU "+charactersRU.Length+"  charactersRC "+charactersRC.Length);

	 	if(charactersRU.Length<=charactersRC.Length){
	 		//impossible d'avoir un suffixe
	 		return false;
	 	}
	 	// vérifier préfixe de reponseUtilisateur est motCorrecte 
	 	// comparer caractère par caractère
	 	for(int i=0;i<charactersRC.Length;i++){
	 	 	if(charactersRC[i]!=charactersRU[i]){
	 	 		return false;
	 	 	}
	 	}
	 	// préfixe de reponseUtilisateur est motCorrecte
	 	// trouver le suffixe,càd les caractères dans reponseUtilisateur apres motcorrecte
	 	string suffixe=(new string(charactersRU)).Substring(charactersRC.Length);
	 	if(QR.explicationBonneReponse.nature=="n."){
	 		//Debug.Log("nature !!!!!!"+" suffixe "+suffixe);
	 		if(SUFFIXE_NOM_NOM.Contains(suffixe)){
	 			typeConfusion="NOM_NOM";
	 			return true;
	 		}
	 		if(SUFFIXE_NOM_ADJ.Contains(suffixe)){
	 			typeConfusion="NOM_ADJ";
	 			return true;
	 		}
	 		if(SUFFIXE_NOM_VERBE.Contains(suffixe)){
	 			typeConfusion="NOM_VERBE";
	 			return true;
	 		}
	 	}
	 	if(QR.explicationBonneReponse.nature=="adj."){
	 		//Debug.Log("nature !!!!!!"+" suffixe "+suffixe);
	 		if(SUFFIXE_ADJ_NOM.Contains(suffixe)){
	 			typeConfusion="ADJ_NOM";
	 			return true;
	 		}
	 		if(SUFFIXE_ADJ_VERBE.Contains(suffixe)){
	 			typeConfusion="ADJ_VERBE";
	 			return true;
	 		}
	 		if(SUFFIXE_ADJ_ADVERBE.Contains(suffixe)){
	 			typeConfusion="ADJ_ADVERBE";
	 			return true;
	 		}
	 	}

	 	if(QR.explicationBonneReponse.nature=="v."){
	 		//Debug.Log("nature !!!!!!"+" suffixe "+suffixe);
	 		if(SUFFIXE_VERBE_ADJ.Contains(suffixe)){
	 			typeConfusion="VERBE_ADJ";
	 			return true;
	 		}
	 		if(SUFFIXE_VERBE_NOM.Contains(suffixe)){
	 			typeConfusion="VERBE_NOM";
	 			return true;
	 		}

	 	}

	 	return false;
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
