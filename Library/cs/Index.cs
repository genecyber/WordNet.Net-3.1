/*
 * This file is a part of the WordNet.Net open source project.
 * 
 * Copyright (C) 2005 Malcolm Crowe, Troy Simpson 
 * 
 * Project Home: http://www.ebswift.com
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

using System;

namespace Wnlib
{
	/// <summary>
	/// Summary description for Index.
	/// </summary>
	public class Index
	{
		public PartOfSpeech PartOfSpeech = null;
		public string Wd;
		public int SenseCnt = 0;		/* sense (collins) count */
		public PointerType[] Ptruse; /* pointer data in index file */
		public int TaggedSensesCount;	/* number senses that are tagged */
		public int[] SynsetOffsets;		/* synset offsets */
		public SynSet[] Syns;   /* cached */
		public Index Next;

		public void Print()
		{
			Console.Write(PartOfSpeech.name + " " + SenseCnt + " ");
			for (int j = 0; j < Ptruse.Length; j++)
				Console.Write(Ptruse[j].Mnemonic + " ");
			Console.WriteLine();
			if (Next != null)
				Next.Print();
		}

		/* From search.c:
		 * Find word in index file and return parsed entry in data structure.
		   Input word must be exact match of string in database. */

		// From the WordNet Manual (http://wordnet.princeton.edu/man/wnsearch.3WN.html)
		// index_lookup() finds searchstr in the index file for pos and returns a pointer 
		// to the parsed entry in an Index data structure. searchstr must exactly match the 
		// form of the word (lower case only, hyphens and underscores in the same places) in 
		// the index file. NULL is returned if a match is not found.
		public static Index lookup(string word, PartOfSpeech pos)
		{
			int j;
			if (word == "")
				return null;
			// TDMS 14 Aug 2005 - changed to allow for numbers as well
			// because the database contains searches that can start with
			// numerals
			//if (!char.IsLetter(word[0]))
			if (!char.IsLetter(word[0]) && !char.IsNumber(word[0]))
				return null;
			string line = WNDB.binSearch(word, pos);
			if (line == null)
				return null;
			Index idx = new Index();
			StrTok st = new StrTok(line);
			idx.Wd = st.next(); /* the word */
			idx.PartOfSpeech = PartOfSpeech.of(st.next()); /* the part of speech */
			idx.SenseCnt = int.Parse(st.next()); /* collins count */
			int ptruse_cnt = int.Parse(st.next()); /* number of pointers types */
			idx.Ptruse = new PointerType[ptruse_cnt];
			for (j = 0; j < ptruse_cnt; j++)
				idx.Ptruse[j] = PointerType.of(st.next());
			int off_cnt = int.Parse(st.next());
			idx.SynsetOffsets = new int[off_cnt];
			idx.TaggedSensesCount = int.Parse(st.next());
			for (j = 0; j < off_cnt; j++)
				idx.SynsetOffsets[j] = int.Parse(st.next());
			return idx;
		}

		public bool HasHoloMero(string s, Search search)
		{
			return HasHoloMero(PointerType.of(s), search);
		}

		public bool HasHoloMero(PointerType p, Search search)
		{
			PointerType pbase;
			if (p.Mnemonic == "HMERONYM")
				pbase = PointerType.of("HASMEMBERPTR");
			else
				pbase = PointerType.of("ISMEMBERPTR");
			for (int i = 0; i < SynsetOffsets.Length; i++)
			{
				SynSet s = new SynSet(SynsetOffsets[i], PartOfSpeech.of("noun"), "", search, 0);
				if (s.has(pbase) || s.has(pbase + 1) || s.has(pbase + 2))
					return true;
			}
			return false;
		}
	}
}
