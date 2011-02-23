using System;
using System.Diagnostics;
using System.Text.RegularExpressions ;
using WnLexicon;

namespace WordsMatching
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class DemoTest
	{
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">The args.</param>
		[STAThread]
		static void Main(string[] args)
		{
			// TDMS 21 Sept 2005 - added dictionary path
            Wnlib.WNCommon.Path = "../../../../WordNet-3.0/dict/";
            Test1();
		}

	    static void Test1()
        {
            var semsim = new SentenceSimilarity();

	        Console.WriteLine("Monkey: ");

            float score = semsim.GetScore(
                "monkey",
                "animal");
            Console.WriteLine("animal: " + score);

            score = semsim.GetScore(
            "monkey",
            "mineral");
            Console.WriteLine("mineral: " + score);

            score = semsim.GetScore(
            "monkey",
            "vegetable");
            Console.WriteLine("vegetable: " + score);

            score = semsim.GetScore(
                "what color is an apple",
                "an apple is pink");
            Console.WriteLine("apple:pink: " + score);

            score = semsim.GetScore("Pepsi is being drunk by Shilpa", "Shilpa is drinking pepsi");
            Console.WriteLine("Pepsi is being drunk by Shilpa:Shilpa is drinking pepsi " + score);

            score = semsim.GetScore("Pepsi is being drunk by Shilpa", "Shilpa is drinking pepsi");
            Console.WriteLine("Pepsi is drinking Shilpa:Shilpa is drinking pepsi " + score);

            Console.WriteLine(Lexicon.FindWordInfo("Smiling",true).ToString());

            Console.ReadLine();
        }
	}
}
