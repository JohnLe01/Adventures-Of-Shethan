using UnityEngine;
using System.Collections;

namespace LimeStone {
	
	// Generic alpha node class
	
	public class CAlpha : CNode {

		public float[] alphas = new float[100];
		
		public virtual void CalculateAlphas (int i, int j, float[,] h, Vector3[,] normals)
		{
			
		}
		
	}
}