//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
// using System.Collections;
// using System.Collections.Generic;
// using System.Text;

// Classe permettant de gérer l'oculomètre, de signaler si l'utilisateur détourne son regard de l'écran
namespace Tobii.Gaming.Examples.GazePointData
{
	public class SignaleRegardOcu : MonoBehaviour
	{
        public Canvas canvas; // Le canvas affichant le jeu
		private Camera camera;

        void Start(){
            camera = Camera.main;
        }
        void Update()
		{
			GazePoint gazePoint = TobiiAPI.GetGazePoint();
            // UnityEngine.Debug.Log(gazePoint.IsValid); // Toujours valide meme si regarde pas l'écran
			if (gazePoint.IsValid)
			{
                // Coordonnée yeux de l'eye tracking
				Vector2 gazePosition = gazePoint.Screen;
				Vector2 roundedSampleInput = new Vector2(Mathf.RoundToInt(gazePosition.x), Mathf.RoundToInt(gazePosition.y));

				// Hit box du canvas sur unity
				Bounds boundCanvas = new Bounds();
				boundCanvas = canvas.GetComponent<BoxCollider2D>().bounds;				

				// Scale à la taille de l'ecran global
				Vector3 origin = new Vector3();
				Vector3 extents = new Vector3();
                Vector3 test = new Vector3(boundCanvas.min.x, boundCanvas.min.y, 0.0f);
				origin = camera.WorldToScreenPoint(test);
				extents = camera.WorldToScreenPoint(new Vector3(boundCanvas.max.x, boundCanvas.max.y, 0.0f));
				//Debug.Log(origin + " " + extents);

				// Redefinition de la hitbox adapté à la taille de l'ecran global
				Rect goodBound = new Rect();
				goodBound = new Rect(origin.x, origin.y, extents.x - origin.x, extents.y - origin.y); // point de référence (haut  gauche), longueur, hauteur
				
				//Debug.Log(goodBound);
				// UnityEngine.Debug.Log(goodBound + " " + roundedSampleInput.x + " " + roundedSampleInput.y);

				// Verifie si le point de l'eye tracking est dans le rectangle hitbox du canvas
				if(goodBound.Contains(new Vector3(roundedSampleInput.x, roundedSampleInput.y, 0)))
                {
                    // Si le point est dans l'écran, on indique à tous les chronomètres qu'il faut se (re)mettre en marche
                    PlayerPrefs.SetInt("ChronosEnPause", 0);
                }else
                {   // PB tjr valide, il faut vérifier que le point regardé est dans l'écran
                    // Si le point n'est pas dans l'écran, cela signifie que l'utilisateur ne regarde plus l'écran
                    // Dans ce cas, on indique à tous les chronomètres qu'il faut se mettre en pause le temps que l'utilisateur se re-concentre
                    PlayerPrefs.SetInt("ChronosEnPause", 1);
                }
			}
		}
	}
}
