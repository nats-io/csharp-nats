﻿// Copyright 2020 The NATS Authors
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using NATS.Client;
using NATS.Client.Api;
using Xunit;

namespace UnitTests.Api
{
    public class TestPublishAck
    {
        [Fact]
        public void IsValidAck() {
            String json = "{\"stream\":\"test\",\"seq\":42, \"duplicate\" : true }";
            PublishAck ack = new PublishAck(json);
            Assert.Equal("test", ack.Stream);
            Assert.Equal(42, ack.Seq);
            Assert.True(ack.Duplicate);
        }
      
        [Fact]
        public void ThrowsOnERR() {
            String json ="{" +
                         "  \"type\": \"io.nats.jetstream.api.v1.pub_ack_response\"," +
                         "  \"error\": {" +
                         "    \"code\": 500," +
                         "    \"description\": \"the description\"" +
                         "  }" +
                         "}";

            JetStreamApiException jsapi = Assert.Throws<JetStreamApiException>(
                () => new PublishAck(json).ThrowOnHasError());
            Assert.Equal(500, jsapi.ErrorCode);
            Assert.Equal("the description", jsapi.ErrorDescription);
        }

        [Fact]
        public void InvalidResponse() {
            String json1 = "+OK {" +
                           "\"missing_stream\":\"test\"" + "," +
                           "\"missing_seq\":\"0\"" +
                           "}";

            NATSException ioe = Assert.Throws<NATSException>(() => new PublishAck(json1));
            Assert.Equal("Invalid JetStream ack.", ioe.Message);

            String json2 = "{\"stream\":\"test\", \"duplicate\" : true }";
            Assert.Throws<NATSException>(() => new PublishAck(json2));
        }
    }
}