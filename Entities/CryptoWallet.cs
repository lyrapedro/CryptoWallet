namespace CryptoWallet;

public class CryptoWallet
{
    public int Id { get; set; }
    public string OwnerName { get; set; }
    public decimal Balance { get; set; }
    public List<Transaction>? Transactions { get; set; }
}
