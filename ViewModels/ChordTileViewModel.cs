using ChordPathFinder.Models;
using ChordPathFinder.Services;
using System.Windows.Input;

namespace ChordPathFinder.ViewModels;

public sealed class ChordTileViewModel : ViewModelBase
{
    private readonly ChordDefinition _definition;
    private bool _isExpanded;
    private bool _isAlphanumeric;

    public ChordTileViewModel(ChordDefinition definition)
    {
        _definition = definition;
        ToggleCommand = new RelayCommand(_ => IsExpanded = !IsExpanded);
    }

    public string Name => NotationMapper.ToDisplay(_definition.Name, _isAlphanumeric);

    public string ExpandedText => $"{NotationMapper.ToDisplay(_definition.Name, _isAlphanumeric)} -> {string.Join(", ", _definition.Transitions.Select(x => NotationMapper.ToDisplay(x, _isAlphanumeric)))}";

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
            {
                return;
            }

            _isExpanded = value;
            OnPropertyChanged();
        }
    }

    public ICommand ToggleCommand { get; }

    public void SetAlphanumericMode(bool isAlphanumeric)
    {
        if (_isAlphanumeric == isAlphanumeric)
        {
            return;
        }

        _isAlphanumeric = isAlphanumeric;
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(ExpandedText));
    }
}
