using Microsoft.Extensions.AI;
using OllamaSharp;
using PuppeteerSharp;

IChatClient chatClient =
    new OllamaApiClient(new Uri("http://localhost:11434/"), "llava");


var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true
});
var page = await browser.NewPageAsync();
Console.WriteLine(Environment.CurrentDirectory);
var output = Path.Combine(AppContext.BaseDirectory, "test.png");
await page.GoToAsync("https://video.autostrade.it/video-mp4_hq/dt7/6ef0d3f7-e103-43b7-a1d1-ff8b0a3ba65a-19.mp4");
await page.ScreenshotAsync(output);


List<ChatMessage> chatHistory = new();


var userPrompt = "The image displays a road with trucks and cars. The definition is not great. how many veicles are present?";
ChatMessage msg = new ChatMessage(ChatRole.User, userPrompt);
msg.Contents.
    Add(new DataContent(File.ReadAllBytes(output), "image/png"));
chatHistory.Add(msg);

// Stream the AI response and add to chat history
Console.WriteLine("AI Response:");
var response = "";
await foreach (ChatResponseUpdate item in
    chatClient.GetStreamingResponseAsync(chatHistory))
{
    Console.Write(item.Text);
    response += item.Text;
}
chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
Console.WriteLine();