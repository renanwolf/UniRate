using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UniRate.Debug;

namespace UniRate.Editor {

    internal static class RateRequestTrackerWindowExtensions {

        internal static RateRequestTrackerWindow.Filters ToggleFlag(this RateRequestTrackerWindow.Filters filter, RateRequestTrackerWindow.Filters flag) {
            return filter ^ flag;
        }

        internal static RateRequestTrackerWindow.Filters Flagged(this RateRequestTrackerWindow.Filters filter, RateRequestTrackerWindow.Filters flag) {
            return filter | flag;
        }

        internal static RateRequestTrackerWindow.Filters Unflagged(this RateRequestTrackerWindow.Filters filter, RateRequestTrackerWindow.Filters flag) {
            return filter & (~flag);
        }

        internal static bool HasAnyFlag(this RateRequestTrackerWindow.Filters filter, RateRequestTrackerWindow.Filters flag) {
            return (filter & flag) != 0;
        }

        internal static string GetStatusAndTypeDisplayText(this RateRequestTrackerWindow.Filters filter, bool allowNone = false) {
            if (!filter.HasAnyFlag(RateRequestTrackerWindow.Filters.Status_All) && !filter.HasAnyFlag(RateRequestTrackerWindow.Filters.Type_All)) {
                return allowNone ? "Nothing" : null;
            }

            var builder = new StringBuilder();

            if (filter.HasFlag(RateRequestTrackerWindow.Filters.Status_All)) {
                builder.Append("All Status");
            }
            else if (!filter.HasAnyFlag(RateRequestTrackerWindow.Filters.Status_All)) {
                if (allowNone) {
                    builder.Append("No Status");
                }
            }
            else {
                if (filter.HasFlag(RateRequestTrackerWindow.Filters.Status_Active)) {
                    builder.Append("Active");
                }
                if (filter.HasFlag(RateRequestTrackerWindow.Filters.Status_Finished)) {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append("Finished");
                }
            }

            if (filter.HasFlag(RateRequestTrackerWindow.Filters.Type_All)) {
                if (builder.Length > 0) builder.Append(", ");
                builder.Append("All Types");
            }
            else if (!filter.HasAnyFlag(RateRequestTrackerWindow.Filters.Type_All)) {
                if (allowNone) {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append("No Types");
                }
            }
            else {
                if (filter.HasFlag(RateRequestTrackerWindow.Filters.Type_UpdateRate)) {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append("Update Rate");
                }
                if (filter.HasFlag(RateRequestTrackerWindow.Filters.Type_FixedUpdateRate)) {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append("Fixed Update Rate");
                }
                if (filter.HasFlag(RateRequestTrackerWindow.Filters.Type_RenderInterval)) {
                    if (builder.Length > 0) builder.Append(", ");
                    builder.Append("Render Interval");
                }
            }

            return builder.ToString();
        }
    }

    public class RateRequestTrackerWindow : EditorWindow {

        #region <<---------- Enum Filters ---------->>

        [Flags]
        internal enum Filters {
            None = 0,

            Status_Active = 1 << 0,
            Status_Finished = 1 << 1,
            Status_All = Status_Active | Status_Finished,

            Type_UpdateRate = 1 << 2,
            Type_FixedUpdateRate = 1 << 3,
            Type_RenderInterval = 1 << 4,
            Type_All = Type_UpdateRate | Type_FixedUpdateRate | Type_RenderInterval,

            KeepSelection = 1 << 5,

            All = ~0
        }

        private static readonly string[] _filtersDisplayName = new[] {
            "Status/Active",
            "Status/Finished",
            "Type/UpdateRate",
            "Type/FixedUpdateRate",
            "Type/RenderInterval",
            "Keep Selection"
        };

        #endregion <<---------- Enum Filters ---------->>




        #region <<---------- Properties and Fields ---------->>

        private RateRequestTrackerTreeView _treeView;
        private Vector2 _scrollPositionTree;

        private readonly List<RateRequestTrackInfo> _trackerInfos = new List<RateRequestTrackInfo>();

        private Filters SelectedFilter {
            get => this._selectedFilter;
            set {
                if (this._selectedFilter == value) return;
                this._selectedFilter = value;
                this.OnSelectedFilterChanged(this._selectedFilter);
            }
        }
        private Filters _selectedFilter;

        private bool TrackerEnabled {
            get => this._trackerEnabled;
            set {
                if (this._trackerEnabled == value) return;
                this._trackerEnabled = value;
                this.OnTrackerEnabledChanged(this._trackerEnabled);
            }
        }
        private bool _trackerEnabled;

        private bool StackTraceEnabled {
            get => this._stackTraceEnabled;
            set {
                if (this._stackTraceEnabled == value) return;
                this._stackTraceEnabled = value;
                this.OnStackTraceEnabledChanged(this._stackTraceEnabled);
            }
        }
        private bool _stackTraceEnabled;

        private float SplitTopNormalizedHeight {
            get => this._splitTopNormalizedHeight;
            set {
                if (this._splitTopNormalizedHeight == value) return;
                this._splitTopNormalizedHeight = value;
                this.OnSplitTopNormalizedHeighChanged(this._splitTopNormalizedHeight);
            }
        }
        private float _splitTopNormalizedHeight;

        private Rect _mainRect;
        private Rect _toolbarRect;
        private bool _isResizingSplit;
        private static readonly Color _lineSplitColor = new Color(0, 0, 0, 0.3f);

        private Vector2 _scrollPositionDetails;
        private static GUIStyle _labelDetailsStyle;

        private Rect _filtersDropdownRect;
        private Rect _selectionDropdownRect;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- EditorWindow ---------->>

        private void OnEnable() {
            this._splitTopNormalizedHeight = EditorPrefs.GetFloat($"{nameof(RateRequestTrackerWindow)}.{nameof(this._splitTopNormalizedHeight)}", 0.7f);

            this._mainRect = this.position;
            this._mainRect.x = 0;
            this._mainRect.y = 0;

            this._selectedFilter = (Filters)EditorPrefs.GetInt($"{nameof(RateRequestTrackerWindow)}.{nameof(this._selectedFilter)}", (int)Filters.All.Unflagged(Filters.Status_Finished));

            this._trackerEnabled = EditorPrefs.GetBool($"{nameof(RateRequestTrackerWindow)}.{nameof(this._trackerEnabled)}", true);
            RateRequestTracker.IsEnabled = this._trackerEnabled;

            this._stackTraceEnabled = SessionState.GetBool($"{nameof(RateRequestTrackerWindow)}.{nameof(this._stackTraceEnabled)}", false);
            RateRequestTracker.IsStackTraceEnabled = this._stackTraceEnabled;

            RateRequestTracker.GetInfos(this._trackerInfos);

            this._treeView = new RateRequestTrackerTreeView();
            this.ReloadTreeViewWithSelectedFilters();

            RateRequestTracker.InfosChanged += this.OnTrackerInfosChanged;
            this._treeView.SelectedIdsChanged += this.OnSelectedIdsChanged;
        }

        private void OnGUI() {
            bool needsRepaint = false;

            // vertical main split
            using (var splitMainScope = new EditorGUILayout.VerticalScope(new[] {
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            })) {
                if (splitMainScope.rect.width != 0f && splitMainScope.rect.height != 0f) {
                    this._mainRect = splitMainScope.rect;
                }

                // toolbar
                using (var toolbarScope = new EditorGUILayout.HorizontalScope(EditorStyles.toolbar)) {
                    this.MakeSelectionDropdown();
                    this.MakeFiltersDropdown();
                    GUILayout.FlexibleSpace();
                    this.TrackerEnabled = GUILayout.Toggle(this._trackerEnabled, "Tracking", EditorStyles.toolbarButton);
                    this.StackTraceEnabled = GUILayout.Toggle(this._stackTraceEnabled, "StackTrace", EditorStyles.toolbarButton);
                    if (toolbarScope.rect.width != 0f && toolbarScope.rect.height != 0f) {
                        this._toolbarRect = toolbarScope.rect;
                    }
                }

                // scroll tree view
                float treeHeight = Math.Min(Math.Max((this._mainRect.height * this._splitTopNormalizedHeight) - this._toolbarRect.height, 80), this._mainRect.height - 80);
                using (new EditorGUILayout.VerticalScope(new[] {
                    GUILayout.ExpandWidth(true),
                    GUILayout.Height(treeHeight)
                })) {
                    using (var scrollScopeTree = new EditorGUILayout.ScrollViewScope(this._scrollPositionTree)) {
                        this._scrollPositionTree = scrollScopeTree.scrollPosition;

                        // tree view
                        var treeRect = EditorGUILayout.GetControlRect(new[] {
                            GUILayout.ExpandWidth(true),
                            GUILayout.ExpandHeight(true)
                        });
                        this._treeView.OnGUI(treeRect);
                    }
                }

                // split line
                var rectResizeLine = this._mainRect;
                rectResizeLine.y = this._toolbarRect.height + treeHeight;
                rectResizeLine.height = 2f;
                EditorGUI.DrawRect(rectResizeLine, _lineSplitColor);
                EditorGUIUtility.AddCursorRect(rectResizeLine, MouseCursor.SplitResizeUpDown);

                // split line movement
                var currentEvent = Event.current;
                if (currentEvent.type == EventType.MouseDown && rectResizeLine.Contains(currentEvent.mousePosition) && !this._isResizingSplit) {
                    currentEvent.Use();
                    this._isResizingSplit = true;
                    needsRepaint = true;
                }
                if (this._isResizingSplit) {
                    this.SplitTopNormalizedHeight = currentEvent.mousePosition.y / this._mainRect.height;
                    needsRepaint = true;
                }
                if (currentEvent.type == EventType.MouseUp && this._isResizingSplit) {
                    currentEvent.Use();
                    this._isResizingSplit = false;
                    needsRepaint = true;
                }

                // details view
                using (var scrollScopeDetails = new EditorGUILayout.ScrollViewScope(this._scrollPositionDetails)) {
                    this._scrollPositionDetails = scrollScopeDetails.scrollPosition;
                    var builder = new StringBuilder();
                    int countSelection = this._treeView.state.selectedIDs.Count;
                    if (countSelection > 1) {
                        builder.Append("Requests selected: ").Append(countSelection).Append('.');
                    }
                    if (countSelection != 1) {
                        if (builder.Length > 0) {
                            builder.Append('\n');
                        }
                        builder.Append("Select one request to see the details.");
                    }
                    else {
                        var id = this._treeView.state.selectedIDs[0];
                        var trackInfo = this._trackerInfos.Find(i => i.Identifier == id);
                        builder.Append("Type: ").AppendLine(trackInfo.Type.ToString());
                        builder.Append("Value: ").AppendLine(trackInfo.Value.ToString());
                        builder.Append("Status: ").AppendLine(trackInfo.GetStatusFormatted());
                        builder.AppendLine();
                        builder.Append("Started: ").AppendLine(trackInfo.GetStartedTimeFormatted());
                        if (!string.IsNullOrEmpty(trackInfo.StackTraceStart)) {
                            builder.AppendLine(trackInfo.StackTraceStart);
                        }
                        builder.AppendLine();
                        builder.Append("Finished: ").AppendLine(trackInfo.GetFinishedTimeFormatted());
                        if (!string.IsNullOrEmpty(trackInfo.StackTraceFinish)) {
                            builder.Append(trackInfo.StackTraceFinish);
                        }
                    }
                    if (_labelDetailsStyle == null) {
                        _labelDetailsStyle = new GUIStyle("CN Message") {
                            wordWrap = true,
                            stretchHeight = true,
                            margin = new RectOffset(3, 3, 3, 3)
                        };
                    }
                    var textDetails = builder.ToString();
                    var labelHeight = _labelDetailsStyle.CalcHeight(new GUIContent(textDetails), this._mainRect.width - 40);
                    EditorGUILayout.SelectableLabel(textDetails, _labelDetailsStyle, new[] {
                        GUILayout.ExpandWidth(true),
                        GUILayout.ExpandHeight(true),
                        GUILayout.MaxWidth(this._mainRect.width),
                        GUILayout.MinHeight(labelHeight)
                    });
                }
            }

            if (needsRepaint) {
                this.Repaint();
            }
        }

        private void OnDisable() {
            this._treeView.SelectedIdsChanged -= this.OnSelectedIdsChanged;
            RateRequestTracker.InfosChanged -= this.OnTrackerInfosChanged;
            RateRequestTracker.IsEnabled = false;
            RateRequestTracker.IsStackTraceEnabled = false;
            this._trackerInfos.Clear();
        }

        #endregion <<---------- EditorWindow ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnSelectedFilterChanged(Filters selectedFilter) {
            EditorPrefs.SetInt($"{nameof(RateRequestTrackerWindow)}.{nameof(this._selectedFilter)}", (int)selectedFilter);
            this.ReloadTreeViewWithSelectedFilters();
        }

        private void OnSelectedIdsChanged(RateRequestTrackerTreeView treeView, IList<int> selectedIds) {
            this.ReloadTreeViewWithSelectedFilters();
        }

        private void OnTrackerEnabledChanged(bool trackerEnabled) {
            EditorPrefs.SetBool($"{nameof(RateRequestTrackerWindow)}.{nameof(this._trackerEnabled)}", trackerEnabled);
            RateRequestTracker.IsEnabled = trackerEnabled;
        }

        private void OnStackTraceEnabledChanged(bool stackTraceEnabled) {
            SessionState.SetBool($"{nameof(RateRequestTrackerWindow)}.{nameof(this._stackTraceEnabled)}", stackTraceEnabled);
            RateRequestTracker.IsStackTraceEnabled = stackTraceEnabled;
        }

        private void OnTrackerInfosChanged() {
            RateRequestTracker.GetInfos(this._trackerInfos);
            this.ReloadTreeViewWithSelectedFilters();
            this.Repaint();
        }

        private void OnSplitTopNormalizedHeighChanged(float splitTopNormalizedHeight) {
            EditorPrefs.SetFloat($"{nameof(RateRequestTrackerWindow)}.{nameof(this._splitTopNormalizedHeight)}", splitTopNormalizedHeight);
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- General ---------->>

        [MenuItem("Window/UniRate Tracker")]
        public static void Open() {
            var window = GetWindow<RateRequestTrackerWindow>();
            window.minSize = new Vector2(400, 200);
            window.titleContent = new GUIContent("UniRate Tracker");
            window.Show();
        }

        private void ReloadTreeViewWithSelectedFilters() {
            this._treeView.Reload(this._trackerInfos.Where(info =>
                (
                    (
                        (info.IsActive && this._selectedFilter.HasFlag(Filters.Status_Active))
                        || (!info.IsActive && this._selectedFilter.HasFlag(Filters.Status_Finished))
                    )
                    && (
                        (info.Type == RateRequestType.UpdateRate && this._selectedFilter.HasFlag(Filters.Type_UpdateRate))
                        || (info.Type == RateRequestType.FixedUpdateRate && this._selectedFilter.HasFlag(Filters.Type_FixedUpdateRate))
                        || (info.Type == RateRequestType.RenderInterval && this._selectedFilter.HasFlag(Filters.Type_RenderInterval))
                    )
                )
                || (
                    this._selectedFilter.HasFlag(Filters.KeepSelection)
                    && this._treeView.state.selectedIDs.Contains(info.Identifier)
                )
            ));
        }

        private void MakeFiltersDropdown() {
            var buttonText = $"Filters:  {this._selectedFilter.GetStatusAndTypeDisplayText(true)}";
            var buttonContent = new GUIContent(buttonText);
            var buttonStyle = EditorStyles.toolbarDropDown;
            var buttonWidth = buttonStyle.CalcSize(buttonContent).x + 1;

            var buttonClicked = EditorGUILayout.DropdownButton(buttonContent, FocusType.Passive, buttonStyle, new[] {
                GUILayout.MinWidth(60),
                GUILayout.MaxWidth(buttonWidth)
            });

            if (!buttonClicked) {
                if (Event.current.type != EventType.Layout) {
                    this._filtersDropdownRect = GUILayoutUtility.GetLastRect();
                }
                return;
            }

            var menu = new GenericMenu();

            if (this._selectedFilter.HasFlag(Filters.Status_All)) {
                menu.AddItem(
                    new GUIContent("Status/None"),
                    false,
                    () => this.SelectedFilter = this._selectedFilter.Unflagged(Filters.Status_All)
                );
            }
            else {
                menu.AddItem(
                    new GUIContent("Status/All"),
                    false,
                    () => this.SelectedFilter = this._selectedFilter.Flagged(Filters.Status_All)
                );
            }
            menu.AddSeparator("Status/");
            menu.AddItem(
                new GUIContent($"Status/{Filters.Status_Active.GetStatusAndTypeDisplayText()}"),
                this._selectedFilter.HasFlag(Filters.Status_Active),
                () => this.SelectedFilter = this._selectedFilter.ToggleFlag(Filters.Status_Active)
            );
            menu.AddItem(
                new GUIContent($"Status/{Filters.Status_Finished.GetStatusAndTypeDisplayText()}"),
                this._selectedFilter.HasFlag(Filters.Status_Finished),
                () => this.SelectedFilter = this._selectedFilter.ToggleFlag(Filters.Status_Finished)
            );

            if (this._selectedFilter.HasFlag(Filters.Type_All)) {
                menu.AddItem(
                    new GUIContent("Type/None"),
                    false,
                    () => this.SelectedFilter = this._selectedFilter.Unflagged(Filters.Type_All)
                );
            }
            else {
                menu.AddItem(
                    new GUIContent("Type/All"),
                    false,
                    () => this.SelectedFilter = this._selectedFilter.Flagged(Filters.Type_All)
                );
            }
            menu.AddSeparator("Type/");
            menu.AddItem(
                new GUIContent($"Type/{Filters.Type_UpdateRate.GetStatusAndTypeDisplayText()}"),
                this._selectedFilter.HasFlag(Filters.Type_UpdateRate),
                () => this.SelectedFilter = this._selectedFilter.ToggleFlag(Filters.Type_UpdateRate)
            );
            menu.AddItem(
                new GUIContent($"Type/{Filters.Type_FixedUpdateRate.GetStatusAndTypeDisplayText()}"),
                this._selectedFilter.HasFlag(Filters.Type_FixedUpdateRate),
                () => this.SelectedFilter = this._selectedFilter.ToggleFlag(Filters.Type_FixedUpdateRate)
            );
            menu.AddItem(
                new GUIContent($"Type/{Filters.Type_RenderInterval.GetStatusAndTypeDisplayText()}"),
                this._selectedFilter.HasFlag(Filters.Type_RenderInterval),
                () => this.SelectedFilter = this._selectedFilter.ToggleFlag(Filters.Type_RenderInterval)
            );

            menu.DropDown(this._filtersDropdownRect);
        }

        private void MakeSelectionDropdown() {
            var buttonContent = new GUIContent("Selection");
            var buttonStyle = EditorStyles.toolbarDropDown;
            var buttonWidth = buttonStyle.CalcSize(buttonContent).x + 1;

            var buttonClicked = EditorGUILayout.DropdownButton(buttonContent, FocusType.Passive, buttonStyle, new[] {
                GUILayout.MinWidth(65),
                GUILayout.MaxWidth(buttonWidth)
            });

            if (!buttonClicked) {
                if (Event.current.type != EventType.Layout) {
                    this._selectionDropdownRect = GUILayoutUtility.GetLastRect();
                }
                return;
            }

            var menu = new GenericMenu();
            if (this._treeView.state.selectedIDs.Count <= 0) {
                menu.AddDisabledItem(new GUIContent("Unselect"));
            }
            else {
                menu.AddItem(
                    new GUIContent("Unselect"),
                    false,
                    () => this._treeView.SetSelection(new List<int>(), TreeViewSelectionOptions.FireSelectionChanged)
                );
            }
            menu.AddItem(
                new GUIContent("Keep selection on list"),
                this._selectedFilter.HasFlag(Filters.KeepSelection),
                () => this.SelectedFilter = this._selectedFilter.ToggleFlag(Filters.KeepSelection)
            );
            menu.DropDown(this._selectionDropdownRect);
        }

        #endregion <<---------- General ---------->>
    }
}