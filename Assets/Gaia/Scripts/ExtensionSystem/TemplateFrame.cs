using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Gaia
{
    public static class TemplateFrames
    {
        public static List<TemplateFrame> List = new List<TemplateFrame>();
    }

    public class TemplateFrameVariable
    {
        public List<int> Indicies;
        public List<int> Positions;
        public TemplateFrameVariable(List<int> indicies, List<int> positions)
        {
            this.Indicies = indicies;
            this.Positions = positions;
        }
    }

    /// <summary>
    /// This class deals with the static part of the Template, the template variable names, indicies, and positions.
    /// The static part is separated so that the template can be parsed once and used multiple times by multiple
    /// requests.
    /// </summary>
    public class TemplateFrame
    {
        public Dictionary<string, TemplateFrameVariable> Variables = new Dictionary<string, TemplateFrameVariable>();
        public string FilePath;
        public string Text;

        private const char BeginChar = '{';
        private const char EndChar = '}';
        private int VariableCount = 0;

        public TemplateFrame(string filePath, bool debug)
        {
            this.FilePath = filePath;
            this.Text = this.Build(filePath);
            if (!debug)
            {
                TemplateFrames.List.Add(this);
            }
        }

        public string Build(string filePath)
        {
            // Parse the template into a StringBuilder and TemplateFrameVariable array
            char[] text = GaiaCommon1.Utils.ReadAllText(filePath).ToCharArray();
            string varName;
            int startIndex;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == BeginChar && text[i + 1] == BeginChar)
                {
                    startIndex = i;
                    i = i + 2;
                    varName = getVarName(text, ref i);
                    if (varName != null)
                    {
                        if (!this.Variables.ContainsKey(varName))
                        {
                            this.Variables.Add(varName, new TemplateFrameVariable(new List<int>(), new List<int>()));
                        }
                        this.Variables[varName].Indicies.Add(startIndex);
                        this.Variables[varName].Positions.Add(VariableCount++);
                    }
                    text = shiftCharArryLeft(text, startIndex, i);
                    i = startIndex - 1; // Minus 1 since new characters have been shifted to start index and there could be consecutive variables.
                }
            }
            return new string(text);
        }

        /// <summary>
        /// Shifts left from endIndex to startIndex.
        /// </summary>
        private char[] shiftCharArryLeft(char[] arry, int startIndex, int endIndex)
        {
            char[] newArry = new char[arry.Length - (endIndex - startIndex + 1)];

            for (int i = 0; i < startIndex; i++)
            {
                newArry[i] = arry[i];
            }

            int j = 0;
            for (int i = endIndex + 1; i < arry.Length; i++)
            {
                newArry[startIndex + j] = arry[i];
                j++;
            }
            return newArry;
        }

        private string getVarName(char[] text, ref int pos)
        {
            char[] ret;
            int startIndex;
            int endIndex;

            pos = skipSpaces(text, pos);

            startIndex = pos;
            while (text[pos] != ' ')
            {
                pos++;
            }
            endIndex = pos;

            ret = new char[endIndex - startIndex];

            for (int i = 0; i < ret.Length; i++)
            {
                ret[i] = text[startIndex + i];
            }

            pos = skipSpaces(text, pos);

            if (text[pos] != EndChar && text[++pos] != EndChar)
            {
                return null;
            }

            pos++;
            return new string(ret);
        }

        private static int skipSpaces(char[] text, int pos)
        {
            while (text[pos] == ' ')
            {
                pos++;
            }
            return pos;
        }
    }
}
