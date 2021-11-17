using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using OrderingService.Domain.Contracts;

namespace OrderingService.Domain.Transcoders
{
    public class Transcoder : ITranscoder
    {
        private readonly ILogger<Transcoder> _logger;
        private readonly IEnumerable<IEncoder> _encoders;
        private readonly IEnumerable<IDecoder> _decoders;

        public Transcoder(ILogger<Transcoder> logger, IEnumerable<IEncoder> encoders, IEnumerable<IDecoder> decoders)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _encoders = Guard.Argument(encoders, nameof(encoders)).NotNull().NotEmpty().Value;
            _decoders = Guard.Argument(decoders, nameof(decoders)).NotNull().NotEmpty().Value;
        }

        public async Task<object> Decode(byte[] item, string contentType)
        {
            IDecoder decoder = _decoders.FirstOrDefault(decoder => string.Equals(contentType, decoder.DomainContentType, StringComparison.OrdinalIgnoreCase));
            if (decoder is null)
            {
                string message = $"{contentType} decoder not found";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            return await decoder.Decode(item);
        }

        public async Task<byte[]> Encode(object item, string contentType)
        {
            IEncoder encoder = _encoders.FirstOrDefault(encoder => string.Equals(contentType, encoder.DomainContentType, StringComparison.OrdinalIgnoreCase));
            if (encoder is null)
            {
                string message = $"{contentType} encoder not found";
                _logger.LogError(message);
                throw new InvalidOperationException(message);
            }
            return await encoder.Encode(item);
        }
    }
}