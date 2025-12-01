using System.Collections.ObjectModel;
using System.Diagnostics;
using Silk.NET.OpenGL;

public class Profiler : IProfiler
{
    public class ProfileResultCpu
    {
        public string Tag { get; }
        public float LastTimeMs { get; set; }

        public ProfileResultCpu(string tag)
        {
            Tag = tag;
            LastTimeMs = 0.0f;
        }
    }

    public class ProfileResultGpu
    {
        public string Tag { get; }
        public readonly uint[] GpuQueries = new uint[2];
        public float LastTimeMs { get; set; }

        public int NextQueryIndex { get; set; }

        public ProfileResultGpu(string tag)
        {
            Tag = tag;
            LastTimeMs = 0.0f;
            NextQueryIndex = 0;
        }
    }

    private readonly Dictionary<string, ProfileResultCpu> m_CpuProfileResultMap;
    private readonly Dictionary<string, ProfileResultGpu> m_RenderProfileResultMap;
    private readonly Dictionary<string, Stopwatch> m_CpuProfileStopwatchMap;
    private readonly IRenderPipeline m_RenderPipe;

    public ReadOnlyDictionary<string, ProfileResultGpu> RenderProfileResults => m_RenderProfileResultMap.AsReadOnly();
    public ReadOnlyDictionary<string, ProfileResultCpu> CpuProfileResults => m_CpuProfileResultMap.AsReadOnly();

    public Profiler()
    {
        m_CpuProfileResultMap = new();
        m_RenderProfileResultMap = new();
        m_CpuProfileStopwatchMap = new();

        m_RenderPipe = Service.Get<IRenderPipeline>() ?? throw new Exception("Render Pipeline couldn't be found by Profiler!");
    }

    public void InitializeRenderProfiler(string tag)
    {
        if (!m_RenderProfileResultMap.TryGetValue(tag, out var result))
        {
            result = new ProfileResultGpu(tag);
            m_RenderProfileResultMap.Add(tag, result);
        }

        if (m_RenderPipe.GlApi == null)
        {
            Logger.Log("SetupRenderTimer couldn't find GlApi!", Logger.LogSeverity.Error);
            return;
        }

        if (result.GpuQueries[0] == 0)
        {
            result.GpuQueries[0] = m_RenderPipe.GlApi.GenQuery();
            result.GpuQueries[1] = m_RenderPipe.GlApi.GenQuery();
        }
    }

    public void StartRenderProfile(string tag)
    {
        if (!m_RenderProfileResultMap.TryGetValue(tag, out var result))
        {
            Logger.Log($"Render profile tag '{tag}' not initialized!", Logger.LogSeverity.Error);
            return;
        }

        uint queryToUse = result.GpuQueries[result.NextQueryIndex];
        m_RenderPipe.GlApi!.BeginQuery(GLEnum.TimeElapsed, queryToUse);
    }

    public void StopRenderProfile(string tag)
    {
        if (!m_RenderProfileResultMap.TryGetValue(tag, out var result))
        {
            Logger.Log($"Render profile couldn't find {tag} in the map!", Logger.LogSeverity.Error);
            return;
        }

        m_RenderPipe.GlApi!.EndQuery(GLEnum.TimeElapsed);

        int readIndex = (result.NextQueryIndex + 1) % 2;
        uint queryToCheck = result.GpuQueries[readIndex];

        m_RenderPipe.GlApi.GetQueryObject(queryToCheck, GLEnum.QueryResultAvailable, out uint available);
        if (available != 0)
        {
            ulong timeNs = 0;
            m_RenderPipe.GlApi.GetQueryObject(queryToCheck, GLEnum.QueryResult, out timeNs);
            result.LastTimeMs = timeNs / 1_000_000.0f;
        }
        result.NextQueryIndex = readIndex;
    }

    public ProfileResultGpu? GetRenderProfile(string tag)
    {
        m_RenderProfileResultMap.TryGetValue(tag, out var result);
        return result;
    }

    public void StartCpuProfile(string tag)
    {
        if (!m_CpuProfileStopwatchMap.TryGetValue(tag, out var stopwatch))
        {
            stopwatch = new Stopwatch();
            m_CpuProfileStopwatchMap.Add(tag, stopwatch);
            m_CpuProfileResultMap.Add(tag, new ProfileResultCpu(tag));
        }
        stopwatch.Restart();
    }

    public void StopCpuProfile(string tag)
    {
        if (m_CpuProfileStopwatchMap.TryGetValue(tag, out var stopwatch))
        {
            stopwatch.Stop();
            if (m_CpuProfileResultMap.TryGetValue(tag, out var profileResult))
            {
                profileResult.LastTimeMs = (float)stopwatch.Elapsed.TotalMilliseconds;
            }
        }
        else
        {
            Logger.Log($"Stopwatch for CPU profile tag '{tag}' not found!", Logger.LogSeverity.Error);
        }
    }

    public ProfileResultCpu? GetCpuProfile(string tag)
    {
        m_CpuProfileResultMap.TryGetValue(tag, out var result);
        return result;
    }

    public void Cleanup()
    {
        m_CpuProfileStopwatchMap.Clear();
        m_CpuProfileResultMap.Clear();
        m_RenderProfileResultMap.Clear();
    }
}
