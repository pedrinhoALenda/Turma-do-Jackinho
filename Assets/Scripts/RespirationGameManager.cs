using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RespirationPhasesManager : MonoBehaviour
{
    [Header("Referência da Barrinha")]
    public RectTransform respirationBar;

    [Header("Áreas de Fase")]
    public RectTransform fase1Area;
    public RectTransform fase2Area;
    public RectTransform fase3Area;

    [Header("Sprites Visuais por Fase")]
    public GameObject[] fase1Sprites;
    public GameObject[] fase2Sprites;
    public GameObject[] fase3Sprites;

    [Header("Movimento da Barrinha")]
    public float riseSpeed = 400f;
    public float fallSpeed = 300f;
    public float minHeight = 0f;
    public float maxHeight = 800f;

    [Header("Tempo necessário por fase (segundos)")]
    public float tempoNecessarioParaCompletar = 2f;

    [Header("TMP: Textos de Instrução e Timers")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI instructionText2;
    public TextMeshProUGUI instructionText3;
    public TextMeshProUGUI fase1TimerText;
    public TextMeshProUGUI fase2TimerText;
    public TextMeshProUGUI fase3TimerText;

    [Header("Textos Personalizáveis")]
    public string textoRespire = "Respire...";
    public string textoSegure = "Segure...";
    public string textoSolte = "Solte...";
    public string textoErro = "Respiração incorreta!";
    public string textoFim = "Parabéns!";

    private enum Fase { Fase1, Fase2, Fase3, Completo }
    private Fase faseAtual = Fase.Fase1;

    private bool isHolding = false;
    private bool faseConcluida = false;
    private bool mostrouErro = false;
    private float tempoNaFase = 0f;

    void Start()
    {
        AtualizarTextoInstrucao(textoRespire);
        AtualizarTimers();
        AtivarSpritesDaFase(faseAtual);
    }

    void Update()
    {
        if (faseAtual == Fase.Completo) return;

        isHolding = Input.GetMouseButton(0);

        float delta = (isHolding ? riseSpeed : -fallSpeed) * Time.deltaTime;
        float newY = Mathf.Clamp(respirationBar.anchoredPosition.y + delta, minHeight, maxHeight);
        respirationBar.anchoredPosition = new Vector2(respirationBar.anchoredPosition.x, newY);

        VerificarFase();
        AtualizarTimers();
    }

    void VerificarFase()
    {
        float barY = respirationBar.anchoredPosition.y;
        RectTransform areaAtual = GetAreaAtual();

        if (DentroDaArea(barY, areaAtual))
        {
            tempoNaFase += Time.deltaTime;

            if (!faseConcluida)
            {
                AtualizarTextoInstrucao(textoSegure);
                mostrouErro = false; // reset do erro ao entrar novamente
            }

            if (tempoNaFase >= tempoNecessarioParaCompletar && !faseConcluida)
            {
                faseConcluida = true;
                AtualizarTextoInstrucao(textoSolte);
            }
        }
        else
        {
            if (!faseConcluida && tempoNaFase > 0f)
            {
                if (!mostrouErro)
                {
                    Derrota();
                    mostrouErro = true;
                }
                return;
            }

            if (!faseConcluida && !mostrouErro)
                AtualizarTextoInstrucao(textoRespire);
        }

        if (faseConcluida && respirationBar.anchoredPosition.y <= minHeight + 1f)
        {
            AvancarFase();
        }
    }

    void AvancarFase()
    {
        faseAtual++;
        if (faseAtual == Fase.Completo)
        {
            AtualizarTextoInstrucao(textoFim);
            DesativarTodosSprites();
            AtualizarTimers();

            // Aguarda 1 segundo antes de trocar de cena
            Invoke("CarregarCenaFinal", 1f);
            return;
        }

        faseConcluida = false;
        tempoNaFase = 0f;
        mostrouErro = false;
        AtivarSpritesDaFase(faseAtual);
        AtualizarTextoInstrucao(textoRespire);
        AtualizarTimers();
    }

    void CarregarCenaFinal()
    {
        SceneManager.LoadScene("TelaVitória"); // <-- Substitua pelo nome da sua cena
    }

    void Derrota()
    {
        faseAtual = Fase.Fase1;
        faseConcluida = false;
        tempoNaFase = 0f;
        mostrouErro = true;
        AtivarSpritesDaFase(faseAtual);
        AtualizarTextoInstrucao(textoErro);
        AtualizarTimers();
    }

    bool DentroDaArea(float barY, RectTransform area)
    {
        float minY = area.anchoredPosition.y - area.rect.height / 2;
        float maxY = area.anchoredPosition.y + area.rect.height / 2;
        return barY >= minY && barY <= maxY;
    }

    RectTransform GetAreaAtual()
    {
        return faseAtual switch
        {
            Fase.Fase1 => fase1Area,
            Fase.Fase2 => fase2Area,
            Fase.Fase3 => fase3Area,
            _ => null
        };
    }

    void AtivarSpritesDaFase(Fase fase)
    {
        DesativarTodosSprites();

        switch (fase)
        {
            case Fase.Fase1:
                foreach (var g in fase1Sprites) g.SetActive(true);
                break;
            case Fase.Fase2:
                foreach (var g in fase2Sprites) g.SetActive(true);
                break;
            case Fase.Fase3:
                foreach (var g in fase3Sprites) g.SetActive(true);
                break;
        }
    }

    void DesativarTodosSprites()
    {
        foreach (var g in fase1Sprites) g.SetActive(false);
        foreach (var g in fase2Sprites) g.SetActive(false);
        foreach (var g in fase3Sprites) g.SetActive(false);
    }

    void AtualizarTextoInstrucao(string texto)
    {
        if (instructionText != null)
            instructionText.text = texto;
        if (instructionText2 != null)
            instructionText2.text = texto;
        if (instructionText3 != null)
            instructionText3.text = texto;
    }

    void AtualizarTimers()
    {
        string tempo = Mathf.Max(tempoNecessarioParaCompletar - tempoNaFase, 0f).ToString("F1") + "s";

        if (fase1TimerText != null)
            fase1TimerText.text = (faseAtual == Fase.Fase1) ? tempo : "";
        if (fase2TimerText != null)
            fase2TimerText.text = (faseAtual == Fase.Fase2) ? tempo : "";
        if (fase3TimerText != null)
            fase3TimerText.text = (faseAtual == Fase.Fase3) ? tempo : "";
    }
}
