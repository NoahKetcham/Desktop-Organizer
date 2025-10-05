using System;
using System.Collections.Generic;

namespace Boxes.Models;

public class OrganizationRule
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public int Priority { get; set; } = 0; // Higher priority rules are checked first

    // Rule conditions
    public RuleConditionType ConditionType { get; set; }
    public List<string> Extensions { get; set; } = new();
    public List<string> NamePatterns { get; set; } = new();
    public long? MinSizeBytes { get; set; }
    public long? MaxSizeBytes { get; set; }
    public DateTime? OlderThan { get; set; }
    public DateTime? NewerThan { get; set; }

    // Rule actions
    public string TargetFolder { get; set; } = string.Empty;
    public string TargetCategory { get; set; } = string.Empty;
    public bool CreateSubfolderByDate { get; set; }
    public bool CreateSubfolderByType { get; set; }

    // Stats
    public int FilesMatched { get; set; }
    public DateTime LastApplied { get; set; }
}

public enum RuleConditionType
{
    FileExtension,
    FileName,
    FileSize,
    FileDate,
    Combined
}

public enum RuleAction
{
    Move,
    Copy,
    Categorize,
    Archive
}
