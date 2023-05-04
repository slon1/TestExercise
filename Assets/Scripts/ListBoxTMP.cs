
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ListBoxTMP : MonoBehaviour {
	public event Action<string> OnClick;
	public event Action OnWin;
	[SerializeField] private GameObject message;
	[SerializeField] private TMP_Text text;

	private int lastLine = -2;
	private Dictionary<string, (string, bool)> hash = new Dictionary<string, (string, bool)>();
	private bool isAcive = true;
	private Color checkColor = Color.blue;


	private string Str2Html(string name, Color color) {
		return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{name}</color>";
	}
	public void Show(bool visible) {
		text.transform.parent.gameObject.SetActive(visible);
	}
	public void ShowMessage(bool visible) {
		if (isAcive != visible) {
			isAcive = visible;
			message.transform.gameObject.SetActive(visible);
		}

	}
	public void ColorMe(string name, Color color) {
		hash[name] = (Str2Html(name, color), color == checkColor);
		Refresh();
	}
	private void Refresh() {
		text.text = "";
		foreach (var item in hash.Values) {
			text.text += item.Item1 + Environment.NewLine;
		}
		if (hash.Values.Where(x => !x.Item2).Count() == 0)
			OnWin?.Invoke();
	}

	public void Init(List<string> names) {
		ShowMessage(false);
		names.ForEach(x => {
			hash.Add(x, (Str2Html(x, Color.white), false));
		});
		Refresh();
	}

	void LateUpdate() {
		if (isAcive) {
			if (Input.GetMouseButtonDown(0)) {
				if (TMP_TextUtilities.IsIntersectingRectTransform(text.rectTransform, Input.mousePosition, null)) {
					var line = TMP_TextUtilities.FindIntersectingWord(text, Input.mousePosition, null);
					if (line != -1 && line != lastLine) {
						OnClick?.Invoke(text.textInfo.wordInfo[lastLine = line].GetWord());						
						return;
					}
				}
			}
			if (Input.GetKeyDown(KeyCode.Space)) {
				Show(true);

			}
		}
	}
	private void OnDestroy() {
		hash.Clear();
		hash = null;
	}

}
