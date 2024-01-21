using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Web.Controllers;
[Route("api/orders")]
[ApiController]
public class OrdersController(IOrderService orderService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    [HttpPost("payment")]
    public IActionResult BankPayment([FromBody] PaymentRequest request)
    {
        if (request.Method != "Bank")
        {
            return BadRequest("Invalid payment method for this endpoint.");
        }

        Guid customerId = Guid.Parse("B30F65493C8946A79B69D91FE6577EB2");

        OrderInformation orderInformation = _orderService.GetOrderInformation(customerId);

        var invoice = InvoiceGenerator.GenerateInvoice(customerId, orderInformation.OrderId, orderInformation.Sum);

        return File(invoice, "application/pdf", "invoice.pdf");
    }

    [HttpGet("payment-methods")]
    public IActionResult GetPaymentMethods()
    {
        var paymentMethods = new List<PaymentMethod>();

        for (int i = 1; i <= 3; i++)
        {
            paymentMethods.Add(new($"Image-{i}", Enum.GetName(typeof(PaymentName), i).ToString(), $"description-{i}"));
        }

        return Ok(paymentMethods);
    }

    [HttpDelete("cart/{key}")]
    public IActionResult RemoveFromCart([FromRoute] string key)
    {
        Guid customerId = Guid.Parse("B30F65493C8946A79B69D91FE6577EB2");
        try
        {
            return Ok(_orderService.RemoveOrder(customerId, key));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public IActionResult GetPaidAndCancelledOrders()
    {
        return Ok(_orderService.GetPaidAndCancelledOrders());
    }

    [HttpGet("{id}")]
    public IActionResult GetOrder([FromRoute] Guid id)
    {
        try
        {
            return Ok(_orderService.GetOrder(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/detais")]
    public IActionResult GetOrderDetails([FromRoute] Guid id)
    {
        return Ok(_orderService.GetOrderDetails(id));
    }

    [HttpGet("cart")]
    public IActionResult GetCart()
    {
        Guid customerId = Guid.Parse("B30F65493C8946A79B69D91FE6577EB2");

        try
        {
            return Ok(_orderService.GetCart(customerId));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
