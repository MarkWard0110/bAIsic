using BAIsic.Interlocutor;
using BAIsic.Interlocutor.Tests;
using BAIsic.LlmApi.Ollama.Tests;
using BAIsic.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAIsic.AgentWithOllama.Tests
{
    public class NumberTeamsTests
    {
        [Theory]
        //[RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        //[RequireModelData("llama3.1:8b-instruct-fp16")]
        //[RequireModelData("llama3:70b-instruct-q8_0")]
        [RequireModelData("llama3.1:70b-instruct-q8_0")]
        public async Task NumberTeamsTest(string model)
        {

            //-------------------------------------------------------------------------
            List<IConversableAgent> agents = [];
            Dictionary<string, List<string>> speakerTransitionsDict = [];
            Dictionary<string, int> secretValues = [];

            // Outer loop for prefixes 'A', 'B', 'C'
            foreach (var prefix in new[] { "A", "B", "C" })
            {
                // Add 3 nodes with each prefix to the graph using a loop
                for (int i = 0; i < 3; i++)
                {
                    string nodeId = $"{prefix}{i}";
                    int secretValue = new Random().Next(1, 6);  // Generate a random secret value
                    secretValues[nodeId] = secretValue;
                    
                    // Create a ConversableAgent for each node 
                    var systemMessage = 
$@"Your name is {nodeId}.
Do not respond as the speaker named in the NEXT tag if your name is not in the NEXT tag. Instead, suggest a relevant team leader to handle the mis-tag, with the NEXT: tag.

You have {secretValue} chocolates.

The list of players are [A0, A1, A2, B0, B1, B2, C0, C1, C2].

Your first character of your name is your team, and your second character denotes that you are a team leader if it is 0.
CONSTRAINTS: Team members can only talk within the team, while team leaders can talk to team leaders of other teams but not team members of other teams.

You can use NEXT: to suggest the next speaker. You have to respect the CONSTRAINTS, and can only suggest one player from the list of players, i.e., do not suggest A3 because A3 is not from the list of players.
Team leaders must make sure that they know the sum of the individual chocolate count of all three players in their own team, i.e., A0 is responsible for team A only.

Keep track of the player's tally using a JSON format so that others can check the total tally. Use
A0:?, A1:?, A2:?,
B0:?, B1:?, B2:?,
C0:?, C1:?, C2:?

If you are the team leader, you should aggregate your team's total chocolate count to cooperate.
Once the team leader knows their team's tally, they can suggest another team leader for them to find their team tally, because we need all three team tallies to succeed.
Use NEXT: to suggest the next speaker, e.g., NEXT: A0.

Once we have the total tally from all nine players, sum up all three teams' tally, then terminate the discussion using TERMINATE.
                    ";
                    agents.Add(new ConversableAgent(
                        name:nodeId,
                        systemPrompt: systemMessage,
                        description: systemMessage
                    ).AddOllamaGenerateReply(model));

                    speakerTransitionsDict[agents.Last().Name] = [];
                }

                // Add edges between nodes with the same prefix using a nested loop
                for (int sourceNode = 0; sourceNode < 3; sourceNode++)
                {
                    string sourceId = $"{prefix}{sourceNode}";
                    for (int targetNode = 0; targetNode < 3; targetNode++)
                    {
                        string targetId = $"{prefix}{targetNode}";
                        if (sourceNode != targetNode)  // To avoid self-loops
                        {
                            speakerTransitionsDict[sourceId].Add(targetId);
                        }
                    }
                }
            }

            // Adding edges between teams
            speakerTransitionsDict["A0"].Add("B0");
            speakerTransitionsDict["A0"].Add("C0");
            speakerTransitionsDict["B0"].Add("A0");
            speakerTransitionsDict["B0"].Add("C0");
            speakerTransitionsDict["C0"].Add("A0");
            speakerTransitionsDict["C0"].Add("B0");

            // --------------------------------------------------------------
            LlmSelectSpeakerAgentConfig config = new LlmSelectSpeakerAgentConfig()
            {
                //                SelectSpeakerSystemMessageTemplate =
                //@"You are a coordinator responsible for managing a group of agents. Your task is to choose which agent should be called next based on the conversation.

                //You have a list of agent names that you may choose from: [{agentlist}].
                //The user's prompt may contain hints, such as ""next {agent name}"", indicating which agent they believe should be called next.
                //You must only select from the list of agent names provided.
                //If the user's suggestion matches a name in the list, select that agent.
                //If the user's suggestion does not match any names in the list, you must ignore the user's suggestion and select an appropriate agent from the list on your own.
                //Your goal is to ensure that only valid agents are chosen in response to the user's prompt.

                //Only return the agent's name.",
                //                SelectSpeakerSystemMessageTemplate =
                //@"You are a coordinator responsible for managing a group of agents. Your primary task is to choose which agent should be called next based on the user's instructions and the context of the conversation.

                //You have a list of agent names that you may choose from: [{agentlist}].

                //Critical Rule:

                //If the user's prompt explicitly specifies ""next"" and an agent name follows ""next"" and that name matches an agent in your list, you must select that agent, without exception. This rule applies even if other agents were mentioned earlier in the conversation.

                //Example:

                //If the user says, ""I think A1 and A2 have done a great job, but next, let's hear from B0,"" you must select B0 as the next agent.


                //Secondary Rules:

                //If the user's suggestion does not match any names in the list, ignore the user's suggestion and select the most contextually appropriate agent from the list.
                //If the user does not provide any explicit instructions, choose the agent that best continues the flow of the conversation, ensuring logical progression.

                //Only return the agent's name and nothing else.",
                //                NoneSelectedPrompt =
                //@"You didn't choose a valid agent name. You have a list of agent names that you may choose from: [{agentlist}].  As a reminder, to determine the next agent use these prioritized rules:
                //1. If the context refers to themselves as a speaker e.g. \""As the...\"" , choose that agent's name
                //2. If it refers to the \""next\"" agent name, choose that name
                //3. Otherwise, choose one of the provided agent's name in the context
                //The names are case-sensitive and should not be abbreviated or changed.
                //The only names that are accepted are [{agentlist}].
                //Respond with ONLY the name of the agent and DO NOT provide a reason.",
                SelectSpeakerSystemMessageTemplate =
@"You are a coordinator responsible for managing a group of agents. Your primary task is to choose which agent should be called next based on the user's instructions and the context of the conversation.

You have a list of agent names that you may choose from: [{agentlist}].

Critical Rule:

If the user's prompt explicitly specifies ""next"" and an agent name follows ""next"" and that name matches an agent in your list, you must select that agent, without exception. This rule applies even if other agents were mentioned earlier in the conversation.

Secondary Rules:

If the user's suggestion does not match any names in the list, ignore the user's suggestion and select the most contextually appropriate agent from the list.
If the user does not provide any explicit instructions, choose the agent that best continues the flow of the conversation, ensuring logical progression.

Only return the agent's name and nothing else.",
                NoneSelectedPrompt =
@"You didn't choose a valid agent name. You have a list of agent names that you may choose from: [{agentlist}].  As a reminder, to determine the next agent use these prioritized rules:
1. If the context refers to themselves as a speaker e.g. \""As the...\"" , choose that agent's name
2. If it refers to the \""next\"" agent name, choose that name
3. Otherwise, choose one of the provided agent's name in the context
The names are case-sensitive and should not be abbreviated or changed.
The only names that are accepted are [{agentlist}].
Respond with ONLY the name of the agent and DO NOT provide a reason.",
                SelectSpeakerPrompt = string.Empty
            };

            var selectSpeakerAgent = new LlmSelectSpeakerAgent(BAIsicTestConventions.Agent.RandomName(), config)
                .AddOllamaGenerateReply(model);

            var agentSelectSpeaker = new LlmSpeakerSelector(selectSpeakerAgent);


            static async Task<bool> Terminate(bool isInitialMessage, IAgent agent, Message message)
            {
                return message.Text.Contains("TERMINATE", StringComparison.OrdinalIgnoreCase);
            }

            var initiatorAgent = new ConversableAgent(BAIsicTestConventions.Agent.RandomName(), "Conversation initiator");

            agents.Add(initiatorAgent);
            speakerTransitionsDict[initiatorAgent.Name] = ["A0"];

            int maxTurnCount = 30;
            var groupConversation = new GroupConversation(agents, agentSelectSpeaker.SelectSpeakerAsync, allowedTransitions: speakerTransitionsDict, maxTurnCount: maxTurnCount, terminationHandler: Terminate);

            var initialMessage = new Message(AgentConsts.Roles.User, @"There are 9 players in this game, split equally into Teams A, B, C. Therefore each team has 3 players, including the team leader.
The task is to find out the sum of chocolate count from all nine players. I will now start with the A0 team leader.
NEXT: A0");

            var result = await groupConversation.InitiateChatAsync(initiatorAgent, initialMessage);

            Assert.NotNull(result);
            Assert.NotEqual(maxTurnCount, result.TurnCount);

            Assert.True(result.Conversation.First().Messages.Last().Text.Contains("TERMINATE", StringComparison.OrdinalIgnoreCase));

            await CheckAnswer(result.Conversation.First().Messages.Last(), secretValues, model);
        }

        private static async Task CheckAnswer(Message numbersAnswer, Dictionary<string, int> secretValues, string model)
        {

            var initiatorAgent = new MockAgent("feeder")
                .AddStringLiteralGenerateReply("Respond with only the classification");

            var answers = BuildAnswers(secretValues);

            var llmAgent = new OllamaAgent("llm", model, $"Read the conversation.  Classify if it is correct or not correct.  Given the following answers \n{answers}\nIt is ok if a Team leader report their team's total.\nCheck the conversation and classify if it has generated correct information based on the answers.  The conversation may not provide how it calculated the answer.");

            var conversation = new DialogueConversation();
            var conversationResult = await conversation.InitiateChat(initiatorAgent, numbersAnswer, llmAgent, maximumTurnCount:2);
            Assert.NotNull(conversationResult);
            if (conversationResult.Conversation.Last().Messages.Last().Text.Contains("not correct", StringComparison.OrdinalIgnoreCase))
            {
                Assert.Fail("The answer was classified as not correct");
            }
            Assert.True(conversationResult.Conversation.Last().Messages.Last().Text.Contains("correct", StringComparison.OrdinalIgnoreCase), "The answer was not classified correctly");
        }

        private static string BuildAnswers(Dictionary<string, int> answers)
        {
            var sb = new System.Text.StringBuilder();

            foreach (var pair in answers)
            {
                sb.AppendLine($"{pair.Key}: {pair.Value}");
            }


            // Outer loop for prefixes 'A', 'B', 'C'
            foreach (var prefix in new[] { "A", "B", "C" })
            {
                int teamTotal = 0;
                // Add 3 nodes with each prefix to the graph using a loop
                for (int i = 0; i < 3; i++)
                {
                    string nodeId = $"{prefix}{i}";
                    teamTotal += answers[nodeId];
                }

                sb.AppendLine($"Team {prefix} total: {teamTotal}");
            }

            return sb.ToString();
        }
    }
}
