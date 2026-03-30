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
        public int amountProgress = 0;

        public QuestData() { }

        public QuestData(bool isStarted, bool isCompleted)
        {
            this.isStarted = isStarted;
            this.isCompleted = isCompleted;
        }

        public QuestData(bool isStarted, bool isCompleted, int amountProgress)
        {
            this.isStarted = isStarted;
            this.isCompleted = isCompleted;
            this.amountProgress = amountProgress;
        }
    }