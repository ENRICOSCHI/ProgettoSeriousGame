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
                Debug.Log("[UmbraEvento] Proprietà _Exposure trovata con successo!");
            }
            else
            {
                Debug.LogWarning("[UmbraEvento] ATTENZIONE: Il materiale della Skybox NON ha una proprietà _Exposure! Non si oscurerà.");
            }
        }
        else
        {
            Debug.LogWarning("[UmbraEvento] Nessuna Skybox assegnata nei RenderSettings!");
        }
    }

    #region"geometria"
    private void CalcolaParametriCono()
    {
        if (stella == null || pianeta == null) return;

        //calcolo distanza tra stella e pianeta
        float d = Vector3.Distance(stella.position, pianeta.position);

        //calcolo lunghezza del cono d'ombra e del cono di penombra
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
        /*Calcolo vettori di riferimento*/
        // Vettore unitario (lunghezza 1) che definisce la direzione dell'asse dell'ombra (da Stella a Pianeta)
        Vector3 dirStellaPianeta = (pianeta.position - stella.position).normalized;
        // Vettore che va dal centro del pianeta fino alla posizione dell'oggetto/navicella
        Vector3 vettorePianetaPos = posizione - pianeta.position; 

        /*Proizione sull'asse e verifica se è davati o dietro*/
        // Tramite prodotto scalare (Dot) calcolo la proiezione della navicella sull'asse dell'ombra.
        float proiezioneAsse = Vector3.Dot(vettorePianetaPos, dirStellaPianeta);
        // Se la proiezione è <= 0, l'oggetto si trova davanti o a fianco del pianeta rispetto alla stella.
        if (proiezioneAsse <= 0f) return StatoOmbra.Luce;
        
        /*Distanza radiale dall'asse (distanza minima assoluta tra due punti*/
        // Vettore della posizione proiettata esattamente lungo l'asse centrale dell'ombra
        Vector3 componenteAssiale = dirStellaPianeta * proiezioneAsse;
        // Calcolo della distanza perpendicolare (radiale) dell'oggetto dall'asse centrale del cono d'ombra
        float distanzaRadiale = (vettorePianetaPos - componenteAssiale).magnitude;
        
        /*Dimensioni dei coni alla distanza dell'oggetto*/
        // Il cono di Umbra (ombra totale) si restringe man mano che ci si allontana dal pianeta.   
        // A proiezioneAsse = _lunghezzaConoUmbra, il raggio diventa 0 (vertice del cono).
        float raggioUmbraADistanza = raggioPianeta * (1f - proiezioneAsse / _lunghezzaConoUmbra);
        float raggioPenombraADistanza = raggioPianeta * (1f + proiezioneAsse / _lunghezzaConoPenombra);

        /*Classificazione dello stato dell'ombra*/

        // Se l'oggetto supera la lunghezza massima del cono d'ombra allora è in piena luce
        if (proiezioneAsse > _lunghezzaConoUmbra) return StatoOmbra.Luce;

        // se la distanza è minore del raggio del cono d'ombra allora siamo in piena ombra
        if (distanzaRadiale <= raggioUmbraADistanza)
            return StatoOmbra.Umbra;

        //calcolo la soglia di penobra come interpolazione lineare tra il raggio del cono d'ombra e il raggio del cono di penombra, in base allo spessore della penombra
        float sogliaPenombra = Mathf.Lerp(raggioUmbraADistanza, raggioPenombraADistanza, spessorePenombra);
        //se è all'interno della soglia...
        if (distanzaRadiale <= sogliaPenombra)
            return StatoOmbra.Penombra;

        return StatoOmbra.Luce;
    }
    #endregion-

    #region"Transizioni e notifiche"
    private void GestisciTransizioni()
    {
        //se lo stato non è cambiato esco dalla funzione
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

        //disegno cono umbra
        Gizmos.color = new Color(0.05f, 0.05f, 0.4f, 0.5f);
        DisegnaCono(pianeta.position, dir, raggioPianeta, _lunghezzaConoUmbra);

        //disegno cono penumbra
        Gizmos.color = new Color(0.3f, 0.3f, 0.8f, 0.25f);
        DisegnaCono(pianeta.position, dir, raggioPianeta, _lunghezzaConoPenombra * (1f + spessorePenombra), espandi: true);

        //disegno la sfera della stella
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(stella.position, raggioStella * 0.05f);
        //disegno la sfera dei pianeti
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