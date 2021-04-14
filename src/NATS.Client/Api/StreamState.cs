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
using NATS.Client.Internals;
using NATS.Client.Internals.SimpleJSON;

namespace NATS.Client.Api
{
    public sealed class StreamState
    {
        public long Messages { get; }
        public long Bytes { get; }
        public long FirstSeq { get; }
        public long LastSeq { get; }
        public long ConsumerCount { get; }
        public DateTime FirstTime { get; }
        public DateTime LastTime { get; }

        internal static StreamState OptionalInstance(JSONNode streamState)
        {
            return streamState == null || streamState.Count == 0 ? null : new StreamState(streamState);
        }

        private StreamState(JSONNode streamState)
        {
            Messages = streamState[ApiConsts.MESSAGES].AsLong;
            Bytes = streamState[ApiConsts.BYTES].AsLong;
            FirstSeq = streamState[ApiConsts.FIRST_SEQ].AsLong;
            LastSeq = streamState[ApiConsts.LAST_SEQ].AsLong;
            ConsumerCount = streamState[ApiConsts.CONSUMER_COUNT].AsLong;
            FirstTime = JsonUtils.AsDate(streamState[ApiConsts.FIRST_TS]);
            LastTime = JsonUtils.AsDate(streamState[ApiConsts.LAST_TS]);
        }
    }
}