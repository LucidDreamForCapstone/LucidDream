using UnityEditor;
using UnityEngine.UIElements;

public class InspectorView : VisualElement {
    public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

    Editor _editor;

    public InspectorView() {

    }

    internal void UpdateSelection(NodeView nodeView) {
        Clear();

        UnityEngine.Object.DestroyImmediate(_editor);
        _editor = Editor.CreateEditor(nodeView._node);
        IMGUIContainer container = new IMGUIContainer(() => {
            if (_editor.target) {
                _editor.OnInspectorGUI();
            }
        });
        Add(container);
    }
}
