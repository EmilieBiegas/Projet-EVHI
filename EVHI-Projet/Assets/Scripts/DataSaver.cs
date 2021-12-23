// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

// Classe permettant de sauvegarder les données contenant beaucoup de variables (comme des instances de UserStats)
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
        //Debug.Log("Chemin = " + path);

        try
        {
            // Sauvegarde des données
            File.WriteAllBytes(tempPath, jsonByte);
            Debug.Log("Données sauvegardées au chemin : " + tempPath.Replace("/", "\\"));
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
            Debug.Log("Le fichier n'existe pas");
            return default(T);
        }

        // Charger les données sauvegardées en Json
        byte[] jsonByte = null;
        try
        {
            jsonByte = File.ReadAllBytes(tempPath);
            Debug.Log("Données chargées depuis le chemin : " + tempPath.Replace("/", "\\"));
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
            Debug.Log("Le dossier n'existe pas");
            return false;
        }

        if (!File.Exists(tempPath))
        {
            Debug.Log("Le fichier n'existe pas");
            return false;
        }

        try
        {
            File.Delete(tempPath);
            Debug.Log("Données effacées sur le chemin : " + tempPath.Replace("/", "\\"));
            success = true;
        }
        catch (Exception e)
        {
            Debug.LogWarning("Echec dans la suppression des données : " + e.Message);
        }

        return success;
    }
}
