using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class FontManager : MonoBehaviour
{
    private string _fontMaterial;

    void Awake()
    {
        _fontMaterial = null;
        AlterFont();
    }

    public void AlterFont()
    {
        if (this.gameObject.TryGetComponent<TMPro.TextMeshPro>(out var textMeshPro))
        {
            if (textMeshPro.fontMaterial.name.Contains(".0.1.2"))
                _fontMaterial = ".0.1.2";
            else if (textMeshPro.fontMaterial.name.Contains(".1.2.3"))
                _fontMaterial = ".1.2.3";
            else if (textMeshPro.fontMaterial.name.Contains(".2.3.4"))
                _fontMaterial = ".2.3.4";

            if (PlayerPrefHelper.GetFont() == Constants.FontAccessibility && textMeshPro.font.name.Contains(Constants.Font3x5))
            {
                textMeshPro.font = Resources.Load<TMP_FontAsset>($"Fonts & Materials/{Constants.FontAccessibility}");
                if (_fontMaterial != null)
                    textMeshPro.fontMaterial = Resources.Load<Material>($"Fonts & Materials/{Constants.FontAccessibility}{_fontMaterial}");
                textMeshPro.fontStyle = FontStyles.Bold | FontStyles.UpperCase;
                textMeshPro.margin += new Vector4(0f, 0.12f, 0f, 0f);
                textMeshPro.wordSpacing = -4.0f;
                textMeshPro.fontSize = 8.2f;
                textMeshPro.text = textMeshPro.text.Replace(Constants.Font3x5, Constants.FontAccessibility);                    
            }
            else if (PlayerPrefHelper.GetFont() == Constants.Font3x5 && textMeshPro.font.name.Contains(Constants.FontAccessibility))
            {
                textMeshPro.font = Resources.Load<TMP_FontAsset>($"Fonts & Materials/{Constants.Font3x5}");
                if (_fontMaterial != null)
                    textMeshPro.fontMaterial = Resources.Load<Material>($"Fonts & Materials/{Constants.Font3x5}{_fontMaterial}");
                textMeshPro.fontStyle = FontStyles.Normal;
                textMeshPro.margin += new Vector4(0f, -0.12f, 0f, 0f);
                textMeshPro.wordSpacing = 0.0f;
                textMeshPro.fontSize = 7.3f;
                textMeshPro.text = textMeshPro.text.Replace(Constants.FontAccessibility, Constants.Font3x5);                    
            }
        }
        if (this.gameObject.TryGetComponent<HoverBhv>(out var hoverBhv))
        {
            if (PlayerPrefHelper.GetFont() == Constants.FontAccessibility)
                hoverBhv.Content = hoverBhv.Content.Replace(Constants.Font3x5, Constants.FontAccessibility);
            else if (PlayerPrefHelper.GetFont() == Constants.Font3x5)
                hoverBhv.Content = hoverBhv.Content.Replace(Constants.FontAccessibility, Constants.Font3x5);
        }
    }
}
