using UnityEngine;
using System;
using System.IO;
using System.Text;
using Newtonsoft.Json; 
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.Collections.Generic;

// Classe permettant de sauvegarder les données contenant beaucoup de variables (comme des instances de UserStats ou de UserInitialisation)
public class DataSaver
{   
    // Fonction permettant de sauvegarder les données
    public static void saveData<T>(T dataToSave, string dataFileName)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        // Convertir en Json puis en bytes
        string jsonData = JsonUtility.ToJson(dataToSave, true);
        byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData);

        // Creer un dossier si il n'existe pas encore
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }

        try
        {
            // Sauvegarde des données
            File.WriteAllBytes(tempPath, jsonByte);
            // Debug.Log("Données sauvegardées au chemin : " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Echec dans la sauvegarde des données au chemin : " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Erreur : " + e.Message);
        }
    }

    // Fonction permettant de charger les données
    public static T loadData<T>(string dataFileName)
    {
        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        // Quitter si le dossier ou le fichier n'existe pas
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            // Debug.Log("Le dossier n'existe pas");
            return default(T);
        }

        if (!File.Exists(tempPath))
        {
            // Debug.Log("Le fichier n'existe pas");
            return default(T);
        }

        // Charger les données sauvegardées en Json
        byte[] jsonByte = null;
        try
        {
            jsonByte = File.ReadAllBytes(tempPath);
            // Debug.Log("Données chargées depuis le chemin : " + tempPath.Replace("/", "\\"));
        }
        catch (Exception e)
        {
            Debug.LogWarning("Echec dans le chargement des données depuis le chemin : " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Erreur : " + e.Message);
        }

        // Convertir en string
        string jsonData = Encoding.ASCII.GetString(jsonByte);

        // Convertir en Object
        object resultValue = JsonUtility.FromJson<T>(jsonData);
        return (T)Convert.ChangeType(resultValue, typeof(T));
    }

    // Fonction permettant d'effacer les données
    public static bool deleteData(string dataFileName)
    {
        bool success = false;

        // Charger les données
        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        // Quitter si le dossier ou le fichier n'existe pas
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            // Debug.Log("Le dossier n'existe pas");
            return false;
        }

        if (!File.Exists(tempPath))
        {
            // Debug.Log("Le fichier n'existe pas");
            return false;
        }

        try
        {
            File.Delete(tempPath);
            // Debug.Log("Données effacées sur le chemin : " + tempPath.Replace("/", "\\"));
            success = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Echec dans la suppression des données : " + e.Message);
        }

        return success;
    }

    public static void SauvegarderTraces(List<Tuple<float, bool>>[] traces, string dataFileName){
        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        // Creer un dossier si il n'existe pas encore
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }

        // On créer un StreamWriter du chemin
        StreamWriter sw = new StreamWriter(tempPath);
        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            if (traces[i].Count>0) // On écrit seulement si la case est non vide
            {
                // On écrit dans le fichier la liste de tuple correspondant au mot de vocabulaire i
                // Pour chaque case non vide, on écrira une ligne dans le fichier de la forme
                // Numéro_case 1er_elem_tuple1 2eme_elem_tuple1 1er_elem_tuple2 2eme_elem_tuple2 ...
                String ligneEcrite = i.ToString();
                // On construit la ligne à écrire
                for (int j = 0; j < traces[i].Count; j++)
                {
                    string correct = "t";
                    if (traces[i][j].Item2 == false)
                    {
                        correct = "f";
                    }
                    ligneEcrite += " " + traces[i][j].Item1.ToString() + " " + correct;
                }

                try
                {
                    // On écrit la ligne
                    sw.WriteLine(ligneEcrite);
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                }
                finally
                {
                    Console.WriteLine("Executing finally block.");
                }
            }
        }
        // On ferme le fichier
        sw.Close();
    } 

    public static List<Tuple<float, bool>>[] ChargerTraces(string dataFileName){
        string tempPath = Path.Combine(Application.persistentDataPath, "data");
        tempPath = Path.Combine(tempPath, dataFileName + ".txt");

        // On initialise le tableau
        List<Tuple<float, bool>>[] traces = new List<Tuple<float, bool>>[PlayerPrefs.GetInt("NbMotsVocab")];
        for (int i = 0; i < PlayerPrefs.GetInt("NbMotsVocab"); i++)
        {
            traces[i] = new List<Tuple<float, bool>>();
        }

        // Quitter si le dossier ou le fichier n'existe pas
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            // Debug.Log("Le dossier n'existe pas");
            return traces; 
        }

        if (!File.Exists(tempPath))
        {
            // Debug.Log("Le fichier n'existe pas");
            return traces; 
        }

        // On lit le fichier pour récupérer les valeurs
        String line;
        try
        {
            // On créer un StreamReader du chemin
            StreamReader sr = new StreamReader(tempPath);
            // On lit la première ligne du fichier
            line = sr.ReadLine();
            while (line != null)
            {
                // On créer l'instance associée
                string[] subs = line.Split(' '); 
                for (int i = 1; i < subs.Length; i+=2)
                {
                    bool correct = true;
                    if (subs[i+1]=="f")
                    {
                        correct = false;
                    }
                    traces[int.Parse(subs[0])].Add(new Tuple<float, bool>(float.Parse(subs[i]), correct));                    
                }

                // On lit la prochaine ligne
                line = sr.ReadLine();
            }
            // On ferme le fichier
            sr.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        finally
        {
            Console.WriteLine("Executing finally block.");
        }

        // Affichage des données récupérées
        // UnityEngine.Debug.Log("Traces récupérées : "+ traces);
        return traces;
    }
}
