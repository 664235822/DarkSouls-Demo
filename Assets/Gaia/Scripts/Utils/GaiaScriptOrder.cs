using System;

namespace Gaia
{
    public class GaiaScriptOrder : Attribute
    {
        public int Order;

        public GaiaScriptOrder(int order)
        {
            Order = order;
        }
    }
}