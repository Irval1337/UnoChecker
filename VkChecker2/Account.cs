using System;
using System.Collections.Generic;

namespace VkChecker2
{
    public enum Gender { Male, Female };

    [Serializable]
    public class AdditionalInfo
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public bool HasPhoto { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Token { get; set; }
        public bool IsBanned { get; set; }
        public int FriendsCount { get; set; }
        public int SubsCount { get; set; }
        public List<long> AdminGroups { get; set; }
        public string ErrorType { get; set; }
    }

    [Serializable]
    public class Account
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public AdditionalInfo AdditionalInfo { get; set; }
        public string AdditionalInfoStr { get; set; }
    }

    [Serializable]
    public class AccountRaw
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AdditionalInfoStr { get; set; }
    }
}
