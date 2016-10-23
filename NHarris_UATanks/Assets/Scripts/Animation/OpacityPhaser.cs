using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class OpacityPhaserSettings
{
    public bool IsPaused { get { return _pause; } }

    public float MinOpacity { get { return _minOpacity; } }
    public float MaxOpacity { get { return _maxOpacity; } }
    public float TransitionSpeed { get { return _transitionSpeed; } }

    [SerializeField]
    private bool _pause;

    [SerializeField]
    [Range(0, 1)]
    private float _minOpacity;
    [SerializeField]
    [Range(0, 1)]
    private float _maxOpacity = 1f;
    [SerializeField]
    [Tooltip("The speed (in seconds) at which the opacity value will change.")]
    [Range(0, 1)]
    private float _transitionSpeed = .1f;
}

[AddComponentMenu("Animation/Effects/Opacity Phasing")]
public class OpacityPhaser : MonoBehaviour
{
    private enum PhaseShift { Visible, Invisible }

    private PhaseShift _currentPhaseShift = PhaseShift.Invisible;
    [SerializeField]
    private OpacityPhaserSettings _settings;

    [SerializeField]
    private List<Material> _meshMaterialsList;
    
    private void Awake()
    {
        // This object may not be created at start, so we do this during Awake

        if (_meshMaterialsList == null)
        {
            // grab all of the renderers, then grab all their materials, concat all the arrays into
            // a single list and assign that to the phase shift settings list for later reference
            _meshMaterialsList = GetComponentsInChildren<MeshRenderer>()
                .Select(x => x.materials)
                .Aggregate((result, array) => { return result.Concat(array).ToArray(); })
                .ToList();
        }
    }
	
	private void Update()
    {
        if (!_settings.IsPaused)
        {
            Color colorRef;
            Func<float> getTransitionValue = () => { return _settings.TransitionSpeed * Time.deltaTime; };

            foreach (var material in _meshMaterialsList)
            {
                if (_currentPhaseShift == PhaseShift.Invisible)
                {
                    if (material.color.a <= _settings.MinOpacity)
                    {
                        _currentPhaseShift = PhaseShift.Visible;
                    }
                    else
                    {
                        colorRef = material.color;
                        colorRef.a -= getTransitionValue();
                        material.color = colorRef;
                    }
                }
                else
                {
                    if (material.color.a >= _settings.MaxOpacity)
                    {
                        _currentPhaseShift = PhaseShift.Invisible;
                    }
                    else
                    {
                        colorRef = material.color;
                        colorRef.a += getTransitionValue();
                        material.color = colorRef;
                    }
                }
            }
        }
	}

    public void Initialize(OpacityPhaserSettings settings)
    {
        _settings = settings;
    }
}
