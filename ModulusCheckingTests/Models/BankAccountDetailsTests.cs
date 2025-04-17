using ModulusChecking.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ModulusCheckingTests.Models
{
    public class BankAccountDetailsTests
    {
        private const int ModulusExceptionFlag = -1;

        [Theory]
        [InlineData("1234567890", "080000", "080000", "12345678")]
        [InlineData("1234567890", "830000", "830000", "34567890")]
        [InlineData("12-34567890", "839000", "839000", "12345678")]
        [InlineData("12-34567890", "600000", "600000", "34567890")]
        [InlineData("123456789", "123456",  "123451", "23456789")]
        [InlineData("1234567", "123456",  "123456", "01234567")]
        [InlineData("123456", "123456",  "123456", "00123456")]
        public void CanInstantiateOddAccounts(string accountNumber, string sortCode, string expectedSortCode, string expectedAccountNumber)
        {
            var account = new BankAccountDetails(sortCode, accountNumber);
            Assert.Equal(expectedAccountNumber, account.AccountNumber.ToString());
            Assert.Equal(expectedSortCode,account.SortCode.ToString());
        }

        [Fact]
        public void CannotInitialiseNonDigitAccountNumber()
        {
            Assert.Throws<ArgumentException>(() => new BankAccountDetails("000000", "something"));
        }

        [Fact]
        public void CannotIntialiseTooLongAccountNumber()
        {
            Assert.Throws<ArgumentException>(() => new BankAccountDetails("000000", "123456123456123456"));
        }

        [Fact]
        public void CannotIntialiseTooShortAccountNumber()
        {
            Assert.Throws<ArgumentException>(() => new BankAccountDetails("000000", "1"));
        }
        
        [Fact]
        public void CanInitialiseSortCode()
        {
            Assert.Equal("123456",new BankAccountDetails("123456","12345678").SortCode.ToString());
        }
        
        [Fact]
        public void CannotInitialiseTooShortSortCode()
        {
            Assert.Throws<ArgumentException>(() => new BankAccountDetails("234","12345678"));
        }
        
        [Fact]
        public void CannotInitialiseTooLongSortCode()
        {
            Assert.Throws<ArgumentException>(() => new BankAccountDetails("1234567","12345678"));
        }
        
        [Fact]
        public void CannotInitialiseLetterAsASortCode()
        {
            Assert.Throws<ArgumentException>(() => new BankAccountDetails("a","12345678"));
        }

        [Theory]
        [InlineData("123456","09345678",false)]
        [InlineData("123456", "09345698", true)]
        [InlineData("123456", "99345678", false)]
        [InlineData("123456", "99345698", true)]
        [InlineData("123456", "19345698", false)]
        public void CanValidateForExceptionTen(string sc, string an, bool expected)
        {
            Assert.Equal(expected,new BankAccountDetails(sc,an).AccountNumber.ExceptionTenShouldZeroiseWeights);
        }

        [Theory]
        [InlineData("123456", "09345678", false)]
        [InlineData("123456", "49345698", false)]
        [InlineData("123456", "59345678", false)]
        [InlineData("123456", "69345698", false)]
        [InlineData("123456", "79345698", false)]
        [InlineData("123456", "89345698", false)]
        [InlineData("123456", "09345677", false)]
        [InlineData("123456", "49345699", true)]
        [InlineData("123456", "59345677", true)]
        [InlineData("123456", "69345699", true)]
        [InlineData("123456", "79345688", true)]
        [InlineData("123456", "89345688", true)]
        public void CanValidateForExceptionSix(string sc, string an, bool expected)
        {
            Assert.Equal(expected, new BankAccountDetails(sc, an).AccountNumber.IsForeignCurrencyAccount);
        }

        [Theory]
        [InlineData("123456", "09345678", false)]
        [InlineData("123456", "09345690", true)]
        [InlineData("123456", "99345671", true)]
        [InlineData("123456", "99345699", true)]
        public void CanValidateAsCouttsAccountNumber(string sc, string an, bool expected)
        {
            Assert.Equal(expected, new BankAccountDetails(sc, an).AccountNumber.IsValidCouttsNumber);
        }

        [Theory]
        [InlineData("123455", "01234567", 1,1)]
        [InlineData("123455", "01234567", 0, 0)]
        [InlineData("123455", "01234567", 2, 2)]
        public void CanSetWeightMappings(string sc, string an, int desiredMappings, int expectedCount) {
            var target = new BankAccountDetails(sc, an) {WeightMappings = BuildMappingList(sc, desiredMappings, ModulusExceptionFlag)};
            Assert.Equal(expectedCount, target.WeightMappings.Count());
        }

        [Fact]
        public void CannotSetIncorrectWeightMappings()
        {
            Assert.Throws<InvalidOperationException>( () => new BankAccountDetails("123455", "01234567")
            {
                WeightMappings = BuildMappingList("123455", 3, ModulusExceptionFlag)
            });
        }

        [Theory]
        [InlineData("123455", "01234567", 0, 0, false)]
        [InlineData("123455", "01234567", 1, 0, false)]
        [InlineData("123455", "01234567", 1, 6, false)]
        [InlineData("123455", "01234567", 2, 6, false)]
        [InlineData("123455", "41234566", 1, 6, true)]
        [InlineData("123455", "51234577", 2, 6, true)]
        public void CanCheckForForeignAccountException(string sc, string an, int desiredMappings, int exception, bool expected)
        {
            var target = new BankAccountDetails(sc, an) { WeightMappings = BuildMappingList(sc, desiredMappings, exception) };
            Assert.Equal(expected, target.IsUncheckableForeignAccount());
        }

        [Theory]
        [InlineData("123455", "01234567", 1, 0, false)]
        [InlineData("123455", "01234567", 1, 6, false)]
        [InlineData("123455", "01234567", 2, 2, true)]
        [InlineData("123455", "41234566", 1, 9, true)]
        [InlineData("123455", "51234577", 2, 10, true)]
        [InlineData("123455", "51234577", 2, 11, true)]
        [InlineData("123455", "51234577", 2, 12, true)]
        [InlineData("123455", "51234577", 2, 13, true)]
        [InlineData("123455", "51234577", 2, 14, true)]
        public void CanIdentifyIfSecondCheckIsRequired(string sc, string an, int desiredMappings, int exception, bool expected)
        {
            var target = new BankAccountDetails(sc, an) { WeightMappings = BuildMappingList(sc, desiredMappings, exception) };
            Assert.Equal(expected, target.IsSecondCheckRequired());
        }

        [Fact]
        public void CannotIdentifyIfSecondCheckRequiredWithZeroMappings()
        {
            Assert.Throws<InvalidOperationException>(() => new BankAccountDetails("123455", "01234567")
            {
                WeightMappings = BuildMappingList("123455", 0, 0)
            }.IsSecondCheckRequired());
        }


        [Theory]
        [InlineData("123455", "01234597", 1, new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13},
                                           new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13})]
        [InlineData("123455", "01234587", 7, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                           new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        [InlineData("123455", "01234597", 7, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
                                           new[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 10, 11, 12, 13, 14, 15 })]
        [InlineData("123455", "01234597", 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                           new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        [InlineData("123455", "91234597", 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                           new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        [InlineData("123455", "19234597", 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                           new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        [InlineData("123455", "12345698", 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                           new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        [InlineData("123455", "09345698", 10, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 },
                                           new[] { 0, 0, 0, 0, 0, 0, 0, 0, 9, 10, 11, 12, 13, 14, 15 })]
        [InlineData("123455", "09345698", 1, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 },
                                           new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        public void CanPreProcessForExceptionSevenTenAndThree(string sortCode, string accountNumber, int exception,
                                                   int[] initialWeightMapping, int[] expectedWeightMapping)
        {
            var target = new BankAccountDetails(sortCode, accountNumber)
                             {
                                 WeightMappings = new []
                                 {
                                     MakeModulusWeightMapping(sortCode, initialWeightMapping, exception)
                                 }
                             };
            Assert.Equal(expectedWeightMapping,target.WeightMappings.First().WeightValues);
        }

        [Theory]
        [InlineData("123455", "00000000", 1, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, false)]
        [InlineData("123455", "00300000", 3, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, false)]
        [InlineData("123455", "00600000", 3, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, true)]
        [InlineData("123455", "00900000", 3, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, true)]
        public void IsExceptionThreeAndCanSkipSecondCheck(string sortCode, string accountNumber, int exception,
                                                   int[] initialWeightMapping, bool expected)
        {
            var target = new BankAccountDetails(sortCode, accountNumber)
            {
                WeightMappings = new []
                {
                    MakeModulusWeightMapping(sortCode, initialWeightMapping, exception),
                    MakeModulusWeightMapping(sortCode, initialWeightMapping, exception)
                }
            };
            Assert.Equal(expected, target.IsExceptionThreeAndCanSkipSecondCheck());
        }

        [Theory]
        [InlineData("123456", "01234567", 1, "123456")]
        [InlineData("123456", "01234567", 8, "090126")]
        public void CanPreProcessForExceptionEight(string sc, string an,int exception,string expectedSortCode)
        {
            var target = new BankAccountDetails(sc, an) {WeightMappings = BuildMappingList(sc, 1, exception)};
            Assert.Equal(expectedSortCode,target.SortCode.ToString());
        }

        [Theory]
        [InlineData("123456", "01234567", 1, false)]
        [InlineData("123456", "01234567", 14, true)]
        public void CanVerifyIfRequiresCouttsCheck(string sc, string an, int exception, bool expected)
        {
            var target = new BankAccountDetails(sc, an) {WeightMappings = BuildMappingList(sc, 1, exception)};
            Assert.Equal(expected,target.RequiresCouttsAccountCheck());
        }

        [Theory]
        [InlineData("123455", "00000000", new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 }, new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 })]
        public void CanGetUnchangedExceptionTwoWeightValues(string sc, string an, int[] initialWeights, int[] expectedWeights)
        {
            var target = new BankAccountDetails(sc, an) { WeightMappings = BuildMappingList(sc, 1, 1) };
            Assert.Equal(expectedWeights,target.GetExceptionTwoAlternativeWeights(initialWeights));
        }

        [Fact]
        public void CanGetFirstAlternativeExceptionTwoValue()
        {
            var target = new BankAccountDetails("123456", "10000000") { WeightMappings = BuildMappingList("123456", 1, 1) };
            Assert.Equal(BankAccountDetails.AisNotZeroAndGisNotNineWeights, target.GetExceptionTwoAlternativeWeights(new int[1]));
        }

        [Fact]
        public void CanGetSecondAlternativeExceptionTwoValue()
        {
            var target = new BankAccountDetails("123456", "10000090")
                             {
                                 WeightMappings = BuildMappingList("123456", 1, 1)
                             };
            Assert.Equal(BankAccountDetails.AisNotZeroAndGisNineWeights, target.GetExceptionTwoAlternativeWeights(new int[1]));
        }

        private static IEnumerable<ModulusWeightMapping> BuildMappingList(string sc, int desiredMappings, int exception)
        {
            var items = new List<ModulusWeightMapping>();
            for (var i = 0; i < desiredMappings; i++)
            {
                exception = i == 0
                                ? exception == ModulusExceptionFlag ? i : exception
                                : i;
                items.Add(
                    ModulusWeightMapping.From(
                        string.Format(
                            "{0} 089999 MOD10    0    0    0    0    0    0    7    1    3    7    1    3    7    7    {1}",
                            sc, exception)));
            }
            return items;
        }

        private static ModulusWeightMapping MakeModulusWeightMapping(string sc, int[] initialWeightMappings, int exception)
        {
            return new ModulusWeightMapping(
                new SortCode(sc),
                new SortCode("999999"),
                ModulusAlgorithm.Mod10,
                initialWeightMappings,
                exception
            );
        }
    }
}
