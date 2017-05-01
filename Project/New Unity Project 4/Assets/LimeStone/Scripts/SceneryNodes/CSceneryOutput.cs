using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

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
		*  Scenery output - connects to scenery generators
		*/
		public class CSceneryOutput : CNode
		{

				public int sceneryIndex = 0;
				public GameObject obj = null;
	
				public override void Initialize (int windowID, int type, int x, int y)
				{
						status_after_click = CNodeManager.STATUS_TASK4;
						InitializeWindow (windowID, type, x, y, "Scenery Output");		
						setupParameters ();
						color = LStyle.Colors [3] * 2;//new Color(0.5f, 0.2f, 1.9f);
						Outputs.Add (new CConnection (this, 0, CConnection.TYPE3));
						
						// eight inputs should be enough, or what?
						for (int i=0; i<8; i++)
								Inputs.Add (new CConnection (this, i + 1, CConnection.TYPE3));
				}

				public CSceneryNode[] getActiveChildren ()
				{
						List<CSceneryNode> l = new List<CSceneryNode> ();
						foreach (CConnection c in Inputs)
								if (c.pointer != null)
										l.Add ((CSceneryNode)c.pointer.parent);

						return l.ToArray ();
				}
	
				public override void Verify ()
				{
						verified = true;
				}
		
				public void setupParameters ()
				{
						parameters ["count"] = new Parameter ("Scale amount", 1, 0, 3);
						for (int i=0; i<7; i++)
								parameters ["a" + i] = new Parameter ("", 1, 0, 10);
				}


				public override void Draw (int ID)
				{
						//verified = true;
						base.Draw (ID);
						buildGUI ("S-Output");
						GUI.DragWindow ();
		
				}
		}
}
