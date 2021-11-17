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
        private readonly IMediator _mediator;
        private readonly ITranscoder _transcoder;

        public OrderController(
            ILogger<OrderController> logger,
            ITranscoder transcoder,
            IMediator mediator
            )
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _transcoder = Guard.Argument(transcoder, nameof(transcoder)).NotNull().Value;
            _mediator = Guard.Argument(mediator, nameof(mediator)).NotNull().Value;
        }

        [HttpPost]
        public async Task<IActionResult> Order()
        {
            _logger.LogTrace("Entering POST endpoint {endpoint} for controller {OrderController}", nameof(Order), typeof(OrderController));
            CreateOrderCommand createOrderCommand = await DecodeRequest();
            _logger.LogTrace("{commandType} value is {command}", typeof(CreateOrderCommand), createOrderCommand);
            await _mediator.Send(createOrderCommand);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Order([FromRoute] string id)
        {
            GetOrderCommand getOrderCommand = new GetOrderCommand
            {
                Id = id
            };

            Order order = await _mediator.Send(getOrderCommand) as Order;
            await EncodeResponse(order);
            return Ok();
        }

        private async Task EncodeResponse(Order order)
        {
            byte[] encodedOrder = await _transcoder.Encode(order, nameof(Domain.Orders.Order));
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
            return await _transcoder.Decode(item, nameof(CreateOrderCommand)) as CreateOrderCommand;
        }
    }
}
