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

    [Header("Debug")]
    [SerializeField] private bool mostraGizmos = true;

    private StatoOmbra _statoCorrente = StatoOmbra.Luce;
    private StatoOmbra _statoPrecedente = StatoOmbra.Luce;

    // Flag per evitare di ripetere la descrizione ogni volta che si entra in umbra
    private bool _descrizioneData = false;

    // Parametri dinamici del cono d'ombra, calcolati in base alla posizione di stella e pianeta
    private float _lunghezzaConoUmbra;
    private float _lunghezzaConoPenombra;

    private void Awake() => CalcolaParametriCono();
    private void OnValidate() => CalcolaParametriCono();

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
                NotificaPersonalizzata(notificaMessaggio[0]);
                Debug.Log("[UmbraEvento] Entrata in PENOMBRA — eclissi parziale.");
                break;

            case StatoOmbra.Umbra:
                NotificaPersonalizzata(notificaMessaggio[1]);
                if (!_descrizioneData)
                {
                    ActiveSubtitlesWithAudio();
                    _descrizioneData = true;
                }
                Debug.Log("[UmbraEvento] Entrata in UMBRA — buio totale.");
                break;

            case StatoOmbra.Luce:
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
}