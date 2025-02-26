using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARPlaneManager))]
public class SceneController : MonoBehaviour
{
    [SerializeField]
    private InputActionReference _togglePlanesAction;

    [SerializeField]
    private InputActionReference _activateAction;

    [SerializeField]
    private GameObject _grabbableKing;

    private ARPlaneManager _planeManager;
    private bool _isVisible = true;
    private int _numPlanesAddedOccured = 0;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("-> SceneController::Start()");

        _planeManager = GetComponent<ARPlaneManager>();

        if (_planeManager is null)
        {
            Debug.LogError("-> Can't find 'ARPlaneManager' :( ");
        }
        
        _togglePlanesAction.action.performed += OnTogglePlanesAction;
        _planeManager.planesChanged += OnPlanesChanged;
        _activateAction.action.performed += OnActivateAction;
    }

    private void OnActivateAction(InputAction.CallbackContext obj)
    {
        SpawnGrabbableKing();
    }

    private void SpawnGrabbableKing()
    {
        Debug.Log("-> SceneController::SpawnGrabbableKing()");

        Vector3 spawnPosition;

        //Iterate Through Each plane found in the scene...
        foreach (var plane in _planeManager.trackables)
        {
            //Detect if the plane is a table, if so, spawn a king on it
            if(plane.classification == PlaneClassification.Table)
            {
                //Debug.Log("->plane class = " + plane.classification);
                spawnPosition = plane.transform.position;
                Debug.Log("->Spawn Position: " + spawnPosition);
                spawnPosition.y += 1;
                Instantiate(_grabbableKing, spawnPosition, Quaternion.identity);
            }

            //if (plane.classification == PlaneClassification<Table>)
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTogglePlanesAction(InputAction.CallbackContext obj)
    {
        _isVisible = !_isVisible;
        float fillAlpha = _isVisible ? 0.3f : 0f;
        float lineAlpha = _isVisible ? 1.0f : 0f;

        Debug.Log("-> OnTogglePlanesAction() - trackables.count: " + _planeManager.trackables.count);
        foreach (var plane in _planeManager.trackables)
        {
            SetPlaneAlpha(plane, fillAlpha, lineAlpha);
        }
    }

    private void SetPlaneAlpha(ARPlane plane, float fillAlpha, float lineAlpha)
    {
        var meshRenderer = plane.GetComponentInChildren<MeshRenderer>();
        var lineRenderer = plane.GetComponentInChildren<LineRenderer>();

        if (meshRenderer != null)
        {
            Color color = meshRenderer.material.color;
            color.a = fillAlpha;
            meshRenderer.material.color = color;
        }

        if (lineRenderer != null)
        {
            //Get the current start and end colors
            Color startColor = lineRenderer.startColor;
            Color endColor = lineRenderer.endColor;

            //set the alpha component
            startColor.a = lineAlpha;
            endColor.a = lineAlpha;

            //Apply the new colors with updated alpha
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (args.added.Count > 0)
        {
            _numPlanesAddedOccured++;

            foreach (var plane in _planeManager.trackables)
            {
                PrintPlaneLabel(plane);
            }

            Debug.Log("-> Number of planes: " + _planeManager.trackables.count);
            Debug.Log("-> Num Planes Added Occured:" + _numPlanesAddedOccured);
        }
    }

    private void PrintPlaneLabel(ARPlane plane)
    {
        string label = plane.classification.ToString();
        string log = $"Plane ID: {plane.trackableId}, Label: {label}";
        Debug.Log(log);
    }

    void OnDestroy()
    {
        Debug.Log("-> SceneController::OnDestroy()");
        _togglePlanesAction.action.performed -= OnTogglePlanesAction;
        _planeManager.planesChanged -= OnPlanesChanged;
        _activateAction.action.performed -= OnActivateAction;
    }
}
