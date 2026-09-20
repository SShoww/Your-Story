namespace BePalV2.Gameplay;

public sealed class EconomyManager
{
    public const int StartingGold = 150;
    public const int DailySubsidyAmount = 100;

    public int Gold { get; private set; } = StartingGold;
    public int Debt { get; private set; } = 0;

    public bool HasDebt => Debt > 0;

    public void AddGold(int amount)
    {
        if (amount > 0)
        {
            Gold += amount;
        }
    }

    public bool CanAfford(int amount) => amount >= 0 && Gold >= amount;

    public bool SpendGold(int amount)
    {
        if (!CanAfford(amount)) return false;
        Gold -= amount;
        return true;
    }

    public void ReceiveDailySubsidy()
    {
        AddGold(DailySubsidyAmount);
    }

    public void IssueEmergencyLoan(int amount = 500)
    {
        if (amount <= 0) return;
        Debt += amount;
        Gold += amount;
    }

    public int CompoundDailyInterest(float rate = 0.20f)
    {
        if (Debt <= 0) return 0;
        int interest = (int)Math.Ceiling(Debt * rate);
        Debt += interest;
        return interest;
    }

    public bool RepayDebt(int amount)
    {
        if (amount <= 0 || Debt <= 0 || Gold < amount) return false;
        int pay = Math.Min(amount, Debt);
        Gold -= pay;
        Debt -= pay;
        return true;
    }
}
