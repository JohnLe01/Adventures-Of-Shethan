using UnityEngine;
using System.Collections;

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
		public class CTerrainManager : MonoBehaviour
		{

				// Terrain grid
				public CTerrain[,] grid = null;
				// Size of the grid (for new sizes etc)
				public Vector2 size = new Vector2 (0, 0);
				// The parent object used to contain the terrain game objects
				public GameObject parent;
				// link to settings
				Settings settings;

				public void Init (GameObject p, Settings s)
				{
						settings = s;
						parent = p;
						if (settings == null)
								return;
								
						size.x = settings.gridSizeX + 1;
						size.y = settings.gridSizeY + 1;
						Reset (false);
						validate ();

				}

				public CTerrainManager (GameObject p, Settings s)
				{
						Init (p, s);
				}

				private void calculateTerrain (CNode generator)
				{
						generator.Verify ();
						generator.map = new C2DMap ();
						foreach (CTerrain t in grid) {
								C2DMap.shift.Set (t.posInGrid.x + 593.123f, t.posInGrid.y + 01412.45f);
								generator.Calculate ();
								t.Set (generator.map.map, settings.getSize ());
						}

				}
				// DO NOT USE YET: in-game terrain calculator. 
				IEnumerator calculateInGame (CNode tg, CNode ag, CNode sg)
				{

						foreach (CTerrain t in grid) {

								tg.map = new C2DMap ();
								//tg.map.shift.Set (t.posInGrid.x +593.123f, t.posInGrid.y + 01412.45f);
								C2DMap.shift.Set (t.posInGrid.x + 593.123f, t.posInGrid.y + 01412.45f);

								yield return null;
								tg.Calculate ();
								t.Set (tg.map.map, settings.getSize ());
								t.CalculatePreAlphas ();
								t.createAlphaMapsGenerator (ag);
								t.setAlphas ();
								t.sceneryGenerator (sg);
						}
						setupNeighbors ();

				}

				public void calculateInGame (COutput generator)
				{
						C2DMap.sizeX = settings.getSize ();
						C2DMap.sizeY = settings.getSize ();
						CNode tg = generator.getInput (0);
						if (tg == null) {
								Debug.Log ("LimeStone error: output not connected to terrain generator! please fix.");
								return;
						}
						tg.Verify ();
						CNode ag = generator.getInput (1);
						if (ag == null) {
								Debug.Log ("LimeStone error: output not connected to alpha generator! please fix.");
								return;
						}
						ag.Verify ();

						CNode sg = generator.getInput (2);
						if (ag == null) {
								Debug.Log ("LimeStone error: output not connected to scenery generator! please fix.");
								return;
						}

						StartCoroutine (calculateInGame (tg, ag, sg));
				}

				public void validate ()
				{
						if (size.x != settings.gridSizeX + 1 || size.y != settings.gridSizeY + 1) {
								size.x = settings.gridSizeX + 1;
								size.y = settings.gridSizeY + 1;
								Reset (true);
						}

						foreach (CTerrain c in grid) {
								c.validate ();
						}

						setupNeighbors ();

				}

				public CTerrain findNextTerrain ()
				{
						foreach (CTerrain t in grid) 
								if (!t.calculated)
										return t;
						return null;
				}

				public Vector2 findNextTerrainPos ()
				{
						foreach (CTerrain t in grid) 
								if (!t.calculated)
										return t.posInGrid;
						return Vector2.zero;
				}

				public void ResetState ()
				{
						foreach (CTerrain t in grid) 
								t.calculated = false;
				}

				public void ClearTerrain ()
				{
						foreach (CTerrain t in grid) 
								t.ClearTerrain ();
				}

				public void ClearAlphas ()
				{
						foreach (CTerrain t in grid) 
								t.ClearAlphas ();
				}

				public void ClearEnvironment ()
				{
					foreach (CTerrain t in grid) 
						t.ClearEnvironment ();
				}
		
				public void Reset (bool hard)
				{
						if (grid != null && hard) 
								foreach (CTerrain c in grid) {
										c.Delete ();
								}

						grid = new CTerrain[(int)size.x, (int)size.y];
						for (int i=0; i<size.x; i++)
								for (int j=0; j<size.y; j++) {
										grid [i, j] = new CTerrain (parent, settings.getSize (), i + j * (int)size.x, settings, i - (int)size.x / 2, j - (int)size.y / 2);
								}



				}

				public void Splats ()
				{
						foreach (CTerrain t in grid)
								t.setupSplats ();

				}

				public void CalculatePreAlphas ()
				{
						foreach (CTerrain t in grid)
								t.CalculatePreAlphas ();	
				}

				public void SetAlphas ()
				{
						foreach (CTerrain t in grid)
								t.setAlphas ();	
				}
		
				public void UpdateAlphas ()
				{
						foreach (CTerrain t in grid)
								t.updateAlphaInformation ();	
				}
				
				public void UpdateTerrainDetails ()
				{
					foreach (CTerrain t in grid)
						t.UpdateTerrainDetails ();	
				}
		
				public void AlphasFromGenerator (CNode generator)
				{
						foreach (CTerrain t in grid) {
								t.setShiftFromGrid ();
								t.createAlphaMapsGenerator (generator);
						}
		
				}
	
				public void SceneryFromGenerator (CNode generator)
				{
						foreach (CTerrain t in grid) {
								t.setShiftFromGrid ();
								t.sceneryGenerator (generator);
						}
		
				}

				private void setupNeighbors ()
				{
						for (int i=0; i<size.x; i++)
								for (int j=0; j<size.y; j++) {
										if (grid [i, j].terrain)
												grid [i, j].terrain.SetNeighbors (getTerrainObject (i - 1, j), getTerrainObject (i, j + 1), getTerrainObject (i + 1, j), getTerrainObject (i, j - 1));
								}
				}

				public CTerrain get (int i, int j)
				{
						if (i < 0 || i >= size.x)
								return null;
						if (j < 0 || j >= size.y)
								return null;

						return grid [i, j];
				}

				public Terrain getTerrainObject (int i, int j)
				{
						if (i < 0 || i >= size.x)
								return null;
						if (j < 0 || j >= size.y)
								return null;
				
						return grid [i, j].terrain;
				}

		}
}
