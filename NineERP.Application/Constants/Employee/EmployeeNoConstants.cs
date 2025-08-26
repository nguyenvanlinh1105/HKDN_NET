namespace NineERP.Application.Constants.Employee;
public static class EmployeeNoConstants
{
    // Employee
    public const string EmployeeNumberKey = "EMPLOYEE_SHORT_NAME_NUMBER";
    public const string EmployeePrefixKey = "EMPLOYEE_SHORT_NAME_APP";
    public const string InternNumberKey = "INTERN_SHORT_NAME_NUMBER";
    public const string InternPrefixKey = "INTERN_SHORT_NAME_APP";
}

public static class ContractNoConstants
{
    public const string OfficialContractNumberKey = "CONTRACT_OFFICIAL_NUMBER";
    public const string OfficialContractPrefixKey = "CONTRACT_OFFICIAL_PREFIX";

    public const string TemporaryContractNumberKey = "CONTRACT_TEMPORARY_NUMBER";
    public const string TemporaryContractPrefixKey = "CONTRACT_TEMPORARY_PREFIX";

    public const string FlexibleContractNumberKey = "CONTRACT_FLEXIBLE_NUMBER";
    public const string FlexibleContractPrefixKey = "CONTRACT_FLEXIBLE_PREFIX";

    public const string ProjectContractNumberKey = "CONTRACT_PROJECT_NUMBER";
    public const string ProjectContractPrefixKey = "CONTRACT_PROJECT_PREFIX";

    public const string InternshipContractNumberKey = "CONTRACT_INTERNSHIP_NUMBER";
    public const string InternshipContractPrefixKey = "CONTRACT_INTERNSHIP_PREFIX";

    public const string ProbationContractNumberKey = "CONTRACT_PROBATION_NUMBER";
    public const string ProbationContractPrefixKey = "CONTRACT_PROBATION_PREFIX";

    public static readonly Dictionary<string, string> GroupCodeToNumberKeyMap = new()
    {
        { "OFFICIAL", OfficialContractNumberKey },
        { "TEMPORARY", TemporaryContractNumberKey },
        { "FLEXIBLE", FlexibleContractNumberKey },
        { "PROJECT", ProjectContractNumberKey },
        { "INTERNSHIP", InternshipContractNumberKey },
        { "PROBATION", ProbationContractNumberKey }
    };

    public static readonly Dictionary<string, string> GroupCodeToPrefixKeyMap = new()
    {
        { "OFFICIAL", OfficialContractPrefixKey },
        { "TEMPORARY", TemporaryContractPrefixKey },
        { "FLEXIBLE", FlexibleContractPrefixKey },
        { "PROJECT", ProjectContractPrefixKey },
        { "INTERNSHIP", InternshipContractPrefixKey },
        { "PROBATION", ProbationContractPrefixKey }
    };
}
