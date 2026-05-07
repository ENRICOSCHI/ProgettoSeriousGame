using System.Collections;
using UnityEditor.ShaderGraph.Internal;
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
    [Range(0f, 1f)]
    [SerializeField] private float moltiplicatoreUmbra = 0.05f; // Intensità della luce durante l'ombra
    [Range(0f, 1f)]
    [SerializeField] private float moltiplicatorePenumbra = 0.5f;
    
    private float intensitaOriginaleSole;
    private float intensitaOriginaleAmbiente;
    private Coroutine coroutineLuce;
    private Material skyboxMaterial;
    private Color coloreOriginaleSkybox;

    
    [Header("Sound Effects")]
    [SerializeField] AudioClip umbraSFX;
    [SerializeField] AudioClip penumbraSFX;

    [Header("Debug")]
    [SerializeField] private bool mostraGizmos = true;

    private StatoOmbra _statoCorrente = StatoOmbra.Luce;
    private StatoOmbra _statoPrecedente = StatoOmbra.Luce;

    // Parametri dinamici del cono d'ombra, calcolati in base alla posizione di stella e pianeta
    private float _lunghezzaConoUmbra;
    private float _lunghezzaConoPenombra;

    private void Awake() => CalcolaParametriCono();
    private void OnValidate() => CalcolaParametriCono();

    private void Start()
    {
        // Salvataggio dei parametri di default per l'illuminazione
        if(soleDirectionalLight != null)
            intensitaOriginaleSole = soleDirectionalLight.intensity;
            
        intensitaOriginaleAmbiente = RenderSettings.ambientIntensity;  

        // Gestione skybox
        if (RenderSettings.skybox != null)
        {
            // 1. Creiamo una copia esatta del materiale originale
            skyboxMaterial = new Material(RenderSettings.skybox);
            
            // 2. Diciamo a Unity di usare questa copia nella scena al posto dell'originale
            RenderSettings.skybox = skyboxMaterial;
            
            // 3. CONTROLLO DEBUG: Scopriamo se trova davvero la proprietà!
            if (skyboxMaterial.HasProperty("_Tint"))
            {
                coloreOriginaleSkybox = skyboxMaterial.GetColor("_Tint");
                Debug.Log("<color=green>[UmbraEvento]</color> Proprietà _Tint trovata con successo!");
            }
            else
            {
                Debug.LogWarning("<color=red>[UmbraEvento]</color> ATTENZIONE: Il materiale della Skybox NON ha una proprietà _Tint! Non si oscurerà.");
            }
        }
        else
        {
            Debug.LogWarning("<color=yellow>[UmbraEvento]</color> Nessuna Skybox assegnata nei RenderSettings!");
        }
    }

    #region"geometria"

    /// <summary>
    /// Calcola la lunghezza dei coni d'ombra (umbra e penombra) basandosi sulla distanza tra stella e pianeta e sui loro raggi.
    /// </summary>
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
        // Gestisce le transizioni di stato e le notifiche solo quando c'è un cambiamento
        GestisciTransizioni();
        _statoPrecedente = _statoCorrente;
    }

    #region"Classificazione ombra"

    /// <summary>
    /// Determina se la navicella si trova in luce, penombra o umbra rispetto alla stella e al pianeta, basandosi sulla posizione della navicella e sulla geometria dei coni d'ombra.
    /// </summary>
    /// <param name="posizione"></param>
    /// <returns></returns>
    public StatoOmbra CalcolaStatoOmbra(Vector3 posizione)
    {
        //Normalizza il vettore dalla stella al pianeta per ottenere la direzione del cono d'ombra
        Vector3 dirStellaPianeta = (pianeta.position - stella.position).normalized;
        //Calcola il vettore dalla stella alla navicella, proiettato sulla direzione del cono d'ombra per determinare la posizione lungo l'asse del cono 
        Vector3 vettorePianetaPos = posizione - pianeta.position;

        //Se la proiezione della navicella sull'asse del cono è negativa, significa che è davanti al pianeta rispetto alla stella, quindi è in luce
        float proiezioneAsse = Vector3.Dot(vettorePianetaPos, dirStellaPianeta);
        if (proiezioneAsse <= 0f) return StatoOmbra.Luce;

        //Calcola la distanza radiale della navicella dall'asse del cono d'ombra, che è fondamentale per determinare se è in umbra o penombra
        Vector3 componenteAssiale = dirStellaPianeta * proiezioneAsse;
        float distanzaRadiale = (vettorePianetaPos - componenteAssiale).magnitude;

        //Calcola i raggi dell'umbra e della penombra alla distanza della navicella lungo l'asse del cono, utilizzando la geometria simile dei triangoli formati dalla stella, pianeta e navicella
        float raggioUmbraADistanza = raggioPianeta * (1f - proiezioneAsse / _lunghezzaConoUmbra);
        float raggioPenombraADistanza = raggioPianeta * (1f + proiezioneAsse / _lunghezzaConoPenombra);

        if (proiezioneAsse > _lunghezzaConoUmbra) return StatoOmbra.Luce;

        if (distanzaRadiale <= raggioUmbraADistanza)
            return StatoOmbra.Umbra;

        // La soglia per la penombra è interpolata tra i raggi dell'umbra e della penombra, modulata dallo spessore definito dall'utente, per creare una transizione più graduale tra i due stati
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

                CambiaLuminosita(moltiplicatorePenumbra);  //Transizione di luce

                NotificaPersonalizzata(notificaMessaggio[0]);
                
                if (penumbraSFX != null)
                    ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(penumbraSFX, navicella.transform, 1f);
                else
                    Debug.LogWarning("Manca penumbraSFX in UmbraEvento del pianeta: " + gameObject.name);

                break;

            case StatoOmbra.Umbra:

                CambiaLuminosita(moltiplicatoreUmbra);  //Transizione di luce

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

                CambiaLuminosita(moltiplicatoreOriginaleSole);  //Transizione di luce

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

    private void DisegnaCono(Vector3 origine, Vector3 direzione, float raggioBase,float lunghezza, bool espandi = false)                     
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
        if(coroutineLuce != null)
            StopCoroutine(coroutineLuce);

        coroutineLuce = StartCoroutine(TransizioneLuceRoutine(targetIntensita));
    }

    private IEnumerator TransizioneLuceRoutine(float moltiplicatore)
    {
        if (soleDirectionalLight == null) yield break;

        // Calcoliamo quali sono i bersagli finali esatti per questa transizione
        float targetSole = intensitaOriginaleSole * moltiplicatore;
        float targetAmbiente = intensitaOriginaleAmbiente * moltiplicatore;

        // Moltiplicando un colore per un moltiplicatore, Unity lo scurisce verso il nero
        Color targetColoreSkybox = coloreOriginaleSkybox * moltiplicatore;
        Color startSkyboxColor = Color.white;
        if (skyboxMaterial != null && skyboxMaterial.HasProperty("_Tint"))
            startSkyboxColor = skyboxMaterial.GetColor("_Tint");
        
        float startSole = soleDirectionalLight.intensity;
        float startAmbiente = RenderSettings.ambientIntensity;
        float elapsed = 0f;

        while (elapsed < velocitaTransizione)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / velocitaTransizione;

            // Transizione di entrambi verso i propri target calcolati
            soleDirectionalLight.intensity = Mathf.Lerp(startSole, targetSole, t);
            RenderSettings.ambientIntensity = Mathf.Lerp(startAmbiente, targetAmbiente, t);
            if (skyboxMaterial != null && skyboxMaterial.HasProperty("_Tint"))
                skyboxMaterial.SetColor("_Tint", Color.Lerp(startSkyboxColor, targetColoreSkybox, t));
            yield return null;
        }

        // Assicuriamoci che i valori finali siano perfetti
        soleDirectionalLight.intensity = targetSole;
        RenderSettings.ambientIntensity = targetAmbiente;

        if(skyboxMaterial != null && skyboxMaterial.HasProperty("_Tint"))
            skyboxMaterial.SetColor("_Tint", targetColoreSkybox);
    }

    private void OnDestroy()
    {
        // Distrugge la copia della Skybox liberando la memoria quando chiudi o cambi scena
        if (skyboxMaterial != null)
        {
            Destroy(skyboxMaterial);
        }
    }
    #endregion
}