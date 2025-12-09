using System.Numerics;
using ImGuiNET;
using Integrity.Core;
using Integrity.Interface;
using Integrity.Objects;

public class ToolInspector
{
    private GameObject? m_SelectedGameObject = null;

    public void DrawInspectorWindow(ref bool inspectorWindowOpened)
    {
        if (ImGui.Begin("Inspector", ref inspectorWindowOpened))
        {
            var sceneManager = Service.Get<ISceneManager>();
            var currentScene = sceneManager?.CurrentScene;

            if (currentScene == null)
            {
                ImGui.Text("No active scene loaded.");
                ImGui.End();
                return;
            }

            ImGui.BeginChild("SceneHierarchy", new Vector2(200, 0));
            ImGui.Text($"Scene: {currentScene.Name}");
            ImGui.Separator();

            if (ImGui.TreeNodeEx("Scene Root", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Leaf))
            {
                if (ImGui.IsItemClicked())
                {
                    m_SelectedGameObject = null; 
                }
                ImGui.TreePop();
            }

            foreach (var gameObject in currentScene.GetAllGameObjects())
            {
                ImGui.PushID(gameObject.GetHashCode()); 
                
                bool isSelected = m_SelectedGameObject == gameObject;
                
                if (ImGui.Selectable(gameObject.Name, isSelected))
                {
                    m_SelectedGameObject = gameObject; 
                }
                
                ImGui.PopID();
            }

            ImGui.EndChild();

            ImGui.SameLine();

            ImGui.BeginChild("PropertiesInspector");

            if (m_SelectedGameObject != null)
            {
                ImGui.Text($"Object: {m_SelectedGameObject.Name}");
                ImGui.Separator();
                
                ImGui.Text("GameObject Properties");
                ToolHelper.DrawObjectProperties(m_SelectedGameObject, "GameObject");
                ImGui.Separator();
                
                ImGui.Text("Components");
                var componentMap = ToolHelper.GetPrivateComponentMap(m_SelectedGameObject);
                
                if (componentMap != null)
                {
                    foreach (var pair in componentMap)
                    {
                        var component = pair.Value;
                        ImGui.PushID(component.GetHashCode());
                        
                        ToolHelper.DrawObjectProperties(component, component.GetType().Name);
                        
                        ImGui.PopID();
                        ImGui.Separator();
                    }
                }
                else
                {
                    ImGui.TextDisabled("No components found or reflection failed.");
                }
            }
            else
            {
                ImGui.Text("Select a GameObject from the hierarchy.");
            }

            ImGui.EndChild();
            
            ImGui.End();
        }
    }
}