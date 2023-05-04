using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuiComponent : MonoBehaviour {
	public Button button;
	public Image image;
	public TextMeshProUGUI text;

	void Start() {
		
	}
	public void AddListener(Action<GuiComponent> action) {
		button.onClick.AddListener(() => action?.Invoke(this));
	}


	// Update is called once per frame
	void Update() {

	}
	private void OnDestroy() {
		button.onClick.RemoveAllListeners();
	}
}
