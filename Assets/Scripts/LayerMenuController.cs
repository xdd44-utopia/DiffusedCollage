using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerMenuController : MonoBehaviour {

	public RectTransform dummy;

	public static RectTransform first;
	
	void Start() {
		
	}


	void Update() {
		
	}

	public void addItem(LayerItem item) {
		item.upper = dummy;
		if (first != null) {
			item.lower = first;
			first.GetComponent<LayerItem>().upper = item.GetComponent<RectTransform>();
		}
		first = item.GetComponent<RectTransform>();
	}
}
