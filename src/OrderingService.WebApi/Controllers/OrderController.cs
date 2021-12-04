using Dawn;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderingService.Commands.CreateOrder;
using OrderingService.Commands.GetOrder;
using OrderingService.Commands.UpdateOrder;
using OrderingService.Domain.Contracts;
using OrderingService.Domain.Orders;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace OrderingService.WebApi.Controllers
{
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ILogger<OrderController> _logger;
        private readonly ITranscoder _transcoder;
        private readonly ICommandHandler<CreateOrderCommand, string> _createOrder;
        private readonly ICommandHandler<GetOrderCommand, Order> _getOrder;
        private readonly ICommandHandler<UpdateOrderCommand, Order> _updateOrder;

        public OrderController(
            ILogger<OrderController> logger,
            ITranscoder transcoder,
            ICommandHandler<CreateOrderCommand, string> createOrder,
            ICommandHandler<GetOrderCommand, Order> getOrder,
            ICommandHandler<UpdateOrderCommand, Order> updateOrder
            )
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _transcoder = Guard.Argument(transcoder, nameof(transcoder)).NotNull().Value;
            _createOrder = Guard.Argument(createOrder, nameof(createOrder)).NotNull().Value;
            _getOrder = Guard.Argument(getOrder, nameof(getOrder)).NotNull().Value;
            _updateOrder = Guard.Argument(updateOrder, nameof(updateOrder)).NotNull().Value;
        }

        [HttpPost]
        public async Task<IActionResult> Order()
        {
            _logger.LogTrace("Entering POST endpoint {endpoint} for controller {OrderController}", nameof(Order), typeof(OrderController));
            CreateOrderCommand createOrderCommand = await DecodePostRequest();
            _logger.LogTrace("{commandType} value is {command}", typeof(CreateOrderCommand), createOrderCommand);
            string newId = await _createOrder.Handle(createOrderCommand);
            return Ok(newId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Order([FromRoute] string id)
        {
            GetOrderCommand getOrderCommand = new GetOrderCommand
            {
                Id = id
            };
            Order order = await _getOrder.Handle(getOrderCommand);
            await EncodeResponse(order);
            return new EmptyResult();
        }

        [HttpPut]
        public async Task<IActionResult> OrderUpdate()
        {
            UpdateOrderCommand updateOrderCommand = await DecodePutRequest();
            Order order = await _updateOrder.Handle(updateOrderCommand);
            await EncodeResponse(order);
            return new EmptyResult();
        }

        private async Task EncodeResponse(Order order)
        {
            byte[] encodedOrder = await _transcoder.Encode(order, nameof(Domain.Orders.Order));
            HttpContext.Response.ContentType = "application/json";
            await HttpContext.Response.Body.WriteAsync(encodedOrder, 0, encodedOrder.Length);
        }

        private async Task<CreateOrderCommand> DecodePostRequest()
        {
            //Read request body into string
            HttpContext.Request.EnableBuffering();
            Stream requestStream = HttpContext.Request.Body;
            using StreamReader requestStreamReader = new StreamReader(requestStream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            string rawRequest = await requestStreamReader.ReadToEndAsync();
            requestStream.Position = 0;

            //Conver to bytes and send to transcoder
            byte[] item = Encoding.UTF8.GetBytes(rawRequest);
            return await _transcoder.Decode(item, nameof(CreateOrderCommand)) as CreateOrderCommand;
        }

        private async Task<UpdateOrderCommand> DecodePutRequest()
        {
            //Read request body into string
            HttpContext.Request.EnableBuffering();
            Stream requestStream = HttpContext.Request.Body;
            using StreamReader requestStreamReader = new StreamReader(requestStream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            string rawRequest = await requestStreamReader.ReadToEndAsync();
            requestStream.Position = 0;

            //Conver to bytes and send to transcoder
            byte[] item = Encoding.UTF8.GetBytes(rawRequest);
            return await _transcoder.Decode(item, nameof(UpdateOrderCommand)) as UpdateOrderCommand;
        }
    }
}
