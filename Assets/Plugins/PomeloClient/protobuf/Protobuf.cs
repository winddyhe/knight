using System;
using Core.WindJson;

namespace Pomelo.Protobuf
{
    public class Protobuf
    {
        private MsgDecoder decoder;
        private MsgEncoder encoder;

        public Protobuf(JsonNode encodeProtos, JsonNode decodeProtos)
        {
            this.encoder = new MsgEncoder(encodeProtos);
            this.decoder = new MsgDecoder(decodeProtos);
        }

        public byte[] encode(string route, JsonNode msg)
        {
            return encoder.encode(route, msg);
        }

        public JsonNode decode(string route, byte[] buffer)
        {
            return decoder.decode(route, buffer);
        }
    }
}