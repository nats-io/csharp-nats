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
    public class TestAccountLimits
    {
        [Fact]
        public void JsonIsReadProperly()
        {
            string json = ApiTestUtil.ReadDataFile("AccountLimits.json");
            AccountLimits al = new AccountLimits(json);
            Assert.Equal(1, al.MaxMemory);
            Assert.Equal(2, al.MaxStorage);
            Assert.Equal(3, al.MaxStreams);
            Assert.Equal(4, al.MaxConsumers);

            al = new AccountLimits("{}");
            Assert.Equal(-1, al.MaxMemory);
            Assert.Equal(-1, al.MaxStorage);
            Assert.Equal(-1, al.MaxStreams);
            Assert.Equal(-1, al.MaxConsumers);
        }
    }
}