namespace Common.Classes.General
{
    public enum CorrelationTypes
    {
        Pearson,

        /* Equals to the Pearson correlation between the rank values of those two variables; 
           while Pearson's correlation assesses linear relationships, Spearman's correlation assesses monotonic relationships (whether linear or not) */
        Spearman
    }
}