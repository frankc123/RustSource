using System;
using System.Collections.Generic;
using System.Text;

public class dfMarkupEntity
{
    private static StringBuilder buffer = new StringBuilder();
    public string EntityChar;
    public string EntityName;
    private static List<dfMarkupEntity> HTML_ENTITIES = new List<dfMarkupEntity> { new dfMarkupEntity("&nbsp;", " "), new dfMarkupEntity("&quot;", "\""), new dfMarkupEntity("&amp;", "&"), new dfMarkupEntity("&lt;", "<"), new dfMarkupEntity("&gt;", ">"), new dfMarkupEntity("&#39;", "'"), new dfMarkupEntity("&trade;", "™"), new dfMarkupEntity("&copy;", "\x00a9"), new dfMarkupEntity("\x00a0", " ") };

    public dfMarkupEntity(string entityName, string entityChar)
    {
        this.EntityName = entityName;
        this.EntityChar = entityChar;
    }

    public static string Replace(string text)
    {
        buffer.EnsureCapacity(text.Length);
        buffer.Length = 0;
        buffer.Append(text);
        for (int i = 0; i < HTML_ENTITIES.Count; i++)
        {
            dfMarkupEntity entity = HTML_ENTITIES[i];
            buffer.Replace(entity.EntityName, entity.EntityChar);
        }
        return buffer.ToString();
    }
}

