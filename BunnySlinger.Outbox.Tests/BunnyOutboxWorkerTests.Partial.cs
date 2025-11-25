using Microsoft.Extensions.DependencyInjection;

using Moq;


namespace BunnySlinger.Outbox.Tests
{
    public partial class BunnyOutboxWorkerTests
    {
	    private class BunnyOutboxWorkerTestsHelper
	    {
		    public BunnyOutboxWorkerTestsHelper()
		    {
			    var serviceScope = new Mock<IServiceScope>();
			    var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
			    ProcessorMock = new Mock<IBunnyOutboxProcessor>();
			    var providerMock = new Mock<IServiceProvider>();

			    serviceScope.Setup(s => s.ServiceProvider).Returns(providerMock.Object);
			    serviceScopeFactoryMock.Setup(s => s.CreateScope()).Returns(serviceScope.Object);
			    ProcessorMock.Setup(p => p.ProcessAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
			    providerMock.Setup(p => p.GetService(typeof(IBunnyOutboxProcessor))).Returns(ProcessorMock.Object);
			    providerMock.Setup(p => p.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactoryMock.Object);

			    ServiceScopeFactory = new TestAsyncServiceScopeFactory(providerMock.Object);
		    }

		    public TestAsyncServiceScopeFactory ServiceScopeFactory { get; }

		    public Mock<IBunnyOutboxProcessor> ProcessorMock { get; }
	    }

	    private class TestAsyncServiceScopeFactory : IServiceScopeFactory
	    {
		    private readonly IServiceProvider _provider;
		    public TestAsyncServiceScopeFactory(IServiceProvider provider) => _provider = provider;
		    public IServiceScope CreateScope() => _provider.CreateScope();
	    }
    }
}
