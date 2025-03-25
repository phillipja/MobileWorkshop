using Graph;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class PersonUtility : EditorWindow
{
    const string ASSETS_PATH = "Assets/";
    const string PATH_ENDING = "/";
    const string NAME_PREFIX = "Person_";
    const string FILE_EXTENTION = ".asset";

    const string FILE_PARSE = "Name:";

    const string PERSON_NAME = @"Name:\s+(\w+)?\s+(\w+)\s";
    const string PERSON_SUBSET = @"Subset:\s+\{(\w\d+)?,\s+(\w\d+)?,\s+(\w\d+)?}";
    const string PERSON_DESCRIPTION = @"Beschreibung:\s+(.+|\s)";

    private string _path;
    private SO_Graph _graph;
    private bool _useFile;

    private TextAsset _textFile;
    private string _content;
    private GUIStyle _style;

    [MenuItem("Graph/PersonUtility")]
    public static void ShowWindow()
    {
        GetWindow<PersonUtility>("Person Utility");
    }

    private void OnEnable()
    {
        _style = new(EditorStyles.textArea)
        {
            wordWrap = true,
        };
    }

    private void OnGUI()
    {
        _path = EditorGUILayout.TextField("Path", _path);
        _graph = EditorGUILayout.ObjectField("Graph", _graph, typeof(SO_Graph), false) as SO_Graph;
        _useFile = EditorGUILayout.Toggle("Textdatei Verwenden", _useFile);

        if(_useFile)
        {
            _textFile = (TextAsset)EditorGUILayout.ObjectField("Text Datei", _textFile, typeof(TextAsset), false);
        }
        else
        {
            EditorGUILayout.LabelField("Personenbeschreibung");
            _content = EditorGUILayout.TextArea(_content, _style);
        }

        if(GUILayout.Button("Generate Person"))
        {
            if(_graph == null)
            {
                Debug.LogError("Please select a graph.");
                return;
            }

            if(_useFile)
            {
                if(_textFile == null)
                {
                    Debug.LogError("Please select a text asset.");
                    return;
                }

                ParseFileContentsToPersons();
            }
            else
            {
                if(string.IsNullOrWhiteSpace(_content))
                {
                    Debug.LogError("Please enter a valid text.");
                    return;
                }

                WritePersonData(_content);
            }
        }
    }

    private void ParseFileContentsToPersons()
    {
        string[] fileContents = _textFile.text.Split(FILE_PARSE, System.StringSplitOptions.RemoveEmptyEntries);
        foreach(var content in fileContents)
        {
            WritePersonData($"{FILE_PARSE}{content}");
        }
    }

    private void WritePersonData(string content)
    {
        SO_Person person = CreateInstance<SO_Person>();

        Match nameMatch = Regex.Match(content, PERSON_NAME);
        person.forename = nameMatch.Groups[1].Value.Trim();
        person.surname = nameMatch.Groups[2].Value.Trim();

        person.nodes.Clear();
        Match subsetMatch = Regex.Match(content, PERSON_SUBSET);
        for(int i = 1; i < subsetMatch.Groups.Count; i++)
        {
            SO_GraphNode node = _graph.nodes.FirstOrDefault(n => n.id.Equals(subsetMatch.Groups[i].Value.Trim()));
            if(node != null)
                person.nodes.Add(node);
        }

        Match descriptionMatch = Regex.Match(content, PERSON_DESCRIPTION);
        person.description = descriptionMatch.Groups[1].Value.Trim();

        person.name = $"{NAME_PREFIX}{person.forename}{person.surname}";
        ValidatePath();
        string path = $"{ASSETS_PATH}{_path}{person.name}{FILE_EXTENTION}";
        AssetDatabase.CreateAsset(person, path);
    }

    private void ValidatePath()
    {
        if(string.IsNullOrWhiteSpace(_path) == false
            && _path.EndsWith(PATH_ENDING) == false)
        {
            _path += PATH_ENDING;
        }
    }
}
