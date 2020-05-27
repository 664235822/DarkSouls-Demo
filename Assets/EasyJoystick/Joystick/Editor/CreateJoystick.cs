using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public static class CreateJoystickMenuItem 
{
	private const string kKnobPath = "UI/Skin/Knob.psd";
	private const string kUILayerName = "UI";

	[MenuItem("GameObject/UI/Joystick", false, 2020)]
	public static void CreateJoystick()
	{
		GameObject joystick = new GameObject("Joystick", typeof(Image), typeof(EasyJoystick) , typeof(CanvasGroup));
		joystick.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
		joystick.GetComponent<RectTransform> ().sizeDelta = Vector2.one * 150;
		joystick.layer = LayerMask.NameToLayer(kUILayerName);
		RectTransform stick = new GameObject("Stick", typeof(Image)).GetComponent<RectTransform>();
		stick.gameObject.GetComponent<Image>().sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>(kKnobPath);
		stick.sizeDelta = Vector2.one * 60;
		stick.SetParent (joystick.transform, false);
		stick.gameObject.layer = LayerMask.NameToLayer(kUILayerName);
		joystick.GetComponent<EasyJoystick> ().stick = stick;
		SetCanvas (joystick.transform);
		Debug.Log("Joystick has been created");
		Selection.activeGameObject = joystick;
	}

	static public GameObject CreateNewUI()
	{
		// Root for the UI
		var root = new GameObject("Canvas");
		root.layer = LayerMask.NameToLayer(kUILayerName);
		Canvas canvas = root.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		root.AddComponent<CanvasScaler>();
		root.AddComponent<GraphicRaycaster>();
		Undo.RegisterCreatedObjectUndo(root, "Create " + root.name);
		
		// if there is no event system add one...
		CreateEventSystem(false);
		return root;
	}

	public static void CreateEventSystem(MenuCommand menuCommand)
	{
		GameObject parent = menuCommand.context as GameObject;
		CreateEventSystem(true, parent);
	}
	
	private static void CreateEventSystem(bool select)
	{
		CreateEventSystem(select, null);
	}

	private static void CreateEventSystem(bool select, GameObject parent)
	{
		var esys = Object.FindObjectOfType<EventSystem>();
		if (esys == null)
		{
			var eventSystem = new GameObject("EventSystem");
			GameObjectUtility.SetParentAndAlign(eventSystem, parent);
			esys = eventSystem.AddComponent<EventSystem>();
			eventSystem.AddComponent<StandaloneInputModule>();
			eventSystem.AddComponent<TouchInputModule>();
			
			Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
		}
		
		if (select && esys != null)
		{
			Selection.activeGameObject = esys.gameObject;
		}
	}

	public static void SetCanvas(Transform thisTransform)
	{
		Canvas[] canvases = (Canvas[])GameObject.FindObjectsOfType(typeof(Canvas));

		if(canvases.Length > 0)
		{
			for (int i = 0; i < canvases.Length; i++)
			{
				if(canvases[i].enabled)
					thisTransform.SetParent (canvases[i].transform, false);
			}
		}
		else
		{
			GameObject canvas = CreateNewUI();
			thisTransform.SetParent(canvas.transform, false);
		}
	}
}
