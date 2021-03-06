﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Nimbus.Configuration;
using Nimbus.Tests.Common.Extensions;
using Nimbus.Tests.Integration.Tests.MulticastRequestResponseTests.MessageContracts;
using Nimbus.Tests.Integration.Tests.MulticastRequestResponseTests.RequestHandlers;
using Nimbus.Tests.Integration.TestScenarioGeneration.ScenarioComposition;
using Nimbus.Tests.Integration.TestScenarioGeneration.TestCaseSources;
using NUnit.Framework;
using Shouldly;

namespace Nimbus.Tests.Integration.Tests.MulticastRequestResponseTests
{
    [TestFixture]
    public class WhenSendingAMulticastRequestThatShouldAllowAllResponders : TestForBus
    {
        private BlackBallResponse[] _response;

        protected override async Task When()
        {
            SlowBlackBallRequestHandler.Reset();
            SlowBlackBallRequestHandler.HandlerThrottle.Release(1);

            var request = new BlackBallRequest
                          {
                              ProspectiveMemberName = "Fred Flintstone"
                          };

            _response = (await Bus.MulticastRequest(request, TimeSpan.FromSeconds(TimeoutSeconds)))
                .Take(3)
                .ToArray();
        }

        [Test]
        [TestCaseSource(typeof (AllBusConfigurations<WhenSendingAMulticastRequestThatShouldAllowAllResponders>))]
        public async Task Run(string testName, IConfigurationScenario<BusBuilderConfiguration> scenario)
        {
            await Given(scenario);
            await When();
            await Then();
        }

        [Then]
        public async Task WeShouldReceiveThreeResponses()
        {
            _response.Count().ShouldBe(3);
        }

        [Then]
        public async Task AllHandlersShouldHaveAtLeastReceivedTheRequest()
        {
            await Timeout.WaitUntil(() => MethodCallCounter.AllReceivedMessages.OfType<BlackBallRequest>().Count() == 4);

            MethodCallCounter.AllReceivedMessages.OfType<BlackBallRequest>().Count().ShouldBe(4);
        }

        [Then]
        public async Task ThereShouldBeNoMessagesInTheDeadLetterOffice()
        {
            (await Bus.DeadLetterOffice.Count()).ShouldBe(0);
        }
    }
}