using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestComponent : MonoBehaviour
{
    public string testVariable;

    public string TestVariableSetter { set { testVariableWithSetter = value; } }
    public string testVariableWithSetter;

    public void testFunction1(string s) { }
    public void testFunction2(string s) { }
}
