using UnityEngine;
using System.Collections.Generic;

namespace ReflectiveProjectionSpace
{
	public class RP_Lerper : MonoBehaviour {
	
		[HideInInspector]
		public List<Vector3> EndPos = new List<Vector3>();

		[HideInInspector]
		int EndPositionsLength;

		[HideInInspector]
		public float lerpTime;
		
		float currentLerpTime = 0f,
		perc;
		
		private bool _move;
		
		[HideInInspector]
		public bool IsActive = false;

		[HideInInspector]
		public bool ShouldStopLerp = false;

		[HideInInspector]
		public Transform EmitterDefaultTrans;

		//property to trigger the lerping function
		public bool StartMove {
			get{ return _move; }

			set{
				_move = value;
				if (_move){
					EndPositionsLength = EndPos.Count;
					currentLerpTime = 0f;
					IsActive = true;
					ShouldStopLerp = false;
				}
			}
		}

		[HideInInspector]
		public int _index = 0;

		[HideInInspector]
		public bool ricochetFinished = true;

		[HideInInspector]
		public ReflectiveProjection mainScript;

		
		//Update is called once per frame
		void Update () 
		{
			if (IsActive) {
				mainScript.ricochetFinished = false;
				Moving (_index);
			}else{
				mainScript.ricochetFinished = true;
			}
		}

		//the main moving method
		void Moving(int index)
		{

			if (!ShouldStopLerp)
				transform.position = Vector3.MoveTowards (transform.position, EndPos[index], mainScript.ricochetSpeed * Time.deltaTime);

			//if distance between two positions of lerp is smaller than threshold then stop lerp
			if (Vector3.Distance (transform.position, EndPos[index]) <= 0.2f) {
				if(index == (EndPositionsLength - 1)) {
					ShouldStopLerp = true;	
					IsActive = false;
					_index = 0;

					mainScript.ObjectRicochet = false;
					transform.position = EmitterDefaultTrans.position;
				}else{
					currentLerpTime = currentLerpTime / 2f;
					_index += 1;
				}			
			}
		}
	}
}
