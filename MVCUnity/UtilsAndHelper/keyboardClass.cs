using UnityEngine;
using UnityEngine.EventSystems;

public class keyboardClass : MonoBehaviour, ISelectHandler {

#if UNITY_WEBGL
	[DllImport("__Internal")]
	private static extern void focusHandleAction (string _name, string _str);


    public void ReceiveInputData(string value) {
		gameObject.GetComponent<InputField> ().text = value;
	}

	public void OnSelect(BaseEventData data) {
		try{
			focusHandleAction (gameObject.name, gameObject.GetComponent<InputField> ().text);
		}
		catch(Exception){}
	}
#else
    public void OnSelect(BaseEventData data) { }
#endif
    }
