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
		/*
		*  COutput is ONLY used for in-game generation of terrains. NOT SUPPORTED YET!
		*/
		public class COutput : CNode
		{


				public override void Initialize (int windowID, int type, int x, int y)
				{
						InitializeWindow (windowID, type, x, y, "Output");		
						map = new C2DMap ();
						color = new Color (1.0f, 0.3f, 0.7f);
						status_after_click = CNodeManager.STATUS_NONE;
						Inputs.Add (new CConnection (this, 0, CConnection.TYPE0));
						Inputs.Add (new CConnection (this, 1, CConnection.TYPE1));
						Inputs.Add (new CConnection (this, 2, CConnection.TYPE3));


				}

				public CNode getInput (int i)
				{
						CConnection c = (CConnection)Inputs [i];
						if (c == null)
								return null;
						if (c.pointer == null)
								return null;
						return c.pointer.parent;

				}

/*				public override void Calculate ()
				{
						if (Inputs == null)
								return;

						if (Inputs.Count == 0)
								return;
						//return;
						CConnection c = (CConnection)Inputs [0];
						if (c.pointer == null)
								return;
						CNode m1 = c.pointer.parent;

						if (m1 == null)
								return;

						//return;
						map = m1.map;

						if (m1.changed) {
								m1.Calculate ();
						}


				}
*/
				public override void Draw (int ID)
				{
						base.Draw (ID);

						buildGUI (Name);

						GUI.DragWindow ();
						window.height = size.y + LStyle.FontSize * 4;		
				}


		}
}
