#if UNITY_EDITOR
using UnityEngine;
// Separate from ReplicaFlow so Home/Restart can cancel game coroutines without cancelling tests.
public sealed class SourceValidationDriver:MonoBehaviour {}
#endif
