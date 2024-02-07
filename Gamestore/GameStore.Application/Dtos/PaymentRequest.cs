using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.Dtos;

public class PaymentRequest
{
    [Required]
    public string Method { get; set; }

    public RequestModel? Model { get; set; }
}
