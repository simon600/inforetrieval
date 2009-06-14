﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Parser
{
    /// <summary>
    /// Identify stop words according to stop-words list
    /// </summary>
    public class StopWords
    {
        /// <summary>
        /// Check if word belongs to stop-words list
        /// </summary>
        /// <param name="word">Word to check</param>
        /// <returns>True if word is stopword, false otherwise</returns>
        public static bool IsStopWord(string word)
        {
            int i = Array.BinarySearch(msStopWords, word);
            return ( i>=0 );
        }

        /// <summary>
        /// List of stop words. It must be sorted.
        /// </summary>
        private static string[] msStopWords = {
          "a", "aby", "acz", "aczkolwiek", "ale", "ależ", "aż", "bardziej", "bardzo", "bez", "bo",
          "bowiem", "by", "byli", "być", "był", "była", "było", "były", "będzie", "będą", "cali", "cała",
          "cały", "co", "cokolwiek", "coś", "czasami", "czasem", "czemu", "czy", "czyli", "dla", "dlaczego",
          "dlatego", "do", "gdy", "gdyż", "gdzie", "gdziekolwiek", "gdzieś", "go", "i", "ich", "ile", "im",
          "inna", "inne", "inny", "innych", "iż", "ja", "jak", "jakaś", "jakichś", "jakie", "jakiś", "jakiż",
          "jakkolwiek", "jako", "jakoś", "jednak", "jednakże", "jego", "jej", "jest", "jeszcze", "jeśli",
          "jeżeli", "już", "ją", "kiedy", "kilka", "kimś", "kto", "ktokolwiek", "ktoś", "który", "która",
          "które", "którego", "której", "który", "których", "którym", "którzy", "lat", "lecz", "lub", "ma",
          "mają", "mi", "mimo", "między", "mnie", "mogą", "moim", "może", "możliwe", "można", "mu", "musi", "na",
          "nad", "nam", "nas", "naszego", "naszych", "natomiast", "nawet", "nic", "nich", "nie", "nigdy", "nim",
          "niż", "no", "o", "obok", "od", "około", "on", "oni", "one", "ono", "oraz", "pan", "pana", "pani", 
          "po", "pod", "podczas", "pomimo", "ponad", "ponieważ", "powinien", "powinna", "powinni", "powinno",
          "poza", "prawie", "przecież", "przed", "przede", "przez", "przy", "roku", "również", "się", "sobie",
          "sobą", "sposób", "swoje", "są", "ta", "tak", "taka", "taki", "takie", "także", "tam", "te", "tego",
          "tej", "ten", "teraz", "też", "to", "tobie", "toteż", "trzeba", "tu", "twoim", "twoja", "twoje", "twym",
          "twój", "ty", "tych", "tylko", "tym", "u", "w", "we", "według", "wiele", "wielu", "więc", "więcej",
          "wszyscy", "wszystkich", "wszystkie", "wszystkim", "wszystko", "właśnie", "z", "za", "zapewne",
          "zawsze", "ze", "znowu", "znów", "został", "zza", "żadna", "żadne", "żadnych", "że", "żeby"
        };

    }
}
