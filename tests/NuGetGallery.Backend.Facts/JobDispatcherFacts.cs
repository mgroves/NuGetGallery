﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NuGetGallery.Jobs;
using Xunit;

namespace NuGetGallery.Backend
{
    public class JobDispatcherFacts
    {
        public class TheDispatchMethod
        {
            [Fact]
            public async Task GivenNoJobWithName_ItThrowsUnknownJobException()
            {
                // Arrange
                var dispatcher = new JobDispatcher(BackendConfiguration.Create(), Enumerable.Empty<JobDescription>(), monitor: null);
                var request = new JobRequest("flarg", "test", new Dictionary<string, string>());
                var invocation = new JobInvocation(Guid.NewGuid(), request, DateTimeOffset.UtcNow);

                // Act/Assert
                var ex = await AssertEx.Throws<UnknownJobException>(() => dispatcher.Dispatch(invocation, null));
                Assert.Equal("flarg", ex.JobName);
            }

            [Fact]
            public async Task GivenJobWithName_ItCreatesAnInvocationAndInvokesJob()
            {
                // Arrange
                var jobImpl = new Mock<JobBase>();
                var job = new JobDescription("test", "blarg", () => jobImpl.Object);

                var dispatcher = new JobDispatcher(BackendConfiguration.Create(), new[] { job }, monitor: null);
                var request = new JobRequest("Test", "test", new Dictionary<string, string>());
                var invocation = new JobInvocation(Guid.NewGuid(), request, DateTimeOffset.UtcNow);

                jobImpl.Setup(j => j.Invoke(It.IsAny<JobInvocationContext>()))
                   .Returns(Task.FromResult(JobResult.Completed()));


                // Act
                var response = await dispatcher.Dispatch(invocation, null);

                // Assert
                Assert.Same(invocation, response.Invocation);
                Assert.Equal(JobResult.Completed(), response.Result);
            }

            [Fact]
            public async Task GivenJobWithName_ItReturnsResponseContainingInvocationAndResult()
            {
                // Arrange
                var jobImpl = new Mock<JobBase>();
                var job = new JobDescription("test", "blarg", () => jobImpl.Object);
                
                var ex = new Exception();
                var dispatcher = new JobDispatcher(BackendConfiguration.Create(), new[] { job }, monitor: null);
                var request = new JobRequest("Test", "test", new Dictionary<string, string>());
                var invocation = new JobInvocation(Guid.NewGuid(), request, DateTimeOffset.UtcNow);

                jobImpl.Setup(j => j.Invoke(It.IsAny<JobInvocationContext>()))
                   .Returns(Task.FromResult(JobResult.Completed()));

                // Act
                var response = await dispatcher.Dispatch(invocation, null);

                // Assert
                Assert.Same(invocation, response.Invocation);
                Assert.Equal(JobResult.Faulted(ex), response.Result);
            }
        }
    }
}