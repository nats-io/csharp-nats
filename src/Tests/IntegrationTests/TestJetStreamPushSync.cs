﻿// Copyright 2021 The NATS Authors
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

using System.Collections.Generic;
using System.Text;
using System.Threading;
using NATS.Client;
using NATS.Client.JetStream;
using Xunit;
using static UnitTests.TestBase;
using static IntegrationTests.JetStreamTestBase;

namespace IntegrationTests
{
    public class TestJetStreamPushSync : TestSuite<JetStreamPushSyncSuiteContext>
    {
        public TestJetStreamPushSync(JetStreamPushSyncSuiteContext context) : base(context)
        {
        }

        [Theory]
        [InlineData(null)]
        [InlineData(DELIVER)]
        public void TestJetStreamPushEphemeral(string deliverSubject)
        {
            Context.RunInJsServer(c =>
            {
                // create the stream.
                CreateMemoryStream(c, STREAM, SUBJECT);

                // Create our JetStream context to receive JetStream messages.
                IJetStream js = c.CreateJetStreamContext();

                // publish some messages
                JsPublish(js, SUBJECT, 1, 5);

                // Build our subscription options.
                PushSubscribeOptions options = PushSubscribeOptions.Builder()
                    .WithDeliverSubject(deliverSubject)
                    .Build();

                // Subscription 1
                IJetStreamPushSyncSubscription sub = js.PushSubscribeSync(SUBJECT, options);
                AssertSubscription(sub, STREAM, null, deliverSubject, false);
                c.Flush(DefaultTimeout); // flush outgoing communication with/to the server

                IList<Msg> messages1 = ReadMessagesAck(sub);
                int total = messages1.Count;
                ValidateRedAndTotal(5, messages1.Count, 5, total);

                // read again, nothing should be there
                IList<Msg> messages0 = ReadMessagesAck(sub);
                total += messages0.Count;
                ValidateRedAndTotal(0, messages0.Count, 5, total);

                // Subscription 2
                sub = js.PushSubscribeSync(SUBJECT, options);
                c.Flush(DefaultTimeout); // flush outgoing communication with/to the server

                // read what is available, same messages
                IList<Msg> messages2 = ReadMessagesAck(sub);
                total = messages2.Count;
                ValidateRedAndTotal(5, messages2.Count, 5, total);

                // read again, nothing should be there
                messages0 = ReadMessagesAck(sub);
                total += messages0.Count;
                ValidateRedAndTotal(0, messages0.Count, 5, total);

                AssertSameMessages(messages1, messages2);
            });
        }

        [Theory]
        [InlineData(null)]
        [InlineData(DELIVER)]
        public void TestJetStreamPushDurable(string deliverSubject)
        {
            Context.RunInJsServer(c =>
            {
                // create the stream.
                CreateMemoryStream(c, STREAM, SUBJECT);

                // Create our JetStream context to receive JetStream messages.
                IJetStream js = c.CreateJetStreamContext();

                // publish some messages
                JsPublish(js, SUBJECT, 1, 5);

                // use ackWait so I don't have to wait forever before re-subscribing
                ConsumerConfiguration cc = ConsumerConfiguration.Builder().WithAckWait(3000).Build();

                // Build our subscription options.
                PushSubscribeOptions options = PushSubscribeOptions.Builder()
                    .WithDurable(DURABLE)
                    .WithConfiguration(cc)
                    .WithDeliverSubject(deliverSubject)
                    .Build();

                // Subscribe.
                IJetStreamPushSyncSubscription sub = js.PushSubscribeSync(SUBJECT, options);
                AssertSubscription(sub, STREAM, DURABLE, deliverSubject, false);
                c.Flush(DefaultTimeout); // flush outgoing communication with/to the server

                // read what is available
                IList<Msg> messages = ReadMessagesAck(sub);
                int total = messages.Count;
                ValidateRedAndTotal(5, messages.Count, 5, total);

                // read again, nothing should be there
                messages = ReadMessagesAck(sub);
                total += messages.Count;
                ValidateRedAndTotal(0, messages.Count, 5, total);

                sub.Unsubscribe();
                c.Flush(DefaultTimeout); // flush outgoing communication with/to the server

                // re-subscribe
                sub = js.PushSubscribeSync(SUBJECT, options);
                c.Flush(DefaultTimeout); // flush outgoing communication with/to the server

                // read again, nothing should be there
                messages = ReadMessagesAck(sub);
                total += messages.Count;
                ValidateRedAndTotal(0, messages.Count, 5, total);
            });
        }

        [Fact]
        public void TestAcks()
        {
            Context.RunInJsServer(c =>
            {
                // create the stream.
                CreateMemoryStream(c, STREAM, SUBJECT);

                // Create our JetStream context to receive JetStream messages.
                IJetStream js = c.CreateJetStreamContext();
                
                ConsumerConfiguration cc = ConsumerConfiguration.Builder().WithAckWait(1500).Build();

                // Build our subscription options.
                PushSubscribeOptions options = PushSubscribeOptions.Builder()
                    .WithConfiguration(cc)
                    .Build();

                IJetStreamPushSyncSubscription sub = js.PushSubscribeSync(SUBJECT, options);
                c.Flush(DefaultTimeout); // flush outgoing communication with/to the server

                // NAK
                JsPublish(js, SUBJECT, "NAK", 1);
                
                Msg m = sub.NextMessage(DefaultTimeout);
                Assert.NotNull(m);
                Assert.Equal("NAK1", Encoding.ASCII.GetString(m.Data));
                m.Nak();
                
                m = sub.NextMessage(DefaultTimeout);
                Assert.NotNull(m);
                Assert.Equal("NAK1", Encoding.ASCII.GetString(m.Data));
                m.Ack();
                
                AssertNoMoreMessages(sub);

                // TERM
                JsPublish(js, SUBJECT, "TERM", 1);

                m = sub.NextMessage(DefaultTimeout);
                Assert.NotNull(m);
                Assert.Equal("TERM1", Encoding.ASCII.GetString(m.Data));
                m.Term();
                
                AssertNoMoreMessages(sub);

                // Ack Wait timeout
                JsPublish(js, SUBJECT, "WAIT", 1);

                m = sub.NextMessage(DefaultTimeout);
                Assert.NotNull(m);
                Assert.Equal("WAIT1", Encoding.ASCII.GetString(m.Data));
                Thread.Sleep(2000);
                m.Ack();
                
                m = sub.NextMessage(DefaultTimeout);
                Assert.NotNull(m);
                Assert.Equal("WAIT1", Encoding.ASCII.GetString(m.Data));
                
                // In Progress
                JsPublish(js, SUBJECT, "PRO", 1);
                
                m = sub.NextMessage(DefaultTimeout);
                Assert.NotNull(m);
                Assert.Equal("PRO1", Encoding.ASCII.GetString(m.Data));
                m.InProgress();
                Thread.Sleep(750);
                m.InProgress();
                Thread.Sleep(750);
                m.InProgress();
                Thread.Sleep(750);
                m.InProgress();
                Thread.Sleep(750);
                m.Ack();
                
                AssertNoMoreMessages(sub);

                // ACK Sync
                JsPublish(js, SUBJECT, "ACKSYNC", 1);
                m = sub.NextMessage(DefaultTimeout);
                Assert.NotNull(m);
                Assert.Equal("ACKSYNC1", Encoding.ASCII.GetString(m.Data));
                m.AckSync(DefaultTimeout);
                
                AssertNoMoreMessages(sub);
            });
        }
    }
}
