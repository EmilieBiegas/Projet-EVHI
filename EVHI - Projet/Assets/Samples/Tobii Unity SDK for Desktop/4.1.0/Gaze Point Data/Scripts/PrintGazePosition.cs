//-----------------------------------------------------------------------
// Copyright 2016 Tobii AB (publ). All rights reserved.
//-----------------------------------------------------------------------

using System;
using UnityEngine;
using UnityEngine.UI;

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
		
		public GameObject cube;

		private float _pauseTimer;
		private Outline _xOutline;
		private Outline _yOutline;

		private Camera camera;

		private float tempo;

		void Start()
		{

			tempo = 0;

			camera = Camera.main;

			_xOutline = xCoord.GetComponent<Outline>();
			_yOutline = yCoord.GetComponent<Outline>();
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
				Vector2 roundedSampleInput =
					new Vector2(Mathf.RoundToInt(gazePosition.x), Mathf.RoundToInt(gazePosition.y));
				xCoord.text = "x (in px): " + roundedSampleInput.x;
				yCoord.text = "y (in px): " + roundedSampleInput.y;

				// Hit box du rectangle sur unity
				Bounds bounds = cube.GetComponent<BoxCollider2D>().bounds;

				// Scale à la taille de l'ecran global
				var origin = camera.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.min.y, 0.0f));
				var extents = camera.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.max.y, 0.0f));

				//Debug.Log(origin + " " + extents);

				// Redefinition de la hitbox adapte à la taille de l'ecran global
				Rect goodBound = new Rect(origin.x, origin.y, extents.x - origin.x, extents.y - origin.y); // point de référence (haut  gauche), longueur, hauteur

				//Debug.Log(goodBound);
				//Debug.Log(goodBound + " " + roundedSampleInput.x + " " + roundedSampleInput.y);

				// Verifie si le point de l'eye tracking est dans le rectangle hitbox
				if(goodBound.Contains(new Vector3(roundedSampleInput.x, roundedSampleInput.y, 0)))
                {
					// Faire ce que tu veux
					tempo += 5f * Time.deltaTime;
					float tempo2 = (float) (Math.Sin(tempo) + 1f) / 2;
					float tempo3 = (float)(Math.Cos(tempo) + 1f) / 2;
					Debug.Log(tempo);
					cube.GetComponent<Image>().color = new Color(1f, tempo2, tempo3);
                }
                else
                {
					//tempo = 0;
					//cube.GetComponent<Image>().color = new Color(1, 0, 1);
				}

			}

			if (Input.GetKeyDown(KeyCode.Space) && gazePoint.IsRecent())
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
	}
}