using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BoolVariable : ScriptableObject, ISerializationCallbackReceiver {

	public bool InitialValue;

	[System.NonSerialized] public bool RuntimeValue;

	public void OnAfterDeserialize(){
		RuntimeValue = InitialValue;
	}

	public void OnBeforeSerialize(){}

}
