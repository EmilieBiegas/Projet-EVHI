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
		ContenuFicheAideQCM.text="Explication pour "+ QR.ReponseCorrecte;
		if(QR.explicationBonneReponse.nature.Length!=0){
			ContenuFicheAideQCM.text+="\n nature de mot : "+QR.explicationBonneReponse.nature;
		}
		if(QR.explicationBonneReponse.definition.Length!=0){
			ContenuFicheAideQCM.text+="\n définition : "+QR.explicationBonneReponse.definition;
		}
		if(QR.explicationBonneReponse.exemple.Length!=0){
			ContenuFicheAideQCM.text+="\n exemple : "+QR.explicationBonneReponse.exemple;
		}
		//si utilisateur ne selectionne pas la bonne reponse
		if(indReponseSelectionnee!=QR.IndReponseCorrecte){
			ContenuFicheAideQCM.text+="\n ----------------------------- ";
			ContenuFicheAideQCM.text+="\n Vous avez confondu avec : "+QR.Reponses[indReponseSelectionnee];
			//si il y a des informations supplementaire sur les autres reponses
			if(QR.explicationsSupplement!=null){
				//cherche s'il y a des informations supplementaire sur la reponse que l'utilisateur a selectionné
				for(int i=0;i<QR.explicationsSupplement.Length;i++){
					//afficher l'explication du mot l'utilisateur a selectionnée
					//Debug.Log("explicationsSupplement"+QR.explicationsSupplement.Length);
					if(QR.Reponses[indReponseSelectionnee]==QR.explicationsSupplement[i].mot){
						if(QR.explicationsSupplement[i].nature.Length!=0){
							//mettreAjourNbConfuNature(QR.explicationBonneReponse.nature,QR.explicationsSupplement[i].nature);
							ContenuFicheAideQCM.text+="\n nature de mot : "+QR.explicationsSupplement[i].nature;
						}
						if(QR.explicationsSupplement[i].definition.Length!=0){
							ContenuFicheAideQCM.text+="\n définition : "+QR.explicationsSupplement[i].definition;
						}
						if(QR.explicationsSupplement[i].exemple.Length!=0){
							ContenuFicheAideQCM.text+="\n exemple : "+QR.explicationsSupplement[i].exemple;
						}
						break;
					}
				}
			}	
		}

	}

	public void updateContenuFicheAideQEnt(string ReponseUtilisateur){
		bool IsmajnbConfuNature=false;
		ContenuFicheAideQEnt.text="Explication pour le mot "+ QR.ReponseCorrecte;
		if(QR.explicationBonneReponse.nature.Length!=0){
			ContenuFicheAideQEnt.text+="\n nature de mot : "+QR.explicationBonneReponse.nature;
		}
		if(QR.explicationBonneReponse.definition.Length!=0){
			ContenuFicheAideQEnt.text+="\n définition : "+QR.explicationBonneReponse.definition;
		}
		if(QR.explicationBonneReponse.exemple.Length!=0){
			ContenuFicheAideQEnt.text+="\n exemple : "+QR.explicationBonneReponse.exemple;
		}
		//si utilisateur n'ecrit pas la bonne reponse
		if(ReponseUtilisateur!=QR.ReponseCorrecte){
			ContenuFicheAideQEnt.text+="\n ----------------------------- ";
			ContenuFicheAideQEnt.text+="\n Vous avez confondu avec : "+ReponseUtilisateur;
			//chercher si l'utilisateur se trompe avec le mot qui a l'explication
			if(QR.explicationsSupplement!=null){
				//cherche s'il y a des informations supplementaire sur la reponse que l'utilisateur a donnée
				for(int i=0;i<QR.explicationsSupplement.Length;i++){
					//afficher l'explication du mot l'utilisateur a taper
					//Debug.Log("explicationsSupplement"+QR.explicationsSupplement.Length);
					if(ReponseUtilisateur==QR.explicationsSupplement[i].mot){
						if(QR.explicationsSupplement[i].nature.Length!=0){
							//IsmajnbConfuNature=mettreAjourNbConfuNature(QR.explicationBonneReponse.nature,QR.explicationsSupplement[i].nature);
							ContenuFicheAideQEnt.text+="\n nature de mot : "+QR.explicationsSupplement[i].nature;
						}
						if(QR.explicationsSupplement[i].definition.Length!=0){
							ContenuFicheAideQEnt.text+="\n définition : "+QR.explicationsSupplement[i].definition;
						}
						if(QR.explicationsSupplement[i].exemple.Length!=0){
							ContenuFicheAideQEnt.text+="\n exemple : "+QR.explicationsSupplement[i].exemple;
						}
						break;
					}
				}
			}
			//se tromper avec faux ami
			if(QR.fauxAmi!=null){
				if(ReponseUtilisateur==QR.fauxAmi.fauxami){
					ContenuFicheAideQEnt.text+="\n ----------------------------- ";
					ContenuFicheAideQEnt.text+="\n Vous avez confondu avec fauxami : "+QR.fauxAmi.fauxami +"\n traduction: "+QR.fauxAmi.traduction;
				}
			}
			if(! IsmajnbConfuNature ){
				//chercher si l'utilisateur se trompe sur la nature de mot
				if(detecterConfusionNature(QR.ReponseCorrecte,ReponseUtilisateur)){
					ContenuFicheAideQEnt.text+="\n       ----------------------------- ";
					ContenuFicheAideQEnt.text+="\n Vous avez confondu la nature de mot ("+typeConfusion+")";
				}
			}
		}



	}
	public bool detecterConfusionNature(string ReponseCorrecte,string ReponseUtilisateur){
		char[] charactersRU=ReponseUtilisateur.ToCharArray();//characteres reponse utilisateur
		char[] charactersRC=ReponseCorrecte.ToCharArray();//characteres reponse correcte
		
		if(ReponseUtilisateur==ReponseCorrecte){
			return false;
		}
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
	 	//List<string>type_nature_check=new List<string>();//list pour verifer le type de nature
	 	//verifier le type de nature est nom: n.,a. & n.
	 	if(QR.explicationBonneReponse.nature.Contains("n.")){
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
	 	if(QR.explicationBonneReponse.nature.Contains("a.")){
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

	 	if(QR.explicationBonneReponse.nature.Contains("v.")){
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

}
