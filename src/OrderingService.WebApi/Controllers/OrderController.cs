using Dawn;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderingService.Commands.CreateOrder;
using OrderingService.Commands.GetOrder;
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
        private readonly ICommandHandler<CreateOrderCommand, string> _createOrderCommandHandler;
        private readonly ICommandHandler<GetOrderCommand, Order> _getOrderCommandHandler;
        private readonly ITranscoder<CreateOrderCommand> _createOrderCommandTranscoder;
        private readonly ITranscoder<Order> _orderTranscoder;

        public OrderController(
            ILogger<OrderController> logger, 
            ITranscoder<CreateOrderCommand> createOrderCommandTranscoder, 
            ICommandHandler<CreateOrderCommand, string> createOrderCommandHandler,
            ICommandHandler<GetOrderCommand, Order> getOrderCommandHandler,
            ITranscoder<Order> orderTranscoder
            )
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _createOrderCommandTranscoder = Guard.Argument(createOrderCommandTranscoder, nameof(createOrderCommandTranscoder)).NotNull().Value;
            _createOrderCommandHandler = Guard.Argument(createOrderCommandHandler, nameof(createOrderCommandHandler)).NotNull().Value;
            _getOrderCommandHandler = Guard.Argument(getOrderCommandHandler, nameof(getOrderCommandHandler)).NotNull().Value;
            _orderTranscoder = Guard.Argument(orderTranscoder, nameof(orderTranscoder)).NotNull().Value;
        }

        [HttpPost]
        public async Task<IActionResult> Order()
        {
            _logger.LogTrace("Entering POST endpoint {endpoint} for controller {OrderController}", nameof(Order), typeof(OrderController));
            CreateOrderCommand createOrderCommand = await DecodeRequest();
            _logger.LogTrace("{commandType} value is {command}", typeof(CreateOrderCommand), createOrderCommand);
            await _createOrderCommandHandler.Handle(createOrderCommand);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Order([FromRoute] string id)
        {
            GetOrderCommand getOrderCommand = new GetOrderCommand
            {
                Id = id
            };

            Order order = await _getOrderCommandHandler.Handle(getOrderCommand);
            await EncodeResponse(order);
            return Ok();
        }

        private async Task EncodeResponse(Order order)
        {
            byte[] encodedOrder = await _orderTranscoder.Encode(order);
            HttpContext.Response.ContentType = "application/json";
            await HttpContext.Response.Body.WriteAsync(encodedOrder, 0, encodedOrder.Length);
        }

        private async Task<CreateOrderCommand> DecodeRequest()
        {
            //Read request body into string
            HttpContext.Request.EnableBuffering();
            Stream requestStream = HttpContext.Request.Body;
            using StreamReader requestStreamReader = new StreamReader(requestStream, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            string rawRequest = await requestStreamReader.ReadToEndAsync();
            requestStream.Position = 0;

            //Conver to bytes and send to transcoder
            byte[] item = Encoding.UTF8.GetBytes(rawRequest);
            return await _createOrderCommandTranscoder.Decode(item);
        }
    }
}
