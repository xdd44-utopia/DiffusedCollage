using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ImageObject : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public GameObject layerPrefab;
	private Transform layerHolder;
	private LayerMenuController layerMenu;
	private GameObject layer;

	protected RectTransform rect;
	private static int size = 512;
	
	private Vector3 posDiff;

	protected virtual void Start() {
		rect = GetComponent<RectTransform>();
		rect.sizeDelta = new Vector2(size, size);
	}

	void Update() {
		
	}

	public void selected() {
		Selector.current = rect;
	}

	public void updateImage(string name, string prompt) {
		byte[] bytes = System.IO.File.ReadAllBytes(Application.dataPath + "/Images/" + name);
		Texture2D tex = new Texture2D(size, size, TextureFormat.RGB24, false);
		tex.LoadImage(bytes);
		GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
		if (layer == null) {
			layerHolder = GameObject.Find("Items").transform;
			layerMenu = GameObject.Find("LayerMenu").GetComponent<LayerMenuController>();
			layer = Instantiate(layerPrefab, layerHolder);
			layer.GetComponent<RectTransform>().anchoredPosition = new Vector3(12.5f, -108, 0);
			layerMenu.addItem(layer.GetComponent<LayerItem>());
		}
		LayerItem.count++;
		layer.transform.GetChild(0).GetComponent<Image>().sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
		layer.transform.GetChild(1).GetComponent<TMP_Text>().text = prompt;
		layer.GetComponent<LayerItem>().image = this.transform;
	}
	
	public void OnBeginDrag(PointerEventData eventData) {
		if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null) {
			RectTransform draggingPlane = eventData.pointerEnter.transform as RectTransform;
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
				posDiff = rect.position - globalMousePos;
			}
		}
	}
	
	public virtual void OnDrag(PointerEventData eventData) {
		if (eventData.pointerEnter != null && eventData.pointerEnter.transform as RectTransform != null) {
			RectTransform draggingPlane = eventData.pointerEnter.transform as RectTransform;
			Vector3 globalMousePos;
			if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggingPlane, eventData.position, eventData.pressEventCamera, out globalMousePos)) {
				rect.position = posDiff + globalMousePos;
			}
		}
	}

	public void OnEndDrag(PointerEventData eventData) {
		
	}

	public virtual void destroyImage() {
		layer.GetComponent<LayerItem>().destroyItem();
		Destroy(this.gameObject);
	}

}
