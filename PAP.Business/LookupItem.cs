namespace PAP.Business
{
    /// <summary>
    /// Id/text pair used to fill dropdown lists.
    /// </summary>
    public class LookupItem
    {
        public LookupItem(int id, string text)
        {
            Id = id;
            Text = text;
        }

        public int Id { get; private set; }
        public string Text { get; private set; }
    }
}
