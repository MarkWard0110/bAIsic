using BAIsic.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BAIsic.Tests.BAIsicTestConventions;

namespace BAIsic.Interlocutor.Tests
{
    public class GroupConversationTests
    {
        [Fact]
        public async Task GroupConversation_DefaultTransition_WhenInitiateChat()
        {
            var agentOne = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("agentOne");
            var agentTwo = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("agentTwo");
            var agentThree = new Agent(BAIsicTestConventions.Agent.RandomName())
                .AddHugGenerateReply("agentThree");

            var converableAgentOne = new GenericConversableAgent(agentOne, "agent one");
            var converableAgentTwo = new GenericConversableAgent(agentTwo, "agent two");
            var converableAgentThree = new GenericConversableAgent(agentThree, "agent three");

            List<IConversableAgent> agents = [converableAgentOne, converableAgentTwo, converableAgentThree];

            Task<(IConversableAgent? Speaker, ConversationResult? SelectSpeakerConversationResult)> SelectSpeakerHandlerWithTestAsync(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
            {
                Assert.Equal(3, agents.Count);
                Assert.Equal(3, allowedTransitions.Count);
                foreach(var agent in agents)
                {
                    Assert.Contains(agent.Name, allowedTransitions.Keys);
                    Assert.Equal(3, allowedTransitions[agent.Name].Count);
                }

                return SelectSpeakerHandlerAsync(speaker, message, agents, allowedTransitions);
            }
            var groupConversation = new GroupConversation(agents, SelectSpeakerHandlerWithTestAsync);

            var initialMessage = new Message(AgentConsts.Roles.User, "hello");

            var result = await groupConversation.InitiateChatAsync(converableAgentOne, initialMessage);

            Assert.NotNull(result);
            Assert.NotNull(result);
            Assert.Equal(1, result[0].TurnCount);
            Assert.NotNull(result[0].Conversation);
            Assert.Equal(3, result[0].Conversation.Count);
            for (int i = 0; i < result[0].Conversation.Count; i++)
            {
                Assert.NotNull(result[0].Conversation[i]);
                Assert.Equal(2, result[0].Conversation[i].Messages.Count);
            }
        }

        [Fact]
        public async Task GroupConversation_SystemPromptInitialized_WhenInitiateChat()
        {
            var agentOne = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent one")
                .AddHugGenerateReply("agentOne");
            var agentTwo = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent two")
                .AddHugGenerateReply("agentTwo");
            var agentThree = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent three")
                .AddHugGenerateReply("agentThree");

            var converableAgentOne = new GenericConversableAgent(agentOne, "agent one");
            var converableAgentTwo = new GenericConversableAgent(agentTwo, "agent two");
            var converableAgentThree = new GenericConversableAgent(agentThree, "agent three");

            List<IConversableAgent> agents = [converableAgentOne, converableAgentTwo, converableAgentThree];

            var groupConversation = new GroupConversation(agents, SelectSpeakerHandlerAsync);

            var initialMessage = new Message(AgentConsts.Roles.User, "hello");

            var result = await groupConversation.InitiateChatAsync(converableAgentOne, initialMessage);

            Assert.NotNull(result);
            Assert.NotNull(result[0]);
            Assert.Equal(1, result[0].TurnCount);
            Assert.NotNull(result[0].Conversation);
            Assert.Equal(3, result[0].Conversation.Count);
            for (int i = 0; i < result[0].Conversation.Count; i++)
            {
                Assert.NotNull(result[0].Conversation[i]);
                Assert.Equal(3, result[0].Conversation[i].Messages.Count);
            }
        }

        [Fact]
        public async Task GroupConversation_MaxTurnCount_WhenInitiateChat()
        {
            var agentOne = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent one")
                .AddHugGenerateReply("agentOne");
            var agentTwo = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent two")
                .AddHugGenerateReply("agentTwo");
            var agentThree = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent three")
                .AddHugGenerateReply("agentThree");

            var converableAgentOne = new GenericConversableAgent(agentOne, "agent one");
            var converableAgentTwo = new GenericConversableAgent(agentTwo, "agent two");
            var converableAgentThree = new GenericConversableAgent(agentThree, "agent three");

            List<IConversableAgent> agents = [converableAgentOne, converableAgentTwo, converableAgentThree];

            var groupConversation = new GroupConversation(agents, SelectSpeakerHandlerAsync, maxTurnCount:3);

            var initialMessage = new Message(AgentConsts.Roles.User, "hello");

            var result = await groupConversation.InitiateChatAsync(converableAgentOne, initialMessage);

            Assert.NotNull(result);
            Assert.NotNull(result[0]);
            Assert.Equal(3, result[0].TurnCount);
            Assert.NotNull(result[0].Conversation);
            Assert.Equal(3, result[0].Conversation.Count);
            for (int i = 0; i < result[0].Conversation.Count; i++)
            {
                Assert.NotNull(result[0].Conversation[i]);
                Assert.Equal(5, result[0].Conversation[i].Messages.Count);
            }
        }

        [Fact]
        public async Task GroupConversation_NullSpeakerTerminate_WhenInitiateChat()
        {
            var agentOne = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent one")
                .AddHugGenerateReply("agentOne");
            var agentTwo = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent two")
                .AddHugGenerateReply("agentTwo");
            var agentThree = new Agent(BAIsicTestConventions.Agent.RandomName(), "you are agent three")
                .AddHugGenerateReply("agentThree");

            var converableAgentOne = new GenericConversableAgent(agentOne, "agent one");
            var converableAgentTwo = new GenericConversableAgent(agentTwo, "agent two");
            var converableAgentThree = new GenericConversableAgent(agentThree, "agent three");

            List<IConversableAgent> agents = [converableAgentOne, converableAgentTwo, converableAgentThree];


            Task<(IConversableAgent? Speaker, ConversationResult? SelectSpeakerConversationResult)> SelectSpeakerHandlerWithTestAsync(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
            {
                if (ReferenceEquals(speaker, converableAgentTwo))
                {
                    return Task.FromResult<(IConversableAgent? Speaker, ConversationResult? SelectSpeakerConversationResult)>((null, null));
                }

                return SelectSpeakerHandlerAsync(speaker, message, agents, allowedTransitions);
            }

            var groupConversation = new GroupConversation(agents, SelectSpeakerHandlerWithTestAsync, maxTurnCount: 3);

            var initialMessage = new Message(AgentConsts.Roles.User, "hello");

            var result = await groupConversation.InitiateChatAsync(converableAgentOne, initialMessage);

            Assert.NotNull(result);
            Assert.NotNull(result[0]);
            Assert.Equal(1, result[0].TurnCount);
            Assert.NotNull(result[0].Conversation);
            Assert.Equal(3, result[0].Conversation.Count);
            for (int i = 0; i < result[0].Conversation.Count; i++)
            {
                Assert.NotNull(result[0].Conversation[i]);
                Assert.Equal(3, result[0].Conversation[i].Messages.Count);
            }
        }


        public static Task<(IConversableAgent? Speaker, ConversationResult? SelectSpeakerConversationResult)> SelectSpeakerHandlerAsync(IConversableAgent speaker, Message message, IList<IConversableAgent> agents, IDictionary<string, List<string>> allowedTransitions)
        {
                int currentIndex = agents.IndexOf(speaker);
                int nextIndex = (currentIndex + 1) % agents.Count;
                return Task.FromResult<(IConversableAgent? Speaker, ConversationResult? SelectSpeakerConversationResult)>((agents[nextIndex], null));
        }
    }
}
