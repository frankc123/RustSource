using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

public class dfMarkupParser
{
    private static Regex ATTR_PATTERN = null;
    private dfRichTextLabel owner;
    private static dfMarkupParser parserInstance = new dfMarkupParser();
    private static Regex STYLE_PATTERN = null;
    private static Regex TAG_PATTERN = null;
    private static Dictionary<string, Type> tagTypes = null;

    static dfMarkupParser()
    {
        RegexOptions options = RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;
        TAG_PATTERN = new Regex(@"(\<\/?)(?<tag>[a-zA-Z0-9$_]+)(\s(?<attr>.+?))?([\/]*\>)", options);
        ATTR_PATTERN = new Regex("(?<key>[a-zA-Z0-9$_]+)=(?<value>(\"((\\\\\")|\\\\\\\\|[^\"\\n])*\")|('((\\\\')|\\\\\\\\|[^'\\n])*')|\\d+|\\w+)", options);
        STYLE_PATTERN = new Regex(@"(?<key>[a-zA-Z0-9\-]+)(\s*\:\s*)(?<value>[^;]+)", options);
    }

    public static dfList<dfMarkupElement> Parse(dfRichTextLabel owner, string source)
    {
        try
        {
            parserInstance.owner = owner;
            return parserInstance.parseMarkup(source);
        }
        finally
        {
        }
    }

    private dfMarkupElement parseElement(Queue<dfMarkupElement> tokens)
    {
        dfMarkupElement element = tokens.Dequeue();
        if (element is dfMarkupString)
        {
            return ((dfMarkupString) element).SplitWords();
        }
        dfMarkupTag original = (dfMarkupTag) element;
        if (!original.IsClosedTag && !original.IsEndTag)
        {
            while (tokens.Count > 0)
            {
                dfMarkupElement node = this.parseElement(tokens);
                if (node is dfMarkupTag)
                {
                    dfMarkupTag tag2 = (dfMarkupTag) node;
                    if (tag2.IsEndTag)
                    {
                        if (tag2.TagName == original.TagName)
                        {
                            break;
                        }
                        return this.refineTag(original);
                    }
                }
                original.AddChildNode(node);
            }
        }
        return this.refineTag(original);
    }

    private dfList<dfMarkupElement> parseMarkup(string source)
    {
        Queue<dfMarkupElement> tokens = new Queue<dfMarkupElement>();
        MatchCollection matchs = TAG_PATTERN.Matches(source);
        int startIndex = 0;
        for (int i = 0; i < matchs.Count; i++)
        {
            Match tag = matchs[i];
            if (tag.Index > startIndex)
            {
                dfMarkupString item = new dfMarkupString(source.Substring(startIndex, tag.Index - startIndex));
                tokens.Enqueue(item);
            }
            startIndex = tag.Index + tag.Length;
            tokens.Enqueue(this.parseTag(tag));
        }
        if (startIndex < source.Length)
        {
            dfMarkupString str4 = new dfMarkupString(source.Substring(startIndex));
            tokens.Enqueue(str4);
        }
        return this.processTokens(tokens);
    }

    private void parseStyleAttribute(dfMarkupTag element, string text)
    {
        MatchCollection matchs = STYLE_PATTERN.Matches(text);
        for (int i = 0; i < matchs.Count; i++)
        {
            Match match = matchs[i];
            string name = match.Groups["key"].Value.ToLowerInvariant();
            string str2 = match.Groups["value"].Value;
            element.Attributes.Add(new dfMarkupAttribute(name, str2));
        }
    }

    private dfMarkupElement parseTag(Match tag)
    {
        string tagName = tag.Groups["tag"].Value.ToLowerInvariant();
        if (tag.Value.StartsWith("</"))
        {
            return new dfMarkupTag(tagName) { IsEndTag = true };
        }
        dfMarkupTag element = new dfMarkupTag(tagName);
        string input = tag.Groups["attr"].Value;
        MatchCollection matchs = ATTR_PATTERN.Matches(input);
        for (int i = 0; i < matchs.Count; i++)
        {
            Match match = matchs[i];
            string name = match.Groups["key"].Value;
            string str4 = dfMarkupEntity.Replace(match.Groups["value"].Value);
            if (str4.StartsWith("\""))
            {
                char[] trimChars = new char[] { '"' };
                str4 = str4.Trim(trimChars);
            }
            else if (str4.StartsWith("'"))
            {
                char[] chArray2 = new char[] { '\'' };
                str4 = str4.Trim(chArray2);
            }
            if (!string.IsNullOrEmpty(str4))
            {
                if (name == "style")
                {
                    this.parseStyleAttribute(element, str4);
                }
                else
                {
                    element.Attributes.Add(new dfMarkupAttribute(name, str4));
                }
            }
        }
        if ((tag.Value.EndsWith("/>") || (tagName == "br")) || (tagName == "img"))
        {
            element.IsClosedTag = true;
        }
        return element;
    }

    private dfList<dfMarkupElement> processTokens(Queue<dfMarkupElement> tokens)
    {
        dfList<dfMarkupElement> list = dfList<dfMarkupElement>.Obtain();
        while (tokens.Count > 0)
        {
            list.Add(this.parseElement(tokens));
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] is dfMarkupTag)
            {
                ((dfMarkupTag) list[i]).Owner = this.owner;
            }
        }
        return list;
    }

    private dfMarkupTag refineTag(dfMarkupTag original)
    {
        if (!original.IsEndTag)
        {
            if (tagTypes == null)
            {
                tagTypes = new Dictionary<string, Type>();
                foreach (Type type in Assembly.GetExecutingAssembly().GetExportedTypes())
                {
                    if (typeof(dfMarkupTag).IsAssignableFrom(type))
                    {
                        object[] customAttributes = type.GetCustomAttributes(typeof(dfMarkupTagInfoAttribute), true);
                        if ((customAttributes != null) && (customAttributes.Length != 0))
                        {
                            for (int i = 0; i < customAttributes.Length; i++)
                            {
                                string tagName = ((dfMarkupTagInfoAttribute) customAttributes[i]).TagName;
                                tagTypes[tagName] = type;
                            }
                        }
                    }
                }
            }
            if (tagTypes.ContainsKey(original.TagName))
            {
                Type type2 = tagTypes[original.TagName];
                object[] args = new object[] { original };
                return (dfMarkupTag) Activator.CreateInstance(type2, args);
            }
        }
        return original;
    }
}

