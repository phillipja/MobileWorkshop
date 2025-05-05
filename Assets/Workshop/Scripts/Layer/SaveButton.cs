using UnityEngine;
using UnityEngine.UI;

public class SaveButton : MonoBehaviour
{
    [SerializeField] private Button _saveButton;
    [SerializeField] private EvaluationSaver _evaluationSaver;

    private void Start()
    {
        if (_saveButton == null)
        {
            _saveButton = GetComponent<Button>();
        }

        _saveButton.onClick.AddListener(OnSaveButtonClicked);
    }

    private void OnDestroy()
    {
        _saveButton.onClick.RemoveListener(OnSaveButtonClicked);
    }

    private void OnSaveButtonClicked()
    {
        _evaluationSaver.SaveToJson();
    }
}