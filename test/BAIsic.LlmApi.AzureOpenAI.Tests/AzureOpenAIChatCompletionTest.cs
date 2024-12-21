using BAIsic.Tests;

namespace BAIsic.LlmApi.AzureOpenAI.Tests
{
    public class AzureOpenAIChatCompletionTest
    {
        [RequireEnvironmentVariableFact(AzureOpenAITestConsts.EnvironmentVariable.ChatCompletionEndpoint)]
        public async Task InvokeChatCompletionAsync_ReturnsChatResponse_WhenCallWithoutStream()
        {
            var client = AzureOpenAIClientExtensions.CreateAzureOpenAIClient();

            var chatRequest = new ChatRequest()
            {
                Model = AzureOpenAITestConsts.Model.Llama3_1_8b,
                Messages =
                [
                    new Message()
                    {
                        Role = "user",
                        Content = "What is the capital of France?"
                    }
                ]
            };

            var chatResponse = await client.InvokeChatCompletionAsync(chatRequest);

            AssertChatResponseFinal(chatResponse);
            Assert.NotEmpty(chatResponse!.Choices![0].Message!.Content);
        }

        private static void AssertChatResponseFinal(ChatResponse chatResponse)
        {
            Assert.NotNull(chatResponse);
            Assert.NotEmpty(chatResponse.Model);
            Assert.NotNull(chatResponse.Choices);
            Assert.NotEmpty(chatResponse.Choices);
            Assert.NotNull(chatResponse.Choices[0]);
            Assert.Equal("stop", chatResponse.Choices[0].FinishReason);
            Assert.NotNull(chatResponse.Choices[0].Message);
            Assert.NotEmpty(chatResponse.Choices[0].Message!.Role);

            Assert.NotNull(chatResponse.Usage);
            Assert.True(chatResponse.Usage.PromptTokens > 0);
            Assert.True(chatResponse.Usage.CompletionTokens > 0);
            Assert.True(chatResponse.Usage.TotalTokens > 0);
        }
    }
}