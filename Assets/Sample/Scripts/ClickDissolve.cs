using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickDissolve : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private float _maxRadius = 2;
    [SerializeField] private float _dissolveSpeedIn = 1, _dissolveSpeedOut = 1;
    private Material _dissolveMaterial;
    private float _resetRadius = 0;
    private AudioSource _audioSource;

    private void Awake()
    {
        if (_meshRenderer == null)
            _meshRenderer = GetComponent<MeshRenderer>();

        _dissolveMaterial = _meshRenderer.sharedMaterial;

        if (_dissolveMaterial.shader != Shader.Find("Universal Render Pipeline/LitDissolve"))
            _dissolveMaterial = null;
        else
        {
            _resetRadius = _dissolveMaterial.GetFloat("_DissolveRadius");
            _dissolveMaterial.SetFloat("_DissolveRadius", 0);
        }

        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) == false)
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) == false)
            return;

        if (hit.collider.gameObject == this.gameObject)
        {
            StopAllCoroutines();
            StartCoroutine(DissolveCoroutine(hit.point));
        }
    }

    private IEnumerator DissolveCoroutine(Vector3 origin)
    {
        if (_dissolveMaterial == null)
            yield break;

        if (_audioSource != null)
            _audioSource.Play();

        float radius = 0;

        _dissolveMaterial.SetFloat("_DissolveRadius", radius);
        _dissolveMaterial.SetVector("_DissolveOrigin", new Vector4(origin.x, origin.y, origin.z));

        while (radius < _maxRadius)
        {
            yield return null;

            radius += Time.deltaTime * _dissolveSpeedIn;
            _dissolveMaterial.SetFloat("_DissolveRadius", radius);
        }

        while (radius > 0)
        {
            yield return null;

            radius -= Time.deltaTime * _dissolveSpeedOut;
            _dissolveMaterial.SetFloat("_DissolveRadius", radius);
        }

        if (_audioSource != null)
            _audioSource.Stop();
    }

    private void OnDestroy()
    {
        _dissolveMaterial.SetFloat("_DissolveRadius", _resetRadius);
    }
}