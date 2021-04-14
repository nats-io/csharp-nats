// Copyright 2021 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Text;
using NATS.Client.Internals.SimpleJSON;

namespace NATS.Client.Api
{
    public abstract class ApiResponse
    {
        public const String NO_TYPE = "io.nats.jetstream.api.v1.no_type";

        public string Type { get; }
        public Error Error { get; }

        internal JSONNode JsonNode { get; }

        internal ApiResponse(Msg msg) : this(Encoding.UTF8.GetString(msg.Data)) { }

        internal ApiResponse(string json)
        {
            JsonNode = JSON.Parse(json);
            Type = JsonNode[ApiConsts.TYPE].Value;
            if (string.IsNullOrEmpty(Type))
            {
                Type = NO_TYPE;
            }
            Error = Error.OptionalInstance(JsonNode[ApiConsts.ERROR]);
        }
        
        public void ThrowOnHasError() {
            if (HasError) {
                throw new JetStreamApiException(this);
            }
        }

        public bool HasError => Error != null;

        public long ErrorCode => Error?.Code ?? -1;

        public string Description => Error?.Desc ?? null;
    }
}
