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

    // Você pode adicionar aqui um método para obter a cor de acordo com o ponto atual
    public Color GetColorForPoint(int point)
    {
        foreach (var range in colorRanges)
        {
            if (point >= range.startPoint && point <= range.endPoint)
            {
                return range.color;
            }
        }
        return Color.white; // Caso não encontre, retorna uma cor padrão (branca)
    }
}
