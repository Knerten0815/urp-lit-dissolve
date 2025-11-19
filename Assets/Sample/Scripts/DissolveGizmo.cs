using System.Collections;
using UnityEngine;

public class DissolveGizmo : MonoBehaviour
{
    [SerializeField] GameObject _gizmoBase, _arrowX, _arrowY, _arrowZ, _radiusIndicator;
    [SerializeField] float _scaleMultiplier = 0.01f;
    [SerializeField] private Material _matX, _matY, _matZ, _matBase;
    [SerializeField] private Material[] _dissolveMaterials;
    private bool _isDragging = false;
    private Vector3 _defaultPosition, _defaultRadius;

    private void Awake()
    {
        _defaultPosition = transform.position;
        _defaultRadius = _radiusIndicator.transform.localScale;
    }
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Check if we are hovering gizmo or radius indicator. Reset visuals if not
        if (Physics.Raycast(ray, out RaycastHit hitGizmos, maxDistance: float.MaxValue, LayerMask.GetMask("Gizmos")) == false
            && Physics.Raycast(ray, out RaycastHit hitIndicator, maxDistance: float.MaxValue, LayerMask.GetMask("Indicator")) == false)
        {
            ResetVisuals();
            return;
        }

        // only continue if gizmos were hit
        if (hitGizmos.collider == null)
        {
            Hover(null);
            return;
        }

        // get material of the arrow/base and highlight that part.
        Material hitMaterial = hitGizmos.collider.GetComponent<MeshRenderer>().sharedMaterial;
        Hover(hitMaterial);

        if (Input.GetMouseButtonDown(0) == false)
            return;

        UpdateDissolveOrigin();
        UpdateDissolveRadius();

        if (hitMaterial == _matBase)
        {
            StartCoroutine(ScaleDrag());
        }
        else
        {
            StartCoroutine(PositionDrag(hitMaterial));
        }
    }

    private void ResetVisuals()
    {
        if (_isDragging)
            return;

        _matX.color = DimColor(Color.red);
        _matY.color = DimColor(Color.green);
        _matZ.color = DimColor(Color.blue);
        _matBase.color = DimColor(Color.white);

        _arrowX.SetActive(false);
        _arrowY.SetActive(false);
        _arrowZ.SetActive(false);
        _radiusIndicator.SetActive(false);
    }

    private void Hover(Material material)
    {
        if (_isDragging)
            return;

        _matX.color = material == _matX ? Color.red : DimColor(Color.red);
        _matY.color = material == _matY ? Color.green : DimColor(Color.green);
        _matZ.color = material == _matZ ? Color.blue : DimColor(Color.blue);
        _matBase.color = material == _matBase ? Color.white : DimColor(Color.white);

        _arrowX.SetActive(true);
        _arrowY.SetActive(true);
        _arrowZ.SetActive(true);
        _radiusIndicator.SetActive(true);
    }

    private Color DimColor(Color color)
    {
        color *= 0.5f;
        color.a = 1;
        return color;
    }

    private void UpdateDissolveOrigin()
    {
        foreach (Material mat in _dissolveMaterials)
            mat.SetVector("_DissolveOrigin", transform.position);
    }

    private void UpdateDissolveRadius()
    {
        foreach (Material mat in _dissolveMaterials)
            mat.SetFloat("_DissolveRadius", _radiusIndicator.transform.localScale.x * 0.5f);
    }

    private Plane GetPlane(Material material)
    {
        if (material == _matX || material == _matY)
            return new Plane(Vector3.forward, transform.position.z);

        if (material == _matZ)
            return new Plane(Vector3.right, transform.position.x);

        return new Plane();
    }

    private Vector3 ProjectMouseToWorldPosition(Plane plane)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        plane.Raycast(ray, out float distanceToPlane);
        return ray.GetPoint(distanceToPlane);
    }

    private IEnumerator PositionDrag(Material hitMaterial)
    {
        _isDragging = true;

        Plane plane = GetPlane(hitMaterial);
        Vector3 currentPos = ProjectMouseToWorldPosition(plane);
        Vector3 lastPos = currentPos;
        Vector3 worldDir;
        Vector3 moveDir;

        while (Input.GetMouseButton(0))
        {
            currentPos = ProjectMouseToWorldPosition(plane);
            worldDir = currentPos - lastPos;
            moveDir = Vector3.zero;

            if (hitMaterial == _matX)
                moveDir = Vector3.right * worldDir.x;
            else if (hitMaterial == _matY)
                moveDir = Vector3.up * worldDir.y;
            else if (hitMaterial == _matZ)
                moveDir = Vector3.forward * worldDir.z;

            transform.position -= moveDir;
            UpdateDissolveOrigin();

            lastPos = currentPos;
            yield return null;
        }

        _isDragging = false;
    }

    private IEnumerator ScaleDrag()
    {
        _isDragging = true;

        Vector3 startMousePos = Input.mousePosition;
        Vector3 startScale = _radiusIndicator.transform.localScale;
        Vector3 currentMousePos;
        Vector3 newScale;
        float diff;

        while (Input.GetMouseButton(0))
        {
            currentMousePos = Input.mousePosition;
            diff = (currentMousePos - startMousePos).y;
            newScale = startScale + Vector3.one * (diff * _scaleMultiplier);

            if (newScale.x <= _gizmoBase.transform.localScale.x)
            {
                yield return null;
                continue;
            }

            _radiusIndicator.transform.localScale = newScale;
            UpdateDissolveRadius();

            yield return null;
        }

        _isDragging = false;
    }

    public void ResetGizmo()
    {
        transform.position = _defaultPosition;
        _radiusIndicator.transform.localScale = _defaultRadius;
        UpdateDissolveOrigin();
        UpdateDissolveRadius();
    }
}
