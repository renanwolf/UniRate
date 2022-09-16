using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UniRate.Debug;

namespace UniRate.Editor {

    public class RateRequestTrackerTreeView : TreeView {

        #region <<---------- Class Item ---------->>

        public class Item : TreeViewItem {

            public Item(RateRequestTrackInfo trackInfo) : base(trackInfo.Identifier, 0) {
                this.TrackInfo = trackInfo;
            }

            public RateRequestTrackInfo TrackInfo { get; }
        }

        #endregion <<---------- Class Item ---------->>




        #region <<---------- Initializers ---------->>

        public RateRequestTrackerTreeView() : this(new MultiColumnHeader(new MultiColumnHeaderState(new[] {
            new MultiColumnHeaderState.Column() {
                headerContent = new GUIContent("Request Type"),
                canSort = false,
                allowToggleVisibility = false,
                minWidth = 60,
                maxWidth = 120
            },
            new MultiColumnHeaderState.Column() {
                headerContent = new GUIContent("Rate or Interval"),
                canSort = false,
                allowToggleVisibility = false,
                minWidth = 40,
                maxWidth = 100
            },
            new MultiColumnHeaderState.Column() {
                headerContent = new GUIContent("Status"),
                canSort = false,
                allowToggleVisibility = false,
                minWidth = 40,
                maxWidth = 80
            },
            new MultiColumnHeaderState.Column() {
                headerContent = new GUIContent("Time"),
                canSort = true,
                allowToggleVisibility = false,
                minWidth = 58,
                maxWidth = 100
            },
            new MultiColumnHeaderState.Column() {
                headerContent = new GUIContent("StackTrace"),
                canSort = false,
                allowToggleVisibility = false,
                minWidth = 60
            }
        }))) {

        }

        private RateRequestTrackerTreeView(MultiColumnHeader header) : base(new TreeViewState(), header) {
            this._header = header;
            this._source = new List<RateRequestTrackInfo>();

            this.rowHeight = 20;
            this.showAlternatingRowBackgrounds = true;
            this.showBorder = true;

            this.Reload();
            this._header.ResizeToFit();
            this._header.SetSorting(
                SessionState.GetInt($"{nameof(RateRequestTrackerTreeView)}_header_sort_column_index", 3),
                SessionState.GetBool($"{nameof(RateRequestTrackerTreeView)}_header_sort_ascending", false)
            );

            this._header.sortingChanged += this.OnHeaderSortChanged;
        }

        #endregion <<---------- Initializers ---------->>




        #region <<---------- Properties and Fields ---------->>

        private readonly MultiColumnHeader _header;

        public IReadOnlyList<RateRequestTrackInfo> Source => this._source;
        private readonly List<RateRequestTrackInfo> _source;

        private bool _headerAutoResizeAfterFirstItems;

        public event Action<RateRequestTrackerTreeView, IList<int>> SelectedIdsChanged {
            add {
                this._selectedIdsChanged -= value;
                this._selectedIdsChanged += value;
            }
            remove {
                this._selectedIdsChanged -= value;
            }
        }
        private Action<RateRequestTrackerTreeView, IList<int>> _selectedIdsChanged;

        #endregion <<---------- Properties and Fields ---------->>




        #region <<---------- Callbacks ---------->>

        private void OnHeaderSortChanged(MultiColumnHeader header) {
            int columnIndex = header.sortedColumnIndex;
            bool isAscending = header.IsSortedAscending(columnIndex);
            SessionState.SetInt($"{nameof(RateRequestTrackerTreeView)}_header_sort_column_index", header.sortedColumnIndex);
            SessionState.SetBool($"{nameof(RateRequestTrackerTreeView)}_header_sort_ascending", isAscending);

            this.rootItem.children.Sort((a, b) => {
                var itemA = (Item)a;
                var itemB = (Item)b;
                switch (columnIndex) {
                    case 3:
                        var aTime = itemA.TrackInfo.IsActive ? itemA.TrackInfo.StartedTime : itemA.TrackInfo.FinishedTime;
                        var bTime = itemB.TrackInfo.IsActive ? itemB.TrackInfo.StartedTime : itemB.TrackInfo.FinishedTime;
                        return isAscending ? aTime.CompareTo(bTime) : bTime.CompareTo(aTime);
                    default: throw new NotImplementedException($"{nameof(this.OnHeaderSortChanged)}() not handling column index {columnIndex.ToString()}");
                }
            });

            this.BuildRows(this.rootItem);
        }

        #endregion <<---------- Callbacks ---------->>




        #region <<---------- TreeView ---------->>

        protected override TreeViewItem BuildRoot() {
            var root = new TreeViewItem() {
                depth = -1
            };
            if (this._source.Count > 0) {
                foreach (var trackInfo in this._source) {
                    root.AddChild(new Item(trackInfo));
                }
                SetupDepthsFromParentsAndChildren(root);
            }
            else {
                root.children = new List<TreeViewItem>();
            }
            return root;
        }

        protected override void RowGUI(RowGUIArgs args) {
            var item = (Item)args.item;

            for (int iVisibleColumn = 0; iVisibleColumn < args.GetNumVisibleColumns(); iVisibleColumn++) {
                var rect = args.GetCellRect(iVisibleColumn);
                this.CenterRectUsingSingleLineHeight(ref rect);
                int iColumn = args.GetColumn(iVisibleColumn);

                switch (iColumn) {
                    case 0: // request type
                        EditorGUI.LabelField(rect, item.TrackInfo.Type.ToString());
                        break;
                    case 1: // rate or interval value
                        EditorGUI.LabelField(rect, item.TrackInfo.Value.ToString());
                        break;
                    case 2: // status
                        EditorGUI.LabelField(rect, item.TrackInfo.GetStatusFormatted());
                        break;
                    case 3: // time
                        if (item.TrackInfo.IsActive) {
                            EditorGUI.LabelField(rect, item.TrackInfo.GetStartedTimeFormatted());
                        }
                        else {
                            EditorGUI.LabelField(rect, item.TrackInfo.GetFinishedTimeFormatted());
                        }
                        break;
                    case 4: // stack trace
                        EditorGUI.LabelField(rect, item.TrackInfo.IsActive ? item.TrackInfo.StackTraceStartFirstLine : item.TrackInfo.StackTraceFinishFirstLine);
                        break;
                    default: throw new NotImplementedException($"{nameof(this.RowGUI)}() not handling column index {iColumn.ToString()}");
                }
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds) {
            base.SelectionChanged(selectedIds);
            var e = this._selectedIdsChanged;
            if (e == null) return;
            e(this, selectedIds);
        }

        #endregion <<---------- TreeView ---------->>




        #region <<---------- General ---------->>

        public void Reload(IEnumerable<RateRequestTrackInfo> source) {
            this._source.Clear();
            this._source.AddRange(source);
            this.Reload();
            if (!this._headerAutoResizeAfterFirstItems && this._source.Count > 0) {
                this._headerAutoResizeAfterFirstItems = true;
                this._header.ResizeToFit();
            }
            this.OnHeaderSortChanged(this._header);
        }

        #endregion <<---------- General ---------->>
    }
}