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
		* Main terrain class. Couples to terrain objects, maintains terrain, alphas, sets heights, calculates scenery etc.  
		*/

		public class CTerrain
		{
				public Terrain terrain = null;
				// Current position in grid. Used or perlin noise calculations
				public Vector2 posInGrid = new Vector2 (0, 0);
				public GameObject go, parent;
				// Link to terrain data
				TerrainData data;
				string name;
				
				int size, nr;
				// Link to settings
				Settings settings;
				public bool calculated = false;
				SplatPrototype[] splatPrototypes = null;
				// Three spalts. 
				public static int splats = 3;
				public static int alphaMapSize = 512;

				// holders for data
				private Vector3[,] normals = null;
				private float[,] terrainHeights = null;
				private float[ ,, ] alphas = null;
		
				// Standard terrain size
				public static Vector3 terrainSize = new Vector3 (2000, 2000, 2000);

				public Vector3 getNormal (float x, float y)
				{
						if (terrain != null)
								return terrain.terrainData.GetInterpolatedNormal (x / size, y / size).normalized;
						return Vector3.zero;
				}
				// smooths the alpha maps
				public void smoothAlpha (float[,,] alphas)
				{
						for (int x = 1; x < alphas.GetLength(0)-1; x++) {
								for (int y = 1; y < alphas.GetLength(1)-1; y++) {
										for (int lay=0; lay<3; lay++) {
												float sum = 0;
												for (int k=-1; k<2; k++)
														for (int l=-1; l<2; l++) {
																sum += alphas [x + k, y + l, lay];
														}
												alphas [x, y, lay] = sum / 9f;
										}
								}
						}

				}
	

				// Calculates pre-threaded values such as heights, normals etc. Needs to be performed in main thread!
				public void CalculatePreAlphas ()
				{
						Terrain currentTerrain = terrain;
						setupSplats ();
						alphaMapSize = data.alphamapResolution;

						normals = new Vector3[currentTerrain.terrainData.alphamapWidth, currentTerrain.terrainData.alphamapHeight];
						terrainHeights = new float[currentTerrain.terrainData.alphamapWidth, currentTerrain.terrainData.alphamapHeight];
						alphas = currentTerrain.terrainData.GetAlphamaps (0, 0, currentTerrain.terrainData.alphamapWidth, currentTerrain.terrainData.alphamapHeight);

						Vector3 p = new Vector3 ();
						for (int x = 0; x < currentTerrain.terrainData.alphamapWidth; x++) {
								for (int y = 0; y < currentTerrain.terrainData.alphamapHeight; y++) {
										normals [x, y] = currentTerrain.terrainData.GetInterpolatedNormal (y / (float)currentTerrain.terrainData.alphamapWidth, x / (float)currentTerrain.terrainData.alphamapHeight).normalized;
				
										p.x = y / (float)currentTerrain.terrainData.alphamapWidth * terrainSize.x + currentTerrain.GetPosition ().x;
										p.y = 0;
										p.z = x / (float)currentTerrain.terrainData.alphamapWidth * terrainSize.z + currentTerrain.GetPosition ().z;
										terrainHeights [x, y] = currentTerrain.SampleHeight (p) / terrainSize.y;
								}
			
						}

				}
				// sets all alpha maps
				public void setAlphas ()
				{
						terrain.Flush ();
						terrain.terrainData.SetAlphamaps (0, 0, alphas);
				}
				
				// Places out random trees according to the scenerynode
				private void TreeGenerator (CSceneryNode n, List<TreeInstance> treeList, float scaleCount)
				{
						int cnt = (int)(n.getValue ("count") * scaleCount);
						float rr = n.getValue ("r");
						float gg = n.getValue ("g");
						float bb = n.getValue ("b");
						float size = n.getValue ("size");
						float ls = n.getValue ("lightscale");
						float norm = n.getValue ("normal");
						float hmin = n.getValue ("hmin");
						float hmax = n.getValue ("hmax");
						float seed = n.getValue ("seed");
						float perlin = n.getValue ("perlin") * 4;
						float perlinCut = n.getValue ("perlinCut");
						float rx = C2DMap.shift.x + seed;
						float rz = C2DMap.shift.y;

						for (int i=0; i<cnt; i++) {
								float x = 1 * (Random.value - 0.0f);
								float z = 1 * (Random.value - 0.0f);
								float y = terrain.terrainData.GetInterpolatedHeight (x, z) / terrainSize.y;
								Vector3 pos = new Vector3 (x, y, z);
			
								Vector3 N = terrain.terrainData.GetInterpolatedNormal (pos.x, pos.z);

								float p = Mathf.PerlinNoise ((x + rx) * perlin, (rz + z) * perlin);
								p = 0.5f * p + 0.5f * Random.value * p;
								if (N.y > norm && y > hmin && y < hmax && p > perlinCut) {
										TreeInstance tree = new TreeInstance ();
										float r = Random.value * rr;
										float g = Random.value * gg;
										float b = Random.value * bb;
				
										tree.color = new Color (1 - r, 1 - g, 1 - b);
										tree.heightScale = 1f + (Random.value - 0.5f) * size;
										tree.position = pos;
										float l = 0.95f + (Random.value - 0.5f) * ls;
										tree.lightmapColor = new Color (l, l, l);
										tree.prototypeIndex = 2 * n.sceneryIndex + Random.Range (0, 2);
										tree.widthScale = tree.heightScale;
				
										treeList.Add (tree);
								}

						}
				}
		
				public void ClearTerrain ()
				{
						C2DMap m = new C2DMap ();
						terrain.terrainData.SetHeights (0, 0, m.map);
				}

				public void ClearEnvironment () {
				// Clears detail
					int dmaps = settings.detailPrototypes.Count;
					for (int n=0;n<dmaps;n++) {
						int[,] dmap = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, n);
						for (int i=0;i<terrain.terrainData.detailWidth;i++)
							for (int j=0;j<terrain.terrainData.detailHeight;j++) {
								dmap[i,j] = 0;
							}
						terrain.terrainData.SetDetailLayer(0, 0, n, dmap);
					}
					List<TreeInstance> treeList = new List<TreeInstance> ();
					terrain.terrainData.treeInstances = treeList.ToArray ();
			
				}
			

				public void ClearAlphas ()
				{
				
						//float[,] map = currentTerrain.terrainData.GetHeights(0,0, size,size);
						alphas = terrain.terrainData.GetAlphamaps (0, 0, terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight);
						for (int x = 0; x < terrain.terrainData.alphamapWidth; x++) {
								for (int y = 0; y < terrain.terrainData.alphamapHeight; y++) {
										alphas [x, y, 0] = 1;
										for (int i=1; i<alphas.GetLength(2); i++)
												alphas [x, y, i] = 0;
								}
						}
						setAlphas ();

				}
				// generates (grass) details
				private void DetailGenerator (CSceneryNode n, float scaleCount)
				{
						// write all detail data to terrain data:
						int nr = n.sceneryIndex; 
						int cnt = (int)(n.getValue ("count") * scaleCount / 1000f);
						float perlin = (n.getValue ("perlin") * 20f);
						float threshold = (n.getValue ("threshold"));
						float seed = (n.getValue ("seed"));
						float tilt = n.getValue ("tilt");
			
			//Debug.Log ("Size :" + size);
						//terrain.terrainData.SetDetailResolution ((size - 1), 8);
						int[,] dmap = terrain.terrainData.GetDetailLayer (0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, nr);
						//int[,] dmap = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];
						for (int i=0; i<terrain.terrainData.detailWidth; i++)
								for (int j=0; j<terrain.terrainData.detailHeight; j++) {
										float x = i / (float)(alphaMapSize - 1) + C2DMap.shift.y + seed;
										float y = j / (float)(alphaMapSize - 1) + C2DMap.shift.x;
										float p = Mathf.Clamp (Mathf.Pow (Mathf.PerlinNoise (x * perlin, y * perlin)*1.3f, threshold),0,1);
										Vector3 N = terrain.terrainData.GetInterpolatedNormal (j/(float)size, i/(float)(size)).normalized;
										//if (N.y > (1.0f - tilt))
											p=100f*p * Mathf.Pow(Mathf.SmoothStep(0, 1, N.y - tilt), 1.0f);//p*(N.y + tilt);
									
										p = Mathf.Clamp(p,0,1);
										dmap [i, j] = (int)(cnt*p);
								}
						Debug.Log("DETAIL MAP"+ nr);
						terrain.terrainData.SetDetailLayer (0, 0, nr, dmap);
		
				}
				
				// General scenery node generator
				public void sceneryGenerator (CNode generator)
				{
						buildTreePrototypes ();
						buildDetailPrototypes ();
						CSceneryOutput sc = (CSceneryOutput)generator;
						CSceneryNode[] list = sc.getActiveChildren ();

						List<TreeInstance> treeList = new List<TreeInstance> ();
			
			
						foreach (CSceneryNode n in list) {

								if (n.Type == CSceneryNode.TYPE_TREE) 
										TreeGenerator (n, treeList, sc.getValue ("count"));

								if (n.Type == CSceneryNode.TYPE_DETAIL) 
										DetailGenerator (n, sc.getValue ("count"));

						}

	
						terrain.terrainData.treeInstances = treeList.ToArray ();
						terrain.Flush ();
				}
				// Used by perlin noise
				public void setShiftFromGrid ()
				{
						C2DMap.shift.x = posInGrid.x + 1234.9581f;
						C2DMap.shift.y = posInGrid.y - 2523.5912f;
				}
				// Generates alpha nodes from all parents
				public void createAlphaMapsGenerator (CNode generator)
				{
						for (int x = 0; x < alphas.GetLength(0); x++) {
								for (int y = 0; y < alphas.GetLength(1); y++) {
										CNodeManager.Progress ();
										((CAlpha)generator).CalculateAlphas (x, y, terrainHeights, normals);
										for (int k=0; k< LStyle.NO_TEXTURES; k++)
												alphas [x, y, k] = ((CAlpha)generator).alphas [k];

								}
						}
						for (int i=0; i<2; i++)
								smoothAlpha (alphas);

				}

				public int getAlphasSize ()
				{
						return alphas.GetLength (0);
				}


				public void updateAlphaInformation() {
					data.alphamapResolution = (int)settings.alphaLayerScale*size;//alphaMapSize;
					alphaMapSize = data.alphamapResolution;
					data.SetDetailResolution (size - 1, 8);
			
				}

				private void createNewTerrain (int size, string name)
				{

						data = new TerrainData ();
						data.heightmapResolution = size;
						data.size = terrainSize;

						updateAlphaInformation();


						go = Terrain.CreateTerrainGameObject (data);
						go.name = name;

						go.transform.position = new Vector3 (posInGrid.x * terrainSize.x, 0, posInGrid.y * terrainSize.z);
						go.transform.parent = parent.transform;

				}

				public void Delete ()
				{
						if (go != null) {
								GameObject.DestroyImmediate (go);
						}
						terrain = null;
						go = null;
				}

				public void buildDetailPrototypes ()
				{
						if (settings.detailPrototypes.Count == 0)
								return;
						if (terrain == null)
								return;

						terrain.terrainData.detailPrototypes = settings.detailPrototypesToArray ();
	
	
				}

				public void buildTreePrototypes ()
				{

						if (settings.trees.Count <= 0)
								return;
						if (terrain == null)
								return;

						TreePrototype[] t = terrain.terrainData.treePrototypes;
						int cnt = settings.trees.Count;


						if (t == null) 
								t = new TreePrototype[2 * cnt];

						if (t.Length != 2 * cnt) 
								t = new TreePrototype[2 * cnt];

						for (int i=0; i<cnt; i++) {
								t [2 * i] = new TreePrototype ();
								t [2 * i].prefab = settings.trees [i];
								t [2 * i].bendFactor = 0.0f;

								t [2 * i + 1] = new TreePrototype ();
								t [2 * i + 1].prefab = settings.trees [i];
								t [2 * i + 1].bendFactor = 1f;

						}
						terrain.terrainData.treePrototypes = t;
				}

				public void validate ()
				{

						name = "terrain" + nr;
						//terrain = null;
						if (parent == null) {
								//Debug.Log ("PARENT IS NULL. SHOULD NOT HAPPEN!");
								return;
						}

						go = GameObject.Find (name);
						if (go == null) {
								createNewTerrain (size, name);

						}

						if (terrain == null) {
								terrain = go.GetComponent<Terrain> ();
								terrain.basemapDistance = 5000;
								terrain.heightmapPixelError = 2;
								terrain.materialTemplate = settings.terrainMaterial;
								data = terrain.terrainData;
								buildTreePrototypes ();
								buildDetailPrototypes ();

						}

						if (splatPrototypes == null) {
								setupSplats ();
						}
						if (terrain == null) {
								Debug.Log ("NULL in validate " + name);
								return;
						}
						
						if (terrain.terrainData.heightmapResolution != size) {
								Debug.Log ("Deleting terrain " + name + " because " + size);
								Delete ();
								//validate();
						}
						if (terrain == null)
								return;



				}

				public void UpdateTerrainDetails() {
					terrain.detailObjectDistance = settings.detailDistance;
					terrain.treeBillboardDistance = settings.treeBillboardStart;
					terrain.heightmapPixelError = settings.terrainPixelError;
					terrain.materialTemplate = settings.terrainMaterial;
				}
	
				public void setupSplats ()
				{
						// Setup splat textures
						splatPrototypes = new SplatPrototype[splats]; 

						for (int i=0; i<splats; i++) {
								if (splatPrototypes [i] == null)
										splatPrototypes [i] = new SplatPrototype ();
								splatPrototypes [i].texture = settings.textures [i];
								// Final one is normal
								splatPrototypes [i].normalMap = settings.textures [splats];
								if (splatPrototypes [i].texture != null) {
										splatPrototypes [i].texture.alphaIsTransparency = true;
										splatPrototypes [i].tileSize = new Vector2 (1, 1) * settings.textureScales [i];
								} else {
										splatPrototypes [i].texture = new Texture2D (8, 8);
								}
			
						}
						terrain.terrainData.splatPrototypes = splatPrototypes;
		
						terrain.terrainData.RefreshPrototypes ();

				}

				public CTerrain (GameObject p, int s, int n, Settings set, int x, int y)
				{
						size = s;
						settings = set;
						nr = n;
						parent = p;
						posInGrid.x = x;
						posInGrid.y = y;

						validate ();

				}

				public void Set (float[,] map, int s)
				{
						size = s;
						validate ();
						terrain.terrainData.SetHeights (0, 0, map);
						calculated = true;
				}
	

		}
}
