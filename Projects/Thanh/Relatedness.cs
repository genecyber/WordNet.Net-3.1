/* Relatedness Search Helper
 * Author : Dao Ngoc Thanh , thanh.dao@gmx.net 
 * (c) Dao Ngoc Thanh, 2005
 */

using Wnlib;

namespace WordsMatching
{
    class Relatedness
    {
        static readonly Tokeniser Tokenize = new Tokeniser();

        static string[] GetAllDefinitionTokens(Search se)
        {
            string rels = "";
            if (se.senses[0].senses != null)
                foreach (SynSet ss in se.senses[0].senses)
                {
                    foreach (var ww in ss.words)
                        rels += " " + ww.word;
                    rels += ss.defn;
                }

            string[] toks = Tokenize.Partition(rels);
            return toks;
        }

        static string[] GetSynsetDefinition(SynSet sense)
        {
            if (sense == null) return null;
            var gloss = sense.defn;
            foreach (Lexeme word in sense.words)            
                gloss += " " + word.word;
            
            string[] toks = Tokenize.Partition(gloss);
            return toks;
        }
        /// <summary>
        /// Gets the relatedness.
        /// </summary>
        /// <param name="pos">The pos.</param>
        /// <returns>
        /// Return list of option for searching relatedness correspond to pos
        /// E.g hypo, hyper of noun
        /// tropo of verb.
        /// </returns>
        public static Opt[] GetRelatedness(PartsOfSpeech pos)
        {
			switch (pos)
			{
					case PartsOfSpeech.Noun:
					{
                        var nounRelatedness = new[] { Opt.at(8), //hyper
												  Opt.at(14), //holo
												  Opt.at(19), //mero
												  Opt.at(12) //hypo												
											  };

						return  nounRelatedness;						
					}
					case PartsOfSpeech.Verb:
					{
                        var verbRelatedness = new[] {
												  Opt.at(31),//hyper
												  Opt.at(36)//tropo // may be 38
											  };
                        return verbRelatedness;						
    				}
					case PartsOfSpeech.Adj:
					{
                        var adjectiveRelatedness = new[] {
													   Opt.at(0)												  
												   };

                        return adjectiveRelatedness;
					}
					case PartsOfSpeech.Adv:
					{
                        var advebRelatedness = new[] {
												       Opt.at(48)												  
											   };
                        return advebRelatedness;
					}				

			}

            return null; 
        }

        /// <summary>
        /// This function is to retrieve all relatedness information of given word, which
        /// will be used for the WSD task or a lesk relatedness measurement.
        /// </summary>
        /// <param name="word"> entry word</param>
        /// <param name="senseCount"> total sense of this word</param>
        /// <param name="relatednessTypes"> searching for relatedness that is specific partOfSpeech of given word</param>
        /// <returns>Return a three dimensions array:
        /// 1. SenseIndex.
        /// 2. Kind of relatedness. e.g : Hypernymy, Holonymy
        /// 3. Tokens list.
        /// </returns>
        public static string[][][] GetAllRelatednessData(string word, int senseCount, Opt[] relatednessTypes)
        {
            if (relatednessTypes == null) return null;
            string[][][] matrix = new string[senseCount][][];
            for (int i = 0; i < senseCount; i++)
            {
                matrix[i] = GetRelatednessGlosses(word, i + 1, relatednessTypes);
            }

            return matrix;
        }

        /// <summary>
        /// Gets the relatedness glosses.
        /// </summary>
        /// <param name="word">The word.</param>
        /// <param name="senseNumber">The sense number.</param>
        /// <param name="relatednessTypes">The relatedness types.</param>
        /// <returns></returns>
        public static string[][] GetRelatednessGlosses(string word, int senseNumber, Opt[] relatednessTypes)
        {
            var relations = new string[relatednessTypes.Length + 1][];

            for (int i = 0; i < relatednessTypes.Length; i++)
            {
                var relateness = relatednessTypes[i];
                var se = new Search(word, true, relateness.pos, relateness.sch, senseNumber);							
                if (se.senses != null && se.senses.Count > 0)
                {
                    if (relations[0] == null)
                        relations[0] = GetSynsetDefinition(se.senses[0]);
                    if (se.senses[0].senses != null)
                        relations[i + 1] = GetAllDefinitionTokens(se);

                }
                else relations[i + 1] = null;
            }

            return relations;
        }


    }
}
