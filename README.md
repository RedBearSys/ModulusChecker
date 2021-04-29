# Modulus Checker 

This is a fork of [Paul D'Ambra's C# implementation of UK Bank Account Modulus Checking](https://github.com/pauldambra/ModulusChecker) but for a .NET Standard 2.0 target. 

Modulus Checking is a process used to determine if a given account number could be valid for a given sort code.

***
Receiving a valid modulus check result only means that the Sort Code and Account Number pair **could** exist not that they do!
***
The algorithms, test cases and reference data can be found  [on the vocalink website]( https://www.vocalink.com/tools/modulus-checking/) . That site should be considered the authoritative source for the modulus checking algorithm and weighting data.

#### Nuget
Modulus Checker is [available on Nuget](https://nuget.org/packages/RedBear.ModulusChecking/). To install it run the following command in the Package Manager Console ```Install-Package RedBear.ModulusChecking``` and reference its namespace as ```using ModulusChecking;```

#### Version Requirements

Targets .NET Standard 2.0.

#### Differences to the Paul D'Ambra version
The `ModulusChecker` class  does **not** return a value of `true` in `ModulusCheckOutcome.Result` if the sort code isn't recognised. It will instead return `null` since `ModulusCheckOutcome.Result` has been changed to a `bool?` type. The logic here is that we don't know if the bank details are valid or not - we just know that we can't validate them.

Additionally, `ModulusChecker` now implements an `IModulusChecker` interface to assist with dependency injection and unit testing.

#### Usage

```csharp
var sortCode = "012345";
var accountNumber = "12345678";
var modulusChecker = new ModulusChecker();
var result = modulusChecker
			.CheckBankAccount(sortCode,accountNumber);
```
If looping over a number of bank account details it is not necessary to initialise the ModulusChecker between checks.

```csharp
var things = new List<BankAccountDetails> { 
  //some items
}; 
var modulusChecker = new ModulusChecker();
var results = things.map(t => 
  modulusChecker.CheckBankAccount(thing.sortCode, thing.accountNumber));
```

#### Explanation Mode

```csharp
const string sortCode = "107999";
const string accountNumber = "88837493";
var modulusChecker = new ModulusChecker();
var outcome = modulusChecker.CheckBankAccountWithExplanation(sortCode,accountNumber);
            
Assert.AreEqual(false, outcome.Result);
Assert.AreEqual("not proceeding to the second check as there is only one weight mapping", outcome.Explanation);

```

or when the sort code is not covered by the modulus checking algorithm 

```csharp
const string sortCode = "000000";
const string accountNumber = "88837493";
var modulusChecker = new ModulusChecker();
var outcome = modulusChecker.CheckBankAccountWithExplanation(sortCode,accountNumber);
            
Assert.IsNull(outcome.Result);
Assert.AreEqual("Cannot invalidate these account details as there are no weight mappings for this sort code", outcome.Explanation);
```

#### License
This software is released under the MIT license. 

NB: the resource text files valacdos.txt and scsubtab.txt are produced and released by Vocalink.

#### Vocalink Version

Currently uses Vocalink Modulus Checking dated 26 April 2021 copied from [their site]( https://www.vocalink.com/tools/modulus-checking/ ).