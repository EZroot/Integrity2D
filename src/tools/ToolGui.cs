using ImGuiNET;
using Integrity.Core;
using Integrity.Interface;
using Integrity.Objects;
using System.Reflection;
using System.Numerics;
using Integrity.Rendering;

namespace Integrity.Tools;

public class ToolGui
{
    private bool m_InspectorWindowOpened = false; 
    private bool m_TileMonitorWindowOpened = false; 


    ToolTileMonitor m_TileMonitor; 
    ToolInspector m_ToolInspector;

    public ToolGui()
    {
        m_TileMonitor = new ToolTileMonitor();
        m_ToolInspector = new ToolInspector();
    }

    public void DrawMenuBar(float fps)
    {
        if (ImGui.BeginMainMenuBar())
        {
            var engineSettings = Service.Get<IEngineSettings>()!;
            if (ImGui.BeginMenu("Tools"))
            {
                // Changed to use the new field m_InspectorWindowOpened
                if (ImGui.MenuItem("Inspector", "", ref m_InspectorWindowOpened)) 
                {
                }
                if (ImGui.MenuItem("Tile Monitor", "", ref m_TileMonitorWindowOpened))
                {
                }

                ImGui.EndMenu();
            }
            ImGui.Separator();

            string statusText = $"FPS({fps}) {engineSettings.Data.EngineName} - {engineSettings.Data.EngineVersion}({BuildInfo.BuildNumber}) OS: {Environment.OSVersion} ({DateTime.Now:d} {DateTime.Now:t})";
            float menuBarWidth = ImGui.GetWindowWidth();
            float textWidth = ImGui.CalcTextSize(statusText).X;
            ImGui.SetCursorPosX(menuBarWidth - textWidth - ImGui.GetStyle().ItemSpacing.X);
            ImGui.Text(statusText);
            ImGui.EndMainMenuBar();
        }
    }
    
    /// <summary>
    /// New method to draw all open tool windows. This should be called from your engine loop.
    /// </summary>
    public void DrawTools()
    {
        // Only draw the inspector if it's open
        if (m_InspectorWindowOpened)
        {
            m_ToolInspector.DrawInspectorWindow(ref m_InspectorWindowOpened);
        }
        
        if (m_TileMonitorWindowOpened)
        {
            m_TileMonitor.DrawTileMonitorWindow(ref m_TileMonitorWindowOpened);
        }

    }

    


}