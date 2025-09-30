using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Boxes.Models;

namespace Boxes.Services;

public class RuleEngine
{
    private readonly List<OrganizationRule> _rules = new();
    
    public void AddRule(OrganizationRule rule)
    {
        _rules.Add(rule);
    }
    
    public void RemoveRule(string ruleId)
    {
        _rules.RemoveAll(r => r.Id == ruleId);
    }
    
    public List<OrganizationRule> GetRules() => _rules.ToList();
    
    public void ApplyRulesToFiles(List<FileItem> files)
    {
        // Sort rules by priority (higher priority first)
        var sortedRules = _rules
            .Where(r => r.IsEnabled)
            .OrderByDescending(r => r.Priority)
            .ToList();
        
        foreach (var file in files)
        {
            // Find the first matching rule
            foreach (var rule in sortedRules)
            {
                if (DoesRuleMatch(file, rule))
                {
                    file.MatchedRule = rule;
                    file.SuggestedCategory = rule.TargetCategory;
                    file.SuggestedPath = GenerateTargetPath(file, rule);
                    rule.FilesMatched++;
                    break; // Stop at first match
                }
            }
        }
    }
    
    private bool DoesRuleMatch(FileItem file, OrganizationRule rule)
    {
        return rule.ConditionType switch
        {
            RuleConditionType.FileExtension => MatchesExtension(file, rule),
            RuleConditionType.FileName => MatchesFileName(file, rule),
            RuleConditionType.FileSize => MatchesFileSize(file, rule),
            RuleConditionType.FileDate => MatchesFileDate(file, rule),
            RuleConditionType.Combined => MatchesCombined(file, rule),
            _ => false
        };
    }
    
    private bool MatchesExtension(FileItem file, OrganizationRule rule)
    {
        if (rule.Extensions == null || rule.Extensions.Count == 0)
            return false;
            
        return rule.Extensions.Any(ext => 
            file.Extension.Equals(ext, StringComparison.OrdinalIgnoreCase) ||
            file.Extension.Equals($".{ext}", StringComparison.OrdinalIgnoreCase));
    }
    
    private bool MatchesFileName(FileItem file, OrganizationRule rule)
    {
        if (rule.NamePatterns == null || rule.NamePatterns.Count == 0)
            return false;
            
        foreach (var pattern in rule.NamePatterns)
        {
            try
            {
                // Convert wildcard pattern to regex
                var regexPattern = "^" + Regex.Escape(pattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".") + "$";
                    
                if (Regex.IsMatch(file.Name, regexPattern, RegexOptions.IgnoreCase))
                    return true;
            }
            catch
            {
                // If regex fails, try simple contains
                if (file.Name.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        
        return false;
    }
    
    private bool MatchesFileSize(FileItem file, OrganizationRule rule)
    {
        if (rule.MinSizeBytes.HasValue && file.SizeInBytes < rule.MinSizeBytes.Value)
            return false;
            
        if (rule.MaxSizeBytes.HasValue && file.SizeInBytes > rule.MaxSizeBytes.Value)
            return false;
            
        return rule.MinSizeBytes.HasValue || rule.MaxSizeBytes.HasValue;
    }
    
    private bool MatchesFileDate(FileItem file, OrganizationRule rule)
    {
        if (rule.OlderThan.HasValue && file.ModifiedDate > rule.OlderThan.Value)
            return false;
            
        if (rule.NewerThan.HasValue && file.ModifiedDate < rule.NewerThan.Value)
            return false;
            
        return rule.OlderThan.HasValue || rule.NewerThan.HasValue;
    }
    
    private bool MatchesCombined(FileItem file, OrganizationRule rule)
    {
        // For combined rules, all specified conditions must match
        bool matches = true;
        
        if (rule.Extensions?.Count > 0)
            matches &= MatchesExtension(file, rule);
            
        if (rule.NamePatterns?.Count > 0)
            matches &= MatchesFileName(file, rule);
            
        if (rule.MinSizeBytes.HasValue || rule.MaxSizeBytes.HasValue)
            matches &= MatchesFileSize(file, rule);
            
        if (rule.OlderThan.HasValue || rule.NewerThan.HasValue)
            matches &= MatchesFileDate(file, rule);
            
        return matches;
    }
    
    private string GenerateTargetPath(FileItem file, OrganizationRule rule)
    {
        var targetPath = rule.TargetFolder;
        
        if (rule.CreateSubfolderByDate)
        {
            targetPath = System.IO.Path.Combine(targetPath, 
                file.ModifiedDate.Year.ToString(), 
                file.ModifiedDate.ToString("MM-MMMM"));
        }
        
        if (rule.CreateSubfolderByType)
        {
            targetPath = System.IO.Path.Combine(targetPath, file.FileType);
        }
        
        return System.IO.Path.Combine(targetPath, file.Name);
    }
    
    public void LoadDefaultRules()
    {
        // Documents Rule
        AddRule(new OrganizationRule
        {
            Name = "Documents",
            Description = "Organize document files",
            ConditionType = RuleConditionType.FileExtension,
            Extensions = new List<string> { ".pdf", ".doc", ".docx", ".txt", ".rtf" },
            TargetFolder = "Documents",
            TargetCategory = "Documents",
            Priority = 10,
            IsEnabled = true
        });
        
        // Images Rule
        AddRule(new OrganizationRule
        {
            Name = "Images",
            Description = "Organize image files",
            ConditionType = RuleConditionType.FileExtension,
            Extensions = new List<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg" },
            TargetFolder = "Images",
            TargetCategory = "Images",
            Priority = 10,
            IsEnabled = true
        });
        
        // Screenshots Rule (higher priority)
        AddRule(new OrganizationRule
        {
            Name = "Screenshots",
            Description = "Organize screenshots",
            ConditionType = RuleConditionType.FileName,
            NamePatterns = new List<string> { "Screenshot*", "Screen Shot*", "screenshot*" },
            TargetFolder = "Screenshots",
            TargetCategory = "Screenshots",
            Priority = 20,
            IsEnabled = true,
            CreateSubfolderByDate = true
        });
        
        // Videos Rule
        AddRule(new OrganizationRule
        {
            Name = "Videos",
            Description = "Organize video files",
            ConditionType = RuleConditionType.FileExtension,
            Extensions = new List<string> { ".mp4", ".avi", ".mkv", ".mov", ".wmv" },
            TargetFolder = "Videos",
            TargetCategory = "Videos",
            Priority = 10,
            IsEnabled = true
        });
        
        // Archives Rule
        AddRule(new OrganizationRule
        {
            Name = "Archives",
            Description = "Organize compressed files",
            ConditionType = RuleConditionType.FileExtension,
            Extensions = new List<string> { ".zip", ".rar", ".7z", ".tar", ".gz" },
            TargetFolder = "Archives",
            TargetCategory = "Archives",
            Priority = 10,
            IsEnabled = true
        });
        
        // Old Files Rule
        AddRule(new OrganizationRule
        {
            Name = "Old Files",
            Description = "Archive files older than 90 days",
            ConditionType = RuleConditionType.FileDate,
            OlderThan = DateTime.Now.AddDays(-90),
            TargetFolder = "Archive",
            TargetCategory = "Archived",
            Priority = 5,
            IsEnabled = false // Disabled by default
        });
    }
}
