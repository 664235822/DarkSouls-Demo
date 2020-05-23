using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace Gaia
{
    /// <summary>
    /// This class implements a Django-like simple templating system for creating clean dynamic html or other text
    /// without using asp.net server controls or string builders that require html and other text to be written
    /// and escaped in .cs files.
    /// 
    /// The template is very high performance, since it indexes a template once (lazy loading the first time)
    /// and uses a string builder insert after that to keep memory use and copy time to a minimum.
    /// 
    /// Example text file:
    /// "Hey {{ Name }}, what's up?"
    /// Example Template.ToString() output after calling Set("Name", "Craig"); :
    /// "Hey Craig, what's up?"
    /// </summary>
    public class Template
    {
        public string FilePath;
        public TemplateFrame Frame;

        private Dictionary<string, TemplateValue> Variables;

        /// <summary>
        /// Passing debug = true in ensures template modifications are noticed.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="debug"></param>
        public Template(string filePath, bool debug)
        {
            this.FilePath = filePath;
            if (this.Frame == null && this.findPreviouslyBuiltFrame() == false)
            {
                Frame = new TemplateFrame(filePath, debug);
            }
            Variables = new Dictionary<string, TemplateValue>();
        }

        private bool findPreviouslyBuiltFrame()
        {
            foreach (TemplateFrame htf in TemplateFrames.List)
            {
                if (htf.FilePath == this.FilePath)
                {
                    this.Frame = htf;
                    return true;
                }
            }
            return false;
        }

        public void Set(string name, string value)
        {
            if (value == null)
            {
                value = "";
            }

            if (this.Variables.ContainsKey(name))
            {
                this.Variables[name].Value = value;
            }
            else
            {
                this.Variables.Add(name, new TemplateValue(value, this.Frame.Variables[name]));
            }
        }

        private int[] CopyIndicies(TemplateFrameVariable tfv)
        {
            int[] indexArry = new int[tfv.Indicies.Count];
            tfv.Indicies.CopyTo(indexArry);
            return indexArry;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(this.Frame.Text);

            foreach (TemplateValue tv in this.Variables.Values)
            {
                // Copy indicies so that the global TemplateFrame stays unchanged when values are inserted and indicies need to be shifted. 
                tv.Indicies = CopyIndicies(tv.FrameVar);
            }
            foreach (TemplateValue tv in this.Variables.Values)
            {
                for (int j = 0; j < tv.Indicies.Length; j++)
                {
                    stringBuilder.Insert(tv.Indicies[j], tv.Value);
                    this.UpdateIndicies(tv.FrameVar.Positions[j], tv.Value.Length);
                }
            }
            return stringBuilder.ToString();
        }

        private void UpdateIndicies(int position, int length)
        {
            foreach (TemplateValue tv in this.Variables.Values)
            {
                for (int i = 0; i < tv.FrameVar.Positions.Count; i++)
                {
                    if (tv.FrameVar.Positions[i] > position)
                    {
                        tv.Indicies[i] = tv.Indicies[i] + length;
                    }
                }
            }
        }
    }

    public class TemplateValue
    {
        public string Value;
        public int[] Indicies;
        public readonly TemplateFrameVariable FrameVar;

        public TemplateValue(string value, TemplateFrameVariable fv)
        {
            this.Value = value;
            this.FrameVar = fv;
        }
    }
}
