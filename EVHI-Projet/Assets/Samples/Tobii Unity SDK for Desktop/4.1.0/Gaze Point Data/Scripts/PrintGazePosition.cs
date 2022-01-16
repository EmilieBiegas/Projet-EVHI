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

// Classe permettant de gérer l'oculomètre
namespace Tobii.Gaming.Examples.GazePointData
{
	/// <summary>
	/// Writes the position of the eye gaze point into a UI Text view
	/// </summary>
	/// <remarks>
	/// Referenced by the Data View in the Eye Tracking Data example scene.
	/// </remarks>
	public class PrintGazePosition : MonoBehaviour
	{
		// PB enlever les trucs inutiles
		public Text xCoord;
		public Text yCoord;
		public GameObject GazePoint;
        public Canvas canvas; // Le canvas affichant le jeu
		public GameObject[] cubes; // Liste des 4 boutons pour le QCM
		private Camera camera;
		private float tempo; // PB a enlever
		private int NbRep; // Le nombre de propositions de réponses aux QCM
		private int DernierCubeRegardé; // L'indice du dernier cube regardé
		private Stopwatch[] timer; // Les chronomètres des différentes propositions de réponse
		private float[] floatTimeSpan; // Les durées sur chacune des réponses
		private int NbChangeCube; // Le nombre de fois où l'utilisateur a changé de cube regardé

		void Start()
		{
			tempo = 0;
			camera = Camera.main;

			NbRep = 4; // PB 4 est le nombre de propositions de réponses
			DernierCubeRegardé = -1; // On initialise le cube dernièrement regardé à -1 pour indiquer que c'est le début
			timer = new Stopwatch[NbRep]; // On initialise les timers des différentes propositions		
			for (int i = 0; i < NbRep; i++)
			{
				timer[i] = new Stopwatch();
			}	
			floatTimeSpan = new float[NbRep]; // On initialise les durées passées sur les différentes propositions
			NbChangeCube = 0; // On initialise le nombre de fois où l'utilisateur a changé de cube regardé
		}

		void Update()
		{
			GazePoint.SetActive(false);

			GazePoint gazePoint = TobiiAPI.GetGazePoint();
			if (gazePoint.IsValid)
			{
				// Coordonnée yeux de l'eye tracking
				Vector2 gazePosition = gazePoint.Screen;
				yCoord.color = xCoord.color = Color.white;
				Vector2 roundedSampleInput = new Vector2(Mathf.RoundToInt(gazePosition.x), Mathf.RoundToInt(gazePosition.y));
				xCoord.text = "x (in px): " + roundedSampleInput.x;
				yCoord.text = "y (in px): " + roundedSampleInput.y;

				// Hit box des rectangles sur unity
				Bounds[] bounds = new Bounds[NbRep];
				for (int i = 0; i < NbRep; i++)
				{
					bounds[i] = cubes[i].GetComponent<BoxCollider2D>().bounds;
				}

				// Hit box du canvas sur unity
				Bounds boundCanvas = new Bounds();
				boundCanvas = canvas.GetComponent<BoxCollider2D>().bounds;				

				// Scale à la taille de l'ecran global
				Vector3[] origin = new Vector3[NbRep];
				Vector3[] extents = new Vector3[NbRep];
				for (int i = 0; i <NbRep; i++)
				{
					origin[i] = camera.WorldToScreenPoint(new Vector3(bounds[i].min.x, bounds[i].min.y, 0.0f));
					extents[i] = camera.WorldToScreenPoint(new Vector3(bounds[i].max.x, bounds[i].max.y, 0.0f));
				}

				// Scale du canvas à la taille de l'ecran global
				Vector3 originCanvas = new Vector3();
				Vector3 extentsCanvas = new Vector3();
				originCanvas = camera.WorldToScreenPoint(new Vector3(boundCanvas.min.x, boundCanvas.min.y, 0.0f));
				extentsCanvas = camera.WorldToScreenPoint(new Vector3(boundCanvas.max.x, boundCanvas.max.y, 0.0f));
				
				// Redefinition de la hitbox adapté à la taille de l'ecran global
				Rect[] goodBound = new Rect[NbRep];
				for (int i = 0; i < NbRep; i++)
				{
					goodBound[i] = new Rect(origin[i].x, origin[i].y, extents[i].x - origin[i].x, extents[i].y - origin[i].y); // point de référence (haut  gauche), longueur, hauteur
				}	

				// Redefinition de la hitbox du canvas adapté à la taille de l'ecran global
				Rect goodBoundCanvas = new Rect();
				goodBoundCanvas = new Rect(originCanvas.x, originCanvas.y, extentsCanvas.x - originCanvas.x, extentsCanvas.y - originCanvas.y); // point de référence (haut  gauche), longueur, hauteur

				// Verifie si le point de l'eye tracking est dans le rectangle hitbox du canvas
				if(goodBoundCanvas.Contains(new Vector3(roundedSampleInput.x, roundedSampleInput.y, 0)))
                {
                    // Si le point est dans l'écran, on indique à tous les chronomètres qu'il faut se (re)mettre en marche
					// Si le point est  dans l'écran et que les chronos étaient en pause, cela signifie que l'utilisateur regarde à nouveau l'écran
					// Dans ce cas, on indique à tous les chronomètres qu'il faut se remettre en marche
					PlayerPrefs.SetInt("ChronosEnPause", 0);

					// Verifie si le point de l'eye tracking est dans le rectangle hitbox
					for (int i = 0; i < NbRep; i++)
					{
						if(goodBound[i].Contains(new Vector3(roundedSampleInput.x, roundedSampleInput.y, 0)))
						{
							// Si c'est le ces, changer la couleur du bouton regardé
							tempo += 5f * Time.deltaTime;
							float tempo2 = (float) (Math.Sin(tempo) + 1f) / 2;
							float tempo3 = (float)(Math.Cos(tempo) + 1f) / 2;
							// UnityEngine.Debug.Log(tempo);
							// cubes[i].GetComponent<Image>().color = new Color(1f, tempo2, tempo3); // On fait varier la couleur du cube regardé
							
							if (DernierCubeRegardé != i) // Dans ce cas, l'utilisateur a bougé son regard sur un autre cube
							{
								// UnityEngine.Debug.Log("Utilisateur a changé de cube regardé, il regardais " + DernierCubeRegardé + " et regarde désormais " + i);
								DernierCubeRegardé = i; // On indique que le dernier cube regardé est le cube i
								NbChangeCube += 1; // On a changé de cube regardé
								// PB on lance (ou relance) le timer du temps passé sur ce mot
								timer[i].Start(); // Pas de Reset comme on veut que les temps s'ajoutent
							}
						}
						else
						{
							//cube.GetComponent<Image>().color = new Color(1, 0, 1); // PB
							timer[i].Stop(); // On arrête le chronomètre de tous les rectangles non regardés 
						}
					}
                }else
                {   
					// UnityEngine.Debug.Log("REGARD HORS DE L ECRAN");
                    // Si le point n'est pas dans l'écran, cela signifie que l'utilisateur ne regarde plus l'écran
                    // Dans ce cas, on indique à tous les chronomètres qu'il faut se mettre en pause le temps que l'utilisateur se re-concentre
                    if(PlayerPrefs.GetInt("ChronosEnPause") == 0){
						for (int i = 0; i < NbRep; i++){
							timer[i].Stop();
							// UnityEngine.Debug.Log("Temps dans le rectangle "+i+" : "+timer[i]);
						}
					}
					PlayerPrefs.SetInt("ChronosEnPause", 1);
                }					
			}
		}

		public Vector2 ObtenirPtOculometre(){ // Fonction déterminant le point oculomètrique à donner à OculometreManager pour estimer la probabilité d'hésitation de l'utilisateur selon l'oculomètre
			// On calcule les deux critères :
			// La différence maximum de temps passé sur un mot entre deux mots et le nombre de fois total où l’utilisateur
			// a bougé son regard sur un autre mot divisé par le nombre de propositions (donc la somme du nombre de
			// fois où il a regardé chaque mot divisé par le nombre de propositions)
			for (int i = 0; i < NbRep; i++)
			{
				TimeSpan tmpsPasse = timer[i].Elapsed; // On regarde le temps passé sur le chronomètre
				// On transforme la durée obtenue en float (nombre de secondes écoulées)
                int days, hours, minutes, seconds, milliseconds;
                days = tmpsPasse.Days;
                hours = tmpsPasse.Hours;
                minutes = tmpsPasse.Minutes;
                seconds = tmpsPasse.Seconds;
                milliseconds = tmpsPasse.Milliseconds;
				floatTimeSpan[i] = ((float)days*24*3600) + ((float)hours*3600) + ((float)minutes*60) + (float)seconds + ((float)milliseconds/1000);
                
				UnityEngine.Debug.Log("Temps passée sur la réponse " + i + " est " + floatTimeSpan[i]);
			}

			float diffMaxTmps = -1;
			for (int i = 0; i < NbRep; i++)
			{
				for (int j = 0; j < NbRep; j++)
				{
					if (diffMaxTmps == -1)
					{
						diffMaxTmps = Math.Abs(floatTimeSpan[i]-floatTimeSpan[j]);
					}

					if (diffMaxTmps < Math.Abs(floatTimeSpan[i]-floatTimeSpan[j]))
					{
						diffMaxTmps = Math.Abs(floatTimeSpan[i]-floatTimeSpan[j]);
					}
				}
			}
			Vector2 pointObtenu = new Vector2(diffMaxTmps, NbChangeCube/NbRep);

			// On remet les compteurs à 0
			RemiseCompteurs0();

			// On donne le point obtenu à l'Occulomètre Manager
			return pointObtenu;
		}

		public void RemiseCompteurs0(){
			DernierCubeRegardé = -1; // On réinitialise le cube dernièrement regardé à -1 pour indiquer que c'est le début		
			for (int i = 0; i < NbRep; i++) // On réinitialise les timers des différentes propositions
			{
				timer[i].Reset();
			}	
			NbChangeCube = 0; // On initialise le nombre de fois où l'utilisateur a changé de cube regardé
		}

	}
}