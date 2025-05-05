using UnityEngine;
using UnityEngine.UI;

public class EvaluationSaverUI : MonoBehaviour
{
    [SerializeField] private Button _togglePanelButton;
    [SerializeField] private GameObject _controlsPanel;
    [SerializeField] private Button _closeButton;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Button _loadButton;
    [SerializeField] private EvaluationSaver _evaluationSaver;

    private void Start()
    {
        if (_togglePanelButton != null)
            _togglePanelButton.onClick.AddListener(OnTogglePanelClicked);

        if (_closeButton != null)
            _closeButton.onClick.AddListener(OnClosePanelClicked);

        if (_controlsPanel != null)
            _controlsPanel.SetActive(false);

        if (_saveButton != null)
            _saveButton.onClick.AddListener(OnSaveClicked);

        if (_loadButton != null)
            _loadButton.onClick.AddListener(OnLoadClicked);
    }

    private void OnDestroy()
    {
        if (_togglePanelButton != null)
            _togglePanelButton.onClick.RemoveListener(OnTogglePanelClicked);

        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(OnClosePanelClicked);

        if (_saveButton != null)
            _saveButton.onClick.RemoveListener(OnSaveClicked);

        if (_loadButton != null)
            _loadButton.onClick.RemoveListener(OnLoadClicked);
    }

    private void OnTogglePanelClicked()
    {
        if (_controlsPanel != null)
        {
            _controlsPanel.SetActive(!_controlsPanel.activeSelf);
        }
    }

    private void OnClosePanelClicked()
    {
        if (_controlsPanel != null)
        {
            _controlsPanel.SetActive(false);
        }
    }

    private void OnSaveClicked()
    {
        _evaluationSaver.SaveToJson();
    }

    private void OnLoadClicked()
    {
        _evaluationSaver.LoadFromJson();
    }
}