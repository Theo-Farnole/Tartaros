using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Display log string in the MonoBehaviour inspector. (only at runtime)
/// </summary>
public interface IInspectorLog
{
    string Log { get; }
}
