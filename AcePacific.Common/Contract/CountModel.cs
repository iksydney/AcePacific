namespace AcePacific.Common.Contract
{
    public class CountModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int Total { get; set; }
        public decimal Info { get; set; }
    }
}
