using Azure;
using Azure.AI.Language.QuestionAnswering;
using Azure.AI.Translation.Text;
using System;

namespace question_answering
{
	class Program
	{
		static TextTranslationClient translationClient;
		static string detectedLanguage = "en";

		static void Main(string[] args)
		{
			Uri cogsv_endpoint = new("https://azcog312.cognitiveservices.azure.com/");
			string cogsv_key = "5ae15c9590174e05b62863a1b6d9d025"	;

			AzureKeyCredential cogsv_credential = new(cogsv_key);
			translationClient = new TextTranslationClient(cogsv_credential, cogsv_endpoint);


			Uri endpoint = new Uri("https://langserv312.cognitiveservices.azure.com/");
			AzureKeyCredential credential = new AzureKeyCredential("c996cbe3fa344648832701bc53028b1b");
			string projectName = "MLFaq";
			string deploymentName = "production";


			QuestionAnsweringClient client = new QuestionAnsweringClient(endpoint, credential);
			QuestionAnsweringProject project = new QuestionAnsweringProject(projectName, deploymentName);


			string question;

			Console.WriteLine("Machine Learning and AI FAQ\n\n");
			DisplayAnswer("A:Hi, ask me about machine learning");

			while ((question = Console.ReadLine()) != "Quit")
			{
				Response<AnswersResult> response = client.GetAnswers(Translate(question, "en"), project);

				foreach (KnowledgeBaseAnswer answer in response.Value.Answers)
				{
					DisplayAnswer($"A:{Translate(answer.Answer, detectedLanguage)}");
				}

			}

			DisplayAnswer("Thank you, hope you learn about machine learning and AI");

		}

		static void DisplayQuestion(string question)
		{
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.WriteLine(question);
			Console.ResetColor();
		}

		static void DisplayAnswer(string answer)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(answer);
			Console.ResetColor();
		}

		static string Translate(string input, string targetLanguage)
		{
			

			Response<IReadOnlyList<TranslatedTextItem>> response = translationClient.Translate(targetLanguage, input);
			IReadOnlyList<TranslatedTextItem> translations = response.Value;

			detectedLanguage = translations.FirstOrDefault()?.DetectedLanguage?.Language;
			string output = translations.FirstOrDefault().Translations?.FirstOrDefault()?.Text;

			return output;
		
		}

	}
}