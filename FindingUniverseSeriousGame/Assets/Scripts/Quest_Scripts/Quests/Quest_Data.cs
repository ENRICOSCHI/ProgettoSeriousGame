    /// <summary>
    /// Struct che rappresenta lo stato di una missione, con i campi:
    /// - booleano isStarted: indica se la missione è stata iniziata
    /// - booleano isCompleted: indica se la missione è stata completata
    /// - intero amountProgress: rappresenta il progresso (in intero, se applicabile) attuale della missione 
    /// </summary>
    /// 
    [System.Serializable]
    public class QuestData
    {
        public bool isStarted = false;
        public bool isCompleted = false;
        public bool isDiscovered = false; //utile per il codex, per sapere se una missione è stata scoperta o meno
        public int amountProgress = 0;


        public QuestData(bool isDiscovered)
        {
            this.isDiscovered = isDiscovered;
        }

        public QuestData(bool isStarted, bool isCompleted)
        {
            this.isStarted = isStarted;
            this.isCompleted = isCompleted;
            this.isDiscovered = true;
        }

        public QuestData(bool isStarted, bool isCompleted, int amountProgress)
        {
            this.isStarted = isStarted;
            this.isCompleted = isCompleted;
            this.amountProgress = amountProgress;
        }
    }