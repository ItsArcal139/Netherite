namespace Netherite.Utils
{
    public static class Preconditions
    {
        public static void IsPositive(double i, string name) => InternalEnsure(i >= 0, i, $"{name} cannot be less than 0.");

        public static void NotNull(object obj, string name) => InternalEnsure(obj != null, obj, $"{name} cannot be null.");

        public static void Ensure(bool b, string message) => InternalEnsure(b, b, message);

        private static void InternalEnsure(bool test, object val, string message)
        {
            if(!test)
            {
                throw new PreconditionFailedException(message, val);
            }
        }
    }
}
