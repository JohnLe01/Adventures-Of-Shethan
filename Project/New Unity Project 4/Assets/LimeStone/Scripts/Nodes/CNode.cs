using UnityEngine;
using UnityEditor;
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
		*
		* Parent node class
		*
		*
		*/
		public class CNode
		{
				public string Name = "CNode";
				// Window ID.
				public int ID;
				public int status_after_click = 0;
				// Internal type - which kind of combiner etc
				public int Type = 0;

				// If changed, update
				public bool changed = true;
				// Correct amount if inputs/outputs
				public bool verified = false;
				// clickerrormessage displayed if node incorrectly used. Help is generic. 
				public string helpMessage = "";
				public string clickErrorMessage = "";
				// Data for popup selection
				protected PopupData popupData;
				List<Parameter> displayParameters = null;
				// Alternate names for parameters
				protected List<AlternativeName> alternativeNames = new List<AlternativeName> ();

				// Font for printing
				static public GUIStyle largeFont = new GUIStyle ();
				// Internal tabbing
				public static int TAB = 0;
				// All parameters 
				public Hashtable parameters = new Hashtable ();

				// Default color
				public Color color = new Color (1, 1, 0.2f);
				public Color errorColor = new Color (1, 0, 0.2f, 0f);

				// The rect for this node
				public Rect window;
				// Default (x,y) size for this window
				public Vector2 size = new Vector3 (180, 0);
				public C2DMap map = null;
				// only used for alpha maps. terrible solution, but quick fix. 
				// List of connections
				public ArrayList Inputs = new ArrayList ();
				public ArrayList Outputs = new ArrayList ();
				public ArrayList Bottoms = new ArrayList ();
				public ArrayList Tops = new ArrayList ();
				// Used for counting the number of children of this node
				public static int childrenCount;
				
				// counds the number of child nodes
				public void getInputsChildren ()
				{
						childrenCount += 1;
						foreach (CConnection c in Inputs) {
								if (c.pointer != null)
										c.pointer.parent.getInputsChildren ();
						}

				}
				// Sets up progress delta for status bar
				public void calculateProgressDelta (float size)
				{
						childrenCount = 0;
						getInputsChildren ();
						CNodeManager.progressDelta = 100.0f / (childrenCount * (size * size));
						CNodeManager.progress = 0;
				}

				// Returns this nodes connection type
				private int getConnectionType ()
				{
						if (Inputs.Count != 0)
								return ((CConnection)Inputs [0]).Type;
						if (Outputs.Count != 0)
								return ((CConnection)Outputs [0]).Type;
						if (Bottoms.Count != 0)
								return ((CConnection)Bottoms [0]).Type;
						if (Tops.Count != 0)
								return ((CConnection)Tops [0]).Type;

						return -1;
				}
				// creates a list of links for the connections in use
				public void SetupLinks (ArrayList links)
				{

						foreach (CConnection c in Outputs) {
								if (c.pointer != null) {
										CLink l = new CLink ();
										l.from = c;
										l.to = c.pointer;
										links.Add (l);
								}
						}
						foreach (CConnection c in Bottoms) {
								if (c.pointer != null) {
										CLink l = new CLink ();
										l.from = c;
										l.to = c.pointer;
										l.drawType = 1;
										links.Add (l);
								}
						}

				}
				// get a link node
				protected CNode getNode (ArrayList l, int i)
				{
						CConnection c = ((CConnection)l [i]);
						if (c == null)
								return null;
						if (c.pointer == null)
								return null;

						return c.pointer.parent;

				}
				// returns a value of a parameter
				public float getValue (string p)
				{
						if ((Parameter)parameters [p] == null)
								return -1;
						return ((Parameter)(parameters [p])).value;
				}
				// sets a parameter value
				public void setValue (string p, float v)
				{
						if ((Parameter)parameters [p] == null)
								return;
						((Parameter)(parameters [p])).value = v;
				}
				// sets a new parameter max val
				public void setMax (string p, float v)
				{
						if ((Parameter)parameters [p] == null)
								return;
						((Parameter)(parameters [p])).max = v;
				}

				protected void Change ()
				{
						changed = true;

				}

				// set up alternative names for labels
				protected void setupAlternativeNames ()
				{
						foreach (AlternativeName an in alternativeNames) {
								if (an.type == Type) {
										Parameter p = (Parameter)parameters [an.hashName];
										if (p == null)
												return;
										p.label = an.label;
								}
						}
				}

				private static int SortByName (Parameter o1, Parameter o2)
				{
						return o1.label.CompareTo (o2.label);
				}
				
				// displays preset options, including loading and saving 
				protected void renderPresets ()
				{

						popupData = CNodeManager.presets.buildData (popupData, this);
						GUILayout.Label ("Presets:");
						popupData.index = EditorGUILayout.Popup (popupData.index, popupData.vals);

						//popupData.name = GUILayout.TextField (popupData.name);
						if (popupData.index != popupData.newIndex) {
								popupData.newIndex = popupData.index;
								popupData.getCurrentPreset ().CopyTo (parameters);
								popupData.name = popupData.getCurrentPreset ().Name;
								rebuildDisplayParams ();

						}
						// Deciding!
						
						/*if (GUILayout.Button ("Save preset")) {
								CNodeManager.presets.AddPreset (this, parameters, popupData.name);
								CNodeManager.presets.Save (LStyle.PresetsFilename);
						}
						if (GUILayout.Button ("Delete preset")) {
								CNodeManager.presets.RemovePreset (popupData.getCurrentPreset ());
								popupData.index = -1;
								CNodeManager.presets.Save (LStyle.PresetsFilename);
						}
						*/
						size.y += LStyle.FontSize * 2; //3 for presets


				}
			
				protected void InitializeWindow(int windowID, int type, int x, int y, string name) {
					window = new Rect (x, y, size.x, size.y);
					ID = windowID;
					Type = type;
					Name = name;
			
				}

				protected void rebuildDisplayParams ()
				{
						displayParameters.Clear ();
						foreach (DictionaryEntry e in parameters)
								displayParameters.Add ((Parameter)e.Value);
		
		
						displayParameters.Sort (SortByName);

				}
				// standard random seed used for various perlin noise methods
				public static float getSeed (float s)
				{
						return s * 152.135f;
				}


	
				// automatically builds the node window
				protected void buildGUI (string lbl)
				{

						largeFont.fontSize = LStyle.LargeFontSize;
						largeFont.normal.textColor = color;

						largeFont.alignment = TextAnchor.UpperCenter;
						float d = 30; // pos of window
		
						if (GUI.Button (new Rect (d, 15, size.x - 2 * d, 32), "")) {
								Verify ();
								if (verified) {
										// only allowed to click when not active
										if (CNodeManager.status == CNodeManager.STATUS_NONE) {
												CNodeManager.status = status_after_click;

												calculateProgressDelta (CNodeManager.size);
												CNodeManager.calculator = this;
										}
								} else 
										CNodeManager.DisplayError (clickErrorMessage);
						}
						size.y = 50;
						// main label
						GUILayout.Label (lbl, largeFont);
						GUILayout.Space (5);

						if (displayParameters == null) {
								displayParameters = new List<Parameter> ();
								rebuildDisplayParams ();
						}

						foreach (Parameter p in displayParameters) {
								size.y += LStyle.FontSize;
								if (p.label == "")
										continue;
								GUILayout.BeginHorizontal (); // "Button" is cool!
								GUILayout.Label (p.label, GUILayout.Width (60));
								//GUILayout.Label (new GUIContent(p.label, p.label),GUILayout.Width(60));
								/*p.stringValue = GUILayout.TextField (p.stringValue);
								//		amplitude = Regex.Replace(amplitude, @"[^a-zA-Z0-9 ]", "");
								p.stringValue = Regex.Replace(p.stringValue, @"[^0-9.]", "");
								GUILayout.EndHorizontal ();
								float test;
								if (float.TryParse(p.stringValue, out test)) {
								p.value = test;
								}*/
								float f = p.value;
								p.value = GUILayout.HorizontalSlider (p.value, p.min, p.max);
								GUILayout.Label ("" + p.value, GUILayout.Width (28));

								if (p.value != f) 
										Change ();

								GUILayout.EndHorizontal ();
						}
						// close and help buttons
						if (GUI.Button (new Rect (size.x - 30, 15, 25, 32), "X"))
								CNodeManager.deleteNode (this);
						if (GUI.Button (new Rect (5, 15, 25, 32), "?")) {
								HelpEditor.Create (helpMessage);                         
						}

						window.height = size.y;
				}

				public virtual void Initialize (int windowID, int type, int x, int y)
				{
				}

				public virtual void Draw (int ID)
				{
						Verify ();
						setColor ();
				}

				// Virtual function for calculating the actual node
				public virtual void Calculate ()
				{

				}

				// sets up connection ids for loading/saving
				public void SetupID ()
				{
						foreach (CConnection c in Inputs) {
								c.parent = this;
								c.setupID ();
						}
						foreach (CConnection c in Outputs) {
								c.parent = this;
								c.setupID ();
						}
						foreach (CConnection c in Bottoms) {
								c.parent = this;
								c.setupID ();
						}
						foreach (CConnection c in Tops) {
								c.parent = this;
								c.setupID ();
						}
				}
				// prepare node for deletion
				public void BreakLinks ()
				{
						foreach (CConnection c in Inputs) 
								c.Break ();
						foreach (CConnection c in Outputs) 
								c.Break ();
						foreach (CConnection c in Bottoms) 
								c.Break ();
				}
				// Render connections of a specific type (top/bottom/input/output)
				private void RenderConnectionType (ArrayList list, Vector2 pos, Vector2 deltaScale, int sx, int sy, string str)
				{

						float w = window.width;
						float h = window.height;

						float dx = w / (float)(list.Count + 1);
						float dy = h / (float)(list.Count + 1);
						float X = dx - sx / 2;
						float Y = dy - sy / 2;
	
						GUI.color = color;//CConnection.connectionColors[i];
						foreach (CConnection c in list) {
								c.setupID ();
								GUI.color = LStyle.connectionColors [c.Type];

								c.position.x = pos.x + X * deltaScale.x + sy / 2;
								c.position.y = pos.y + Y * deltaScale.y + sy / 2;
			
								if (GUI.Button (new Rect (pos.x + X * deltaScale.x, pos.y + Y * deltaScale.y, sx, sy), str)) {
										CNodeManager.connection = c.Click (CNodeManager.connection);
								}
			
								X += dx;
								Y += dy;
						}
						GUI.color = color;

				}

				public void RenderConnections ()
				{
						int sx = LStyle.ConnectionTypeX;
						int sy = LStyle.ConnectionTypeY;
						RenderConnectionType (Inputs, new Vector2 (window.x - sx, window.y), new Vector2 (0, 1), sx, sy, ">");
						RenderConnectionType (Outputs, new Vector2 (window.x + window.width, window.y), new Vector2 (0, 1), sx, sy, ">");
						RenderConnectionType (Bottoms, new Vector2 (window.x, window.y + window.height), new Vector2 (1, 0), sx, sy, "|");
						RenderConnectionType (Tops, new Vector2 (window.x, window.y - sy), new Vector2 (1, 0), sx, sy, "|");
				}
				// Sets default normal + non-verified colors
				protected void setColor ()
				{
						float s = 0.75f;
						errorColor.a = 1.0f;
						errorColor.r = color.r * s;
						errorColor.g = color.g * s;
						errorColor.b = color.b * s;
						if (verified)
								GUI.color = color;
						else
								GUI.color = errorColor;


				}
				// Really not in use right now
				public void MouseMenu (Event evt, Vector2 mousePos)
				{
						if (evt.type != EventType.ContextClick)
								return;

						{
						}

				}
				// Default verify method. Can be overridden.
				public virtual void Verify ()
				{
						verified = true;

						foreach (CConnection c in Inputs)
								if (c.pointer == null)
										verified = false;

						setupAlternativeNames ();
				}



		}
}
