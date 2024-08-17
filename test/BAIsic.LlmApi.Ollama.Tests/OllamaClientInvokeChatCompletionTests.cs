using BAIsic.Tests;
using Moq;
using Moq.Protected;
using System;
using System.Net;
using System.Text.Json;

namespace BAIsic.LlmApi.Ollama.Tests
{
    public class OllamaClientInvokeChatCompletionTests
    {
        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task InvokeChatCompletionAsync_ReturnsChatResponse_WhenCallTextWithStream(string model)
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();

            var chatRequest = new ChatRequest()
            {
                Model = model,
                Stream = true,
                Options = new RequestOptions()
                {
                    Temperature = 0.5f,
                    TopP = 1.0f,
                },
                Messages = [
                    new Message()
                    {
                        Role = "user",
                        Content = "Generate an example of a C# XUnit unit test that asserts an AI chat completion."
                    }
                ]
            };

            var chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest);

            AssertChatResponseFinal(chatResponse);
        }

        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task InvokeChatCompletionAsync_ReturnsChatResponse_WhenCallTextWithNoStream(string model)
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();

            var chatRequest = new ChatRequest()
            {
                Model = model,
                Stream = false,
                Options = new RequestOptions()
                {
                    Temperature = 0.5f,
                    TopP = 1.0f,
                },
                Messages = [
                    new Message()
                    {
                        Role = "user",
                        Content = "Generate an example of a C# XUnit unit test that asserts an AI chat completion."
                    }
                ]
            };

            var chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest);

            AssertChatResponseFinal(chatResponse);
        }

        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llava_Phi3_3_8b)]
        public async Task InvokeChatCompletionAsync_ReturnsChatResponse_WhenCallImageWithNoStream(string model)
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();

            var image = Path.Combine("resource", "images", "burman.jpg");
            var binaryData = BinaryData.FromBytes(File.ReadAllBytes(image), "image/jpg");
            byte[]? data = binaryData.ToArray();
            var base64Image = Convert.ToBase64String(data);

            var chatRequest = new ChatRequest()
            {
                Model = model,
                Stream = false,
                Options = new RequestOptions()
                {
                    Temperature = 0.5f,
                    TopP = 1.0f,
                },
                Messages = [
                    new Message()
                    {
                        Role = "system",
                        Content = "You are a helpful AI assistant"
                    },
                    new Message()
                    {
                        Role = "user",
                        Content = "Descibe what is in this image.",
                        Images= [base64Image]
                    }
                ]
            };

            var chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest);

            AssertChatResponseFinal(chatResponse);
        }

        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task InvokeChatCompletionAsync_WritesStream_WhenCallTextWithStream(string model)
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();

            var chatRequest = new ChatRequest()
            {
                Model = model,
                Stream = true,
                Options = new RequestOptions()
                {
                    Temperature = 0.5f,
                    TopP = 1.0f,
                },
                Messages = [
                    new Message()
                    {
                        Role = "user",
                        Content = "Generate an example of a C# XUnit unit test."
                    }
                ]
            };

            var stringWriter = new StringWriter();
            var chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest, outputStream:stringWriter);

            var streamContent = stringWriter.ToString();
            AssertChatResponseFinal(chatResponse);
            Assert.NotEmpty(streamContent);
            Assert.NotNull(chatResponse.Message);
            Assert.Equal(chatResponse.Message.Content, streamContent);
        }

        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task InvokeChatCompletionAsync_InvokesTool_WhenCallWithToolsNoStream(string model)
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();

            var chatRequest = new ChatRequest()
            {
                Model = model,
                Stream = false,
                Options = new RequestOptions()
                {
                    Temperature = 0.5f,
                    TopP = 1.0f,
                },
                Messages = [
                    new Message()
                    {
                        Role = "user",
                        Content = "What is the flight time from New York (NYC) to Los Angeles (LAX)?"
                    }
                ],
                Tools = [
                    new Tool(){
                        Function = new ToolFunction(){
                            Name = "get_flight_times",
                            Description = "Get flight times between two cities",
                            Parameters = new FunctionParameters() {
                                Required = ["departure", "arrival"],
                                Properties = new Dictionary<string, FunctionProperty>(){
                                    ["departure"] = new FunctionProperty(){
                                        Type = "string",
                                        Description = "The departure city code (airport code)"
                                    },
                                    ["arrival"] = new FunctionProperty(){
                                        Type = "string",
                                        Description = "The arrival city code (airport code)"
                                    }
                                }
                            }
                        }
                    }]
            };

            var chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest);

            AssertChatResponseFinal(chatResponse);

            Assert.NotNull(chatResponse);
            Assert.NotNull(chatResponse.Message);
            Assert.NotNull(chatResponse.Message.ToolCalls);
            Assert.Single(chatResponse.Message.ToolCalls);
            var toolCall = chatResponse.Message.ToolCalls[0];
            Assert.NotNull(toolCall);
            Assert.NotNull(toolCall.Function);
            Assert.Equal("get_flight_times", toolCall.Function.Name);
            Assert.NotNull(toolCall.Function.Arguments);
            Assert.True(toolCall.Function.Arguments.ContainsKey("departure"), $"Arguments keys: {string.Join(", ", toolCall.Function.Arguments.Keys)}");
            Assert.Equal("NYC", toolCall.Function.Arguments["departure"]);
            Assert.True(toolCall.Function.Arguments.ContainsKey("arrival"), $"Arguments keys: {string.Join(", ", toolCall.Function.Arguments.Keys)}");
            Assert.Equal("LAX", toolCall.Function.Arguments["arrival"]);

            // perform tool invocation (mocked)
            var toolResult = "{ departure: \"08:00 AM\", arrival: \"11:30 AM\", duration: \"5h 30m\" }";

            chatRequest.Messages.Add(new Message(){
                Role = "tool",
                Content = toolResult
            });

            // re-invoke chat completion with tool result
            chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest);
            AssertChatResponseFinal(chatResponse);
            Assert.NotNull(chatResponse.Message);
        }

        [Theory]
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        public async Task InvokeChatCompletionAsync_InvokesTool_WhenCallWithToolsStream(string model)
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();

            var chatRequest = new ChatRequest()
            {
                Model = model,
                Stream = true,
                Options = new RequestOptions()
                {
                    Temperature = 0.5f,
                    TopP = 1.0f,
                },
                Messages = [
                    new Message()
                    {
                        Role = "user",
                        Content = "What is the flight time from New York (NYC) to Los Angeles (LAX)?"
                    }
                ],
                Tools = [
                    new Tool(){
                        Function = new ToolFunction(){
                            Name = "get_flight_times",
                            Description = "Get flight times between two cities",
                            Parameters = new FunctionParameters() {
                                Required = ["departure", "arrival"],
                                Properties = new Dictionary<string, FunctionProperty>(){
                                    ["desparture"] = new FunctionProperty(){
                                        Type = "string",
                                        Description = "The departure city code (airport code)"
                                    },
                                    ["arrival"] = new FunctionProperty(){
                                        Type = "string",
                                        Description = "The arrival city code (airport code)"
                                    }
                                }
                            }
                        }
                    }]
            };

            var chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest);

            AssertChatResponseFinal(chatResponse);

            Assert.NotNull(chatResponse);
            Assert.NotNull(chatResponse.Message);
            Assert.NotNull(chatResponse.Message.ToolCalls);
            Assert.Single(chatResponse.Message.ToolCalls);
            var toolCall = chatResponse.Message.ToolCalls[0];
            Assert.NotNull(toolCall);
            Assert.NotNull(toolCall.Function);
            Assert.Equal("get_flight_times", toolCall.Function.Name);
            Assert.NotNull(toolCall.Function.Arguments);
            Assert.Equal("NYC", toolCall.Function.Arguments["departure"]);
            Assert.Equal("LAX", toolCall.Function.Arguments["arrival"]);

            // perform tool invocation (mocked)
            var toolResult = "{ departure: \"08:00 AM\", arrival: \"11:30 AM\", duration: \"5h 30m\" }";

            chatRequest.Messages.Add(new Message()
            {
                Role = "tool",
                Content = toolResult
            });

            // re-invoke chat completion with tool result
            chatResponse = await ollamaClient.InvokeChatCompletionAsync(chatRequest);
            AssertChatResponseFinal(chatResponse);
            Assert.NotNull(chatResponse.Message);
        }

        private static void AssertChatResponseFinal(ChatResponse chatResponse)
        {
            Assert.NotNull(chatResponse);
            Assert.NotNull(chatResponse.Message);
            Assert.NotNull(chatResponse.Message.Role);
            Assert.NotEmpty(chatResponse.Model);
            Assert.True(chatResponse.Done);
            Assert.Equal("stop", chatResponse.DoneReason);

            Assert.NotNull(chatResponse.TotalDuration);
            Assert.NotNull(chatResponse.LoadDuration);
            Assert.NotNull(chatResponse.PromptEvalCount);
            Assert.NotNull(chatResponse.PromptEvalDuration);
            Assert.NotNull(chatResponse.EvalCount);
            Assert.NotNull(chatResponse.EvalDuration);
        }   
    }
}