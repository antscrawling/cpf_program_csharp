// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;

public class CPFAccount
{
    // Existing properties
    public double OABalance { get; set; }
    public double SABalance { get; set; }
    public double MABalance { get; set; }
    public double RABalance { get; set; }
    public double ExcessBalance { get; set; }
    public double LoanBalance { get; set; }
    public int Age { get; set; }
    public double Salary { get; set; }
    public double SalaryCap { get; set; }
    public double TotalContribution { get; set; }
    
    // Retirement sum properties
    public double RetirementSumBRSAmount { get; set; }
    public double RetirementSumFRSAmount { get; set; }
    public double RetirementSumERSAmount { get; set; }
    public double RetirementSumBRSPayout { get; set; }
    public double RetirementSumFRSPayout { get; set; }
    public double RetirementSumERSPayout { get; set; }
    
    // CPF policy properties 
    public string? OwnHDB { get; set; }
    public string? PayoutType { get; set; }
    public string? PledgeYourHDBAt55 { get; set; }
    public int CPFPayoutAge { get; set; }
    public double MonthlyPayout { get; set; }
    
    // Interest rate properties
    public double OAInterestRateBelow55 { get; set; }
    public double OAInterestRateAbove55 { get; set; }
    public double SAInterestRate { get; set; }
    public double MAInterestRate { get; set; }
    public double RAInterestRate { get; set; }
    
    // Extra interest rate properties
    public double ExtraInterestBelow55 { get; set; }
    public double ExtraInterestFirst30kAbove55 { get; set; }
    public double ExtraInterestNext30kAbove55 { get; set; }
    
    // Contribution rate properties
    public double EmployeeContributionRateBelow55 { get; set; }
    public double EmployerContributionRateBelow55 { get; set; }
    public double EmployeeContributionRate55to60 { get; set; }
    public double EmployerContributionRate55to60 { get; set; }
    public double EmployeeContributionRate60to65 { get; set; }
    public double EmployerContributionRate60to65 { get; set; }
    public double EmployeeContributionRate65to70 { get; set; }
    public double EmployerContributionRate65to70 { get; set; }
    public double EmployeeContributionRateAbove70 { get; set; }
    public double EmployerContributionRateAbove70 { get; set; }
    
    // Allocation rates
    public double AllocationBelow55OA { get; set; }
    public double AllocationBelow55SA { get; set; }
    public double AllocationBelow55MA { get; set; }
    
    public double Allocation55to60OA { get; set; }
    public double Allocation55to60SA { get; set; }
    public double Allocation55to60MA { get; set; }
    public double Allocation55to60RA { get; set; }
    
    public double Allocation60to65OA { get; set; }
    public double Allocation60to65SA { get; set; }
    public double Allocation60to65MA { get; set; }
    public double Allocation60to65RA { get; set; }
    
    public double Allocation65to70OA { get; set; }
    public double Allocation65to70SA { get; set; }
    public double Allocation65to70MA { get; set; }
    public double Allocation65to70RA { get; set; }
    
    public double AllocationAbove70OA { get; set; }
    public double AllocationAbove70SA { get; set; }
    public double AllocationAbove70MA { get; set; }
    public double AllocationAbove70RA { get; set; }
    
    // Loan payment properties
    public double LoanPaymentYear12 { get; set; }
    public double LoanPaymentYear3 { get; set; }
    public double LoanPaymentYear4beyond { get; set; }

    public void PrintProjection(DateTime startDate, int currentAge, int targetAge, DateTime birthDate)
    {
        Console.WriteLine("Monthly CPF Balance Projection:");
        Console.WriteLine("Month-Year | Age | OA Balance | SA Balance | MA Balance | RA Balance | Loan Balance | Excess Balance");

        DateTime currentDate = startDate; // Start from the provided start date
        int projectionYear = 1; // Track which year of projection we're in
        
        while (true)
        {
            // Calculate age based on the LAST DAY of the current month 
            // This ensures the age reflects any birthdays that occur during the month
            DateTime lastDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 
                                                 DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            
            int age = lastDayOfMonth.Year - birthDate.Year;
            if (birthDate.Month > lastDayOfMonth.Month || 
                (birthDate.Month == lastDayOfMonth.Month && birthDate.Day > lastDayOfMonth.Day))
            {
                age--;
            }

            Console.WriteLine($"{currentDate:MMM-yyyy} | {age} | {OABalance:F2} | {SABalance:F2} | {MABalance:F2} | {RABalance:F2} | {LoanBalance:F2} | {ExcessBalance:F2}");

            // Step 1: Calculate total contribution based on age and salary cap
            double employeeRate = 0.0;
            double employerRate = 0.0;
            
            if (age < 55)
            {
                employeeRate = EmployeeContributionRateBelow55;
                employerRate = EmployerContributionRateBelow55;
            }
            else if (age >= 55 && age < 60)
            {
                employeeRate = EmployeeContributionRate55to60;
                employerRate = EmployerContributionRate55to60;
            }
            else if (age >= 60 && age < 65)
            {
                employeeRate = EmployeeContributionRate60to65;
                employerRate = EmployerContributionRate60to65;
            }
            else if (age >= 65 && age < 70)
            {
                employeeRate = EmployeeContributionRate65to70;
                employerRate = EmployerContributionRate65to70;
            }
            else
            {
                employeeRate = EmployeeContributionRateAbove70;
                employerRate = EmployerContributionRateAbove70;
            }
            
            double employeeContribution = SalaryCap * employeeRate;
            double employerContribution = SalaryCap * employerRate;
            TotalContribution = employeeContribution + employerContribution;
            
            // Step 2: Allocate contributions to each account based on age
            double oaAllocation = 0.0;
            double saAllocation = 0.0;
            double maAllocation = 0.0;
            double raAllocation = 0.0;
            
            if (age < 55)
            {
                oaAllocation = TotalContribution * AllocationBelow55OA;
                saAllocation = TotalContribution * AllocationBelow55SA;
                maAllocation = TotalContribution * AllocationBelow55MA;
            }
            else if (age >= 55 && age < 60)
            {
                oaAllocation = TotalContribution * Allocation55to60OA;
                // SA is closed at age 55. 
               // saAllocation = TotalContribution * Allocation55to60SA;
                maAllocation = TotalContribution * Allocation55to60MA;
                raAllocation = TotalContribution * Allocation55to60RA;
            }
            else if (age >= 60 && age < 65)
            {
                oaAllocation = TotalContribution * Allocation60to65OA;
                saAllocation = TotalContribution * Allocation60to65SA;
                maAllocation = TotalContribution * Allocation60to65MA;
                raAllocation = TotalContribution * Allocation60to65RA;
            }
            else if (age >= 65 && age < 70)
            {
                oaAllocation = TotalContribution * Allocation65to70OA;
                saAllocation = TotalContribution * Allocation65to70SA;
                maAllocation = TotalContribution * Allocation65to70MA;
                raAllocation = TotalContribution * Allocation65to70RA;
            }
            else
            {
                oaAllocation = TotalContribution * AllocationAbove70OA;
                saAllocation = TotalContribution * AllocationAbove70SA;
                maAllocation = TotalContribution * AllocationAbove70MA;
                raAllocation = TotalContribution * AllocationAbove70RA;
            }
            
            // Add allocated contributions to appropriate accounts
            // After age 55, OA and SA work differently
            if (age < 55)
            {
                // Before age 55, contributions go to OA, SA, and MA as normal
                OABalance += oaAllocation;
                SABalance += saAllocation;
                MABalance += maAllocation;
                
                // Redistribute RA allocation to OA, SA, and MA according to below-55 allocation ratios
                // since RA doesn't exist before age 55
                if (raAllocation > 0)
                {
                    double totalRatio = AllocationBelow55OA + AllocationBelow55SA + AllocationBelow55MA;
                    OABalance += raAllocation * (AllocationBelow55OA / totalRatio);
                    SABalance += raAllocation * (AllocationBelow55SA / totalRatio);
                    MABalance += raAllocation * (AllocationBelow55MA / totalRatio);
                }
            }
            else
            {
                // After age 55
                if (RABalance == 0 && age == 55)
                {
                    // For the first month at age 55, we still add to OA and SA
                    // These will be transferred to RA later in Step 6
                    OABalance += oaAllocation;
                    SABalance += saAllocation;
                    MABalance += maAllocation;
                    // Store the RA allocation for use in Step 6
                    // We already calculated raAllocation above, no need to recalculate
                }
                else
                {
                    // After the first month at 55, OA is still used but SA is closed
                    OABalance += oaAllocation;
                    // No allocation to SA after 55
                    MABalance += maAllocation;
                    
                    // Always add RA allocation to RA balance directly (not to excess)
               //     Console.WriteLine($"Adding monthly RA allocation: ${raAllocation:F2} to RA balance");
                    RABalance += raAllocation;
                }
            }
            
            // Step 3: Calculate the interest rate based on age
            double oaRate = (age < 55) ? OAInterestRateBelow55 : OAInterestRateAbove55;

            // Calculate base interest for each account
            // paid every December
            double oaBaseInterest = OABalance * oaRate;
            double saBaseInterest = SABalance * SAInterestRate;
            double maBaseInterest = MABalance * MAInterestRate;
            // Only calculate RA interest if age is 55 or above
            double raBaseInterest = (age >= 55) ? RABalance * RAInterestRate : 0;
            
            // Step 4 & 5: Calculate extra interest based on age
            double extraInterest = 0.0;
            
            if (age < 55)
            {
                // Step 4: Extra interest for below 55
                // The first 60000 combined balances get extra interest
                // OA Balance has a cap of 20000
                double oaForExtraInterest = Math.Min(OABalance, 20000);
                double remainingBalanceForExtraInterest = Math.Min(60000 - oaForExtraInterest, SABalance + MABalance);
                double combinedBalanceForExtraInterest = oaForExtraInterest + remainingBalanceForExtraInterest;
                // paid only in December
                extraInterest = combinedBalanceForExtraInterest * ExtraInterestBelow55 ;
            }
            else
            {
                // Step 5: Extra interest for above 55
                // The first 30000 combined balance gets 2% extra interest
                // OA Balance has a cap of 20000
                double oaForExtraInterest = Math.Min(OABalance, 20000);
                
                // Include RA in combined balance only at age 55 or above
                double combinedBalance = (age >= 55) 
                    ? oaForExtraInterest + SABalance + MABalance + RABalance
                    : oaForExtraInterest + SABalance + MABalance;
                
                double first30kBalance = Math.Min(combinedBalance, 30000);
                extraInterest += first30kBalance * ExtraInterestFirst30kAbove55;
                
                // The next 30000 combined balance gets 1% extra interest
                double next30kBalance = Math.Min(Math.Max(0, combinedBalance - 30000), 30000);
                extraInterest += next30kBalance * ExtraInterestNext30kAbove55;
            }
            // Base Interest is allocated to respective accounts in December
            // Extra interest allocation depends on age
            if (currentDate.Month == 12)
            {
                OABalance += oaBaseInterest;
                SABalance += saBaseInterest;
                MABalance += maBaseInterest;
                
                if (age >= 55)
                {
                    // Log interest calculation for RA account
                    Console.WriteLine($"Applying annual interest in December - Base RA Interest: ${raBaseInterest:F2}, Extra Interest: ${extraInterest:F2}");
                    Console.WriteLine($"Before interest - RA: ${RABalance:F2}");
                    
                    // After 55, extra interest goes to RA
                    RABalance += raBaseInterest + extraInterest;
                    
                    Console.WriteLine($"After interest - RA: ${RABalance:F2}");
                }
                else
                {
                    // Before 55, distribute extra interest proportionally to SA and MA
                    // since RA doesn't exist and OA doesn't get extra interest
                    double totalNonOABalance = SABalance + MABalance;
                    if (totalNonOABalance > 0)
                    {
                        SABalance += extraInterest * (SABalance / totalNonOABalance);
                        MABalance += extraInterest * (MABalance / totalNonOABalance);
                    }
                }
            }
           
           
           
           
           
            

            // Apply loan payment
            double monthlyLoanPayment = 0.0;
            if (projectionYear <= 2)
            {
                monthlyLoanPayment = LoanPaymentYear12;
            }
            else if (projectionYear == 3)
            {
                monthlyLoanPayment = LoanPaymentYear3;
            }
            else
            {
                monthlyLoanPayment = LoanPaymentYear4beyond;
            }
            
            // Apply loan payment if loan balance is greater than the payment
            if (LoanBalance > monthlyLoanPayment)
            {
                LoanBalance -= monthlyLoanPayment;
            }
            else if (LoanBalance > 0)
            {
                // If loan balance is less than the payment, pay off the remaining balance
                LoanBalance = 0;
            }

            // Step 6 : One time event at age 55
            // At age == 55, handle the creation of RA account
            if (age == 55)
            {
                // Check if this is the first month at age 55 by looking at RABalance
                // If RABalance is 0, this is the first month at age 55
                if (RABalance == 0)
                {
                    Console.WriteLine($"Creating RA account at age 55 on {currentDate:yyyy-MM-dd}");
                    Console.WriteLine($"Before transfer - OA: ${OABalance:F2}, SA: ${SABalance:F2}, RA: ${RABalance:F2}, RA Allocation: ${raAllocation:F2}");
                    
                    // Transfer OA and SA balances to RA and include this month's RA allocation
                    RABalance = OABalance + SABalance + raAllocation - LoanBalance;
                    LoanBalance = 0; // Loan balance is cleared after transfer
                    OABalance = 0; // OA Balance will be used for new contributions only
                    SABalance = 0; // SA Balance is now closed
                    
                    Console.WriteLine($"After transfer - OA: ${OABalance:F2}, SA: ${SABalance:F2}, RA: ${RABalance:F2}");
                }
            }
            
            // Step 7: Calculate excess balance based on policy
            // This should happen right after the RA account is created and funded
            if (age == 55 && RABalance > 0)
            {
                // Calculate excess balance only after OA and SA have been transferred to RA (first month at 55)
                double retirementSumCap = 0.0;
                
                if (OwnHDB == "yes" && PledgeYourHDBAt55 == "yes")
                {
                    // If own HDB and pledge it, then RA Balance is half of FRS amount
                    retirementSumCap = RetirementSumFRSAmount / 2;
                }
                else
                {
                    // Otherwise use the payout type from config
                    if (PayoutType == "brs")
                    {
                        // BRS: Basic Retirement Sum
                        retirementSumCap = RetirementSumBRSAmount;
                    }
                    else if (PayoutType == "ers")
                    {
                        // ERS: Enhanced Retirement Sum
                        retirementSumCap = RetirementSumERSAmount;
                    }
                    else if (PayoutType == "frs")
                    {
                        // FRS: Full Retirement Sum
                        retirementSumCap = RetirementSumFRSAmount;
                    }
                }
                
                // Only calculate excess balance once when RA is first created
                // This ensures monthly RA allocations after age 55 are added to RA
                if (ExcessBalance == 0)
                {
                    // Calculate excess if RA balance exceeds retirement sum cap
                    if (RABalance > retirementSumCap)
                    {
                        ExcessBalance = RABalance - retirementSumCap;
                        RABalance = retirementSumCap;
                        
                        Console.WriteLine($"Excess calculated: RA capped at ${retirementSumCap:F2}, Excess: ${ExcessBalance:F2}");
                    }
                }
                
                // Ensure excess balance is not negative
                if (ExcessBalance < 0)
                {
                    ExcessBalance = 0;
                }
            }            // Step 8: Calculate the CPF Payout at payout age
            if (age == CPFPayoutAge)
            {
                // Calculate monthly payout based on the retirement sum type
            //   if (OwnHDB == "yes" && PledgeYourHDBAt55 == "yes")
            //   {
            //       // If own HDB and pledge it, payout is based on BRS
            //       MonthlyPayout = RetirementSumBRSPayout;
            //   }
                if (PayoutType == "brs")
                {
                    MonthlyPayout = RetirementSumBRSPayout;
                }
                else if (PayoutType == "ers")
                {
                    MonthlyPayout = RetirementSumERSPayout;
                }
                else if (PayoutType == "frs")
                {
                    MonthlyPayout = RetirementSumFRSPayout;
                }
                
                // Log the start of payout
                Console.WriteLine($"CPF payout starts at age {CPFPayoutAge}: ${MonthlyPayout:F2} per month");
            }
            
            // Apply monthly payout if age is at or above payout age
            if (age >= CPFPayoutAge && MonthlyPayout > 0)
            {
                // Logging to track payout process
                Console.WriteLine($"Processing monthly payout at age {age}: ${MonthlyPayout:F2}");
                Console.WriteLine($"Before payout - RA: ${RABalance:F2}, Excess: ${ExcessBalance:F2}");
                
                // Deduct monthly payout from RA balance
                if (RABalance >= MonthlyPayout)
                {
                    RABalance -= MonthlyPayout;
                    ExcessBalance += MonthlyPayout; // Add to excess balance if RA payout is sufficient
                }
                else
                {
                    // If RA balance is insufficient, take from excess
                    double remainingPayout = MonthlyPayout - RABalance;
                    ExcessBalance += RABalance; // Add whatever is left in RA to excess
                    RABalance = 0;
                }
                
                Console.WriteLine($"After payout - RA: ${RABalance:F2}, Excess: ${ExcessBalance:F2}");
            }

            currentDate = currentDate.AddMonths(1); // Move to the next month
            
            // Check if we've moved to a new projection year
            if (currentDate.Month == startDate.Month && currentDate.Day == startDate.Day)
            {
                projectionYear++;
            }
            
            // Check if we've reached the target age using the same last-day-of-month logic for consistency
            DateTime nextMonthLastDay = new DateTime(currentDate.Year, currentDate.Month, 
                                                    DateTime.DaysInMonth(currentDate.Year, currentDate.Month));
            
            int nextAge = nextMonthLastDay.Year - birthDate.Year;
            if (birthDate.Month > nextMonthLastDay.Month || 
                (birthDate.Month == nextMonthLastDay.Month && birthDate.Day > nextMonthLastDay.Day))
            {
                nextAge--;
            }
            
            // If reached target age + 1, break
            if (nextAge > targetAge)
                break;
        }
    }
}

public class Config
{
    // Existing properties
    public double OABalance { get; set; }
    public double SABalance { get; set; }
    public double MABalance { get; set; }
    public double RABalance { get; set; }
    public double ExcessBalance { get; set; }
    public double LoanBalance { get; set; }
    public double Salary { get; set; }
    public double SalaryCap { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime BirthDate { get; set; }
    
    // Interest rate properties - match JSON property names exactly
    public double InterestratesOabelow55 { get; set; }
    public double InterestratesOaabove55 { get; set; }
    public double InterestratessA { get; set; }
    public double InterestratesMa { get; set; }
    public double InterestratesRa { get; set; }
    
    // Extra interest rate properties - match JSON property names exactly
    public double ExtrainterestBelow55 { get; set; }
    public double ExtrainterestFirst30kabove55 { get; set; }
    public double ExtrainterestNext30kabove55 { get; set; }
    
    // Retirement sum properties - match JSON property names exactly
    public double RetirementsumsbrSamount { get; set; }
    public double RetirementsumsfrSamount { get; set; }
    public double RetirementsumserSamount { get; set; }
    public double RetirementsumsbrSpayout { get; set; }
    public double RetirementsumsfrSpayout { get; set; }
    public double RetirementsumserSpayout { get; set; }
    
    // CPF policy properties - match JSON property names exactly
    public string? Ownhdb { get; set; }
    public string? Payouttype { get; set; }
    public string? Pledgeyourhdbat55 { get; set; }
    public int Cpfpayoutage { get; set; }
    
    // Loan payment properties - match JSON property names exactly
    public double LoanpaymentsYear12 { get; set; }
    public double LoanpaymentsYear3 { get; set; }
    public double LoanpaymentsYear4beyond { get; set; }
    
    // Contribution rate properties - match JSON property names exactly
    public double CpfcontributionratesBelow55employee { get; set; }
    public double CpfcontributionratesBelow55employer { get; set; }
    public double Cpfcontributionrates55to60employee { get; set; }
    public double Cpfcontributionrates55to60employer { get; set; }
    public double Cpfcontributionrates60to65employee { get; set; }
    public double Cpfcontributionrates60to65employer { get; set; } 
    public double Cpfcontributionrates65to70employee { get; set; }
    public double Cpfcontributionrates65to70employer { get; set; }
    public double CpfcontributionratesAbove70employee { get; set; }
    public double CpfcontributionratesAbove70employer { get; set; }
    
    // Allocation rate properties - match JSON property names exactly
    public double AllocationBelow55oaallocation { get; set; }
    public double AllocationBelow55saallocation { get; set; }
    public double AllocationBelow55maallocation { get; set; }
    
    public double Allocationabove55oa56to60allocation { get; set; }
    public double Allocationabove55saallocation { get; set; }
    public double Allocationabove55ma56to60allocation { get; set; }
    public double Allocationabove55ra56to60allocation { get; set; }
    
    public double Allocationabove55oa61to65allocation { get; set; }
    public double Allocationabove55ma61to65allocation { get; set; }
    public double Allocationabove55ra61to65allocation { get; set; }
    
    public double Allocationabove55oa66to70allocation { get; set; }
    public double Allocationabove55ma66to70allocation { get; set; }
    public double Allocationabove55ra66to70allocation { get; set; }
    
    public double Allocationabove55oaabove70allocation { get; set; }
    public double Allocationabove55maabove70allocation { get; set; }
    public double Allocationabove55raabove70allocation { get; set; }
    
    public Dictionary<string, object> AdditionalAttributes { get; set; } = new();

    // Calculate age based on birthdate and current date
    public int GetAge(DateTime referenceDate)
    {
        int age = referenceDate.Year - BirthDate.Year;
        
        // Adjust age if birthday hasn't occurred yet this year
        if (BirthDate.Month > referenceDate.Month || 
            (BirthDate.Month == referenceDate.Month && BirthDate.Day > referenceDate.Day))
        {
            age--;
        }
        
        return age;
    }

    public static Config LoadFromFile(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        
        // Configure JsonSerializerOptions to handle date formats
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Allow case-insensitive property matching
        };
        
        var config = JsonSerializer.Deserialize<Config>(json, options);

        if (config == null)
        {
            throw new InvalidOperationException("Failed to load configuration from the JSON file.");
        }

        return config;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Please provide the path to the JSON configuration file.");
            return;
        }

        string jsonFilePath = args[0];
        try
        {
            var config = Config.LoadFromFile(jsonFilePath);

            // Calculate age based on birthdate and start date
            int currentAge = config.GetAge(config.StartDate);
            
            var cpfAccount = new CPFAccount
            {
                OABalance = config.OABalance,
                SABalance = config.SABalance,
                MABalance = config.MABalance,
                RABalance = config.RABalance,
                ExcessBalance = config.ExcessBalance,
                LoanBalance = config.LoanBalance,
                Age = currentAge,
                Salary = config.Salary,
                SalaryCap = config.SalaryCap,
                
                // Set interest rates from config
                OAInterestRateBelow55 = config.InterestratesOabelow55 / 100.0,
                OAInterestRateAbove55 = config.InterestratesOaabove55 / 100.0,
                SAInterestRate = config.InterestratessA / 100.0,
                MAInterestRate = config.InterestratesMa / 100.0,
                RAInterestRate = config.InterestratesRa / 100.0,
                
                // Set extra interest rates from config
                ExtraInterestBelow55 = config.ExtrainterestBelow55 / 100.0,
                ExtraInterestFirst30kAbove55 = config.ExtrainterestFirst30kabove55 / 100.0,
                ExtraInterestNext30kAbove55 = config.ExtrainterestNext30kabove55 / 100.0,
                
                // Set contribution rates from config
                EmployeeContributionRateBelow55 = config.CpfcontributionratesBelow55employee,
                EmployerContributionRateBelow55 = config.CpfcontributionratesBelow55employer,
                EmployeeContributionRate55to60 = config.Cpfcontributionrates55to60employee,
                EmployerContributionRate55to60 = config.Cpfcontributionrates55to60employer,
                EmployeeContributionRate60to65 = config.Cpfcontributionrates60to65employee,
                EmployerContributionRate60to65 = config.Cpfcontributionrates60to65employer,
                EmployeeContributionRate65to70 = config.Cpfcontributionrates65to70employee,
                EmployerContributionRate65to70 = config.Cpfcontributionrates65to70employer,
                EmployeeContributionRateAbove70 = config.CpfcontributionratesAbove70employee,
                EmployerContributionRateAbove70 = config.CpfcontributionratesAbove70employer,
                
                // Set allocation rates from config
                AllocationBelow55OA = config.AllocationBelow55oaallocation,
                AllocationBelow55SA = config.AllocationBelow55saallocation,
                AllocationBelow55MA = config.AllocationBelow55maallocation,
                
                Allocation55to60OA = config.Allocationabove55oa56to60allocation,
                Allocation55to60SA = config.Allocationabove55saallocation,
                Allocation55to60MA = config.Allocationabove55ma56to60allocation,
                Allocation55to60RA = config.Allocationabove55ra56to60allocation,
                
                Allocation60to65OA = config.Allocationabove55oa61to65allocation,
                Allocation60to65SA = config.Allocationabove55saallocation,
                Allocation60to65MA = config.Allocationabove55ma61to65allocation,
                Allocation60to65RA = config.Allocationabove55ra61to65allocation,
                
                Allocation65to70OA = config.Allocationabove55oa66to70allocation,
                Allocation65to70SA = config.Allocationabove55saallocation,
                Allocation65to70MA = config.Allocationabove55ma66to70allocation,
                Allocation65to70RA = config.Allocationabove55ra66to70allocation,
                
                AllocationAbove70OA = config.Allocationabove55oaabove70allocation,
                AllocationAbove70SA = config.Allocationabove55saallocation,
                AllocationAbove70MA = config.Allocationabove55maabove70allocation,
                AllocationAbove70RA = config.Allocationabove55raabove70allocation,
                
                // Set loan payments from config
                LoanPaymentYear12 = config.LoanpaymentsYear12,
                LoanPaymentYear3 = config.LoanpaymentsYear3,
                LoanPaymentYear4beyond = config.LoanpaymentsYear4beyond,
                
                // Set retirement sum properties
                RetirementSumBRSAmount = config.RetirementsumsbrSamount,
                RetirementSumFRSAmount = config.RetirementsumsfrSamount,
                RetirementSumERSAmount = config.RetirementsumserSamount,
                RetirementSumBRSPayout = config.RetirementsumsbrSpayout,
                RetirementSumFRSPayout = config.RetirementsumsfrSpayout,
                RetirementSumERSPayout = config.RetirementsumserSpayout,
                
                // Set CPF policy properties
                OwnHDB = config.Ownhdb,
                PayoutType = config.Payouttype,
                PledgeYourHDBAt55 = config.Pledgeyourhdbat55,
                CPFPayoutAge = config.Cpfpayoutage
            };

            DateTime startDate = config.StartDate;
            int targetAge = 90;

            Console.WriteLine($"Starting projection at age {currentAge} (born on {config.BirthDate:yyyy-MM-dd})");
            
            // Calculate initial contribution based on age
            double employeeRate = 0.0;
            double employerRate = 0.0;
            
            if (currentAge < 55)
            {
                employeeRate = cpfAccount.EmployeeContributionRateBelow55;
                employerRate = cpfAccount.EmployerContributionRateBelow55;
            }
            else if (currentAge >= 55 && currentAge < 60)
            {
                employeeRate = cpfAccount.EmployeeContributionRate55to60;
                employerRate = cpfAccount.EmployerContributionRate55to60;
            }
            else if (currentAge >= 60 && currentAge < 65)
            {
                employeeRate = cpfAccount.EmployeeContributionRate60to65;
                employerRate = cpfAccount.EmployerContributionRate60to65;
            }
            else if (currentAge >= 65 && currentAge < 70)
            {
                employeeRate = cpfAccount.EmployeeContributionRate65to70;
                employerRate = cpfAccount.EmployerContributionRate65to70;
            }
            else
            {
                employeeRate = cpfAccount.EmployeeContributionRateAbove70;
                employerRate = cpfAccount.EmployerContributionRateAbove70;
            }
            
            double employeeContribution = cpfAccount.SalaryCap * employeeRate;
            double employerContribution = cpfAccount.SalaryCap * employerRate;
            cpfAccount.TotalContribution = employeeContribution + employerContribution;

            Console.WriteLine($"Initial Calculation:");
            Console.WriteLine($"Employee Contribution Rate: {employeeRate:P1}");
            Console.WriteLine($"Employer Contribution Rate: {employerRate:P1}");
            Console.WriteLine($"Employee Contribution: ${employeeContribution:F2}");
            Console.WriteLine($"Employer Contribution: ${employerContribution:F2}");
            Console.WriteLine($"Total Contribution: ${cpfAccount.TotalContribution:F2}");
            Console.WriteLine();

            cpfAccount.PrintProjection(startDate, currentAge, targetAge, config.BirthDate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}


