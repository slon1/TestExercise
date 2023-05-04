using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class GameControl : MonoBehaviour {
	[SerializeField] private Player player;
	[SerializeField] private List<Transform> targets;
	[SerializeField] private Transform Root;
	[SerializeField] private float radius;
	[SerializeField] private ListBoxTMP listBox;
	[SerializeField] private GameObject win;
	[SerializeField] private TextMeshProUGUI text;

	private List<string> wordList;
	private float radiusPow2;		
	private List<TargetComponent> targetComponents;	
	private const string urlSet = "http://158.160.3.255:8021/exercises/set_exercise_data";
	private const string urlGet = "http://158.160.3.255:8021/exercises/get_exercise_data?record_id=";
	void Start() {		
		targetComponents = new List<TargetComponent>(); 
		radiusPow2= radius * radius;
		var tmp = Root.Cast<Transform>().Except(targets).ToList();
		InitTarget(tmp);		
		listBox.Init(tmp.Select(x => x.name).ToList());
		listBox.OnClick += ListBox_OnClick;
		listBox.OnWin += ListBox_OnWin;
	}

	private void ListBox_OnWin() {
		win.SetActive(true);
	}

	private void ListBox_OnClick(string obj) {		
		listBox.ColorMe(obj, wordList.Contains(obj)? Color.blue: Color.red);
		
	}
	private void SaveData(List<string> neighbours) {		
		Server.RequestHelper.HttpPost(urlSet, neighbours, GetData, OnFail);
		
	}
	private void GetData(string id) {		 
		Server.RequestHelper.HttpGet(urlGet+id, (response) => wordList = JsonConvert.DeserializeObject<List<string>>(response), OnFail);		
		
	}
	private void InitTarget(List<Transform> goList) {

		targets.ForEach(t => {
			List<string> neighbours = new List<string>();
			goList.ForEach(go => {				
				if ((t.position - go.position).sqrMagnitude <= radiusPow2) {
					neighbours.Add(go.name);
				}
			});
			TargetComponent tc = t.gameObject.AddComponent<TargetComponent>();
			tc.neighbours = neighbours;
			targetComponents.Add(tc);				
		});
	}
	
	private TargetComponent CollisionCheck() {		
		foreach (var item in targetComponents) {
			if (player.CollisionCheck(item.position)) {				
				return item;
			}
		}
		return null;
	}

	private void OnFail(string error) {
		print(error);
	}
	
	public void CloseDlg() {		
		listBox.Show(false);
		player.pauseInput = false;
	}
	
	void Update() {
		TargetComponent target = CollisionCheck();
		if (target!=null) {
			listBox.ShowMessage(true);
			if (Input.GetKeyDown(KeyCode.Space)) {
				listBox.Show(true);
				player.pauseInput = true;
				SaveData(target.neighbours);
			}
		}
		else {
			listBox.ShowMessage(false);
		}
	}

	private void OnDestroy() {
		listBox.OnClick -= ListBox_OnClick;
		listBox.OnWin -= ListBox_OnWin;
		Server.RequestHelper.Dispose();
	}
}
