using System;
using System.Collections.Generic;

namespace NaughtyAttributes
{
    public class DrawOrderAttribute : NaughtyAttribute
    {
        public int  Order = 0;

        public DrawOrderAttribute(int nOrder)
        {
            this.Order = nOrder;
        }
    }
}
