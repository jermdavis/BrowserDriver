using BrowserDriver;
using BrowserDriver.Browsers;
using BrowserDriver.States;

using (var browser = BrowserConnectionFactory.Create())
{
    browser.Open("about:blank");

    var connection = await browser.Connect();

    var stateMachine = new StateMachine(PageEventEnableState.Instance, connection);
    stateMachine.State["nextUrl"] = "https://bbc.co.uk/news/";

    await stateMachine.Start();
    await stateMachine.Wait();

    var html = stateMachine.State["HTML"] as string;
    Console.WriteLine($"HTML: {html}");
}