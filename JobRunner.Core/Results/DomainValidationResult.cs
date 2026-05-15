using System.Collections.Generic;
using System.ComponentModel;

namespace JobRunner.Core.Results
{
    public class DomainValidationResult
    {
        [DisplayName("IsValid")]
        public bool IsValid { get; set; }

        [DisplayName("Errors")]
        public List<string> Errors { get; set; } = new();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static DomainValidationResult Success() => new() { IsValid = true };
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static DomainValidationResult Fail(string error) => new() { IsValid = false, Errors = { error } };
    }

}
