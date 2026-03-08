using ChordPathFinder.Models;
using ChordPathFinder.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChordPathFinder.ViewModels;

public enum AppScreen
{
    Blocks,
    FindWay,
    Donation
}

public enum PickerTarget
{
    None,
    Left,
    Right
}

public sealed class MainViewModel : ViewModelBase
{
    private readonly IReadOnlyDictionary<(string start, string end), PathSet> _pathCache;
    private AppScreen _currentScreen = AppScreen.Blocks;
    private PickerTarget _pickerTarget = PickerTarget.None;
    private string? _leftChord;
    private string? _rightChord;
    private bool _isAlphanumericMode;

    public MainViewModel()
    {
        ChordTiles = new ObservableCollection<ChordTileViewModel>(
            ChordGraph.Chords.Select(chord => new ChordTileViewModel(chord)));

        AvailableChords = new ObservableCollection<ChordOption>();

        CreativeResult = new PathResultItem("Creative");
        ShortestResult = new PathResultItem("Shortest");
        SmoothResult = new PathResultItem("Smooth");

        var finder = new PathFinder(ChordGraph.Graph);
        var precalculator = new PathPrecalculator(finder);
        _pathCache = precalculator.BuildCache(ChordGraph.OrderedChordNames);

        ShowDonationCommand = new RelayCommand(_ => CurrentScreen = AppScreen.Donation);
        ShowFindWayCommand = new RelayCommand(_ => CurrentScreen = AppScreen.FindWay);
        ShowBlocksCommand = new RelayCommand(_ => CurrentScreen = AppScreen.Blocks);
        ClearBlocksCommand = new RelayCommand(_ => ClearExpandedTiles());
        ClearFindWayCommand = new RelayCommand(_ => ClearFindWay());
        ClassicCommand = new RelayCommand(_ => ToggleNotationMode());
        OpenLeftPickerCommand = new RelayCommand(_ => PickerTarget = PickerTarget.Left);
        OpenRightPickerCommand = new RelayCommand(_ => PickerTarget = PickerTarget.Right);
        SelectChordCommand = new RelayCommand(chord => SelectChord((chord as ChordOption)?.Key ?? chord?.ToString()));
        GoCommand = new RelayCommand(_ => CalculatePaths(), _ => CanCalculatePaths);

        RefreshNotationDependentData();
    }

    public ObservableCollection<ChordTileViewModel> ChordTiles { get; }

    public ObservableCollection<ChordOption> AvailableChords { get; }

    public PathResultItem CreativeResult { get; }

    public PathResultItem ShortestResult { get; }

    public PathResultItem SmoothResult { get; }

    public string CreativeDisplayPath => FormatPath(CreativeResult.Nodes);

    public string ShortestDisplayPath => FormatPath(ShortestResult.Nodes);

    public string SmoothDisplayPath => FormatPath(SmoothResult.Nodes);

    public string NotationToggleButtonText => IsAlphanumericMode ? "Classic" : "Alphanumeric";

    public AppScreen CurrentScreen
    {
        get => _currentScreen;
        set
        {
            if (_currentScreen == value)
            {
                return;
            }

            _currentScreen = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsBlocksScreen));
            OnPropertyChanged(nameof(IsFindWayScreen));
            OnPropertyChanged(nameof(IsDonationScreen));
        }
    }

    public bool IsBlocksScreen => CurrentScreen == AppScreen.Blocks;

    public bool IsFindWayScreen => CurrentScreen == AppScreen.FindWay;

    public bool IsDonationScreen => CurrentScreen == AppScreen.Donation;

    public PickerTarget PickerTarget
    {
        get => _pickerTarget;
        set
        {
            if (_pickerTarget == value)
            {
                return;
            }

            _pickerTarget = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPickerVisible));
        }
    }

    public bool IsPickerVisible => PickerTarget != PickerTarget.None;

    public bool IsAlphanumericMode
    {
        get => _isAlphanumericMode;
        private set
        {
            if (_isAlphanumericMode == value)
            {
                return;
            }

            _isAlphanumericMode = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(NotationToggleButtonText));
        }
    }

    public string LeftChord
    {
        get => _leftChord ?? "";
        private set
        {
            if (_leftChord == value)
            {
                return;
            }

            _leftChord = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(LeftChordLabel));
            OnPropertyChanged(nameof(CanCalculatePaths));
            RaiseGoCanExecute();
        }
    }

    public string RightChord
    {
        get => _rightChord ?? "";
        private set
        {
            if (_rightChord == value)
            {
                return;
            }

            _rightChord = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(RightChordLabel));
            OnPropertyChanged(nameof(CanCalculatePaths));
            RaiseGoCanExecute();
        }
    }

    public string LeftChordLabel => string.IsNullOrWhiteSpace(LeftChord) ? "..." : NotationMapper.ToDisplay(LeftChord, IsAlphanumericMode);

    public string RightChordLabel => string.IsNullOrWhiteSpace(RightChord) ? "..." : NotationMapper.ToDisplay(RightChord, IsAlphanumericMode);

    public bool CanCalculatePaths => !string.IsNullOrWhiteSpace(LeftChord) && !string.IsNullOrWhiteSpace(RightChord);

    public ICommand ShowDonationCommand { get; }

    public ICommand ShowFindWayCommand { get; }

    public ICommand ShowBlocksCommand { get; }

    public ICommand ClearBlocksCommand { get; }

    public ICommand ClearFindWayCommand { get; }

    public ICommand ClassicCommand { get; }

    public ICommand OpenLeftPickerCommand { get; }

    public ICommand OpenRightPickerCommand { get; }

    public ICommand SelectChordCommand { get; }

    public ICommand GoCommand { get; }

    private void SelectChord(string? chord)
    {
        if (string.IsNullOrWhiteSpace(chord))
        {
            return;
        }

        if (PickerTarget == PickerTarget.Left)
        {
            LeftChord = chord;
        }
        else if (PickerTarget == PickerTarget.Right)
        {
            RightChord = chord;
        }

        PickerTarget = PickerTarget.None;
    }

    private void CalculatePaths()
    {
        if (!_pathCache.TryGetValue((LeftChord, RightChord), out var pathSet))
        {
            return;
        }

        ShortestResult.SetNodes(pathSet.ShortestPath);
        SmoothResult.SetNodes(pathSet.SmoothPath);
        CreativeResult.SetNodes(pathSet.CreativePaths.FirstOrDefault()?.ToList() ?? Array.Empty<string>());

        RaisePathChangedProperties();
    }

    private void ToggleNotationMode()
    {
        IsAlphanumericMode = !IsAlphanumericMode;
        RefreshNotationDependentData();
    }

    private void RefreshNotationDependentData()
    {
        foreach (var tile in ChordTiles)
        {
            tile.SetAlphanumericMode(IsAlphanumericMode);
        }

        AvailableChords.Clear();
        foreach (var chord in ChordGraph.OrderedChordNames)
        {
            AvailableChords.Add(new ChordOption
            {
                Key = chord,
                Display = NotationMapper.ToDisplay(chord, IsAlphanumericMode)
            });
        }

        OnPropertyChanged(nameof(LeftChordLabel));
        OnPropertyChanged(nameof(RightChordLabel));
        RaisePathChangedProperties();
    }

    private string FormatPath(IReadOnlyList<string> nodes)
    {
        if (nodes.Count == 0)
        {
            return string.Empty;
        }

        return string.Join("\n↑\n", nodes.Select(x => NotationMapper.ToDisplay(x, IsAlphanumericMode)).Reverse());
    }

    private void RaisePathChangedProperties()
    {
        OnPropertyChanged(nameof(CreativeDisplayPath));
        OnPropertyChanged(nameof(ShortestDisplayPath));
        OnPropertyChanged(nameof(SmoothDisplayPath));
    }

    private void ClearExpandedTiles()
    {
        foreach (var tile in ChordTiles)
        {
            tile.IsExpanded = false;
        }
    }

    private void ClearFindWay()
    {
        LeftChord = string.Empty;
        RightChord = string.Empty;
        PickerTarget = PickerTarget.None;

        CreativeResult.SetNodes(Array.Empty<string>());
        ShortestResult.SetNodes(Array.Empty<string>());
        SmoothResult.SetNodes(Array.Empty<string>());

        RaisePathChangedProperties();
    }

    private void RaiseGoCanExecute()
    {
        if (GoCommand is RelayCommand relay)
        {
            relay.RaiseCanExecuteChanged();
        }
    }
}
