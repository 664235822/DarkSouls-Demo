using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnablerExample : MonoBehaviour {
	public Button Enable;
	public EasyJoystick Move, Look;
	private bool enable = true;
	private Text enableText;

	// Use this for initialization
	void Start () 
	{
		enableText = Enable.transform.Find ("Text").GetComponent("Text") as Text;
		Enable.onClick.AddListener (()=> enable = !enable);
	}
	
	// Update is called once per frame
	void Update () 
	{
		enableText.text = enable ? "<color=red>Disable</color>" : "<color=lime>Enable</color>";

		Move.Enable (enable);
		Look.Enable (enable);
	}
}
