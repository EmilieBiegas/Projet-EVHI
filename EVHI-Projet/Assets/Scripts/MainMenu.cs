using System.Collections;
using System.Collections.Generic;
//using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame(){
        SceneManager.LoadScene("QCM"); //On load la scène des QCM à 4 choix
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void GoMenuParametres(){
        SceneManager.LoadScene("MenuParametres"); //On load la scène des paramètres
    }

    public void BackMenu(){
        SceneManager.LoadScene("MainMenu"); //On load la scène de menu principal
    }
}
