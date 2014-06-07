namespace Facepunch.Load
{
    using LitJson;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;

    public sealed class Reader : Stream
    {
        [CompilerGenerated]
        private static Dictionary<string, int> <>f__switch$map4;
        private bool disposed;
        private readonly bool disposesTextReader;
        private bool insideOrderList;
        private bool insideRandomList;
        private Facepunch.Load.Item item;
        private JsonReader json;
        private readonly string prefix;
        private Facepunch.Load.Token token;

        private Reader(JsonReader json, string bundlePath) : this(json, bundlePath, false)
        {
        }

        private Reader(TextReader reader, string bundlePath) : this(new JsonReader(reader), bundlePath, false)
        {
        }

        private Reader(string text, string bundlePath) : this(new JsonReader(text), bundlePath, true)
        {
        }

        private Reader(JsonReader json, string bundlePath, bool createdForThisInstance)
        {
            if (json == null)
            {
                throw new ArgumentNullException("json");
            }
            this.json = json;
            this.disposesTextReader = createdForThisInstance;
            this.prefix = bundlePath;
            if (string.IsNullOrEmpty(this.prefix))
            {
                this.prefix = string.Empty;
            }
            else
            {
                switch (this.prefix[this.prefix.Length - 1])
                {
                    case '/':
                    case '\\':
                        return;
                }
                this.prefix = this.prefix + "/";
            }
        }

        public static Reader CreateFromFile(string openFilePath)
        {
            return CreateFromFile(openFilePath, Path.GetDirectoryName(openFilePath));
        }

        public static Reader CreateFromFile(string openFilePath, string bundlePath)
        {
            return new Reader(new JsonReader(File.OpenText(openFilePath)), bundlePath, true);
        }

        public static Reader CreateFromReader(JsonReader textReader, string bundlePath)
        {
            return new Reader(textReader, bundlePath);
        }

        public static Reader CreateFromReader(TextReader textReader, string bundlePath)
        {
            return new Reader(textReader, bundlePath);
        }

        public static Reader CreateFromText(string jsonText, string bundlePath)
        {
            return new Reader(jsonText, bundlePath);
        }

        public override void Dispose()
        {
            if (!this.disposed)
            {
                if (!this.disposesTextReader)
                {
                    while ((this.token != Facepunch.Load.Token.End) && (this.token != Facepunch.Load.Token.DownloadQueueEnd))
                    {
                        try
                        {
                            this.Read();
                        }
                        catch (JsonException)
                        {
                            this.token = Facepunch.Load.Token.End;
                        }
                    }
                }
                else
                {
                    try
                    {
                        this.json.Dispose();
                    }
                    catch (ObjectDisposedException)
                    {
                    }
                }
                this.json = null;
                this.disposed = true;
            }
        }

        private static Type ParseType(string str)
        {
            Type type = Type.GetType(str, false, true);
            if (type != null)
            {
                return type;
            }
            type = Type.GetType("Facepunch.MeshBatch." + str, false, true);
            if (type != null)
            {
                return type;
            }
            return Type.GetType(str, true, true);
        }

        private string PathToBundle(string incomingPathFromJson)
        {
            if ((!incomingPathFromJson.Contains("//") && !incomingPathFromJson.Contains(":/")) && (!incomingPathFromJson.Contains(@":\") && !Path.IsPathRooted(incomingPathFromJson)))
            {
                return (this.prefix + incomingPathFromJson);
            }
            return incomingPathFromJson;
        }

        public bool Read()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException("Reader");
            }
            this.item = new Facepunch.Load.Item();
            if (!this.json.Read())
            {
                this.token = Facepunch.Load.Token.End;
                return false;
            }
            if (!this.insideOrderList)
            {
                switch (this.json.Token)
                {
                    case JsonToken.None:
                        this.token = Facepunch.Load.Token.End;
                        return false;

                    case JsonToken.ArrayStart:
                        this.token = Facepunch.Load.Token.DownloadQueueBegin;
                        this.insideOrderList = true;
                        return true;
                }
            }
            else if (this.insideRandomList)
            {
                switch (this.json.Token)
                {
                    case JsonToken.PropertyName:
                        this.token = Facepunch.Load.Token.BundleListing;
                        this.ReadBundleListing(this.json.Value.AsString);
                        return true;

                    case JsonToken.ObjectEnd:
                        this.token = Facepunch.Load.Token.RandomLoadOrderAreaEnd;
                        this.insideRandomList = false;
                        return true;
                }
            }
            else
            {
                switch (this.json.Token)
                {
                    case JsonToken.ObjectStart:
                        this.token = Facepunch.Load.Token.RandomLoadOrderAreaBegin;
                        this.insideRandomList = true;
                        return true;

                    case JsonToken.ArrayEnd:
                        this.token = Facepunch.Load.Token.DownloadQueueEnd;
                        this.insideOrderList = false;
                        return true;
                }
            }
            throw new JsonException("Bad json state");
        }

        private void ReadBundleListing(string nameOfBundle)
        {
            if (!this.json.Read())
            {
                throw new JsonException("End of stream unexpected");
            }
            if (this.json.Token != JsonToken.ObjectStart)
            {
                throw new JsonException("Expected object start for bundle name (property) " + nameOfBundle);
            }
            this.item.Name = nameOfBundle;
            this.item.ByteLength = -1;
        Label_0055:
            if (!this.json.Read())
            {
                throw new JsonException("Unexpected end of stream");
            }
            if (this.json.Token == JsonToken.ObjectEnd)
            {
                if (string.IsNullOrEmpty(this.item.Path))
                {
                    throw new JsonException("Path to bundle not defined for bundle listing " + nameOfBundle);
                }
                if (this.item.ByteLength == -1)
                {
                    throw new JsonException("There was no size property for bundle listing " + nameOfBundle);
                }
                ContentType contentType = this.item.ContentType;
                if (contentType != ContentType.Assets)
                {
                    if (contentType == ContentType.Scenes)
                    {
                        if (this.item.TypeOfAssets != null)
                        {
                            throw new JsonException("There should not have been a type property for scene bundle listing " + nameOfBundle);
                        }
                        return;
                    }
                    object[] objArray1 = new object[] { "The content ", this.item.ContentType, " was not handled for bundle listing ", nameOfBundle };
                    throw new JsonException(string.Concat(objArray1));
                }
                if (this.item.TypeOfAssets == null)
                {
                    throw new JsonException("There was no valid type property for asset bundle listing " + nameOfBundle);
                }
            }
            else
            {
                if (this.json.Token != JsonToken.PropertyName)
                {
                    throw new JsonException("Unexpected token in json : JsonToken." + this.json.Token);
                }
                bool flag = false;
                string asString = this.json.Value.AsString;
                if (asString != null)
                {
                    int num;
                    if (<>f__switch$map4 == null)
                    {
                        Dictionary<string, int> dictionary = new Dictionary<string, int>(5);
                        dictionary.Add("type", 0);
                        dictionary.Add("size", 1);
                        dictionary.Add("content", 2);
                        dictionary.Add("filename", 3);
                        dictionary.Add("url", 4);
                        <>f__switch$map4 = dictionary;
                    }
                    if (<>f__switch$map4.TryGetValue(asString, out num))
                    {
                        switch (num)
                        {
                            case 0:
                                if (!this.json.Read())
                                {
                                    throw new JsonException("Unexpected end of stream at type");
                                }
                                switch (this.json.Token)
                                {
                                    case JsonToken.String:
                                        try
                                        {
                                            this.item.TypeOfAssets = ParseType(this.json.Value.AsString);
                                        }
                                        catch (TypeLoadException exception)
                                        {
                                            throw new JsonException(this.json.Value.AsString, exception);
                                        }
                                        goto Label_0055;

                                    case JsonToken.Null:
                                        this.item.TypeOfAssets = null;
                                        goto Label_0055;
                                }
                                throw new JsonException("the type property expects only null or string. got : " + this.json.Token);

                            case 1:
                                if (!this.json.Read())
                                {
                                    throw new JsonException("Unexpected end of stream at size");
                                }
                                switch (this.json.Token)
                                {
                                    case JsonToken.Int:
                                    case JsonToken.Float:
                                        this.item.ByteLength = this.json.Value.AsInt;
                                        goto Label_0055;
                                }
                                throw new JsonException("the size property expects a number. got : " + this.json.Token);

                            case 2:
                                if (!this.json.Read())
                                {
                                    throw new JsonException("Unexpected end of stream at content");
                                }
                                switch (this.json.Token)
                                {
                                    case JsonToken.Int:
                                        this.item.ContentType = (ContentType) ((byte) this.json.Value.AsInt);
                                        goto Label_0055;

                                    case JsonToken.String:
                                        try
                                        {
                                            this.item.ContentType = (ContentType) ((byte) Enum.Parse(typeof(ContentType), this.json.Value.AsString, true));
                                        }
                                        catch (ArgumentException exception2)
                                        {
                                            throw new JsonException(this.json.Value.AsString, exception2);
                                        }
                                        catch (OverflowException exception3)
                                        {
                                            throw new JsonException(this.json.Value.AsString, exception3);
                                        }
                                        goto Label_0055;
                                }
                                throw new JsonException("the content property expects a string or int. got : " + this.json.Token);

                            case 3:
                                if (!this.json.Read())
                                {
                                    throw new JsonException("Unexpected end of stream at filename");
                                }
                                if (this.json.Token != JsonToken.String)
                                {
                                    throw new JsonException("the filename property expects a string. got : " + this.json.Token);
                                }
                                if (!flag)
                                {
                                    try
                                    {
                                        this.item.Path = this.PathToBundle(this.json.Value.AsString);
                                    }
                                    catch (Exception exception4)
                                    {
                                        throw new JsonException(this.json.Value.AsString, exception4);
                                    }
                                }
                                goto Label_0055;

                            case 4:
                                if (!this.json.Read())
                                {
                                    throw new JsonException("Unexpected end of stream at url");
                                }
                                if (this.json.Token != JsonToken.String)
                                {
                                    throw new JsonException("the url property expects a string. got : " + this.json.Token);
                                }
                                try
                                {
                                    this.item.Path = this.json.Value.AsString;
                                }
                                catch (Exception exception5)
                                {
                                    throw new JsonException(this.json.Value.AsString, exception5);
                                }
                                flag = true;
                                goto Label_0055;
                        }
                    }
                }
                throw new JsonException("Unhandled property named " + this.json.Value.AsString);
            }
        }

        public Facepunch.Load.Item Item
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Reader");
                }
                if (this.token != Facepunch.Load.Token.BundleListing)
                {
                    throw new InvalidOperationException("You may only retreive Item when Token is Token.BundleListing!");
                }
                return this.item;
            }
        }

        public Facepunch.Load.Token Token
        {
            get
            {
                if (this.disposed)
                {
                    throw new ObjectDisposedException("Reader");
                }
                return this.token;
            }
        }
    }
}

