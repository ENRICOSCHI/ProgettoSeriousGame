using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class InGameMenuController : MonoBehaviour
{
    #region Inizializzazione variabili

    [Header("Riferimenti")]
    private GameObject menuContainer;  // Riferimento al container del menu, cercato in Awake se non assegnato manualmente
    private bool isMenuOpen = false;  // Stato del menu

    [Tooltip("Tasto per aprire/chiudere il menu")]
    [SerializeField] private KeyCode menuKeyCode = KeyCode.Escape;  // Tasto da settare per aprire / chiudere il menu
    [SerializeField] AudioClip closeMenuSFX; // sfx chiusura menu
    [SerializeField] AudioClip openMenuSFX; // sfx apertura menu

    // Riferimento a MenuAethetics per aggiornare l'estetica dei bottoni in base al pannello attivo
    private MenuAsthetics menuAsthetics; 



    [Header("Gestione Pannelli")]  //Sezione di inizializzazione dei pannelli del menu
    [Tooltip("Inserire qui i pannelli nell'ordine: 0 = Minimappa, 1 = Missioni, 2 = Codex, 3 = Opzioni")]
    [SerializeField] private GameObject[] menuPanels;  // Array di pannelli del menu, da assegnare in Inspector
    [SerializeField] private KeyCode rightPanelKey = KeyCode.D;  // Tasto per scorrere a destra i pannelli
    [SerializeField] private KeyCode leftPanelKey = KeyCode.A;  // Tasto per scorrere a sinistra i pannelli

    private int currentPanelIndex = 0;  // Indice del pannello attualmente attivo, inizialmente 0 (Minimappa)


    [Header("Gestione UI di base")]
    [Tooltip("Trascinare qui l'oggetto padre della UI di gioco")]
    [SerializeField] private GameObject inGameUI;  // Riferimento alla UI di gioco

    [Header("Sound Effects")]
    [SerializeField] AudioClip mapSFX;
    [SerializeField] AudioClip scorrimentoMenuCategorySFX;

    #endregion

    void Awake()  // Configurazione iniziale di sicurezza per il riferimento al container del menu
    {
        Transform contaierTransform = transform.Find("MenuContainer");  // Cerca un figlio chiamato "MenuContainer" per assegnarlo se non è stato assegnato manualmente

        if (contaierTransform != null)
        {
            menuContainer = contaierTransform.gameObject;  // Assegna il GameObject del container se trovato
        }
        else
        {
            Debug.LogWarning("MenuContainer non trovato come figlio di " + gameObject.name + ". Assicurati di assegnarlo manualmente o di avere un figlio chiamato 'MenuContainer'.");
        }


        // Cerca un componente MenuAsthetics sullo stesso GameObject per poter aggiornare l'estetica dei bottoni in base al pannello attivo
        menuAsthetics = GetComponent<MenuAsthetics>();

        if(menuAsthetics == null)  // Controllo di sicurezza
        {
            Debug.LogWarning("MenuAsthetics non trovato su " + gameObject.name + ". Assicurati di aggiungere un componente MenuAsthetics se vuoi aggiornare l'estetica dei bottoni in base al pannello attivo.");
        }

    }


    void Start()  // Configurazione iniziale di sicurezza
    {
        menuContainer.SetActive(false);  // Assicura che il menu sia chiuso all'inizio
        isMenuOpen = false;  // Setting di sicurezza sul bool di apertura del menu
        Time.timeScale = 1f;  // Assicura che il tempo sia normale all'inizio
    }


    void Update()
    {
        if (Input.GetKeyDown(menuKeyCode))  //Espandibile se vogliamo aggiungere altri tasti per il controllo del menu
        {
            if (isMenuOpen)
            {
                ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(closeMenuSFX, MovimentoNavicella.GetNavicellaTransform(), 1f); // Suono di chiusura menu
                ResumeGame();  // Se il menu è aperto, chiudilo
            }
            else
            {
                ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(openMenuSFX, MovimentoNavicella.GetNavicellaTransform(), 1f);   // Suono di apertura menu
                PauseGame();  // Se il menu è chiuso, aprilo
            }
        }


        if(isMenuOpen)  // Se il menu è aperto, controlla l'input per cambiare pannello (A e D per scorrere i pannelli)
        {
            if (Input.GetKeyDown(rightPanelKey))
            {
                SlideMenuPanel(1);
            }

            else if (Input.GetKeyDown(leftPanelKey))
            {
                SlideMenuPanel(-1);
            }
        }
    }





    #region Metodi per apertura / chiusura del menu 
    public void PauseGame()
    {
        menuContainer.SetActive(true);  // Mostra il menu
        Time.timeScale = 0f;  // Ferma il tempo di gioco
        isMenuOpen = true;  // Aggiorna lo stato del menu


        if(inGameUI != null)  // Se il riferimento alla UI di gioco è stato assegnato, nascondila quando si apre il menu
        {
            inGameUI.SetActive(false);
        }


        // Sblocca il cursore per poter cliccare sui bottoni (dobbiamo ancora lockare il cursore in game)
        /*Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;*/

        currentPanelIndex = 0;  // Resetta l'indice del pannello attivo al primo pannello (Minimappa) ogni volta che si apre il menu

        CheckPanelSFX(currentPanelIndex);
        ChangeMenuPanel(currentPanelIndex);  // Aggiorna la visualizzazione dei pannelli in base all'indice
    }

    public void ResumeGame()
    {
        menuContainer.SetActive(false);  // Nasconde il menu
        Time.timeScale = 1f;  // Riprende il tempo di gioco
        isMenuOpen = false;  // Aggiorna lo stato del menu


        if(inGameUI != null)  // Se il riferimento alla UI di gioco è stato assegnato, nascondila quando si apre il menu
        {
            inGameUI.SetActive(true);
        }

        // Locka il cursore per non farlo uscire dalla finestra di gioco (dobbiamo ancora lockare il cursore in game)
        /*Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;*/
    }
    #endregion





    #region Metodi per il controllo delle schede (panel) del menu
    /// <summary>
    /// Accende il pannello corrispondente all'indice e spegne tutti gli altri pannelli. Utilizzato per i bottoni del menu.
    /// </summary>
    public void SlideMenuPanel(int panelIndex)
    {
        currentPanelIndex += panelIndex;  // Aggiorna l'indice del pannello attivo in base al parametro passato (1 per avanti, -1 per indietro)

        // Logica a Loop, se supera il limite destro, si torna all'inizio e viceversa
        if(currentPanelIndex >= menuPanels.Length)
        {
            currentPanelIndex = 0;  // Se supera il limite destro, torna al primo pannello
        }
        else if(currentPanelIndex < 0)
        {
            currentPanelIndex = menuPanels.Length - 1;  // Se supera il limite sinistro, torna all'ultimo pannello
        }

        CheckPanelSFX(currentPanelIndex);
        ChangeMenuPanel(currentPanelIndex);  // Aggiorna la visualizzazione dei pannelli in base all'indice
    }


    /// <summary>
    /// Accende il pannello corrispondente all'indice e spegne tutti gli altri pannelli. Utilizzato per i bottoni del menu.
    /// </summary>
    public void ChangeMenuPanel(int panelIndex)
    {
        for (int i = 0; i < menuPanels.Length; i++)
        {
            if (i == panelIndex)
            {
                menuPanels[i].SetActive(true);  // Accende il pannello corrispondente all'indice
            }
            else
            {
                menuPanels[i].SetActive(false);  // Spegne tutti gli altri pannelli
            }
        }


        if(menuAsthetics != null)  // Se il riferimento a MenuAsthetics è stato assegnato, aggiorna l'estetica dei bottoni in base al pannello attivo
        {
            menuAsthetics.UpdateTabVisuals(panelIndex);  // Passa l'indice del pannello attivo a MenuAsthetics per aggiornare i colori dei bottoni
        }
    }

    /// <summary>
    /// Attivo il sound effect in base al panel aperto
    /// </summary>
    /// <param name="currentPanelIndex"></param>
    private void CheckPanelSFX(int currentPanelIndex)
    {
        if (scorrimentoMenuCategorySFX != null)
            ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(scorrimentoMenuCategorySFX, MovimentoNavicella.GetNavicellaTransform(), 1f);
        else
            Debug.LogWarning("Manca scorrimentoMenuCategorySFX in InGameMenuController.cs");

        if (currentPanelIndex == 0)
        {
            if (mapSFX != null)
                ManagerHandler.ManagerInstance.SFXManager.PlaySoundEffect(mapSFX, MovimentoNavicella.GetNavicellaTransform(), 1f);
            else
                Debug.LogWarning("Manca mapSFX in InGameMenuController.cs");
        }
        
    }
    #endregion
}
