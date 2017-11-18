using System.Collections;
using System;
using System.Diagnostics;

namespace Core
{
    public class RuntimePerformance : TSingleton<RuntimePerformance>
    {
        public class Counter
        {
            public string   Key;
            public long     StartTime;
            public long     TotalTime;
            public long     MaxTime;
            public int      Number;
        }

        private Dict<string, Counter> mRuntimeCounters;

        private RuntimePerformance() 
        {
            mRuntimeCounters = new Dict<string, Counter>();
        }

        [ConditionalAttribute("RuntimePerformance")]
        public void Begin(string rKey)
        {
            Counter rCounter = null;
            if (!mRuntimeCounters.TryGetValue(rKey, out rCounter))
            {
                long nCurTime = DateTime.Now.Ticks;
                rCounter = new Counter() { TotalTime = 0, Key = rKey, Number = 1, MaxTime = 0, StartTime = nCurTime };
            }
            else
            {
                rCounter.Number++;
            }
        }

        [ConditionalAttribute("RuntimePerformance")]
        public void End(string rKey)
        {
            Counter rCounter = null;
            if (mRuntimeCounters.TryGetValue(rKey, out rCounter))
            {
                long nCurTime = DateTime.Now.Ticks;
                rCounter.TotalTime += (nCurTime - rCounter.StartTime);
                if (rCounter.MaxTime < (nCurTime - rCounter.StartTime))
                {
                    rCounter.MaxTime = nCurTime - rCounter.StartTime;
                }

                float fTotalTime = rCounter.TotalTime / 1000000.0f;
                float fAverageTime = (rCounter.TotalTime / rCounter.Number) / 1000000.0f;
                float fMaxTime = rCounter.MaxTime / 1000000.0f;

                Model.Log.Debug(
                    $"RuntimePerformance ==> {rCounter.Key} Count = {rCounter.Number}, TotalTime = {fTotalTime}s, AverageTime = {fAverageTime}s, MaxTime = {fMaxTime}s");
            }
        }
    }
}
