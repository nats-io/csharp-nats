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

using NATS.Client.Internals;

namespace NATS.Client.JetStream
{
    public sealed class PublishOptions
    {          
        /// <summary>
        /// The default timeout (2000ms)
        /// </summary>
        public static readonly Duration DefaultTimeout = Duration.OfMillis(Defaults.Timeout);

        /// <summary>
        /// The default stream name (unset)
        /// </summary>
        public const string DefaultStream = null;

        /// <summary>
        /// Default Last Sequence Number (unset)
        /// </summary>
        public const long DefaultLastSequence = -1;

        /// <summary>
        /// The stream name.
        /// </summary>
        public string Stream { get; }

        /// <summary>
        /// The stream timeout.
        /// </summary>
        public Duration StreamTimeout { get; }
        
        /// <summary>
        /// The Expected Stream.
        /// </summary>
        public string ExpectedStream { get; }
        
        /// <summary>
        /// The Expected Last Message Id.
        /// </summary>
        public string ExpectedLastMsgId { get; }
        
        /// <summary>
        /// The Expected Last Sequence.
        /// </summary>
        public long ExpectedLastSeq { get; }
        
        /// <summary>
        /// The Expected Message Id.
        /// </summary>
        public string MessageId { get; }

        private PublishOptions(string stream, Duration streamTimeout, string expectedStream,
            string expectedLastMsgId, long expectedLastSeq, string messageId)
        {
            Stream = stream;
            StreamTimeout = streamTimeout;
            ExpectedStream = expectedStream;
            ExpectedLastMsgId = expectedLastMsgId;
            ExpectedLastSeq = expectedLastSeq;
            MessageId = messageId;
        }
        
        /// <summary>
        /// Gets the publish options builder.
        /// </summary>
        /// <returns>
        /// The builder
        /// </returns>
        public static PublishOptionsBuilder Builder()
        {
            return new PublishOptionsBuilder();
        }

        /// <summary>
        /// The PublishOptionsBuilder builds PublishOptions
        /// </summary>
        public sealed class PublishOptionsBuilder
        {
            private string _stream = DefaultStream;
            private Duration _streamTimeout = DefaultTimeout;
            private string _expectedStream;
            private string _expectedLastMsgId;
            private long _expectedLastSeq = DefaultLastSequence;
            private string _messageId;
            
            /// <summary>
            /// Set the stream name.
            /// </summary>
            /// <param name="stream">Name of the stream</param>
            /// <returns>The Builder</returns>
            public PublishOptionsBuilder WithStream(string stream)
            {
                _stream = Validator.ValidateStreamName(stream, false);
                return this;
            }

            /// <summary>
            /// Set the stream timeout with a Duration
            /// </summary>
            /// <param name="timeout">The publish acknowledgement timeout as a Duration.</param>
            /// <returns>The PublishOptionsBuilder</returns>
            public PublishOptionsBuilder WithTimeout(Duration timeout)
            {
                _streamTimeout = Validator.EnsureNotNullAndNotLessThanMin(timeout, DefaultTimeout, 1);
                return this;
            }

            /// <summary>
            /// Set the stream timeout in milliseconds
            /// </summary>
            /// <param name="timeoutMillis">The publish acknowledgement timeout as millis</param>
            /// <returns>The PublishOptionsBuilder</returns>
            public PublishOptionsBuilder WithTimeout(long timeoutMillis)
            {
                _streamTimeout = Validator.EnsureDurationNotLessThanMin(timeoutMillis, DefaultTimeout, 1);
                return this;
            }

            /// <summary>
            /// Set the message id.
            /// </summary>
            /// <param name="msgId">The message ID of these options.</param>
            /// <returns>The PublishOptionsBuilder</returns>
            public PublishOptionsBuilder WithMessageId(string msgId) 
            {
                _messageId = Validator.ValidateNotEmpty(msgId, nameof(msgId));
                return this;
            }

            /// <summary>
            /// Set the expected stream name.
            /// </summary>
            /// <param name="stream">The expected stream name.</param>
            /// <returns>The PublishOptionsBuilder</returns>
            public PublishOptionsBuilder WithExpectedStream(string stream)
            {
                _expectedStream = stream;
                return this;
            }

            /// <summary>
            /// Set the expected last message ID.
            /// </summary>
            /// <param name="lastMessageId">The expected last message ID.</param>
            /// <returns>The PublishOptionsBuilder</returns>
            public PublishOptionsBuilder WithExpectedLastMsgId(string lastMessageId)
            {
                _expectedLastMsgId = Validator.ValidateNotEmpty(lastMessageId, nameof(lastMessageId));
                return this;
            }        

            /// <summary>
            /// Set the expected stream name.
            /// </summary>
            /// <param name="lastSequence">The expected sequence.</param>
            /// <returns>The PublishOptionsBuilder</returns>
            public PublishOptionsBuilder WithExpectedLastSequence(long lastSequence)
            {
                _expectedLastSeq = Validator.ValidateNotNegative(lastSequence, nameof(lastSequence));
                return this;
            }

            /// <summary>
            /// Clears the expected so the build can be re-used.
            /// Clears the expectedLastId, expectedLastSequence and messageId fields.
            /// </summary>
            /// <returns>The PublishOptionsBuilder</returns>
            public PublishOptionsBuilder ClearExpected() 
            {
                _expectedLastMsgId = null;
                _expectedLastSeq = DefaultLastSequence;
                _messageId = null;
                return this;
            }

            /// <summary>
            /// Builds the PublishOptions
            /// </summary>
            /// <returns>The PublishOptions object.</returns>
            public PublishOptions Build() 
            {
                return new PublishOptions(_stream, _streamTimeout, _expectedStream, _expectedLastMsgId, _expectedLastSeq, _messageId);
            }
        }
    }
}
