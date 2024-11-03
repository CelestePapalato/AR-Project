using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Linq;
using Unity.AI.Navigation;

public class SurfaceBaker : MonoBehaviour
{
    public static SurfaceBaker Instance;

    public ARPlaneManager planeManager;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        planeManager.planesChanged += UpdatePlaneList;
    }

    private void OnDisable()
    {
        planeManager.planesChanged -= UpdatePlaneList;
    }

    Dictionary<ARPlane, NavMeshSurface> surfaces = new Dictionary<ARPlane, NavMeshSurface>();

    ARPlane[] toUpdate;

    void UpdatePlaneList(ARPlanesChangedEventArgs args)
    {
        List<ARPlane> planesToBake = new List<ARPlane>();
        if (args.removed.Count > 0)
        {
            foreach(ARPlane plane in args.removed)
            {
                surfaces.Remove(plane);
            }
        }
        if (args.updated.Count > 0)
        {
            planesToBake.AddRange(args.updated);
        }

        planesToBake.AddRange(args.added);

        foreach(ARPlane plane in args.added)
        {
            surfaces[plane] = plane.GetComponent<NavMeshSurface>();
        }

        toUpdate = planesToBake.ToArray();
    }

    [ContextMenu("Bake")]
    public void BakeSurfaces()
    {
        if(toUpdate.Length > 0)
        {
            foreach (ARPlane plane in toUpdate)
            {
                surfaces[plane].BuildNavMesh();
            }
        }
    }
}
