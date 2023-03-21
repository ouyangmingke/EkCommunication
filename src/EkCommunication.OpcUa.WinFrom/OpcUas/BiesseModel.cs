namespace OpcUaTool
{
    public class BiesseModel
    {
        public BiesseModel(string biesseModelName)
        {
            BiesseModelName = biesseModelName;
        }

        public readonly string BiesseModelName;

        public int ProGramId { get; set; }
        public string ProGram { get; set; }
    }
}
