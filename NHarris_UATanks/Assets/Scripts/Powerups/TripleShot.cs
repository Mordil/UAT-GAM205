using UnityEngine;

[CreateAssetMenu(fileName = "New Triple Shot", menuName = "Powerup/Triple Shot")]
public class TripleShot : Powerup
{
    [SerializeField]
    [Range(0.01f, 60)]
    private float _duration = 1.5f;
    public override float Duration { get { return _duration; } }
}
