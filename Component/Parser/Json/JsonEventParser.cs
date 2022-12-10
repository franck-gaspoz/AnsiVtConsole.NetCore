
using Newtonsoft.Json;

namespace AnsiVtConsole.NetCore.Component.Parser.Json
{
    public delegate void AddObjectDelegate(List<(string name, object value)> properties);

    public delegate void AddPropertyDelegate(string name, object value);

    public delegate void DebugDelegate(string s);

    internal sealed class JsonEventParser
    {
        public List<(string name, object value)> ReadJSonObjectFromFile(
            string path,
            AddObjectDelegate addObjectDelegate = null,
            AddPropertyDelegate addPropertyDelegate = null,
            DebugDelegate debugDelegate = null
            )
        {
            using var fileReader = File.OpenText(path);
            var reader = new JsonTextReader(fileReader);
            var varpath = new List<string>();
            var lastPropertyName = "";

            return ReadJSonObject(
                reader,
                varpath,
                ref lastPropertyName,
                addObjectDelegate,
                addPropertyDelegate,
                debugDelegate
                );
        }

        public List<(string name, object value)> ReadJSonObjectFromText(
            string text,
            AddObjectDelegate addObjectDelegate = null,
            AddPropertyDelegate addPropertyDelegate = null,
            DebugDelegate debugDelegate = null
            )
        {
            using var textReader = new StringReader(text);
            var reader = new JsonTextReader(textReader);
            var varpath = new List<string>();
            var lastPropertyName = "";

            return ReadJSonObject(
                reader,
                varpath,
                ref lastPropertyName,
                addObjectDelegate,
                addPropertyDelegate,
                debugDelegate
                );
        }

        private List<(string name, object value)> ReadJSonObject(
            JsonTextReader reader,
            List<string> path,
            ref string lastPropertyName,
            AddObjectDelegate addObjectDelegate = null,
            AddPropertyDelegate addPropertyDelegate = null,
            DebugDelegate debugDelegate = null
            )
        {
            static string Path(List<string> s) => string.Join(".", s.Skip(1));
            static string PathVar(List<string> s, string p) => string.Join(".", s.Append(p).Skip(1));

            var end = false;
            var inArray = false;
            var properties = new List<(string name, object value)>();
            var allProperties = new List<(string name, object value)>();
            var currentSequence = new List<object>();
            while (!end && reader.Read())
            {
                debugDelegate?.Invoke(string.Format("Token: {0}, Value: {1}", reader.TokenType, reader.Value));

                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                        if (lastPropertyName != null)
                        {
                            var newPath = path.Append(lastPropertyName).ToList();
                            lastPropertyName = null;

                            debugDelegate?.Invoke("object: " + Path(newPath));

                            allProperties.AddRange(
                                ReadJSonObject(
                                    reader,
                                    newPath,
                                    ref lastPropertyName,
                                    addObjectDelegate,
                                    addPropertyDelegate,
                                    debugDelegate
                                    ));
                        }
                        break;
                    case JsonToken.StartArray:
                        inArray = true;
                        currentSequence.Clear();
                        break;
                    case JsonToken.EndArray:
                        var tprop = (PathVar(path, lastPropertyName), currentSequence.ToArray());
                        properties.Add(tprop);
                        allProperties.Add(tprop);
                        lastPropertyName = null;
                        inArray = false;
                        break;
                    case JsonToken.PropertyName:
                        lastPropertyName = (string)reader.Value;
                        break;
                    case JsonToken.Boolean:
                    case JsonToken.Bytes:
                    case JsonToken.Date:
                    case JsonToken.Float:
                    case JsonToken.Integer:
                    case JsonToken.Null:
                    case JsonToken.String:
                        if (!inArray)
                        {
                            var prop = (PathVar(path, lastPropertyName), reader.Value);
                            properties.Add(prop);
                            allProperties.Add(prop);
                            lastPropertyName = null;
                            addPropertyDelegate?.Invoke(prop.Item1, prop.Value);
                        }
                        else
                        {
                            currentSequence.Add(reader.Value);
                        }
                        break;
                    case JsonToken.EndObject:
                        end = true;
                        break;
                }
            }

            if (debugDelegate != null)
            {
                foreach (var (name, value) in properties)
                    debugDelegate?.Invoke($"{Path(path)}.{name} = {value}");
            }

            addObjectDelegate?.Invoke(properties);
            return allProperties;
        }
    }
}
