using System.Collections.Concurrent;
using System.Net;
using FluentAssertions;
using Xunit;

namespace PepperDash.Essentials.Plugin.TriplePlay.IptvServer.Tests;

public class SerialRequestSenderTests
{
    private static readonly TimeSpan Wait = TimeSpan.FromSeconds(5);
    private static readonly Uri BaseAddress = new("http://10.1.2.3:8080/");

    [Fact]
    public void Sends_Requests_In_Order_One_At_A_Time()
    {
        var handler = new FakeHandler(async request =>
        {
            await Task.Delay(20);
            return Ok(request.RequestUri!.AbsolutePath);
        });
        using var sender = CreateSender(handler);
        var bodies = new ConcurrentQueue<string>();
        using var done = new CountdownEvent(5);
        sender.ResponseReceived += (_, args) => { bodies.Enqueue(args.ContentString); done.Signal(); };

        for (var i = 1; i <= 5; i++)
            sender.Queue(new HttpRequestMessage(HttpMethod.Get, "r" + i)).Should().BeTrue();

        done.Wait(Wait).Should().BeTrue();
        bodies.Should().Equal("/r1", "/r2", "/r3", "/r4", "/r5");
        handler.MaxInFlight.Should().Be(1);
    }

    [Fact]
    public void Raises_Status_Code_And_Body_For_Error_Responses()
    {
        var handler = new FakeHandler(_ => Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("missing")
        }));
        using var sender = CreateSender(handler);
        GenericClientResponseEventArgs? received = null;
        using var done = new ManualResetEventSlim();
        sender.ResponseReceived += (_, args) => { received = args; done.Set(); };

        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "x"));

        done.Wait(Wait).Should().BeTrue();
        received!.Code.Should().Be(404);
        received.ContentString.Should().Be("missing");
    }

    [Fact]
    public void Reports_Failure_And_Keeps_Sending()
    {
        var handler = new FakeHandler(request => request.RequestUri!.AbsolutePath == "/fail"
            ? throw new HttpRequestException("connection refused")
            : Task.FromResult(Ok("ok")));
        using var sender = CreateSender(handler);
        RequestFailedEventArgs? failure = null;
        using var done = new ManualResetEventSlim();
        sender.RequestFailed += (_, args) => failure = args;
        sender.ResponseReceived += (_, _) => done.Set();

        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "fail"));
        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "next"));

        done.Wait(Wait).Should().BeTrue();
        failure!.Exception.Should().BeOfType<HttpRequestException>();
        // HttpClient resolves the queued relative URI against BaseAddress when it sends
        failure.RequestUri.AbsoluteUri.Should().Be("http://10.1.2.3:8080/fail");
    }

    [Fact]
    public void A_Throwing_Response_Handler_Does_Not_Stop_The_Worker()
    {
        using var sender = CreateSender(new FakeHandler(request => Task.FromResult(Ok(request.RequestUri!.AbsolutePath))));
        var failures = 0;
        using var done = new ManualResetEventSlim();
        sender.RequestFailed += (_, _) => Interlocked.Increment(ref failures);
        sender.ResponseReceived += (_, args) =>
        {
            if (args.ContentString == "/first") throw new InvalidOperationException("handler bug");
            done.Set();
        };

        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "first"));
        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "second"));

        done.Wait(Wait).Should().BeTrue();
        failures.Should().Be(1);
    }

    [Fact]
    public void A_Throwing_Failure_Handler_Does_Not_Stop_The_Worker()
    {
        var handler = new FakeHandler(request => request.RequestUri!.AbsolutePath == "/fail"
            ? throw new HttpRequestException("down")
            : Task.FromResult(Ok("ok")));
        using var sender = CreateSender(handler);
        using var done = new ManualResetEventSlim();
        sender.RequestFailed += (_, _) => throw new InvalidOperationException("logger bug");
        sender.ResponseReceived += (_, _) => done.Set();

        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "fail"));
        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "next"));

        done.Wait(Wait).Should().BeTrue();
    }

    [Fact]
    public void Queue_Returns_False_After_Dispose()
    {
        var sender = CreateSender(new FakeHandler(_ => Task.FromResult(Ok(""))));
        sender.Dispose();

        using var request = new HttpRequestMessage(HttpMethod.Get, "x");
        sender.Queue(request).Should().BeFalse();
    }

    [Fact]
    public void Sends_TriplePlay_Query_Against_Base_Address_Still_Encoded()
    {
        var handler = new FakeHandler(_ => Task.FromResult(Ok("")));
        using var sender = CreateSender(handler);
        using var done = new ManualResetEventSlim();
        sender.ResponseReceived += (_, _) => done.Set();
        var query = TriplePlayQuery.Build(3, "SetVolume", null!, 15, null);

        sender.Queue(new HttpRequestMessage(HttpMethod.Get, "triplecare/JsonRpcHandler.php?" + query));

        done.Wait(Wait).Should().BeTrue();
        var sent = handler.Requests.Single();
        sent.GetLeftPart(UriPartial.Path).Should().Be("http://10.1.2.3:8080/triplecare/JsonRpcHandler.php");
        sent.Query.Should().Be("?" + query);
        Uri.UnescapeDataString(sent.Query["?call=".Length..])
            .Should().Be(@"{""jsonrpc"":""2.0"",""method"":""SetVolume"",""params"":[3,15]}");
    }

    private static SerialRequestSender CreateSender(HttpMessageHandler handler) =>
        new(new HttpClient(handler) { BaseAddress = BaseAddress }, 20, "test-requests");

    private static HttpResponseMessage Ok(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body) };

    private sealed class FakeHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, Task<HttpResponseMessage>> _respond;
        private int _inFlight;
        private int _maxInFlight;

        public FakeHandler(Func<HttpRequestMessage, Task<HttpResponseMessage>> respond) => _respond = respond;

        public ConcurrentQueue<Uri> Requests { get; } = new();

        public int MaxInFlight => Volatile.Read(ref _maxInFlight);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var inFlight = Interlocked.Increment(ref _inFlight);
            int max;
            while (inFlight > (max = Volatile.Read(ref _maxInFlight)) &&
                   Interlocked.CompareExchange(ref _maxInFlight, inFlight, max) != max)
            {
            }

            try
            {
                Requests.Enqueue(request.RequestUri!);
                return await _respond(request);
            }
            finally
            {
                Interlocked.Decrement(ref _inFlight);
            }
        }
    }
}
