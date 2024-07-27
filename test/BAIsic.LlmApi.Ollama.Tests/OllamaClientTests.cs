using BAIsic.Tests;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace BAIsic.LlmApi.Ollama.Tests
{
    public class OllamaClientTests
    {
        [RequireEnvironmentVariableFactAttribute(OllamaTestConsts.EnvironmentVariable.OllamaHost)]
        public async Task ListLocalModelsAsync_ReturnsNotNull_WhenCalled()
        {
            var ollamaClient = OllamaClientExtensions.CreateOllamaClient();

            var models = await ollamaClient.ListLocalModelsAsync();

            Assert.NotNull(models);
        }

        [Fact]
        public async Task ListLocalModelAsync_ReturnsEmptyList_WhenOllamaHasZeroModels()
        {
            // Arrange
            var modelResponse = new ListLocalModelsResponse()
            {
                Models = []
            };
            var modelResponseJson = JsonSerializer.Serialize(modelResponse);

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(modelResponseJson)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            }; 
            var client = new OllamaClient(httpClient);

            // Act
            var result = await client.ListLocalModelsAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task ListLocalModelAsync_ReturnsMultipleModels()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"models\":[{\"name\":\"nomic-embed-text:137m-v1.5-fp16\",\"model\":\"nomic-embed-text:137m-v1.5-fp16\",\"modified_at\":\"2024-07-26T02:58:49.859337914Z\",\"size\":274302450,\"digest\":\"0a109f422b47e3a30ba2b10eca18548e944e8a23073ee3f3e947efcf3c45e59f\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"nomic-bert\",\"families\":[\"nomic-bert\"],\"parameter_size\":\"137M\",\"quantization_level\":\"F16\"}},{\"name\":\"mistral-nemo:12b-instruct-2407-fp16\",\"model\":\"mistral-nemo:12b-instruct-2407-fp16\",\"modified_at\":\"2024-07-26T02:58:49.367338922Z\",\"size\":24504289232,\"digest\":\"6a38e02e88ecd2c9cb3b7bab028db7ec70579500c7a25e24534a9162d21393cb\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"12.2B\",\"quantization_level\":\"F16\"}},{\"name\":\"mistral-large:123b-instruct-2407-q4_1\",\"model\":\"mistral-large:123b-instruct-2407-q4_1\",\"modified_at\":\"2024-07-26T02:58:48.895339888Z\",\"size\":76718077686,\"digest\":\"19d48e361da3ec1dd2a5c70f41c04073a213f485a115ebbd1c70b9ba0e0c89cb\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"122.6B\",\"quantization_level\":\"Q4_1\"}},{\"name\":\"llava:latest\",\"model\":\"llava:latest\",\"modified_at\":\"2024-07-26T02:46:43.034241977Z\",\"size\":4733363377,\"digest\":\"8dd30f6b0cb19f555f2c7a7ebda861449ea2cc76bf1f44e262931f45fc81d081\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\",\"clip\"],\"parameter_size\":\"7B\",\"quantization_level\":\"Q4_0\"}},{\"name\":\"llama3.1:70b-instruct-q8_0\",\"model\":\"llama3.1:70b-instruct-q8_0\",\"modified_at\":\"2024-07-26T02:46:42.538245504Z\",\"size\":74975064195,\"digest\":\"613c5de138f9bf89f92fea313ae5d5550aca02d9adec5c99b1ec09a3813865a0\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"70.6B\",\"quantization_level\":\"Q8_0\"}},{\"name\":\"llama3.1:70b-instruct-q6_K\",\"model\":\"llama3.1:70b-instruct-q6_K\",\"modified_at\":\"2024-07-26T02:34:54.127192114Z\",\"size\":57888157827,\"digest\":\"7eb2993e26a7bc464843d47f94939760cd2a8d3d004d94d2405e6cd3bfe35090\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"70.6B\",\"quantization_level\":\"Q6_K\"}},{\"name\":\"llama3.1:70b-instruct-q2_K\",\"model\":\"llama3.1:70b-instruct-q2_K\",\"modified_at\":\"2024-07-26T02:25:39.394896289Z\",\"size\":26375123075,\"digest\":\"6366f3d7588ce5e3656b700f8d731c493f6abb4bb407b61b76044f17cb446fc4\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"70.6B\",\"quantization_level\":\"Q2_K\"}},{\"name\":\"llama3:70b-instruct-q8_0\",\"model\":\"llama3:70b-instruct-q8_0\",\"modified_at\":\"2024-07-26T02:25:38.394895862Z\",\"size\":74975062469,\"digest\":\"13ef6d4ac2af9cb999295f71edfeb4a5ec9f044752b4a91a2e36d43ea34f11fd\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"70.6B\",\"quantization_level\":\"Q8_0\"}},{\"name\":\"llama3.1:8b-instruct-fp16\",\"model\":\"llama3.1:8b-instruct-fp16\",\"modified_at\":\"2024-07-26T02:25:38.898896077Z\",\"size\":16068905889,\"digest\":\"9d95e89188d4315cedc62d969a7b4257cce295797013044c4094823c3ced502f\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"8.0B\",\"quantization_level\":\"F16\"}},{\"name\":\"llama3:70b-instruct-q2_K\",\"model\":\"llama3:70b-instruct-q2_K\",\"modified_at\":\"2024-07-26T02:25:37.586895518Z\",\"size\":26375121349,\"digest\":\"693db6efd8f9eab52a9db2bbbc7bab0c379f006ec50dc290d27a6bba4c0bcaa3\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"70.6B\",\"quantization_level\":\"Q2_K\"}},{\"name\":\"llama3:70b-instruct-q6_K\",\"model\":\"llama3:70b-instruct-q6_K\",\"modified_at\":\"2024-07-26T02:25:37.982895686Z\",\"size\":57888156101,\"digest\":\"a50a7631e818b0c960b3c5624c2f6a749b0797de599d24212337a7a0fbe2ddd2\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"70.6B\",\"quantization_level\":\"Q6_K\"}},{\"name\":\"llama3:8b-instruct-fp16\",\"model\":\"llama3:8b-instruct-fp16\",\"modified_at\":\"2024-07-26T02:25:37.138895327Z\",\"size\":16068904163,\"digest\":\"c666fe422df7d438104ce2ff06d99a567576f5369985a6e928bbee58ec86928f\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"llama\",\"families\":[\"llama\"],\"parameter_size\":\"8.0B\",\"quantization_level\":\"F16\"}},{\"name\":\"gemma2:9b-instruct-fp16\",\"model\":\"gemma2:9b-instruct-fp16\",\"modified_at\":\"2024-07-26T02:25:36.25489495Z\",\"size\":18490690080,\"digest\":\"28e6684b085085f78551db7c96a9daa546161b1da9d055ea01b84cb1163013cf\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"gemma2\",\"families\":[\"gemma2\"],\"parameter_size\":\"9.2B\",\"quantization_level\":\"F16\"}},{\"name\":\"gemma2:27b-instruct-fp16\",\"model\":\"gemma2:27b-instruct-fp16\",\"modified_at\":\"2024-07-26T02:25:36.666895125Z\",\"size\":54462030977,\"digest\":\"4a8f851205c5761d976b5c39096b4f39e56b2ddbc1e0d7cc50bfcccc6470fe8e\",\"details\":{\"parent_model\":\"\",\"format\":\"gguf\",\"family\":\"gemma2\",\"families\":[\"gemma2\"],\"parameter_size\":\"27.2B\",\"quantization_level\":\"F16\"}}]}")
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            var client = new OllamaClient(httpClient);

            // Act
            var result = await client.ListLocalModelsAsync();

            // Assert
            Assert.Equal(14, result.Length);
            Assert.All(result, model => Assert.False(string.IsNullOrEmpty(model.Name)));
        }

        [Fact]
        public async Task ListLocalModelAsync_ThrowsHttpRequestException_WhenOllamaReturnsNon200Status()
        {
            // Arrange
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost")
            };
            var client = new OllamaClient(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => client.ListLocalModelsAsync());
        }
    }
}