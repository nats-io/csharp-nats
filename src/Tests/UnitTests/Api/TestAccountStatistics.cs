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

using NATS.Client.Api;
using Xunit;

namespace UnitTests.Api
{
    public class TestAccountStatistics
    {
        [Fact]
        public void JsonIsReadProperly()
        {
            string json = ApiTestUtil.ReadDataFile("AccountStatistics.json");
            AccountStatistics stat = new AccountStatistics(json);
            Assert.Equal(1, stat.Memory);
            Assert.Equal(2, stat.Storage);
            Assert.Equal(3, stat.Streams);
            Assert.Equal(4, stat.Consumers);

            stat = new AccountStatistics("{}");
            Assert.Equal(0, stat.Memory);
            Assert.Equal(0, stat.Storage);
            Assert.Equal(0, stat.Streams);
            Assert.Equal(0, stat.Consumers);
        }
    }
}
