using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWalkIntoNewScene
{
    public IEnumerator WalkIntoNewScene(Vector2 exitDir, float delay);
}
