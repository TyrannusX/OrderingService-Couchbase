using Dawn;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OrderingService.Commands.CreateOrder;
using OrderingService.Commands.DeleteOrder;
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
        private readonly ICommandDispatcher _commandDispatcher;

        public OrderController(
            ILogger<OrderController> logger,
            ITranscoder transcoder,
            ICommandDispatcher commandDispatcher
            )
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _transcoder = Guard.Argument(transcoder, nameof(transcoder)).NotNull().Value;
            _commandDispatcher = Guard.Argument(commandDispatcher, nameof(commandDispatcher)).NotNull().Value;
        }

        [HttpPost]
        public async Task<IActionResult> Order()
        {
            CreateOrderCommand createOrderCommand = await DecodePostRequest();
            string newId = await _commandDispatcher.SendAsync<CreateOrderCommand, string>(createOrderCommand);
            return Ok(newId);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Order([FromRoute] string id)
        {
            GetOrderCommand getOrderCommand = new GetOrderCommand
            {
                Id = id
            };
            Order order = await _commandDispatcher.SendAsync<GetOrderCommand, Order>(getOrderCommand);
            await EncodeResponse(order);
            return new EmptyResult();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> OrderDelete([FromRoute] string id)
        {
            DeleteOrderCommand deleteOrderCommand = new DeleteOrderCommand
            {
                Id = id
            };
            await _commandDispatcher.SendAsync(deleteOrderCommand);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> OrderUpdate()
        {
            UpdateOrderCommand updateOrderCommand = await DecodePutRequest();
            Order order = await _commandDispatcher.SendAsync<UpdateOrderCommand, Order>(updateOrderCommand);
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
