using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Core
{
    public static class AsyncWrap
    {
        public static async void WrapErrors(this Task rTask)
        {
            try
            {
                await rTask;
            }
            catch (Exception e)
            {
                LogManager.LogError(e.GetType());
                LogManager.LogException(e);
            }
        }

        public static async void WrapErrors(this UniTask rTask)
        {
            try
            {
                await rTask;
            }
            catch (Exception e)
            {
                LogManager.LogError(e.GetType());
                LogManager.LogException(e);
            }
        }
    }
}
