#if UNITY_EDITOR
using UnityEngine;
// Separate from Match3DFlow so Home/Restart can cancel game coroutines without cancelling tests.
public sealed class BoardValidationDriver:MonoBehaviour {}
#endif
