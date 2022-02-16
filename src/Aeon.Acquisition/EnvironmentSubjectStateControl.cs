using System;
using System.Linq;
using System.Windows.Forms;

namespace Aeon.Acquisition
{
    public partial class EnvironmentSubjectStateControl : UserControl
    {
        readonly ColumnHeader idHeader;
        RemoveState removeState;
        AddState addState;
        ViewState viewState;

        public EnvironmentSubjectStateControl(EnvironmentSubjectState source)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            InitializeComponent();
            subjectListView.Columns.Add(string.Empty);
            idHeader = subjectListView.Columns.Add(nameof(EnvironmentSubjectStateMetadata.Id));
            propertyGrid.Enabled = false;
        }

        public EnvironmentSubjectState Source { get; }

        public void AddSubject(EnvironmentSubjectStateEntry metadata)
        {
            if (subjectListView.Items.ContainsKey(metadata.Id))
            {
                return;
            }

            var item = subjectListView.Items.Add(metadata.Id, subjectListView.Items.Count.ToString(), 0);
            metadata.Type = EnvironmentSubjectChangeType.Exit;
            item.SubItems.Add(metadata.Id);
            item.Tag = metadata;
            idHeader.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        public void RemoveSubject(string id)
        {
            subjectListView.Items.RemoveByKey(id);
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
                var metadata = new EnvironmentSubjectStateEntry { Type = EnvironmentSubjectChangeType.Enter };
                propertyGrid.SelectedObject = metadata;
                RefreshViewState(ViewState.Adding);
            }
            else if (viewState == ViewState.Adding)
            {
                var metadata = (EnvironmentSubjectStateEntry)propertyGrid.SelectedObject;
                if (subjectListView.Items.ContainsKey(metadata.Id))
                {
                    MessageBox.Show(
                        $"A subject with id {metadata.Id} has already been added.",
                        ((Bonsai.INamedElement)Source).Name);
                    return;
                }
                Source.OnNext(new EnvironmentSubjectStateMetadata(metadata, EnvironmentSubjectChangeType.Enter));
                RefreshViewState(ViewState.Browse);
            }
            else if (viewState == ViewState.Removing)
            {
                var selectedItems = subjectListView.SelectedItems.OfType<ListViewItem>().ToArray();
                foreach (var item in selectedItems)
                {
                    var metadata = (EnvironmentSubjectStateEntry)item.Tag;
                    Source.OnNext(new EnvironmentSubjectStateMetadata(metadata, EnvironmentSubjectChangeType.Exit));
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
