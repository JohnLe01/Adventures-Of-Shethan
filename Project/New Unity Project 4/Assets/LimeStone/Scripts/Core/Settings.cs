using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
 * 
 * © 2014 LemonSpawn. All rights reserved. Source code in this project ("LimeStone") are not supported under any LemonSpawn standard support program or service. 
 * The scripts are provided AS IS without warranty of any kind. LemonSpawn disclaims all implied warranties including, without limitation, 
 * any implied warranties of merchantability or of fitness for a particular purpose. 
 * 
 * Basically, do what you want with this code, but don't blame us if something blows up. 
 * 
 * 
*/

namespace LimeStone
{

/*
* The settings class, a monobehavior that contains all LimeStone user settings (texture, grid size, detail objects etc)
* 
*/
		public class Settings : MonoBehaviour
		{

				public Texture2D[] textures = null;
				public bool textureShow;
				public int newSize;
				public int gridSizeX;
				public int gridSizeY;
				public float detailDistance = 200f;
				public float treeBillboardStart = 100f;
				public float terrainPixelError = 2f;
				public float alphaLayerScale = 1.0f;
				public int TerrainSmoothVal = 1;
				public Material terrainMaterial = null;
				public string currentFile = "";
				public string[] sizesStrings = {
						"16",
						"32",
						"64",
						"128",
						"256",
						"512",
						"1024",
						"2048",
						"4096"
				};
				public string[] gridStrings = {"1", "2","3","4","5", "6","7","8" };
				public string[] gridStrings2 = {"1", "2","3","4","5", "6","7","8" };
				public int[] sizes = {
						16 + 1,
						32 + 1,
						64 + 1,
						128 + 1,
						256 + 1,
						512 + 1,
						1024 + 1,
						2048 + 1,
						4096 + 1
				};
				public float[] textureScales = null;
				public List<GameObject> trees = new List<GameObject> ();
				[SerializeField]
				public List<DetailPrototypeSerializable>
						detailPrototypes = new List<DetailPrototypeSerializable> ();

				public DetailPrototype[] detailPrototypesToArray ()
				{
						DetailPrototype[] dt = new DetailPrototype[detailPrototypes.Count];
						for (int i=0; i<dt.Length; i++)
								dt [i] = detailPrototypes [i].toDetailPrototype ();
						return dt;
				}

				public int getSize ()
				{
						return sizes [newSize];
				}

				public Settings ()
				{
						textureShow = true;
						newSize = 4; 

						terrainMaterial = null;

						textures = new Texture2D[4];
						textureScales = new float[4];
						for (int i=0; i<textureScales.Length; i++)
								textureScales [i] = 100.0f;
								
						gridSizeX = 0;
						gridSizeY = 0;
				}



		}
}
