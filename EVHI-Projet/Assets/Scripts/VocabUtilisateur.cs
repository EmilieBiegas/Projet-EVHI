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
    // Paramètre beta permettant de prendre plus ou moins en compte l'hésitation de l'utilisateur ou la probabilité de guess
    private const float beta = 0.5f; // Paramètre entre 0 et 1
    public float probaL_init=0f; 
    public float proba_G=0.25f;//proba guess
    public float proba_S=0.02f;//proba slip
    public float proba_T=0.5f;//proba de passer etat not learned vers learned
    // public float [][] probaNature;
    // public bool[] detecterConfusionNature;
    public float[] PI;//PI[0]=probaL_init(proba de connais deja le mot au debut) (pour hmm)
    public float[,] A;//tableau de transition (pour hmm)
    public float[,] B;//tableau d'emission(pour hmm)
    public float[] list_probaL_init;//meme taille que myQr proba d'acquision au debut pour chaque mot,predire avec la formule beta*intCorrect + (1-beta)*(1-hesite)*intCorrect
    public float[] list_proba_G;//proba guess
    public float[] list_proba_S;//proba slip
    public float[] list_proba_T;//proba de passer etat not learned vers learned

    public void Initialise() // Initialisation des statistiques (à n'appeler que pour un nouvel utilisateur)
    {
        probaAcquisition = new float[PlayerPrefs.GetInt("NbMotsVocab")];
        nbRencontres = new int[PlayerPrefs.GetInt("NbMotsVocab")];
        list_probaL_init=new float [PlayerPrefs.GetInt("NbMotsVocab")];
        list_proba_S=new float [PlayerPrefs.GetInt("NbMotsVocab")];
        list_proba_G=new float [PlayerPrefs.GetInt("NbMotsVocab")];
        list_proba_T=new float [PlayerPrefs.GetInt("NbMotsVocab")];

        // On initialise toutes les données à 0
        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            probaAcquisition[i] = 0;
            nbRencontres[i] = 0;
            list_probaL_init[i]=0f;
            list_proba_S[i]=0.1f;
            list_proba_G[i]=0.25f;//comme on a 4 choix
            list_proba_T[i]=0.5f;
        }

        dateDerniereRencontre = new string[PlayerPrefs.GetInt("NbMotsVocab")];
    }

    public float calcul_alpha_i(List<bool> sequenceReponse,int indice,int etat){
        
        if(indice==0){
            int indi= ((bool)sequenceReponse[0]) ? 0 : 1;
            //UnityEngine.Debug.Log("alpha value "+PI[etat]*B[etat,indi]+" etat "+etat+"indi "+indi);
            return PI[etat]*B[etat,indi];//vaut 0 si vrai,vaut 1 si true
        }
        float parite1=A[0,etat]*calcul_alpha_i(sequenceReponse,indice-1,1);
        float partie2=A[1,etat]*calcul_alpha_i(sequenceReponse,indice-1,0);
        return B[etat,sequenceReponse[indice]? 0 : 1]*(parite1+partie2);
    }
    public float calcul_beta_i(List<bool> sequenceReponse,int indice,int etat){
        if(indice==sequenceReponse.Count-1){
            return 1;
        }
        float parite1=calcul_beta_i(sequenceReponse,indice+1,0)*A[0,etat]*B[0,(int)(sequenceReponse[indice]? 0 : 1)];
        float parite2=calcul_beta_i(sequenceReponse,indice+1,1)*A[1,etat]*B[1,(int)(sequenceReponse[indice]? 0 : 1)];
        return parite1+parite2;
    }
    public float calcul_gama_i(List<bool> sequenceReponse,int indice,int etat){
        float partie1=calcul_alpha_i(sequenceReponse,indice,etat)*calcul_beta_i(sequenceReponse,indice,etat);
        int etat2=0;
        if(etat==0){
            etat2=1;
        }
        float parite2=calcul_alpha_i(sequenceReponse,indice,etat2)*calcul_beta_i(sequenceReponse,indice,etat2);
        return partie1/(partie1+parite2);
    }
    public float calcul_sigma_ij(List<bool> sequenceReponse,int indice,int etat1,int etat2){
        float parite1=A[etat1,etat2]*calcul_alpha_i(sequenceReponse,indice,etat1)*calcul_beta_i(sequenceReponse,indice+1,etat2)*B[etat2,sequenceReponse[indice+1]? 0 : 1];
        float parite2=0;
        for(int i=0;i<2;i++){
            for(int j =0;j<2;j++){
                parite2+=A[i,j]*calcul_alpha_i(sequenceReponse,indice,i)*calcul_beta_i(sequenceReponse,indice+1,j)*B[j,sequenceReponse[indice+1]? 0 : 1];
            }
        }
        return parite1/parite2;
    }
    public void predire_acquision(List<bool> sequenceReponse,int indiceQuestion){
        probaAcquisition[indiceQuestion]=list_probaL_init[indiceQuestion];

        float partie1=0f;
        float partie2=0f;
        for(int i=0;i<sequenceReponse.Count;i++){
            bool reponse=sequenceReponse[i];
            if(reponse){
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*list_proba_G[indiceQuestion];
                partie2=list_proba_G[indiceQuestion]+((1- list_proba_S[indiceQuestion]- list_proba_G[indiceQuestion])*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);                
            }else{
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*(1-list_proba_G[indiceQuestion]);
                partie2=(1-list_proba_G[indiceQuestion])-((1- list_proba_S[indiceQuestion]- list_proba_G[indiceQuestion])*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);                
            }
        }

    }
    public void hmm_EM(List<bool> sequenceReponse,int indiceQuestion){
        PI=new float[2];
        PI[0]=probaL_init;//peut-etre estimer par la premier fois
        PI[1]=1-probaL_init;
        //float pi=calcul_gama_i(sequenceReponse,1,0);
        A=new float[2,2];
        A[0,0]=1f;//etat learned to learned peut-etre modifie selon courbe oublie
        A[0,1]=0f;//etat learned to  not learned 
        A[1,0]=proba_T;//etat not learned to learned
        A[1,1]=1f- proba_T;//etat not learned to not learned
        B=new float[2,2];
        B[0,0]=1f- proba_S;//reponse correcte dans etat learned 
        B[0,1]=proba_S;//reponse incorrecte dans etat learned(slip)
        B[1,0]=proba_G;//reponse correcte dans etat learned(guess)
        B[1,1]=1f-proba_G;//reponse incorrecte dans etat not learned
        float proba_S_courant=list_proba_S[indiceQuestion];
        float proba_G_courant=list_proba_G[indiceQuestion];
        int iter=0;
        while(iter==0 ||( B[0,1]-proba_S_courant>0.001 && iter<100 ) ){
            iter++;
            UnityEngine.Debug.Log("========iter"+iter);
            proba_S_courant=B[0,1];
            proba_G_courant=B[1,0];
            for (int i=0;i<2;i++){
                for(int j=0;j<2;j++){
                    
                    //float partie1=0;
                    float partie2=0f;
                    float partie1B=0f;
                    for(int t=0;t<sequenceReponse.Count;t++ ){
                        //partie1+=calcul_sigma_ij(sequenceReponse,t,i,j);
                        partie2+=calcul_gama_i(sequenceReponse,t,i);
                        int correct=sequenceReponse[t]? 0 : 1;
                        int indicateur=(correct==j)?1:0;
                        partie1B+=indicateur*calcul_gama_i(sequenceReponse,t,i);
                    }
                    //A[i,j]=partie1/partie2;
                    B[i,j]=partie1B/partie2;
                }
            }

        }
        list_proba_S[indiceQuestion]=B[0,1];
        list_proba_G[indiceQuestion]=B[1,0];

        //UnityEngine.Debug.Log("===proba_s: "+B[0,1]+"proba_g : "+B[1,0]+"=======");
    }

    // Fonction qui màj la proba d'acquisition lorsque l'on répond à un QCM
    public void UpdateProbaAcquisitionQCM(int indiceQuestion, bool correct, float hesite, List<bool> sequenceReponse){
        // correct vaut true si l'utilisateur a donné la bonne réponse et false sinon
        // hesite est déterminé par la probabilité d’hésitation de l’utilisateur, et vaut 1 si l’utilisateur hésite totalement (d’où le 1-cette proba)
        
        int intCorrect = correct ? 0 : 1; // Vaut 0 si true et 1 si false
        
        //utilise parametre initiale pour predire l'acquistion
        float proba_g=beta*list_proba_G[indiceQuestion]+beta*hesite;//pondere proba guess avec hesitation
        float proba_s=0.8f*list_proba_S[indiceQuestion]+0.2f*(1-hesite);//
        if(nbRencontres[indiceQuestion]==1){
            //list_probaL_init[indiceQuestion]=beta*intCorrect + (1-beta)*(1-hesite)*intCorrect;
            list_probaL_init[indiceQuestion]=(1-proba_g)*(1-intCorrect) + proba_s*intCorrect;//si reponse vrai (proba_init=1-proba_G) si reponse false(proba_init =proba slip),ponderer avec proba_init definit initialement
            probaAcquisition[indiceQuestion] = probaAcquisition[indiceQuestion]+(1-list_probaL_init[indiceQuestion])*list_proba_T[indiceQuestion];//proba de connais le mot avant + proba connais pas le mot et il apprend dans cette etape
        }

        else if(nbRencontres[indiceQuestion]<3 ){
            float partie1=0f;
            float partie2=0f;
            //quand parametre stable,il suffit appliquer avec reponse actuelle
            if(correct){
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*proba_g;
                partie2=proba_s+((1- proba_s- proba_g)*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);                
            }else{
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*(1-proba_g);
                partie2=(1-proba_g)-((1- proba_s- proba_g)*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);//probaAcquisition ne depasse pas de 1           
            }
            //probaAcquisition[indiceQuestion] = beta*intCorrect + (1-beta)*(1-hesite)*intCorrect; // On veut que ça vaille 0 si l'utilisateur a mal répondu
            //probaAcquisition[indiceQuestion] = probaAcquisition[indiceQuestion]+(1-probaAcquisition[indiceQuestion])*list_proba_T[indiceQuestion];//proba de connais le mot avant + proba connais pas le mot et il apprend dans cette etape
        }
        //l'apprentissage de parametre
        else if(nbRencontres[indiceQuestion]<=10 ){
            //UnityEngine.Debug.Log("=====trace=========="+sequenceReponse.Count);
            hmm_EM(sequenceReponse,indiceQuestion);//predire proba guess et proba slip
            predire_acquision(sequenceReponse,indiceQuestion);//changer proboacquisition direcetement dans la fonction(comme le parametre changer on dois tous recacluler pour chaque etape)
        }
        else{
            float partie1=0f;
            float partie2=0f;
            //quand parametre stable,il suffit appliquer avec reponse actuelle
            if(correct){
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*proba_g;
                partie2=proba_s+((1- proba_s- proba_g)*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);                
            }else{
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*(1-proba_g);
                partie2=(1-proba_g)-((1- proba_s- proba_g)*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);//probaAcquisition ne depasse pas de 1           
            }
            //UnityEngine.Debug.Log("=======probaAcquisition "+probaAcquisition[indiceQuestion]);
        }
        probaAcquisition[indiceQuestion]=Math.Min(probaAcquisition[indiceQuestion],1);//probaAcquisition ne depasse pas de 1
        probaAcquisition [indiceQuestion]=Math.Max(0,probaAcquisition[indiceQuestion]); 
        UnityEngine.Debug.Log("========nb  reponse : "+sequenceReponse.Count+" ==probaAcquisition : "+probaAcquisition[indiceQuestion]+" proba_init "+list_probaL_init[indiceQuestion]+" proba_S "+proba_s+" proba_G "+proba_g);
        
        // Ancienne formule :
        // probaAcquisition[indiceQuestion] = beta*(1-intCorrect) + (1-beta)*(1-hesite)*(1-intCorrect) + (1-beta)*hesite*intCorrect; 
        // On veut que l'utilisateur soit pénalisé s'il hésite alors qu'il a donné la bonne réponse et soit avantagé lorsqu'il hésite mais a donné la mauvaise réponse
    }

    // Fonction qui màj la proba d'acquisition lorsque l'on répond à un Questionnaire à réponse entière 
    public void UpdateProbaAcquisitionQEntier(int indiceQuestion, bool correct, float hesite, List<bool> sequenceReponse){ 
        // correct vaut true si l'utilisateur a donné la bonne réponse et false sinon
        // hesite est déterminé par la probabilité d’hésitation de l’utilisateur, et vaut 1 si l’utilisateur hésite totalement (d’où le 1-cette proba)
        UnityEngine.Debug.Log("=====UpdateProbaAcquisitionQEntier");
        int intCorrect = correct ? 0 : 1; // Vaut 0 si true et 1 si false
        
        //utilise parametre initiale pour predire l'acquistion
        float proba_g=beta*list_proba_G[indiceQuestion]+beta*hesite;//pondere proba guess avec hesitation
        float proba_s=0.8f*list_proba_S[indiceQuestion]+0.2f*(1-hesite);//
        if(nbRencontres[indiceQuestion]==1){
            //list_probaL_init[indiceQuestion]=beta*intCorrect + (1-beta)*(1-hesite)*intCorrect;
            list_probaL_init[indiceQuestion]=(1-proba_g)*(1-intCorrect) + proba_s*intCorrect;//si reponse vrai (proba_init=1-proba_G) si reponse false(proba_init =proba slip),ponderer avec proba_init definit initialement
            probaAcquisition[indiceQuestion] = probaAcquisition[indiceQuestion]+(1-list_probaL_init[indiceQuestion])*list_proba_T[indiceQuestion];//proba de connais le mot avant + proba connais pas le mot et il apprend dans cette etape
        }

        else if(nbRencontres[indiceQuestion]<3 ){
            //probaAcquisition[indiceQuestion] = beta*intCorrect + (1-beta)*(1-hesite)*intCorrect; // On veut que ça vaille 0 si l'utilisateur a mal répondu
            probaAcquisition[indiceQuestion] = probaAcquisition[indiceQuestion]+(1-probaAcquisition[indiceQuestion])*list_proba_T[indiceQuestion];//proba de connais le mot avant + proba connais pas le mot et il apprend dans cette etape
        }
        //l'apprentissage de parametre
        else if(nbRencontres[indiceQuestion]<=5 ){
            //UnityEngine.Debug.Log("=====trace=========="+sequenceReponse.Count);
            hmm_EM(sequenceReponse,indiceQuestion);//predire proba guess et proba slip
            predire_acquision(sequenceReponse,indiceQuestion);//changer proboacquisition direcetement dans la fonction(comme le parametre changer on dois tous recacluler pour chaque etape)
        }
        else{
            float partie1=0f;
            float partie2=0f;
            //quand parametre stable,il suffit appliquer avec reponse actuelle
            if(correct){
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*proba_g;
                partie2=proba_s+((1- proba_s- proba_g)*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);                
            }else{
                partie1=(1- list_proba_T[indiceQuestion])*(1-probaAcquisition[indiceQuestion])*(1-proba_g);
                partie2=(1-proba_g)-((1- proba_s- proba_g)*probaAcquisition[indiceQuestion]);
                probaAcquisition[indiceQuestion]=1-(partie1/partie2);//probaAcquisition ne depasse pas de 1           
            }
            //UnityEngine.Debug.Log("=======probaAcquisition "+probaAcquisition[indiceQuestion]);
        }
        probaAcquisition[indiceQuestion]=Math.Min(probaAcquisition[indiceQuestion],1);//probaAcquisition ne depasse pas de 1
        probaAcquisition [indiceQuestion]=Math.Max(0,probaAcquisition[indiceQuestion]); 
        UnityEngine.Debug.Log("========nb  reponse : "+sequenceReponse.Count+" ==probaAcquisition : "+probaAcquisition[indiceQuestion]+" proba_init "+list_probaL_init[indiceQuestion]+" proba_S "+proba_s+" proba_G "+proba_g);
        
        // Ancienne formule :
        // probaAcquisition[indiceQuestion] = beta*intCorrect + (1-beta)*(1-hesite)*intCorrect + (1-beta)*hesite*(1-intCorrect);  
        // On veut que l'utilisateur soit pénalisé s'il hésite alors qu'il a donné la bonne réponse et soit avantagé lorsqu'il hésite mais a donné la mauvaise réponse
    }

    // Fonction qui retourne les probas d'acquisition actuelle en fonction du temps passé depuis la dernière rencontre du mot et de la proba d'acquisition enregistrée à cette date
    public float[] UpdateProbaAcquisitionOubli(){ 
        // Estime chaque proba d'acquisition au temps actuel en fonction de la courbe de l'oubli
        float[] newProbaAcquisition = new float[PlayerPrefs.GetInt("NbMotsVocab")];

        Array.Copy(probaAcquisition, newProbaAcquisition, PlayerPrefs.GetInt("NbMotsVocab")); // On fait une copy pour ne pas perdre les valeurs de proba d'acquisition au temps de la dernière rencontre

        // On converti les dates en DateTime depuis le type string
        DateTime[] DerniereRencontreDATE = new DateTime[PlayerPrefs.GetInt("NbMotsVocab")];
        TimeSpan[] TempsEntreDerniereDateEtAuj = new TimeSpan[PlayerPrefs.GetInt("NbMotsVocab")]; // Temps écoulé depuis la dernière rencontre du mot
        float[] floatTimeSpan = new float[PlayerPrefs.GetInt("NbMotsVocab")]; // Temps écoulé depuis la dernière rencontre du mot en float (en seconde par ex.)

        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            if (dateDerniereRencontre[i] != null && dateDerniereRencontre[i] != "")
            {
                // On récupère la date de dernière rencontre en type DateTime
                DerniereRencontreDATE[i] = System.DateTime.ParseExact(dateDerniereRencontre[i], "MM/dd/yyyy HH:mm:ss", null);
                
                // On calcule le temps écoulé entre ce temps et maintenant
                TempsEntreDerniereDateEtAuj[i] = DateTime.Now - DerniereRencontreDATE[i]; // Sous forme HH:mm:ss
                
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
                // floatTimeSpan[i] /= 60; // On met le résultat en minutes plutôt
                // floatTimeSpan[i] /= 3600; // On met le résultat en heures plutôt 
                floatTimeSpan[i] /= 24*3600; // On met le résultat en jours plutôt comme c'est plus cohérent pour la fonction d'oubli 
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
