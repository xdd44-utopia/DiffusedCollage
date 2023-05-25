using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour {
	
	public static RectTransform current;
	public RectTransform lt;
	public RectTransform rt;
	public RectTransform lb;
	public RectTransform rb;
	public HandlerController ltc;
	public HandlerController rtc;
	public HandlerController lbc;
	public HandlerController rbc;

	void Start() {
		current = GameObject.Find("DummyImageObject").GetComponent<RectTransform>();
	}

	void Update() {
		if (current == null) {
			GameObject.Find("DummyImageObject").GetComponent<RectTransform>();
		}
		if (ltc.isDragging || rbc.isDragging) {
			current.anchoredPosition = (lt.anchoredPosition + rb.anchoredPosition) / 2;
			current.localScale = new Vector3(
				(rb.anchoredPosition.x - lt.anchoredPosition.x) / 512,
				(lt.anchoredPosition.y - rb.anchoredPosition.y) / 512,
				1
			);
			lb.anchoredPosition = new Vector2(lt.anchoredPosition.x, rb.anchoredPosition.y);
			rt.anchoredPosition = new Vector2(rb.anchoredPosition.x, lt.anchoredPosition.y);
		}
		else if (lbc.isDragging || rtc.isDragging) {
			current.anchoredPosition = (lb.anchoredPosition + rt.anchoredPosition) / 2;
			current.localScale = new Vector3(
				(rt.anchoredPosition.x - lb.anchoredPosition.x) / 512,
				(rt.anchoredPosition.y - lb.anchoredPosition.y) / 512,
				1
			);
			lt.anchoredPosition = new Vector2(lb.anchoredPosition.x, rt.anchoredPosition.y);
			rb.anchoredPosition = new Vector2(rt.anchoredPosition.x, lb.anchoredPosition.y);
		}
		else if (current.gameObject.name == "DummyImageObject") {
			lt.anchoredPosition = new Vector2(10000, 10000);
			lb.anchoredPosition = new Vector2(10000, 10000);
			rt.anchoredPosition = new Vector2(10000, 10000);
			rb.anchoredPosition = new Vector2(10000, 10000);
		}
		else {
			lt.anchoredPosition = new Vector2(
				current.anchoredPosition.x - 256 * current.localScale.x,
				current.anchoredPosition.y + 256 * current.localScale.y
			);
			lb.anchoredPosition = new Vector2(
				current.anchoredPosition.x - 256 * current.localScale.x,
				current.anchoredPosition.y - 256 * current.localScale.y
			);
			rt.anchoredPosition = new Vector2(
				current.anchoredPosition.x + 256 * current.localScale.x,
				current.anchoredPosition.y + 256 * current.localScale.y
			);
			rb.anchoredPosition = new Vector2(
				current.anchoredPosition.x + 256 * current.localScale.x,
				current.anchoredPosition.y - 256 * current.localScale.y
			);
		}

		if (Input.GetKeyDown(KeyCode.Delete)) {
			current.GetComponent<ImageObject>().destroyImage();
			current = GameObject.Find("DummyImageObject").GetComponent<RectTransform>();
		}
	}
}
