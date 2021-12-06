using MassTransit;
using Microsoft.AspNetCore.Mvc;
using SagaOrchPattern.Messages.Order.Event;
using SagaOrchPattern.Order.Infra;
using SagaOrchPattern.Order.Models;
using System;
using System.Threading.Tasks;

namespace SagaOrchPattern.Order.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {        
        private readonly IOrderPriceDataAccess _orderDataAccess;
        private readonly IBus _bus;

        public OrderController(IOrderPriceDataAccess orderDataAccess, IBus bus)
        {
            _bus = bus;
            _orderDataAccess = orderDataAccess;
        }
        [HttpPost]
        [Route("createorder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderPrice orderModel)
        {
            _orderDataAccess.SaveOrder(orderModel);

            await _bus.Publish<IOrderStartedEvent>(new
            {
                OrderId = orderModel.OrderId
                ,
                PaymentCardNumber = orderModel.PaymentCardNumber
                ,
                ProductName = orderModel.ProductName
                ,
                Cancel = false
            });


            return Ok("Success");
        }
    }
}
