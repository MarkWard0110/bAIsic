using BAIsic.Interlocutor;
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
        [RequireModelData(OllamaTestConsts.Model.Llama3_1_8b)]
        //[RequireModelData("llama3:70b-instruct-q8_0")]
        //[RequireModelData("llama3.1:8b-instruct-fp16")]
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
            LlmSelectSpeakerAgentConfig config = new LlmSelectSpeakerAgentConfig();
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
        }
    }
}
