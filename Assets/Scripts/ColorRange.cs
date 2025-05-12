using UnityEngine;
using System.Collections.Generic;


public class ColorRange : MonoBehaviour
{
    public Color color;
    public int startPoint;
    public int endPoint;
}

public class ColorRangeManager : MonoBehaviour
{
    public List<ColorRange> colorRanges;

    // Voc� pode adicionar aqui um m�todo para obter a cor de acordo com o ponto atual
    public Color GetColorForPoint(int point)
    {
        foreach (var range in colorRanges)
        {
            if (point >= range.startPoint && point <= range.endPoint)
            {
                return range.color;
            }
        }
        return Color.white; // Caso n�o encontre, retorna uma cor padr�o (branca)
    }
}
