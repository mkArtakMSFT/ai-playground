// See https://aka.ms/new-console-template for more information
using Microsoft.Extensions.AI;

Console.WriteLine("Hello, World!");

//var innerChatClient = new AzureOpenAIClient(
//    new Uri(builder.Configuration["AI:Endpoint"]!),
//    new ApiKeyCredential(builder.Configuration["AI:Key"]!))
//    .AsChatClient("gpt-4o-mini");

var ollamaChatClient = new OllamaChatClient(new Uri("http://127.0.0.1:11434"), "llama3.1");

IChatClient client =
    new ChatClientBuilder(ollamaChatClient)
    .UseFunctionInvocation()
    .Build();

var nextWordTool = AIFunctionFactory.Create(
    GetNextWord,
    new AIFunctionFactoryOptions
    {
        Description = "Evaluates each word that is being generated. If the tool returns false, the word shouldn't be used in the response, and a new suggestion should be tried instead."
    });
List<AITool> tools = [nextWordTool];

var chatOptions = new ChatOptions { Tools = tools };

var history = new List<ChatMessage>();
var prompt = new ChatMessage(ChatRole.User, "Count from 1 to 5. When you're done, respond with <END>");
history.Add(prompt);

var response = await client.GetResponseAsync(history, chatOptions);

Console.WriteLine(response.Text);

static bool GetNextWord(string word)
{
    Console.WriteLine($"probing word `{word}`");

    if (word.Length < 5)
        return false;

    return true;
}
/*
 * 
 * var response = new StringBuilder();
await foreach (var item in client.GetStreamingResponseAsync(history, chatOptions))
{
    response.Append(item.Text);
    Console.Write(item.Text);
}

history.Add(new ChatMessage(ChatRole.Assistant, response.ToString()));

Console.WriteLine();
 * */