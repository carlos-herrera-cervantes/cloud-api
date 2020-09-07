namespace Api.Domain.Constants
{
    public static class Operators
    {
        public const string Same = "=";
        public const string NotSame = "!=";
        public const string Greather = ">";
        public const string GreaterThan = ">=";
        public const string Lower = "<";
        public const string LowerThan = "<=";
        public const string SameRegex = @"\W*=\W*";
        public const string NotSameRegex = @"\W*!=\W*";
        public const string GreatherRegex = @"\W*>\W*";
        public const string GreatherThanRegex = @"\W*>=\W*";
        public const string LowerRegex = @"\W*<\W*";
        public const string LowerThanRegex = @"\W*<=\W*";
    }
}