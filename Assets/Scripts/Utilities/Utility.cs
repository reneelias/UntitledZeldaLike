using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static Color GetColorShade(Color baseColor, float shadePercentage){
        Color shadedColor = baseColor;

        if(shadePercentage > 0){
            shadedColor.r = Mathf.Min((1f - baseColor.r) * shadePercentage + baseColor.r, 1f);
            shadedColor.g = Mathf.Min((1f - baseColor.g) * shadePercentage + baseColor.g, 1f);
            shadedColor.b = Mathf.Min((1f - baseColor.b) * shadePercentage + baseColor.b, 1f);
        } else {
            shadedColor.r = Mathf.Max((baseColor.r - 0f) * shadePercentage + baseColor.r, 0f);
            shadedColor.g = Mathf.Max((baseColor.g - 0f) * shadePercentage + baseColor.g, 0f);
            shadedColor.b = Mathf.Max((baseColor.b - 0f) * shadePercentage + baseColor.b, 0f);
        }

        return shadedColor;
    }
}
