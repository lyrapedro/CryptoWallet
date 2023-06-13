using Microsoft.AspNetCore.Mvc;

namespace CryptoWallet.Controllers;

[ApiController]
[Route("/api/wallets")]
public class WalletsController : ControllerBase
{
    private static List<CryptoWallet> wallets = new List<CryptoWallet>();
    private static int nextId = 1;

    [HttpPost]
    public IActionResult Post([FromBody] CryptoWallet wallet)
    {
        wallet.Id = nextId++;
        wallets.Add(wallet);
        return Created("Get", wallet.Id);
    }

    [HttpGet]
    public IActionResult Get(int id)
    {
        var wallet = wallets.FirstOrDefault(w => w.Id == id);
        if (wallet == null)
            return NotFound();

        return Ok(wallet);
    }

    [HttpPatch]
    public IActionResult Patch(int id, [FromBody] CryptoWallet updatedWallet)
    {
        var wallet = wallets.FirstOrDefault(w => w.Id == id);
        if (wallet == null)
            return NotFound();

        wallet.OwnerName = updatedWallet.OwnerName;
        wallet.Balance = updatedWallet.Balance;

        return Ok(wallet);
    }

    [HttpDelete]
    public IActionResult Delete(int id)
    {
        var wallet = wallets.FirstOrDefault(w => w.Id == id);
        if (wallet == null)
            return NotFound();

        wallets.Remove(wallet);
        return Ok();
    }

    // POST api/wallets/{id}/transactions
    [Route("/{id}/transactions")]
    [HttpPost]
    public IActionResult AddTransaction(int id, [FromBody] BuySellRequest request)
    {
        var wallet = wallets.FirstOrDefault(w => w.Id == id);
        var targetWallet = wallets.FirstOrDefault(t => t.Id == request.TargetId);

        if (wallet == null || targetWallet == null)
            return NotFound();

        var transaction = new Transaction
        {
            Amount = -request.Amount,
            Date = DateTime.Now,
            Description = $"Transfer to wallet {request.TargetId}"
        };

        var targetTransaction = new Transaction
        {
            Amount = request.Amount,
            Date = DateTime.Now,
            Description = $"Transfer from wallet {id}"
        };

        if (wallet.Transactions == null)
            wallet.Transactions = new List<Transaction>();

        if(targetWallet.Transactions == null)
            targetWallet.Transactions = new List<Transaction>();

        wallet.Transactions.Add(transaction);
        targetWallet.Transactions.Add(targetTransaction);
        return Ok(wallet);
    }

    // GET api/wallets/{id}/transactions
    [Route("/{id}/transactions")]
    [HttpGet]
    public IActionResult GetTransactions(int id)
    {
        var wallet = wallets.FirstOrDefault(w => w.Id == id);
        if (wallet == null)
            return NotFound();

        return Ok(wallet.Transactions);
    }

    // POST api/wallets/{id}/buy
    [Route("/{id}/buy")]
    [HttpPost]
    public IActionResult BuyCryptocurrency(int id, [FromBody] BuySellRequest request)
    {
        var wallet = wallets.FirstOrDefault(w => w.Id == id);
        if (wallet == null)
            return NotFound();

        // Simulação da operação de compra
        var amount = request.Amount;

        // Atualizar a carteira com a nova criptomoeda e saldo
        wallet.Balance += amount;

        // Adicionar a transação ao histórico
        var transaction = new Transaction
        {
            Date = DateTime.Now,
            Amount = amount,
            Description = $"Bought {amount} Bitcoins"
        };

        if (wallet.Transactions == null)
            wallet.Transactions = new List<Transaction>();

        wallet.Transactions.Add(transaction);

        return Ok(wallet);
    }

    // POST api/wallets/{id}/sell
    [Route("/{id}/sell")]
    [HttpPost]
    public IActionResult SellCryptocurrency(int id, [FromBody] BuySellRequest request)
    {
        var wallet = wallets.FirstOrDefault(w => w.Id == id);
        if (wallet == null)
            return NotFound();

        // Simulação da operação de venda
        var amount = request.Amount;

        // Verificar se a carteira possui a criptomoeda suficiente para vender
        if (wallet.Balance < amount)
            return BadRequest($"The wallet does not have bitcoin to sell.");

        // Atualizar a carteira com a criptomoeda vendida e saldo
        wallet.Balance -= amount;

        // Adicionar a transação ao histórico
        var transaction = new Transaction
        {
            Date = DateTime.Now,
            Amount = -amount,
            Description = $"Sold {amount} Bitcoins"
        };
        if (wallet.Transactions == null)
            wallet.Transactions = new List<Transaction>();

        wallet.Transactions.Add(transaction);

        return Ok(wallet);
    }
}
