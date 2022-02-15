using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Aeon.Acquisition
{
    public partial class SubjectChangeControl : UserControl
    {
        readonly ColumnHeader idHeader;
        RemoveState removeState;
        AddState addState;
        ViewState viewState;

        public SubjectChangeControl(SubjectChange source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
            subjectListView.Columns.Add(string.Empty);
            idHeader = subjectListView.Columns.Add(nameof(SubjectChangeMetadata.Id));
            propertyGrid.Enabled = false;
            var state = Source.State;
            if (state != null)
            {
                foreach (var subject in state.ActiveSubjects)
                {
                    AddSubject(subject);
                }
            }
        }

        public SubjectChange Source { get; }

        private void AddSubject(SubjectChangeEntry metadata)
        {
            var item = subjectListView.Items.Add(subjectListView.Items.Count.ToString());
            metadata.Type = SubjectChangeType.Exit;
            item.SubItems.Add(metadata.Id);
            item.Tag = metadata;
            idHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        private void RefreshViewState(ViewState view)
        {
            viewState = view;
            switch (viewState)
            {
                case ViewState.Adding:
                case ViewState.Removing:
                    addState = AddState.OK;
                    removeState = RemoveState.Cancel;
                    subjectListView.Enabled = false;
                    break;
                default:
                    addState = AddState.Add;
                    removeState = RemoveState.Remove;
                    subjectListView.Enabled = true;
                    propertyGrid.SelectedObject = null;
                    break;
            }

            addButton.Text = addState.ToString();
            removeButton.Text = removeState.ToString();
            propertyGrid.Enabled = !subjectListView.Enabled;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (viewState == ViewState.Browse)
            {
                var metadata = new SubjectChangeEntry { Type = SubjectChangeType.Enter };
                propertyGrid.SelectedObject = metadata;
                RefreshViewState(ViewState.Adding);
            }
            else if (viewState == ViewState.Adding)
            {
                var metadata = (SubjectChangeEntry)propertyGrid.SelectedObject;
                AddSubject(metadata);
                Source.OnNext(new SubjectChangeMetadata(metadata, SubjectChangeType.Enter));
                RefreshViewState(ViewState.Browse);
            }
            else if (viewState == ViewState.Removing)
            {
                var selectedItems = subjectListView.SelectedItems.OfType<ListViewItem>().ToArray();
                foreach (var item in selectedItems)
                {
                    var metadata = (SubjectChangeEntry)item.Tag;
                    subjectListView.Items.Remove(item);
                    Source.OnNext(new SubjectChangeMetadata(metadata, SubjectChangeType.Exit));
                }

                for (int i = 0; i < subjectListView.Items.Count; i++)
                {
                    subjectListView.Items[i].Text = i.ToString();
                }
                RefreshViewState(ViewState.Browse);
            }
            addButton.Text = addState.ToString();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (viewState == ViewState.Browse)
            {
                var selectedItem = subjectListView.SelectedItems.OfType<ListViewItem>().FirstOrDefault();
                if (selectedItem != null)
                {
                    var metadata = selectedItem.Tag;
                    propertyGrid.SelectedObject = metadata;
                    RefreshViewState(ViewState.Removing);
                }
            }
            else
            {
                RefreshViewState(ViewState.Browse);
            }
        }

        private void subjectListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (addState == AddState.OK) return;
            var selectedSubjects = subjectListView
                .SelectedItems
                .OfType<ListViewItem>()
                .Select(item => item.Tag)
                .ToArray();
            propertyGrid.SelectedObjects = selectedSubjects;
        }

        enum AddState
        {
            Add,
            OK,
        }

        enum RemoveState
        {
            Remove,
            Cancel,
        }

        enum ViewState
        {
            Browse,
            Adding,
            Removing
        }
    }
}
