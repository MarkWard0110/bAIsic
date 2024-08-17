using BAIsic.Tests;

namespace BAIsic.Agent.Tests
{
    public class AgentTests
    {
        [Fact]
        public void Agent_Default_WhenConstructed()
        {
            var agentName = BAIsicTestConventions.Agent.RandomName();
            var justTheAgent = new JustTheAgent(agentName);

            Assert.Equal(agentName, justTheAgent.Name);
            Assert.Null(justTheAgent.SystemPrompt);
            Assert.Single(justTheAgent.PrepareSendHandlers);
            Assert.Single(justTheAgent.PrepareReceiveHandlers);
        }

        [Fact]
        public async Task Agent_DefaultBehavior_WhenConsntructed()
        {
            var justTheAgent = new JustTheAgent(BAIsicTestConventions.Agent.RandomName());
            var message = new Message("arole", "prompt text");

            var reply = await justTheAgent.GenerateReplyAsync([message]);
            Assert.Null(reply);

            var preparedSendMessage = await justTheAgent.PrepareSendMessageAsync(message);
            Assert.NotEqual(message, preparedSendMessage);

            var preparedReceiveMessage = await justTheAgent.PrepareReceiveMessageAsync(message);
            Assert.NotEqual(message, preparedReceiveMessage);
        }

        [Fact]
        public async Task Agent_MessageIsAsIs_WhenPrepareSendRoleIsAssistant()
        {
            var justTheAgent = new JustTheAgent(BAIsicTestConventions.Agent.RandomName());
            var message = new Message(AgentConsts.Roles.Assistant, "prompt text");

            var preparedSendMessage = await justTheAgent.PrepareSendMessageAsync(message);
            Assert.Equal(message, preparedSendMessage);
            Assert.Equal(AgentConsts.Roles.Assistant, preparedSendMessage.Role);
        }

        [Fact]
        public async Task Agent_MessageIsAsIs_WhenPrepareReceiveRoleIsUser()
        {
            var justTheAgent = new JustTheAgent(BAIsicTestConventions.Agent.RandomName());
            var message = new Message(AgentConsts.Roles.User, "prompt text");

            var preparedReceiveMessage = await justTheAgent.PrepareReceiveMessageAsync(message);
            Assert.Equal(message, preparedReceiveMessage);
            Assert.Equal(AgentConsts.Roles.User, preparedReceiveMessage.Role);
        }

        [Fact]
        public async Task Agent_RoleChangedToAssistant_WhenPrepareSendRoleIsNotAssistant()
        {
            var justTheAgent = new JustTheAgent(BAIsicTestConventions.Agent.RandomName());

            var preparedSendMessage = await justTheAgent.PrepareSendMessageAsync(new Message("arole", "prompt text"));
            Assert.Equal(AgentConsts.Roles.Assistant, preparedSendMessage.Role);

            preparedSendMessage = await justTheAgent.PrepareSendMessageAsync(new Message("user", "prompt text"));
            Assert.Equal(AgentConsts.Roles.Assistant, preparedSendMessage.Role);

            preparedSendMessage = await justTheAgent.PrepareSendMessageAsync(new Message("Assistant", "prompt text"));
            Assert.Equal(AgentConsts.Roles.Assistant, preparedSendMessage.Role);
        }

        [Fact]
        public async Task Agent_RoleChangedToUser_WhenPrepareReceiveRoleIsNotUser()
        {
            var justTheAgent = new JustTheAgent(BAIsicTestConventions.Agent.RandomName());

            var preparedReceiveMessage = await justTheAgent.PrepareReceiveMessageAsync(new Message("arole", "prompt text"));
            Assert.Equal(AgentConsts.Roles.User, preparedReceiveMessage.Role);

            preparedReceiveMessage = await justTheAgent.PrepareReceiveMessageAsync(new Message("User", "prompt text"));
            Assert.Equal(AgentConsts.Roles.User, preparedReceiveMessage.Role);

            preparedReceiveMessage = await justTheAgent.PrepareReceiveMessageAsync(new Message("assistant", "prompt text"));
            Assert.Equal(AgentConsts.Roles.User, preparedReceiveMessage.Role);
        }

        [Fact]
        public async Task Agent_MessageMutatesToUpper_WhenPrepareSendIsCalled()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddToLowerPrepareSend()
                .AddToUpperPrepareSend();
            var message = new Message(AgentConsts.Roles.Assistant, "PRoMpT TeXT");

            var preparedSendMessage = await mockAgent.PrepareSendMessageAsync(message);
            Assert.NotEqual(message, preparedSendMessage);
            Assert.Equal(AgentConsts.Roles.Assistant, preparedSendMessage.Role);
            Assert.Equal(message.Text.ToUpper(), preparedSendMessage.Text);
        }

        [Fact]
        public async Task Agent_MessageMutatesToLower_WhenPrepareSendIsCalled()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddToUpperPrepareSend()
                .AddToLowerPrepareSend();
            var message = new Message(AgentConsts.Roles.Assistant, "PRoMpT TeXT");

            var preparedSendMessage = await mockAgent.PrepareSendMessageAsync(message);
            Assert.NotEqual(message, preparedSendMessage);
            Assert.Equal(AgentConsts.Roles.Assistant, preparedSendMessage.Role);
            Assert.Equal(message.Text.ToLower(), preparedSendMessage.Text);
        }

        [Fact]
        public async Task Agent_MessageMutatesToUpper_WhenPrepareReceiveIsCalled()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddToLowerPrepareReceive()
                .AddToUpperPrepareReceive();
            var message = new Message(AgentConsts.Roles.User, "PRoMpT TeXT");

            var preparedReceiveMessage = await mockAgent.PrepareReceiveMessageAsync(message);
            Assert.NotEqual(message, preparedReceiveMessage);
            Assert.Equal(AgentConsts.Roles.User, preparedReceiveMessage.Role);
            Assert.Equal(message.Text.ToUpper(), preparedReceiveMessage.Text);
        }

        [Fact]
        public async Task Agent_MessageMutatesToLower_WhenPrepareReceiveIsCalled()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddToUpperPrepareReceive()
                .AddToLowerPrepareReceive();
            var message = new Message(AgentConsts.Roles.User, "PRoMpT TeXT");

            var preparedReceiveMessage = await mockAgent.PrepareReceiveMessageAsync(message);
            Assert.NotEqual(message, preparedReceiveMessage);
            Assert.Equal(AgentConsts.Roles.User, preparedReceiveMessage.Role);
            Assert.Equal(message.Text.ToLower(), preparedReceiveMessage.Text);
        }

        [Fact]
        public async Task Agent_GenerateReplyIsDoneWithMessage_WhenCalledWithLiteralGenerate()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddStringLiteralGenerateReply("reply text");

            var reply = await mockAgent.GenerateReplyAsync([new Message("user", "prompt text")]);
            Assert.NotNull(reply);
            Assert.Equal("reply text", reply.Text);
            Assert.Equal(AgentConsts.Roles.Assistant, reply.Role);
        }

        [Fact]
        public async Task Agent_GenerateReplyContinuesUntilIsDone_WhenCalled()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(false, "one")
                .AddLiteralGenerateReply(false, "two")
                .AddLiteralGenerateReply(true, "three");

            var reply = await mockAgent.GenerateReplyAsync([new Message("user", "prompt text")]);
            Assert.NotNull(reply);
            Assert.Equal("three", reply.Text);
        }

        [Fact]
        public async Task Agent_GenerateReplyStopsAtFirstIsDone_WhenCalled()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(false, "one")
                .AddLiteralGenerateReply(true, "two")
                .AddLiteralGenerateReply(false, "three");

            var reply = await mockAgent.GenerateReplyAsync([new Message("user", "prompt text")]);
            Assert.NotNull(reply);
            Assert.Equal("two", reply.Text);
        }

        [Fact]
        public async Task Agent_GenerateReplyIsDoneWithNull_WhenCalled()
        {
            var mockAgent = new MockAgent(BAIsicTestConventions.Agent.RandomName())
                .AddLiteralGenerateReply(false, "one")
                .AddLiteralGenerateReply(true, null)
                .AddLiteralGenerateReply(true, "three");

            var reply = await mockAgent.GenerateReplyAsync([new Message("user", "prompt text")]);
            Assert.Null(reply);
        }

    }
}