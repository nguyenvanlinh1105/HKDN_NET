namespace NineERP.Application.Constants.Employee
{
    public static class StaticVariable
    {
        // Employee Status
        public enum EmployeeStatus
        {
            OFFICIAL = 1,
            UNOFFICIAL = 2,
            RETIRED = 3
        }
        // My leave type
        public enum MyLeaveType
        {            
            ANNUAL_LEAVE = 1,   // nghỉ có trừ phép, có lương (ngày)            
            UNPAID_LEAVE = 2,   // nghỉ không bị trừ phép, không lương (ngày)            
            GO_IN_OUT = 3       // xin phép ra vào, không lương (giờ)
        }

        public enum NumberOfUpdateEmployeeLogTime
        {
            Number = 3
        }

        public enum ApproveStatus : short
        {
            Waiting = 0,
            Accepted = 1,
            Declined = 2,
            Confirm = 3,
            Cancel = 4,
            New = 5,
        }

        public enum ApproveType : short
        {
            Approve = 1,
            Cancel = 2,
            View = 3
        }
        // Customer Status
        public enum CustomerStatus
        {
            NEW = 1,
            CONNECTION = 2,
            UNCONNECTION = 3
        }

        // Customer Type
        public enum CustomerType
        {
            Company = 1,
            Personal = 2,
        }
    }
}