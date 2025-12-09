using System.Numerics;
using ImGuiNET;
using Integrity.Core;
using Integrity.Interface;
using Integrity.Rendering;
using System;

namespace Integrity.Tools;
public class ToolTileMonitor
{
    private TileChunk? m_SelectedTileChunk = null;

    public void DrawTileMonitorWindow(ref bool tileMonitorWindowOpened)
    {
        if (ImGui.Begin("Tile Monitor", ref tileMonitorWindowOpened))
        {
            var sceneManager = Service.Get<ISceneManager>();
            var currentScene = sceneManager?.CurrentScene;

            if (currentScene == null)
            {
                ImGui.Text("No active scene loaded.");
                ImGui.End();
                return;
            }

            var tileSystem = currentScene.TileRenderSystem;
            var tileChunks = tileSystem.TileChunks;

            ImGui.Text($"Scene: {currentScene.Name}");
            ImGui.Text($"Total Chunks: {tileChunks.Count}");
            ImGui.Separator();

            ImGui.BeginChild("ChunkList", new Vector2(250, 0));
            
            ImGui.Text("🌐 Tile Chunks");
            ImGui.Separator();

            ImGui.PushID("ChunkListRoot");
            
            if (ImGui.TreeNodeEx("Loaded Chunks", ImGuiTreeNodeFlags.DefaultOpen))
            {
                foreach (var kvp in tileChunks)
                {
                    var chunkId = kvp.Key;
                    var chunk = kvp.Value;
                    
                    ImGui.PushID(chunk.GetHashCode());

                    string label = $"Chunk ({chunkId.X}, {chunkId.Y}) | Tiles: {chunk.TileDataMap.Count} | Dirty: {chunk.IsDirty}";
                    
                    ImGuiTreeNodeFlags nodeFlags = ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.NoTreePushOnOpen;
                    bool isSelected = m_SelectedTileChunk == chunk;

                    if (isSelected)
                    {
                        nodeFlags |= ImGuiTreeNodeFlags.Selected;
                    }

                    ImGui.TreeNodeEx(label, nodeFlags);

                    if (ImGui.IsItemClicked())
                    {
                        m_SelectedTileChunk = chunk;
                    }
                    
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip($"Coords: ({chunkId.X}, {chunkId.Y})\nVertices: {chunk.VertexCount}\nTexture ID: {chunk.Texture.TextureId}");
                    }

                    ImGui.PopID();
                }
                ImGui.TreePop();
            }

            ImGui.PopID();
            ImGui.EndChild();
            ImGui.SameLine();

            ImGui.BeginChild("ChunkDetails");

            if (m_SelectedTileChunk != null)
            {
                var chunk = m_SelectedTileChunk;
                ImGui.Text($"Details for Chunk: ({chunk.ChunkId.X}, {chunk.ChunkId.Y})");
                ImGui.Separator();
                
                ImGui.Text($"Texture ID: {chunk.Texture.TextureId}");
                ImGui.Text($"Tile Count: **{chunk.TileDataMap.Count}**");
                ImGui.Text($"Vertices: **{chunk.VertexCount}**");
                
                bool isDirty = chunk.IsDirty;
                if (ImGui.Checkbox("Is Dirty (Needs Update)", ref isDirty))
                {
                    chunk.IsDirty = isDirty;
                }
                ImGui.Spacing();

                if (ImGui.CollapsingHeader("Raw Vertex Data"))
                {
                    if (chunk.VertexCount == 0)
                    {
                        ImGui.TextDisabled("Chunk contains no vertices.");
                    }
                    else
                    {
                        if (ImGui.BeginTable("VertexTable", 5, ImGuiTableFlags.Borders | ImGuiTableFlags.RowBg | ImGuiTableFlags.ScrollY, new Vector2(0, 200)))
                        {
                            ImGui.TableSetupColumn("Index", ImGuiTableColumnFlags.WidthFixed, 40);
                            ImGui.TableSetupColumn("Pos X", ImGuiTableColumnFlags.WidthFixed, 80);
                            ImGui.TableSetupColumn("Pos Y", ImGuiTableColumnFlags.WidthFixed, 80);
                            ImGui.TableSetupColumn("UV X", ImGuiTableColumnFlags.WidthFixed, 80);
                            ImGui.TableSetupColumn("UV Y", ImGuiTableColumnFlags.WidthFixed, 80);
                            ImGui.TableHeadersRow();

                            int maxVertices = Math.Min(chunk.VertexCount / 4, 100);
                            
                            for (int i = 0; i < maxVertices * 4; i += 4)
                            {
                                ImGui.TableNextRow();
                                
                                ImGui.TableNextColumn(); ImGui.Text($"{i / 4:D3}");
                                ImGui.TableNextColumn(); ImGui.Text($"{chunk.Vertices[i]:F2}");
                                ImGui.TableNextColumn(); ImGui.Text($"{chunk.Vertices[i + 1]:F2}");
                                ImGui.TableNextColumn(); ImGui.Text($"{chunk.Vertices[i + 2]:F4}");
                                ImGui.TableNextColumn(); ImGui.Text($"{chunk.Vertices[i + 3]:F4}");
                            }

                            if (chunk.VertexCount / 4 > maxVertices)
                            {
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn(); ImGui.Text("...");
                                ImGui.TableNextColumn(); ImGui.TableNextColumn(); 
                                ImGui.TableNextColumn(); ImGui.TableNextColumn();
                            }

                            ImGui.EndTable();
                        }
                    }
                }
                
                ImGui.Spacing();
                
                if (ImGui.CollapsingHeader($"Tile Data Map ({chunk.TileDataMap.Count} Tiles)"))
                {
                    if (chunk.TileDataMap.Count == 0)
                    {
                        ImGui.TextDisabled("Chunk contains no tile data.");
                    }
                    else
                    {
                        foreach (var tileKVP in chunk.TileDataMap)
                        {
                            var localId = tileKVP.Key;
                            var tileData = tileKVP.Value;
                            
                            ImGui.PushID(tileData.GetHashCode());
                            
                            if (ImGui.TreeNode($"Tile ({localId.X}, {localId.Y})"))
                            {
                                bool isVisible = tileData.IsVisible;
                                if (ImGui.Checkbox("Visible", ref isVisible))
                                {
                                    tileData.IsVisible = isVisible;
                                    chunk.IsDirty = true;
                                }
                                
                                ImGui.Text($"Texture ID: {tileData.Texture.TextureId}");
                                
                                ImGui.Text("Source Rect:");
                                
                                float x = tileData.SourceRect.X;
                                float y = tileData.SourceRect.Y;
                                float w = tileData.SourceRect.Width;
                                float h = tileData.SourceRect.Height;
                                
                                bool changed = false;
                                
                                ImGui.PushItemWidth(60);
                                if (ImGui.InputFloat("X##TileSrcX", ref x)) changed = true; ImGui.SameLine();
                                if (ImGui.InputFloat("Y##TileSrcY", ref y)) changed = true; ImGui.SameLine();
                                if (ImGui.InputFloat("W##TileSrcW", ref w)) changed = true; ImGui.SameLine();
                                if (ImGui.InputFloat("H##TileSrcH", ref h)) changed = true;
                                ImGui.PopItemWidth();

                                if (changed)
                                {
                                    var sourceRect = tileData.SourceRect;
                                    sourceRect.X = x;
                                    sourceRect.Y = y;
                                    sourceRect.Width = w;
                                    sourceRect.Height = h;
                                    tileData.SourceRect = sourceRect;
                                    chunk.IsDirty = true; 
                                }

                                ImGui.TreePop();
                            }
                            
                            ImGui.PopID();
                        }
                    }
                }
            }
            else
            {
                ImGui.Text("Select a Tile Chunk from the list.");
            }

            ImGui.EndChild();
            ImGui.End();
        }
    }
}