using System.Collections;
using UnityEngine;

public class UmbraEvento : Eventi
{
    public enum StatoOmbra { Luce, Penombra, Umbra }

    [Header("Riferimenti Scena")]
    [SerializeField] private Transform stella;
    [SerializeField] private Transform navicella;
    [SerializeField] private Transform pianeta;

    [Header("Geometria Cono d'Ombra")]
    [SerializeField] private float raggioPianeta = 10f;
    [SerializeField] private float raggioStella = 1371f;
    [SerializeField] private float spessorePenombra = 0.15f;

    [Header("Impostazioni luce")]
    [SerializeField] private Light soleDirectionalLight;
    [SerializeField] private float velocitaTransizione = 2f; // Secondi per completare la transizione

    [Range(0f, 1f)]
    [SerializeField] private float moltiplicatoreOriginaleSole = 1f; // Intensità originale della luce del sole, per poterla ripristinare
    [Tooltip("Mettilo a 0 per avere il nero assoluto e spegnere le stelle!")]
    [Range(0f, 1f)]
    [SerializeField] private float moltiplicatoreUmbra = 0f; // Intensità della luce durante l'ombra
    [Range(0f, 1f)]
    [SerializeField] private float moltiplicatorePenumbra = 0.5f;

    private float intensitaOriginaleSole;
    private float intensitaOriginaleAmbiente;
    private Coroutine coroutineLuce;
    
    private Material skyboxMaterial;
    private float esposizioneOriginaleSkybox = 1f;

    [Header("Sound Effects")]
    [SerializeField] AudioClip umbraSFX;
    [SerializeField] AudioClip penumbraSFX;

    [Header("Debug")]
    [SerializeField] private bool mostraGizmos = true;

    private StatoOmbra _statoCorrente = StatoOmbra.Luce;
    private StatoOmbra _statoPrecedente = StatoOmbra.Luce;

    private float _lunghezzaConoUmbra;
    private float _lunghezzaConoPenombra;

    private void Awake() => CalcolaParametriCono();
    private void OnValidate() => CalcolaParametriCono();

    private void Start()
    {
        // Salvataggio dei parametri di default per l'illuminazione
        if (soleDirectionalLight != null)
            intensitaOriginaleSole = soleDirectionalLight.intensity;

        intensitaOriginaleAmbiente = RenderSettings.ambientIntensity;

        // Gestione skybox
        if (RenderSettings.skybox != null)
        {
            // NESSUN CLONE. Prendiamo il materiale originale
            skyboxMaterial = RenderSettings.skybox;

            if (skyboxMaterial.HasProperty("_Exposure"))
            {
                esposizioneOriginaleSkybox = skyboxMaterial.GetFloat("_Exposure");
                Debug.Log("<color=green>[UmbraEvento]</color> Proprietà _Exposure trovata con successo!");
            }
            else
            {
                Debug.LogWarning("<color=red>[UmbraEvento]</color> ATTENZIONE: Il materiale della Skybox NON ha una proprietà _Exposure! Non si oscurerà.");
            }
        }
        else
        {
            Debug.LogWarning("<color=yellow>[UmbraEvento]</color> Nessuna Skybox assegnata nei RenderSettings!");
        }
    }

    #region"geometria"
    private void CalcolaParametriCono()
    {
        if (stella == null || pianeta == null) return;
        float d = Vector3.Distance(stella.position, pianeta.position);
        _lunghezzaConoUmbra = d * raggioPianeta / Mathf.Max(raggioStella - raggioPianeta, 0.001f);
        _lunghezzaConoPenombra = d * raggioPianeta / Mathf.Max(raggioStella + raggioPianeta, 0.001f);
    }
    #endregion

    private void Update()
    {
        if (stella == null || pianeta == null || navicella == null) return;

        CalcolaParametriCono();
        _statoCorrente = CalcolaStatoOmbra(navicella.position);
        GestisciTransizioni();
        _statoPrecedente = _statoCorrente;
    }

    #region"Classificazione ombra"
    public StatoOmbra CalcolaStatoOmbra(Vector3 posizione)
    {
        Vector3 dirStellaPianeta = (pianeta.position - stella.position).normalized;
        Vector3 vettorePianetaPos = posizione - pianeta.position;

        float proiezioneAsse = Vector3.Dot(vettorePianetaPos, dirStellaPianeta);
        if (proiezioneAsse <= 0f) return StatoOmbra.Luce;

        Vector3 componenteAssiale = dirStellaPianeta * proiezioneAsse;
        float distanzaRadiale = (vettorePianetaPos - componenteAssiale).magnitude;

        float raggioUmbraADistanza = raggioPianeta * (1f - proiezioneAsse / _lunghezzaConoUmbra);
        float raggioPenombraADistanza = raggioPianeta * (1f + proiezioneAsse / _lunghezzaConoPenombra);

        if (proiezioneAsse > _lunghezzaConoUmbra) return StatoOmbra.Luce;

        if (distanzaRadiale <= raggioUmbraADistanza)
            return StatoOmbra.Umbra;

        float sogliaPenombra = Mathf.Lerp(raggioUmbraADistanza, raggioPenombraADistanza, spessorePenombra);
        if (distanzaRadiale <= sogliaPenombra)
            return StatoOmbra.Penombra;

        return StatoOmbra.Luce;
    }
    #endregion

    #region"Transizioni e notifiche"
    private void GestisciTransizioni()
    {
        if (_statoCorrente == _statoPrecedente) return;

        switch (_statoCorrente)
        {
            case StatoOmbra.Penombra:
                CambiaLuminosita(moltiplicatorePenumbra);
                NotificaPersonalizzata(notificaMessaggio[0]);
                
                if (penumbraSFX != null)
                    ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(penumbraSFX, navicella.transform, 1f);
                else
                    Debug.LogWarning("Manca penumbraSFX in UmbraEvento del pianeta: " + gameObject.name);
                break;

            case StatoOmbra.Umbra:
                CambiaLuminosita(moltiplicatoreUmbra);
                NotificaPersonalizzata(notificaMessaggio[1]);
                if (!PersistentSceneData.Instance.isDescriptionUmbraHappened)
                {
                    ActiveSubtitlesWithAudio();
                    UnlockOnCodexMenu();
                    PersistentSceneData.Instance.isDescriptionUmbraHappened = true;
                }

                if (umbraSFX != null)
                    ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(umbraSFX, navicella.transform, 1f);
                else
                    Debug.LogWarning("Manca umbraSFX in UmbraEvento del pianeta: " + gameObject.name);
                break;

            case StatoOmbra.Luce:
                CambiaLuminosita(moltiplicatoreOriginaleSole);
                NotificaPersonalizzata(notificaMessaggio[2]);
                Debug.Log("[UmbraEvento] Ritorno alla luce solare.");
                break;
        }
    }
    #endregion

    public StatoOmbra GetStatoCorrente() => _statoCorrente;

    #region"Gizmo"
    private void OnDrawGizmosSelected()
    {
        if (!mostraGizmos || stella == null || pianeta == null) return;

        CalcolaParametriCono();
        Vector3 dir = (pianeta.position - stella.position).normalized;

        Gizmos.color = new Color(0.05f, 0.05f, 0.4f, 0.5f);
        DisegnaCono(pianeta.position, dir, raggioPianeta, _lunghezzaConoUmbra);

        Gizmos.color = new Color(0.3f, 0.3f, 0.8f, 0.25f);
        DisegnaCono(pianeta.position, dir, raggioPianeta, _lunghezzaConoPenombra * (1f + spessorePenombra), espandi: true);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(stella.position, raggioStella * 0.05f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(pianeta.position, raggioPianeta);
    }

    private void DisegnaCono(Vector3 origine, Vector3 direzione, float raggioBase, float lunghezza, bool espandi = false)
    {
        int segmenti = 16;
        Vector3 apice = origine + direzione * lunghezza;
        Vector3 perp1 = Vector3.Cross(direzione, Vector3.up).normalized;
        if (perp1.sqrMagnitude < 0.001f)
            perp1 = Vector3.Cross(direzione, Vector3.right).normalized;
        Vector3 perp2 = Vector3.Cross(direzione, perp1).normalized;

        Vector3 prevPoint = Vector3.zero;
        for (int i = 0; i <= segmenti; i++)
        {
            float angolo = (i / (float)segmenti) * Mathf.PI * 2f;
            float r = espandi ? raggioBase * 1.4f : raggioBase;
            Vector3 punto = origine + (perp1 * Mathf.Cos(angolo) + perp2 * Mathf.Sin(angolo)) * r;

            if (i > 0) Gizmos.DrawLine(prevPoint, punto);
            Gizmos.DrawLine(punto, apice);
            prevPoint = punto;
        }
    }
    #endregion

    #region Funzioni per il cambio illuminazione
    private void CambiaLuminosita(float targetIntensita)
    {
        if (coroutineLuce != null)
            StopCoroutine(coroutineLuce);

        coroutineLuce = StartCoroutine(TransizioneLuceRoutine(targetIntensita));
    }

    private IEnumerator TransizioneLuceRoutine(float moltiplicatore)
    {
        if (soleDirectionalLight == null) yield break;

        float targetSole = intensitaOriginaleSole * moltiplicatore;
        float targetAmbiente = intensitaOriginaleAmbiente * moltiplicatore;
        float targetEsposizione = esposizioneOriginaleSkybox * moltiplicatore;

        float startSole = soleDirectionalLight.intensity;
        float startAmbiente = RenderSettings.ambientIntensity;
        float startEsposizione = 1f;

        if (skyboxMaterial != null && skyboxMaterial.HasProperty("_Exposure"))
            startEsposizione = skyboxMaterial.GetFloat("_Exposure");

        float elapsed = 0f;

        while (elapsed < velocitaTransizione)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / velocitaTransizione;

            soleDirectionalLight.intensity = Mathf.Lerp(startSole, targetSole, t);
            RenderSettings.ambientIntensity = Mathf.Lerp(startAmbiente, targetAmbiente, t);
            
            if (skyboxMaterial != null && skyboxMaterial.HasProperty("_Exposure"))
                skyboxMaterial.SetFloat("_Exposure", Mathf.Lerp(startEsposizione, targetEsposizione, t));
                
            yield return null;
        }

        soleDirectionalLight.intensity = targetSole;
        RenderSettings.ambientIntensity = targetAmbiente;

        if (skyboxMaterial != null && skyboxMaterial.HasProperty("_Exposure"))
            skyboxMaterial.SetFloat("_Exposure", targetEsposizione);
    }

    private void OnDestroy()
    {
        // Siccome stiamo modificando il materiale originale, dobbiamo rimetterlo a posto!
        // Altrimenti la skybox rimarrà nera anche nell'Editor dopo aver chiuso il gioco.
        if (skyboxMaterial != null && skyboxMaterial.HasProperty("_Exposure"))
        {
            skyboxMaterial.SetFloat("_Exposure", esposizioneOriginaleSkybox);
        }
    }
    #endregion
}