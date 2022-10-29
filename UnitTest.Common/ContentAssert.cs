using System;
using KellermanSoftware.CompareNetObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Common
{
    public static class ContentAssert
    {
        public static void AreEqual(object expected, object actual, string errorMessage = null)
        {
            var compareObjects = new CompareLogic(new ComparisonConfig());

            var comparisonResult = compareObjects.Compare(expected, actual);

            if (comparisonResult.AreEqual)
                return;

            throw new AssertFailedException(string.Format("{0}.\n {1}", comparisonResult.DifferencesString,
                errorMessage));
        }
    }
}