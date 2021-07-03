using System.Collections;
using UnityEngine;

public class Fractal : MonoBehaviour
{
    [SerializeField]
    private Mesh _mesh;

    /// <summary>
    /// Material to be used by each fractal.
    /// </summary>
    [SerializeField]
    private Material _material;

    /// <summary>
    /// The current number of layers deep into the fractal.
    /// </summary>
    [SerializeField]
    private int _maxDepth = 4;

    /// <summary>
    /// The scale of each child fractal.
    /// </summary>
    [SerializeField]
    private float _childScale;

    /// <summary>
    /// The current depth of the child fractal being created, not to reach more than _maxDepth.
    /// </summary>
    private int _depth;

    /// <summary>
    /// Cached Transform component.
    /// </summary>
    private Transform _transform;

    private void Start()
    {
        Init();
    }

    /// <summary>
    /// Initializes variables and caches components.
    /// </summary>
    private void Init()
    {
        if(transform != null && _transform != transform) _transform = transform;
        
        gameObject.AddComponent<MeshFilter>().mesh = _mesh;
        gameObject.AddComponent<MeshRenderer>().material = _material;
        
        //This check prevents Unity from crashing since we will be creating instances of this class repeatedly.
        if (_depth < _maxDepth)
        {
            //Start is not called on these new game objects until the next frame after Fractal is added as a component.
            //This means none of the code here in Start prior to its creation will be applied immediately - keep that in mind.
            StartCoroutine(CreateFractalChildren());
        }
    }

    /// <summary>
    /// Creates the child game objects for each parent.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateFractalChildren()
    {
        yield return new WaitForSeconds(.5f);
        //Create the first child going in one direction.
        new GameObject("Fractal Child").AddComponent<Fractal>().CreateFractalInstance(this, Vector3.up, Quaternion.identity);

        yield return new WaitForSeconds(.5f);
        //Create a second child in a different direction.
        new GameObject("Fractal Child").AddComponent<Fractal>().CreateFractalInstance(this, Vector3.right, Quaternion.Euler(0, 0, -90f));
        
        yield return new WaitForSeconds(.5f);
        //Create a third child in a different direction.
        new GameObject("Fractal Child").AddComponent<Fractal>().CreateFractalInstance(this, Vector3.left, Quaternion.Euler(0, 0, 90f));
    }
    
    /// <summary>
    /// Create an instance of the fractal that takes the components and variables from its parent in the Hierarchy.
    /// </summary>
    private void CreateFractalInstance(Fractal _parentFractal, Vector3 _direction, Quaternion _orientation)
    {
        _transform = transform;
        _mesh = _parentFractal._mesh;
        _material = _parentFractal._material;
        _maxDepth = _parentFractal._maxDepth;
        _depth = _parentFractal._depth + 1;
        _childScale = _parentFractal._childScale;
        _transform.SetParent(_parentFractal._transform);
        _transform.localScale = Vector3.one * _childScale;
        
        //Move each child by .5f in the direction specified and again by half its scale.
        _transform.localPosition = _direction * (.5f + .5f * _childScale);
        _transform.localRotation = _orientation;
    }
}
