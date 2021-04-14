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
    public abstract class SourceBase
    {
        public string Name { get; }
        public long StartSeq { get; }
        public DateTime StartTime { get; }
        public string FilterSubject { get; }
        public External External { get; }

         internal SourceBase(JSONNode sourceBaseNode)
        {
            Name = sourceBaseNode[ApiConsts.NAME].Value;
            StartSeq = sourceBaseNode[ApiConsts.OPT_START_SEQ].AsLong;
            StartTime = JsonUtils.AsDate(sourceBaseNode[ApiConsts.OPT_START_TIME]);
            FilterSubject = sourceBaseNode[ApiConsts.FILTER_SUBJECT].Value;
            External = External.OptionalInstance(sourceBaseNode[ApiConsts.EXTERNAL]);
        }
    }
}