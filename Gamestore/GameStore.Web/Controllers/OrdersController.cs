using System.Net;
using System.Text;
using GameStore.Application.Dtos;
using GameStore.Application.IServices;
using GameStore.Domain.Entities;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GameStore.Web.Controllers;
[Route("api/orders")]
[ApiController]
public class OrdersController(IOrderService orderService, IHttpClientFactory httpClientFactory) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;

    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("PaymentMicroservice");

    private readonly Guid _customerId = Guid.Parse("3fa85f6457174562b3fc2c963f66afa6");

    private static readonly Dictionary<string, string> PaymentTypes = new()
    {
        { "https://www.pngitem.com/pimgs/m/101-1016890_icon-bank-logo-png-transparent-png.png", "Bank" },
        { "https://a0.anyrgb.com/pngimg/990/1580/ibox-new-payment-terminal-privatbank-rates-chernihiv-selfservice-ukraine-value-cash.png", "IBox terminal" },
        { "https://www.visa.com.au/dam/VCOM/regional/ve/romania/blogs/hero-image/visa-logo-800x450.jpg", "Visa" },
    };

    [HttpPost("payment")]
    public async Task<IActionResult> BankPayment([FromBody] PaymentRequest request)
    {
        if (!PaymentTypes.ContainsValue(request.Method))
        {
            return BadRequest("Invalid payment method for this endpoint.");
        }

        OrderInformation orderInformation = await _orderService.GetOrderInformation(_customerId);
        string route;
        object apiObj;
        HttpResponseMessage response;

        if (request.Method == PaymentTypes.First().Value)
        {
            var invoice = InvoiceGenerator.GenerateInvoice(_customerId, orderInformation.OrderId, orderInformation.Sum);

            await _orderService.UpdateOrder(orderInformation.OrderId, OrderStatus.Checkout);

            return File(invoice, "application/pdf", "invoice.pdf");
        }
        else if (request.Method == PaymentTypes.Last().Value)
        {
            route = $"{_httpClient.BaseAddress}/visa";

            if (request.Model == null)
            {
                return BadRequest();
            }

            apiObj = new
            {
                transactionAmount = orderInformation.Sum,
                cardHolderName = request.Model.Holder,
                cardNumber = request.Model.CardNumber,
                expirationMonth = request.Model.MonthExpire,
                expirationYear = request.Model.YearExpire,
                cvv = request.Model.CVV2,
            };
        }
        else
        {
            route = $"{_httpClient.BaseAddress}/ibox";

            apiObj = new
            {
                transactionAmount = orderInformation.Sum,
                accountNumber = _customerId,
                invoiceNumber = orderInformation.OrderId,
            };
        }

        var jsonContent = JsonConvert.SerializeObject(apiObj);

        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        response = await _httpClient.PostAsync(route, content);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return BadRequest(await response.Content.ReadAsStringAsync());
        }

        await _orderService.UpdateOrder(orderInformation.OrderId, OrderStatus.Paid);

        var returnObj = new
        {
            UserId = _customerId,
            OrderId = orderInformation.OrderId,
            PaymentDate = orderInformation.CreationDate,
            Sum = orderInformation.Sum,
        };

        return Ok(returnObj);
    }

    [HttpGet("payment-methods")]
    public IActionResult GetPaymentMethods()
    {
        var paymentMethods = new List<PaymentMethod>();

        foreach (var type in PaymentTypes)
        {
            paymentMethods.Add(new(type.Key, type.Value, "Descr"));
        }

        object returnObj = new
        {
            PaymentMethods = paymentMethods,
        };
        return Ok(returnObj);
    }

    [HttpDelete("cart/{key}")]
    public async Task<IActionResult> RemoveFromCart([FromRoute] string key)
    {
        try
        {
            return Ok(await _orderService.RemoveOrder(_customerId, key));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetPaidAndCancelledOrders()
    {
        return Ok(await _orderService.GetPaidAndCancelledOrders());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder([FromRoute] Guid id)
    {
        try
        {
            return Ok(await _orderService.GetOrder(id));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetOrderDetails([FromRoute] Guid id)
    {
        return Ok(await _orderService.GetOrderDetails(id));
    }

    [HttpGet("cart")]
    public async Task<IActionResult> GetCart()
    {
        try
        {
            return Ok(await _orderService.GetCart(_customerId));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetOrdersHistory([FromQuery] string? start, [FromQuery] string? end)
    {
        try
        {
            DateTime? startDate = null;
            DateTime? endDate = null;
            if (!start.IsNullOrEmpty())
            {
                int endIndex = start.IndexOf('G') - 1;

                startDate = DateTime.Parse(start[..endIndex]);
            }

            if (!end.IsNullOrEmpty())
            {
                int endIndex = end.IndexOf('G') - 1;

                endDate = DateTime.Parse(end[..endIndex]);
            }

            var orderHistory = await _orderService.GetOrderHistory(startDate, endDate);

            return Ok(orderHistory);
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
