using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Threading;

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
* Main editor class for LimeStone. Displays nodes, buttons, maintains everyhing. You know it, baby!
*
*/
		public class LimeStoneEditor : CNodeManager
		{
				public GUIStyle largeFont = new GUIStyle ();
				static public string[] statusStrings = {
				"Done",
				"Initializing",
				"Calculating.",
				"Calculating..",
				"Thread done",
				"Generating alpha maps",
				"Generating alpha maps",
				"Setting alpha maps"
		};
				static private string aboutMessage = 
						"<size=24>" + LStyle.hexColors [4] + "LimeStone Terrain Editor </color></size>\n" +
						"<size=16>" + LStyle.hexColors [2] + "by LemonSpawn </color></size>\n" +
						"<size=12>" + LStyle.hexColors [3] + "http://www.lemonspawn.com </color></size>\n" +
						"\n" +
						"\n" +
				"LimeStone is a node-based terrain generator extension for Unity developed by LemonSpawn. \n\n" +
				"To get started, right-click anywhere on the canvas and select a terrain generator, or load an example file. Click on the header button on any node " +
				"to assemble the current node, linking to its parent nodes. \n\n" +
				"Please visit our home page for instructions and tutorial videos!\n" +
				"\n";
				// Initialize certain things at startup		
				private static bool startInitialize = false;
				// Link to the terrain manager
				public static CTerrainManager terrainManager = null;
				// Link to settings
				public static Settings settings = null;
				// Link to active terrain 
				CTerrain curTerrain = null;
				// LimeStone Logo
				public static Texture2D logo = null, background = null, background2 = null;
				// Parent object
				GameObject parent = null;

				// Define menu item
				[MenuItem ("Window/LemonSpawn/LimeStone Terrain Generator")]
				static void Init ()
				{
						EditorWindow.GetWindow (typeof(LimeStoneEditor));
				}

				// Sets automatic shift for perlin noise
				private void setShift ()
				{
						if (curTerrain != null && calculator != null) {
								curTerrain.setShiftFromGrid ();
						}
				}
				// Makes sure that the parent always exists (in case user deletes terrains)	
				private void maintainParent ()
				{
						parent = GameObject.Find (LStyle.TerrainGameobjectName);
						if (parent == null) {
								parent = new GameObject (LStyle.TerrainGameobjectName);
								parent.AddComponent<Settings> ();
						}

						settings = parent.GetComponent<Settings> ();

						if (terrainManager == null)
								terrainManager = parent.GetComponent<CTerrainManager> ();
						if (terrainManager == null) 
								terrainManager = parent.AddComponent<CTerrainManager> (); //new CTerrainManager(parent, settings);

						terrainManager.Init (parent, settings);

						terrainManager.parent = parent;

						curTerrain = terrainManager.findNextTerrain ();
						setShift ();
				}

				// Renders the LimeStone logo and sets up the retro background color. 		
				private void renderLogo ()
				{
						if (logo == null) {
								logo = (Texture2D)Resources.Load ("LimeStone", typeof(Texture2D));
						}

						if (background == null) {
								background = new Texture2D (1, 1, TextureFormat.RGBA32, false);
								background.SetPixel (0, 0, LStyle.Colors [0] * 0.75f);
								background.Apply ();
						}
						if (background2 == null) {
							background2 = new Texture2D (1, 1, TextureFormat.RGBA32, false);
							Color c =  LStyle.Colors[1] * 0.35f;
							
							c.a = 0.75f;
							background2.SetPixel (0, 0, c);
							background2.Apply ();
						}
						if (logo != null)
								GUI.DrawTexture (new Rect (0, 0, logo.width, 1.0f * logo.height), logo);
						}
				
				// Buttons and stuff
				private void renderMenuButtons ()
				{
						string s = ">>";
						float dy = Screen.height/15f;///34;
						float hy = Screen.height/15 - 4;
						float y = hy/2;
						float i = 0;
					
						int bsx = 200;
						int dbsx = (int)(bsx*1.15f);
						GUI.DrawTexture (new Rect (Screen.width-dbsx, 0, Screen.width, Screen.height), background2);
						int db = dbsx - bsx; 
						GUI.color = LStyle.Colors [4];
			
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "New")) {
								terrainManager.ClearTerrain ();
								terrainManager.ClearAlphas ();
								terrainManager.ClearEnvironment();
								New ();
						}
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Load"))
								Load ();
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Save"))
								Save ();
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Save as"))
								SaveAs ();
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "About"))
								About ();

						y+=hy;
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Clear terrain"))
								terrainManager.ClearTerrain ();
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Clear alpha"))
								terrainManager.ClearAlphas ();
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Clear environment"))
							terrainManager.ClearEnvironment ();

						if (settings.textureShow)
							s = "<<";
						y+=hy;
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Texture settings" + s)) 
							settings.textureShow = ! settings.textureShow;
						
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Setup terrain details")) 
							TerrainDetailEditor.Init (settings);
			
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Setup detail prototypes (trees)")) 
							SceneryEditor.Init ();
						
						if (GUI.Button (new Rect (Screen.width - bsx, y + dy * i++, bsx-db, hy), "Setup detail textures (grass)")) 
							DetailListEditor.Init ();
			

				}
				
				// Displays buttons for settings + textures
				private void displaySettings ()
				{
						float y = 220;
		
						float x = 50;
						float di = 0;
						float dy2 = 120;
						float dtext = 25;
						for (int i=0; i<CTerrain.splats+1; i++) {

			
								GUI.Label (new Rect (x, y + dy2 * di, 100, 50), LStyle.TextureNames [i]);
								GUI.Label (new Rect (x, y + dy2 * di + dtext, 100, 50), "Scale:");
								float f = settings.textureScales [i];
								string t = "" + f;
								if (f == 0)
										t = "";
								string text = GUI.TextField (new Rect (x + 35, y + dy2 * di + dtext * 1, 45, 22), t);
								float.TryParse (text, out f);
								if (f != settings.textureScales [i]) {
										settings.textureScales [i] = f;
										terrainManager.Splats ();

								}
								settings.textureScales [i] = f;
								GUI.Label (new Rect (x, y + dy2 * di, 100, 50), LStyle.TextureNames [i]);
								settings.textures [i] = EditorGUI.ObjectField (new Rect (x + 100, y + dy2 * di++, 100, 100),
			                                               settings.textures [i],
			                                               typeof(Texture2D), true) as Texture2D;
								if (i == 1) {
										di = 0;
										x += 250;
								}
						}

				}
				// prepares GUI with settings, buttons, background and whatnot
				public override void SetupGUI ()
				{
						largeFont.fontSize = 16;
						largeFont.normal.textColor = LStyle.Colors [2];//new Color(0.5f, 0.5f, 0.5f);

						GUI.DrawTexture (new Rect (0, 0, Screen.width-200, Screen.height), background);
						renderLogo ();

						if (!settings.textureShow) {

								//GUI.color = LStyle.Colors[4];
								string s = settings.currentFile;
								if (s == "") 
										s = "[ No file. Be warned: running game or recompiling will discard unsaved work, so remember to save! ]"; 

								GUI.Label (new Rect (40, Screen.height - 50, 700, 50), "Current file: " + s, largeFont);
						} 
				
						if (status != STATUS_NONE) 
								EditorUtility.DisplayProgressBar ("Calculating", "Please stand by...", progress / 100.0f);
						else
								EditorUtility.ClearProgressBar ();

						if (settings.textureShow) 
								displaySettings ();

				}

				public LimeStoneEditor () : base()
				{
						startInitialize = false;
				}
				
				// Makes sure the terrain is always hooked up and has the correct size. Takes care of clicking & initializing calculations
				private void maintainTerrain ()
				{
						if (size == 0) // NEEDS TO BE SET
								return;
						if (size != settings.getSize ()) {
								size = settings.getSize ();
								C2DMap.sizeX = size;
								C2DMap.sizeY = size;
								resetMaps ();
								ResetState ();

								terrainManager.Reset (true);
								terrainManager.ResetState ();
								status = STATUS_NONE;
								progress = 0;
						}

						terrainManager.validate ();
						if (status == STATUS_CHANGED) {
								progressDelta /= terrainManager.size.x * terrainManager.size.y;
								terrainManager.ResetState ();
								status = STATUS_START_THREAD;
								maintainParent ();
								startThread ();
								if (calculator == null)
										status = STATUS_NONE;
						}

				}

				public void Update ()
				{
						if (!startInitialize)
								return;

						InternalUpdate ();
						maintainTerrain ();
						Tasks ();

				}

				public override void PreAssemble ()
				{
				}
				
				// Ready to create something - called for terrain generation
				public override void Create ()
				{
						curTerrain = terrainManager.findNextTerrain ();

						if (curTerrain == null)
								return;
						if (calculator == null)
								return;
						if (calculator.map == null) {
								calculator = null;
								status = STATUS_NONE;

								return;
						}
						// Always smooth.. this should be a %)$/ setting.
						calculator.map.Smooth (settings.TerrainSmoothVal);
		
						curTerrain.Set (calculator.map.map, size);
						curTerrain = terrainManager.findNextTerrain ();
						if (curTerrain == null) {
								status = STATUS_NONE;
								//createAlpha();
								return;
						}
						setShift ();
						status = STATUS_START_THREAD;
						startThread ();
				}

				void threadedAlphaUpdate ()
				{
						terrainManager.AlphasFromGenerator (calculator);
						status = STATUS_TASK3;
				}

				private void Tasks ()
				{
						if (status == STATUS_TASK1) {
							// Does nothing right now.
							status = STATUS_NONE;
						}

						if (status == STATUS_IN_THREAD) {
								// Needs to update the status bar. bloody hell. 
								ForceRepaint = true;
						}
						if (status == STATUS_TASK2) {
								// Starts alpha map calculation								
								status = STATUS_IN_THREAD;
								terrainManager.CalculatePreAlphas ();
								progress = 0;
								progressDelta = 100.0f / Mathf.Pow (terrainManager.grid [0, 0].getAlphasSize (), 2f) / (terrainManager.size.x * terrainManager.size.y);
								Thread t = new Thread (threadedAlphaUpdate);
								t.Start ();

								//threadedAlphaUpdate();
						}
						if (status == STATUS_TASK3) {
								// Sets alpha maps
								status = STATUS_NONE;
								terrainManager.SetAlphas ();
						}
						if (status == STATUS_TASK4) {
								// Calculate scenery
								terrainManager.SceneryFromGenerator (calculator);
								status = STATUS_NONE;
						}

				}

				public override void setupMenu ()
				{
						menuItems.Clear ();
						menuItems.Add (new MenuType ("Terrain/Perlin", typeof(CProceduralMap), CProceduralMap.TYPE_PERLIN));
						menuItems.Add (new MenuType ("Terrain/Multiridged", typeof(CProceduralMap), CProceduralMap.TYPE_MULTIRIDGED));
						menuItems.Add (new MenuType ("Terrain/Swiss noise", typeof(CProceduralMap), CProceduralMap.TYPE_SWISS));

						menuItems.Add (new MenuType ("Terrain/Filter", typeof(CFilter), CFilter.TYPE_EXP));

						menuItems.Add (new MenuType ("Terrain/Combiner", typeof(CCombiner), CCombiner.TYPE_BLEND));

						menuItems.Add (new MenuType ("Alpha/Height", typeof(CAlphaGenerator), CAlphaGenerator.TYPE_HEIGHT));
						menuItems.Add (new MenuType ("Alpha/Normal", typeof(CAlphaGenerator), CAlphaGenerator.TYPE_NORMAL));
						menuItems.Add (new MenuType ("Alpha/Curvature", typeof(CAlphaGenerator), CAlphaGenerator.TYPE_CURVATURE));
						menuItems.Add (new MenuType ("Alpha/Combiner", typeof(CAlphaCombiner), CAlphaCombiner.TYPE_BLEND));

						menuItems.Add (new MenuType ("Scenery/Tree Generator", typeof(CSceneryNode), 0));
						menuItems.Add (new MenuType ("Scenery/Detail Generator", typeof(CSceneryNode), 1));
						menuItems.Add (new MenuType ("Scenery/Combiner", typeof(CSceneryOutput), 0));

						menuItems.Add (new MenuType ("Output", typeof(COutput), 0, true, "You can only have one output!"));

						base.setupMenu ();
				}
				
				// First run
				void StartInitialize ()
				{

						if (settings == null) {
								return;
						}
						size = settings.getSize ();
						C2DMap.sizeX = size;
						C2DMap.sizeY = size;

						maintainParent ();
						if (settings.currentFile == "")
								New ();
						else 
								LoadDirect (settings.currentFile);

						startInitialize = true;
						ForceRepaint = true;

				}

				void OnGUI ()
				{
				
						if (settings == null) 
								maintainParent ();

						if (!startInitialize) 
								StartInitialize ();

						renderNodes = !settings.textureShow;
						
						Render ();

						renderMenuButtons ();

				}
				// Create new node system
				void New ()
				{
						nodes.Clear ();
						links.Clear ();
						settings.currentFile = "";
						Initialize ();
						ResetState ();
				}

				public override void Initialize ()
				{
						base.Initialize ();
						initialized = true;
				}
		
				void Load ()
				{
						string fn = EditorUtility.OpenFilePanel (
							"Load terrain",
							"",
						LStyle.TerrainFiletype);
						if (fn.Length == 0)
								return;
						settings.currentFile = fn;
						LoadDirect (settings.currentFile);
				}

				void Save ()
				{
						if (settings.currentFile.Length != 0)
								CSerializedNode.Save (settings.currentFile, nodes);
						else {
								SaveAs ();
						}

				}

				void SaveAs ()
				{
						string file = EditorUtility.SaveFilePanel (
			"Save terrain as",
			"",
			"",
				LStyle.TerrainFiletype);
			
						if (file.Length != 0) {
								settings.currentFile = file;
								Save ();
						}
				}

				void About ()
				{
						HelpEditor.Create (aboutMessage);                         
				}
		}


}