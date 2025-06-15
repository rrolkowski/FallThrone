using UnityEngine;
using System.Collections.Generic;

public class OutlineHover : MonoBehaviour
{
	public Material outlineMaterialTemplate; // Szablon materiału z outline'em

	private string targetTag = "EnemySelected";

	private List<Renderer> modifiedRenderers = new List<Renderer>();
	private Dictionary<Renderer, Material[]> originalMaterials = new Dictionary<Renderer, Material[]>();

	void Update()
	{
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit))
		{
			GameObject hitObject = hit.collider.gameObject;

			// Szukamy w hierarchii obiektu z tagiem
			Transform taggedParent = hitObject.transform;
			while (taggedParent != null && !taggedParent.CompareTag(targetTag))
				taggedParent = taggedParent.parent;

			if (taggedParent != null && taggedParent.CompareTag(targetTag))
			{
				// Jeżeli zmieniliśmy wcześniej inny obiekt — resetujemy
				if (modifiedRenderers.Count == 0 || modifiedRenderers[0].transform.root != taggedParent.root)
				{
					Debug.Log("[OutlineHover] Trafiono nowy obiekt: " + taggedParent.name);
					ResetAllRenderers();

					// Szukamy i podmieniamy MeshRenderer i SkinnedMeshRenderer
					Renderer[] meshRenderers = taggedParent.GetComponentsInChildren<MeshRenderer>(true);
					SkinnedMeshRenderer[] skinnedMeshRenderers = taggedParent.GetComponentsInChildren<SkinnedMeshRenderer>(true);

					foreach (Renderer rend in meshRenderers)
						ReplaceMaterials(rend);

					foreach (SkinnedMeshRenderer rend in skinnedMeshRenderers)
						ReplaceMaterials(rend);
				}

				return; // nie resetuj, jeśli jesteśmy najechani
			}
		}

		ResetAllRenderers();
	}

	void ReplaceMaterials(Renderer rend)
	{
		if (rend == null)
			return;

		Debug.Log("[OutlineHover] Podmieniam materiały dla: " + rend.name);

		// Zachowaj oryginalne materiały
		originalMaterials[rend] = rend.materials;

		// Tworzymy unikalną instancję materiału outline
		Material outlineMatInstance = new Material(outlineMaterialTemplate);

		// Przenosimy BaseMap z pierwszego oryginalnego materiału (jeśli istnieje)
		Texture baseMap = rend.material.GetTexture("_BaseMap");
		if (baseMap == null)
			Debug.LogWarning("[OutlineHover] Brak _BaseMap w materiale: " + rend.material.name);
		else
			outlineMatInstance.SetTexture("_BaseMap", baseMap);

		// Tworzymy tablicę materiałów
		Material[] mats = new Material[rend.materials.Length];
		for (int i = 0; i < mats.Length; i++)
			mats[i] = outlineMatInstance;

		rend.materials = mats;

		// Dodajemy do listy zmodyfikowanych
		modifiedRenderers.Add(rend);
	}

	void ResetAllRenderers()
	{
		if (modifiedRenderers.Count == 0)
			return;

		Debug.Log("[OutlineHover] Reset wszystkich materiałów");

		foreach (Renderer rend in modifiedRenderers)
		{
			if (rend != null && originalMaterials.ContainsKey(rend))
			{
				rend.materials = originalMaterials[rend];
			}
		}

		modifiedRenderers.Clear();
		originalMaterials.Clear();
	}
}
