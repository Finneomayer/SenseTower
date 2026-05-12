using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;

// Simple example of stripping of a debug build configuration
class ShaderDebugBuildProcessor : IPreprocessShaders
{
    ShaderKeyword m_KeywordDebug;

    public ShaderDebugBuildProcessor()
    {
        m_KeywordDebug = new ShaderKeyword("DEBUG");
    }

    // Multiple callback may be implemented.
    // The first one executed is the one where callbackOrder is returning the smallest number.
    public int callbackOrder { get { return 0; } }

    public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> shaderCompilerData)
    {
#if UNITY_SERVER
        for (int i = 0; i < shaderCompilerData.Count; ++i)
        {
                shaderCompilerData.RemoveAt(i);
                --i;
        }
#endif
    }
}