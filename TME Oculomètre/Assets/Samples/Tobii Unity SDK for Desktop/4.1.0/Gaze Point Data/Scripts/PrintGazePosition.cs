//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
// using System.Collections;
// using System.Collections.Generic;
//using System.Text;

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
		public Text xCoord;
		public Text yCoord;
		public GameObject GazePoint;
		public GameObject[] cubes; // Liste des 4 boutons pour le QCM
		private float _pauseTimer;
		private Outline _xOutline;
		private Outline _yOutline;
		private Camera camera;
		private float tempo;
		private int NbRep; // Le nombre de propositions de réponses aux QCM
		private int DernierCubeRegardé; // L'indice du dernier cube regardé
		private Stopwatch[] timer; // Les chronomètres des différentes propositions de réponse
		private TimeSpan[] timeTaken; // Les durées sur chacune des réponses
		private int NbChangeCube; // Le nombre de fois où l'utilisateur a changé de cube regardé

		void Start()
		{

			tempo = 0;

			camera = Camera.main;

			_xOutline = xCoord.GetComponent<Outline>();
			_yOutline = yCoord.GetComponent<Outline>();

			NbRep = 4; // PB 4 est le nombre de propositions de réponses
			DernierCubeRegardé = -1; // On initialise le cube dernièrement regardé à -1 pour indiquer que c'est le début
			timer = new Stopwatch[NbRep]; // On initialise les timers des différentes propositions
			timeTaken = new TimeSpan[NbRep]; // On initialise les durées passées sur les différentes propositions
			NbChangeCube = 0; // On initialise le nombre de fois où l'utilisateur a changé de cube regardé
		}

		void Update()
		{
			if (_pauseTimer > 0)
			{
				_pauseTimer -= Time.deltaTime;
				return;
			}

			GazePoint.SetActive(false);
			_xOutline.enabled = false;
			_yOutline.enabled = false;

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

				// Scale à la taille de l'ecran global
				Vector3[] origin = new Vector3[NbRep];
				Vector3[] extents = new Vector3[NbRep];
				for (int i = 0; i <NbRep; i++)
				{
					origin[i] = camera.WorldToScreenPoint(new Vector3(bounds[i].min.x, bounds[i].min.y, 0.0f));
					extents[i] = camera.WorldToScreenPoint(new Vector3(bounds[i].max.x, bounds[i].max.y, 0.0f));
				}
				//Debug.Log(origin + " " + extents);

				// Redefinition de la hitbox adapte à la taille de l'ecran global
				Rect[] goodBound = new Rect[NbRep];
				for (int i = 0; i < NbRep; i++)
				{
					goodBound[i] = new Rect(origin[i].x, origin[i].y, extents[i].x - origin[i].x, extents[i].y - origin[i].y); // point de référence (haut  gauche), longueur, hauteur
				}
				

				//Debug.Log(goodBound);
				//Debug.Log(goodBound + " " + roundedSampleInput.x + " " + roundedSampleInput.y);

				// Verifie si le point de l'eye tracking est dans le rectangle hitbox
				for (int i = 0; i < NbRep; i++)
				{
					if(goodBound[i].Contains(new Vector3(roundedSampleInput.x, roundedSampleInput.y, 0)))
					{
						// Si c'est le ces, changer la couleur du bouton regardé
						tempo += 5f * Time.deltaTime;
						float tempo2 = (float) (Math.Sin(tempo) + 1f) / 2;
						float tempo3 = (float)(Math.Cos(tempo) + 1f) / 2;
						UnityEngine.Debug.Log(tempo);
						cubes[i].GetComponent<Image>().color = new Color(1f, tempo2, tempo3); // On fait varier la couleur du cube regardé
						
						if (DernierCubeRegardé != i) // Dans ce cas, l'utilisateur a bougé son regard sur un autre cube
						{
							UnityEngine.Debug.Log("Utilisateur a changé de cube regardé, il regardais " + DernierCubeRegardé + " et regarde désormais " + i);
							DernierCubeRegardé = i; // On indique que le dernier cube regardé est le cube i
							NbChangeCube += 1; // On a changé de cube regardé
							// PB on lance (ou relance) le timer du temps passé sur ce mot
							// timer[i].Start(); // Pas de Reset comme on veut que les temps s'ajoutent
						}
					}
					else
					{
						//tempo = 0;
						//cube.GetComponent<Image>().color = new Color(1, 0, 1);
					}
				}
			}

			if (Input.GetKeyDown(KeyCode.Space) && gazePoint.IsRecent()) // PB appui barre espace
			{
				_pauseTimer = 3f;
				GazePoint.transform.localPosition = (gazePoint.Screen - new Vector2(Screen.width, Screen.height) / 2f) /
				                                    GetComponentInParent<Canvas>().scaleFactor;
				yCoord.color = xCoord.color = new Color(0 / 255f, 190 / 255f, 255 / 255f);
				GazePoint.SetActive(true);
				_xOutline.enabled = true;
				_yOutline.enabled = true;
			}
		}

		public float EstimationHesitationOculometre(){ // Fonction estimant et retournant la probabilité d'hésitation de l'utilisateur selon l'oculomètre
			// PB
			// On calcule les deux critères
			for (int i = 0; i < NbRep; i++)
			{
				timeTaken[i] = timer[i].Elapsed; // On regarde le temps passé sur le chronomètre
				UnityEngine.Debug.Log("Temps passée sur la réponse " + i + " est " + timeTaken[i]);
			}

			// On ajoute le point obtenu à la base de donnée
			// On estime l'hésitation de l'utilisateur
			// On remet les compteurs à 0
			return 1;
		}

	}
}