namespace GameStore.Application.Dtos;

public class RequestModel
{
    public string Holder { get; set; }

    public string CardNumber { get; set; }

    public byte MonthExpire { get; set; }

    public int YearExpire { get; set; }

    public short CVV2 { get; set; }
}
