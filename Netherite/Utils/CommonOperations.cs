namespace Netherite.Utils
{
    public static class CommonOperations
    {
        public static void ToPercent(ref double n, string name = "n")
        {
            Preconditions.IsPositive(n, name);
            if (n > 1) n = 1;
        }
    }
}
