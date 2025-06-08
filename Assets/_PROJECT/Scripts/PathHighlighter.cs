using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathHighlighter : MonoBehaviour
{
    [ColorUsage(true, true)]
    [SerializeField] private Color _highlightColor = Color.yellow;

    [Header("Settings")]
    [SerializeField] private float _intensity = 1f;
    [SerializeField] private float _stepDelay = 0.1f;
    [SerializeField] private float _highlightDuration = 0.4f;

    [Header("A* References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private PathFinderManager _pathManager;

    [Header("Checkpoint Positions")]
    [SerializeField] private Vector3Int _mustPassA1;
    [SerializeField] private Vector3Int _mustPassA2;
    [SerializeField] private Vector3Int _mustPassA3;
    [Header("")]
    [SerializeField] private Vector3Int _mustPassB1;
    [SerializeField] private Vector3Int _mustPassB2;
    [SerializeField] private Vector3Int _mustPassB3;


    private Dictionary<Vector3Int, Transform> _tileLookup = new();
    private HashSet<Renderer> _activeFlashes = new();



    private void Start()
    {
        BuildTileLookup();
        StartCoroutine(DelayedStart());
    }

    private IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(WaitUntilReadyAndHighlight());
    }

    private void BuildTileLookup()
    {
        _tileLookup.Clear();

        foreach (Transform child in _gridManager.tilemap.transform)
        {
            Vector3Int gridPos = _gridManager.tilemap.WorldToCell(child.position);
            if (!_tileLookup.ContainsKey(gridPos))
                _tileLookup.Add(gridPos, child);
        }

    }

    private IEnumerator WaitUntilReadyAndHighlight()
    {
        yield return new WaitForSeconds(0.5f);

        var spawnPoints = _pathManager.GetSpawnPositions();
        if (spawnPoints == null || spawnPoints.Count == 0)
            yield break;

        //1 Spawn
        if (spawnPoints.Count == 1)
        {
            var spawn = spawnPoints[0];

            var pathA1 = _pathManager.GetPathFromTo(spawn, _mustPassA1);
            var pathA2 = _pathManager.GetPathFromTo(_mustPassA1, _mustPassA2);
            var pathA3 = _pathManager.GetPathFromTo(_mustPassA2, _mustPassA3);
            var pathA4 = _pathManager.GetPathFromTo(_mustPassA3, _pathManager.endPoint);

            if (pathA1 != null && pathA2 != null && pathA3 != null && pathA4 != null)
            {
                var fullA = new List<TileNode>(pathA1);
                fullA.AddRange(pathA2);
                fullA.AddRange(pathA3);
                fullA.AddRange(pathA4);
                StartCoroutine(HighlightPath(fullA));
            }

            var pathB1 = _pathManager.GetPathFromTo(spawn, _mustPassB1);
            var pathB2 = _pathManager.GetPathFromTo(_mustPassB1, _mustPassB2);
            var pathB3 = _pathManager.GetPathFromTo(_mustPassB2, _mustPassB3);
            var pathB4 = _pathManager.GetPathFromTo(_mustPassB3, _pathManager.endPoint);

            if (pathB1 != null && pathB2 != null && pathB3 != null && pathB4 != null)
            {
                var fullB = new List<TileNode>(pathB1);
                fullB.AddRange(pathB2);
                fullB.AddRange(pathB3);
                fullB.AddRange(pathB4);
                StartCoroutine(HighlightPath(fullB));
            }
        }
        else
        {
            // More than 1 spawn
            foreach (var spawn in spawnPoints)
            {
                var path = _pathManager.GetPathFromTo(spawn, _pathManager.endPoint);
                if (path != null)
                    StartCoroutine(HighlightPath(path));
            }
        }
    }



    private IEnumerator HighlightPath(List<TileNode> path)
    {
        foreach (var node in path)
        {
            if (!_tileLookup.TryGetValue(node.position, out Transform tile))
                continue;
            
            Renderer rend = null;
            foreach (Renderer r in tile.GetComponentsInChildren<Renderer>())
            {
                if (r.enabled)
                {
                    rend = r;
                    break;
                }
            }

            if (rend == null)
                continue;

            StartCoroutine(FlashTile(rend));
            yield return new WaitForSeconds(_stepDelay);
        }
    }

    private IEnumerator FlashTile(Renderer rend)
{
    if (_activeFlashes.Contains(rend))
        yield break; 

    _activeFlashes.Add(rend);

    Material mat = rend.material;
    Color originalEmission = mat.GetColor("_EmissionColor");

    mat.EnableKeyword("_EMISSION");
    mat.SetColor("_EmissionColor", _highlightColor * _intensity);

    yield return new WaitForSeconds(_highlightDuration);

    mat.SetColor("_EmissionColor", originalEmission);

    _activeFlashes.Remove(rend);
}

}
