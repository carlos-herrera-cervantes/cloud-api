using System;
using System.Linq;
using System.Text.RegularExpressions;
using Api.Domain.Constants;
using Api.Domain.Models;

namespace Api.Repository.Extensions
{
    public static class StringExtensions
    {
        public static TypeOperator ClassifyOperation(this string value) =>
            Regex.IsMatch(value, Operators.NotSameRegex) ? new TypeOperator { 
                Key = Regex.Split(value, Operators.NotSameRegex).First(),
                Operation = Operators.NotSame, 
                Value = Regex.Split(value, Operators.NotSameRegex).Last() } :
            Regex.IsMatch(value, Operators.GreatherThanRegex) ? new TypeOperator {
                Key = Regex.Split(value, Operators.GreatherThanRegex).First(),
                Operation = Operators.GreaterThan,
                Value = Regex.Split(value, Operators.GreatherThanRegex).Last() } :
            Regex.IsMatch(value, Operators.LowerThanRegex) ? new TypeOperator {
                Key = Regex.Split(value, Operators.LowerThanRegex).First(),
                Operation = Operators.LowerThan,
                Value = Regex.Split(value, Operators.LowerThanRegex).Last() } :
            Regex.IsMatch(value, Operators.SameRegex) ? new TypeOperator {
                Key = Regex.Split(value, Operators.SameRegex).First(), 
                Operation = Operators.Same, 
                Value = Regex.Split(value, Operators.SameRegex).Last() } :
            Regex.IsMatch(value, Operators.GreatherRegex) ? new TypeOperator { 
                Key = Regex.Split(value, Operators.GreatherRegex).First(), 
                Operation = Operators.Greather, 
                Value = Regex.Split(value, Operators.GreatherRegex).Last() } :
            Regex.IsMatch(value, Operators.LowerRegex) ? new TypeOperator {
                Key = Regex.Split(value, Operators.LowerRegex).First(),
                Operation = Operators.Lower,
                Value = Regex.Split(value, Operators.LowerRegex).Last() } : 
            throw new InvalidOperationException();
    }
}