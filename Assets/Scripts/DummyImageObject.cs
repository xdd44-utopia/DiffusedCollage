using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DummyImageObject : ImageObject {

	protected override void Start() {
		rect = GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(4000, 2500);
	}
	
	public override void OnDrag(PointerEventData eventData) {
		return;
	}

}
