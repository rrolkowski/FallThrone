using UnityEngine;

public class OutlineHover : MonoBehaviour
{
	public Material outlineMaterialTemplate; // Szablon materiału z outline'em

	private string targetTag = "EnemySelected";

	private Renderer lastRenderer;
	private Material[] originalMaterials;

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
				Renderer rend = taggedParent.GetComponentInChildren<Renderer>();
				if (rend != null)
				{
					if (rend != lastRenderer)
					{
						Debug.Log("[OutlineHover] Trafiono: " + hit.collider.name);

						ResetLastRenderer();

						originalMaterials = rend.materials;

						Debug.Log("[OutlineHover] Materiały oryginalne:");
						foreach (var mat in originalMaterials)
							Debug.Log(" - " + (mat != null ? mat.name : "null"));

						// Tworzymy unikalną instancję materiału na runtime
						Material outlineMatInstance = new Material(outlineMaterialTemplate);

						// Przenosimy BaseMap z oryginalnego materiału
						Texture baseMap = rend.material.GetTexture("_BaseMap");
						if (baseMap == null)
							Debug.LogWarning("[OutlineHover] Brak _BaseMap w materiale: " + rend.material.name);
						else
							outlineMatInstance.SetTexture("_BaseMap", baseMap);

						// Podmieniamy wszystkie materiały
						Material[] mats = new Material[rend.materials.Length];
						for (int i = 0; i < mats.Length; i++)
							mats[i] = outlineMatInstance;

						Debug.Log("[OutlineHover] Podmieniam materiały (ilość: " + mats.Length + ")");

						rend.materials = mats;
						lastRenderer = rend;
					}

					return; // nie resetuj, jeśli jesteśmy najechani
				}
				else
				{
					Debug.LogWarning("[OutlineHover] Nie znaleziono Renderer'a w obiekcie z tagiem: " + taggedParent.name);
				}
			}
		}

		ResetLastRenderer();
	}

	void ResetLastRenderer()
	{
		if (lastRenderer != null && originalMaterials != null)
		{
			Debug.Log("[OutlineHover] Reset materiałów");
			lastRenderer.materials = originalMaterials;
			lastRenderer = null;
			originalMaterials = null;
		}
	}
}
